/// <summary>
/// Shader Control - (C) Copyright 2016-2020 Ramiro Oliva (Kronnect)
/// </summary>
/// 
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace ShaderControl {

    public partial class SCWindow : EditorWindow {

        enum SortType {
            VariantsCount = 0,
            EnabledKeywordsCount = 1,
            ShaderFileName = 2,
            Keyword = 3,
            Material = 4
        }

        enum KeywordScopeFilter {
            Any = 0,
            GlobalKeywords = 1,
            LocalKeywords = 2
        }

        enum PragmaTypeFilter {
            Any = 0,
            MultiCompile = 1,
            ShaderFeature = 2
        }

        enum ModifiedStatus {
            Any = 0,
            OnlyModified = 1,
            NonModified = 2
        }

        SortType sortType = SortType.VariantsCount;
        GUIStyle blackStyle, commentStyle, disabledStyle, foldoutBold, foldoutNormal, foldoutDim, foldoutRTF;
        GUIStyle titleStyle, redButtonStyle, labelDim, labelNormal;
        Vector2 scrollViewPosProject;
        bool firstTime;
        GUIContent matIcon, shaderIcon;
        bool scanAllShaders;
        string keywordFilter;
        KeywordScopeFilter keywordScopeFilter;
        PragmaTypeFilter pragmaTypeFilter;
        ModifiedStatus modifiedStatus;
        bool showShadersNotUsedInBuild;
        bool showKeywordsNotUsedInBuild;
        string projectShaderNameFilter;

        void DrawProjectGUI() {

            GUILayout.Box(new GUIContent("List of shader files in your project and modify their keywords.\nOnly shaders with source code or referenced by materials are included in this tab.\nUse the <b>Build View</b> tab for a list of shaders and keywords used in your build, including hidden/internal Unity shaders."), titleStyle, GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            scanAllShaders = EditorGUILayout.Toggle(new GUIContent("Force scan all shaders", "Also includes shaders that are not located within a Resources folder"), scanAllShaders);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Scan Project Folders", "Quickly scans the project and finds shaders that use keywords."))) {
                ScanProject();
                GUIUtility.ExitGUI();
                return;
            }
            if (shaders != null && shaders.Count > 0 && GUILayout.Button(new GUIContent("Clean All Materials", "Removes all disabled keywords in all materials.  This option is provided to ensure no existing materials are referencing a disabled shader keyword.\n\nTo disable keywords, first expand any shader from the list and uncheck the unwanted keywords (press 'Save' to modify the shader file and to clean any existing material that uses that specific shader)."), GUILayout.Width(130))) {
                CleanAllMaterials();
                GUIUtility.ExitGUI();
                return;
            }
            if (GUILayout.Button("Help", GUILayout.Width(40))) {
                ShowHelpWindowProjectView();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (shaders != null) {
                string totalGlobalKeywordsTxt = totalGlobalKeywords != totalKeywords ? " (" + totalGlobalKeywords + " global) " : " ";
                string statsText;
                if (totalKeywords == totalUsedKeywords || totalKeywords == 0) {
                    statsText = "Shaders Found: " + totalShaderCount.ToString() + "  Shaders Using Keywords: " + shaders.Count.ToString() + "\nKeywords: " + totalKeywords.ToString() + totalGlobalKeywordsTxt + " Variants: " + totalVariants.ToString();
                } else {
                    int keywordsPerc = totalUsedKeywords * 100 / totalKeywords;
                    int variantsPerc = totalBuildVariants * 100 / totalVariants;
                    statsText = "Shaders Found: " + totalShaderCount.ToString() + "  Shaders Using Keywords: " + shaders.Count.ToString() + "\nUsed Keywords: " + totalUsedKeywords.ToString() + " of " + totalKeywords.ToString() + " (" + keywordsPerc.ToString() + "%) " + totalGlobalKeywordsTxt + " Actual Variants: " + totalBuildVariants.ToString() + " of " + totalVariants.ToString() + " (" + variantsPerc.ToString() + "%)";
                }
                if (totalBuildShaders == 0) {
                    statsText += "\nNote: Total keyword count maybe higher. Use 'Build View' tab to detect additional shaders/keywords used by Unity.";
                } else if (plusBuildKeywords > 0) {
                    statsText += "\n+ " + plusBuildKeywords + " additional keywords detected in last build.";
                }
                EditorGUILayout.HelpBox(statsText, MessageType.Info);
                EditorGUILayout.Separator();
                int shaderCount = shaders.Count;
                if (shaderCount > 0) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Sort By", GUILayout.Width(100));
                    SortType prevSortType = sortType;
                    sortType = (SortType)EditorGUILayout.EnumPopup(sortType);
                    if (sortType != prevSortType) {
                        ScanProject();
                        GUIUtility.ExitGUI();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Filter", GUILayout.Width(100));
                    projectShaderNameFilter = EditorGUILayout.TextField(projectShaderNameFilter);
                    if (GUILayout.Button(new GUIContent("Clear", "Clear filter."), EditorStyles.miniButton, GUILayout.Width(60))) {
                        projectShaderNameFilter = "";
                        GUIUtility.keyboardControl = 0;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (sortType != SortType.Keyword) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Keywords >=", GUILayout.Width(100));
                        minimumKeywordCount = EditorGUILayout.IntSlider(minimumKeywordCount, 0, maxKeywordsCountFound);
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Keyword Filter", GUILayout.Width(100));
                    keywordFilter = EditorGUILayout.TextField(keywordFilter);
                    if (GUILayout.Button(new GUIContent("Clear", "Clear filter."), EditorStyles.miniButton, GUILayout.Width(60))) {
                        keywordFilter = "";
                        GUIUtility.keyboardControl = 0;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (sortType != SortType.Material) {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Keyword Scope", GUILayout.Width(100));
                        keywordScopeFilter = (KeywordScopeFilter)EditorGUILayout.EnumPopup(keywordScopeFilter);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Pragma Type", GUILayout.Width(100));
                        pragmaTypeFilter = (PragmaTypeFilter)EditorGUILayout.EnumPopup(pragmaTypeFilter);
                        EditorGUILayout.EndHorizontal();

                        if (sortType != SortType.Keyword) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Modified", GUILayout.Width(100));
                            modifiedStatus = (ModifiedStatus)EditorGUILayout.EnumPopup(modifiedStatus);
                            EditorGUILayout.EndHorizontal();
                            showShadersNotUsedInBuild = EditorGUILayout.ToggleLeft("List Shaders Not Used In Last Build", showShadersNotUsedInBuild);
                            if (showShadersNotUsedInBuild) {
                                if (totalBuildShaders == 0) {
                                    EditorGUILayout.HelpBox("No build data yet. Perform a build to detect which shaders are being included in your build. Use 'Quick Build' from the Build View tab to make a fast build.", MessageType.Error);
                                }
                            }
                        } else {
                            showKeywordsNotUsedInBuild = EditorGUILayout.ToggleLeft("List Keywords Not Used In Last Build", showKeywordsNotUsedInBuild);
                        }

                        if (keywordScopeFilter != KeywordScopeFilter.GlobalKeywords || pragmaTypeFilter != PragmaTypeFilter.ShaderFeature) {
                            if (totalGlobalShaderFeatures > 0) {
                                if (GUILayout.Button(new GUIContent("Click to show " + totalGlobalShaderFeatures + " global keyword(s) found of type 'shader_feature'", "Lists all shaders using global keywords that can be converted to local safely."))) {
                                    sortType = SortType.Keyword;
                                    keywordScopeFilter = KeywordScopeFilter.GlobalKeywords;
                                    pragmaTypeFilter = PragmaTypeFilter.ShaderFeature;
                                    keywordFilter = "";
                                    GUIUtility.keyboardControl = 0;
                                    minimumKeywordCount = 0;
                                }
                            }
                        } else {
                            if (sortType == SortType.Keyword && totalGlobalShaderFeatures > 1) {
                                if (GUILayout.Button(new GUIContent("Convert all these " + totalGlobalShaderFeatures + " keyword(s) to local", "Converts all global keywords of type Shader_Feature to local keywords, reducing the total keyword usage count.\n A local keyword does not count towards the limit of 256 total keywords in project."))) {
                                    ConvertToLocalAll();
                                    EditorUtility.DisplayDialog("Process complete!", "All keywords of type shader_feature have been converted to local.\n\nPlease restart Unity to refresh keyword changes.", "Ok");
                                    ScanProject();
                                    GUIUtility.ExitGUI();
                                }
                            }
                        }
                    }
                    EditorGUILayout.Separator();
                }
                scrollViewPosProject = EditorGUILayout.BeginScrollView(scrollViewPosProject);
                if (sortType == SortType.Material) {
                    if (projectMaterials != null) {
                        int matCount = projectMaterials.Count;
                        for (int s = 0; s < matCount; s++) {
                            SCMaterial mat = projectMaterials[s];
                            int keywordCount = mat.keywords.Count;
                            if (keywordCount < minimumKeywordCount) continue;
                            if (!string.IsNullOrEmpty(projectShaderNameFilter) && mat.unityMaterial.name.IndexOf(projectShaderNameFilter, StringComparison.InvariantCultureIgnoreCase) < 0)
                                continue;

                            mat.foldout = EditorGUILayout.Foldout(mat.foldout, new GUIContent("Material <b>" + mat.unityMaterial.name + "</b> referencing " + keywordCount + " keywords."), foldoutRTF);
                            if (mat.foldout) {
                                for (int m = 0; m < keywordCount; m++) {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("", GUILayout.Width(30));
                                    EditorGUILayout.LabelField(shaderIcon, GUILayout.Width(18));
                                    EditorGUILayout.LabelField(mat.keywords[m].name);
                                    if (GUILayout.Button(new GUIContent("Locate", "Locates material in project."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                        Selection.activeObject = mat.unityMaterial;
                                        EditorGUIUtility.PingObject(mat.unityMaterial);
                                    }
                                    if (GUILayout.Button(new GUIContent("Prune", "Removes keyword from this material."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                        if (EditorUtility.DisplayDialog("Prune Keyword", "Remove this keyword from the material?", "Yes", "No")) {
                                            mat.unityMaterial.DisableKeyword(mat.keywords[m].name);
                                            EditorUtility.SetDirty(mat.unityMaterial);
                                            mat.RemoveKeyword(mat.keywords[m].name);
                                            GUIUtility.ExitGUI();
                                            return;
                                        }
                                    }
                                    if (GUILayout.Button(new GUIContent("Prune All", "Removes this keyword from all materials."), EditorStyles.miniButton, GUILayout.Width(80))) {
                                        if (EditorUtility.DisplayDialog("Prune All", "Remove this keyword from all materials?", "Yes", "No")) {
                                            PruneMaterials(mat.keywords[m].name);
                                            UpdateProjectStats();
                                            GUIUtility.ExitGUI();
                                            return;
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                } else if (sortType == SortType.Keyword) {
                    if (keywordView != null) {
                        int kvCount = keywordView.Count;
                        bool usesKeywordFilter = !string.IsNullOrEmpty(keywordFilter);
                        for (int s = 0; s < kvCount; s++) {
                            SCKeyword keyword = keywordView[s].keyword;
                            if (keywordScopeFilter != KeywordScopeFilter.Any && ((keywordScopeFilter == KeywordScopeFilter.GlobalKeywords && !keyword.isGlobal) || (keywordScopeFilter == KeywordScopeFilter.LocalKeywords && keyword.isGlobal))) continue;
                            if (pragmaTypeFilter != PragmaTypeFilter.Any && ((pragmaTypeFilter == PragmaTypeFilter.MultiCompile && !keyword.isMultiCompile) || (pragmaTypeFilter == PragmaTypeFilter.ShaderFeature && keyword.isMultiCompile))) continue;
                            if (usesKeywordFilter && keyword.name.IndexOf(keywordFilter, StringComparison.InvariantCultureIgnoreCase) < 0)
                                continue;
                            if (showKeywordsNotUsedInBuild && totalBuildShaders > 0) {
                                bool included = false;
                                for (int sb = 0; sb < totalBuildShaders; sb++) {
                                    ShaderBuildInfo sbi = shadersBuildInfo.shaders[sb];
                                    int count = sbi.keywords.Count;
                                    for (int kb = 0; kb < count; kb++) {
                                        if (sbi.keywords[kb].includeInBuild && sbi.keywords[kb].keyword.Equals(keyword.name)) {
                                            included = true;
                                            break;
                                        }
                                    }
                                    if (included) break;
                                }
                                if (included) continue;
                            }
                            EditorGUILayout.BeginHorizontal();
                            keywordView[s].foldout = EditorGUILayout.Foldout(keywordView[s].foldout, new GUIContent("Keyword <b>" + keywordView[s].keyword + "</b> found in " + keywordView[s].shaders.Count + " shader(s)"), foldoutRTF);
                            GUILayout.FlexibleSpace();
                            if (!keyword.isMultiCompile && keyword.isGlobal) {
                                if (GUILayout.Button("Convert To Local Keyword", EditorStyles.miniButtonRight, GUILayout.Width(190))) {
                                    if (EditorUtility.DisplayDialog("Convert global keyword to local", "Keyword " + keyword.name + " will be converted to local. This means the keyword won't count toward the maximum global keyword limit (256). Continue?", "Ok", "Cancel")) {
                                        ConvertToLocal(keyword);
                                        ScanProject();
                                        GUIUtility.ExitGUI();
                                    }
                                }
                            } else if (!keyword.isGlobal) {
                                GUILayout.Label("(Local keyword)");
                            }
                            EditorGUILayout.EndHorizontal();
                            if (keywordView[s].foldout) {
                                KeywordView kv = keywordView[s];
                                int kvShadersCount = kv.shaders.Count;
                                for (int m = 0; m < kvShadersCount; m++) {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("", GUILayout.Width(30));
                                    EditorGUILayout.LabelField(shaderIcon, GUILayout.Width(18));
                                    SCShader shader = kv.shaders[m];
                                    string shaderName = shader.name;
                                    if (shader.isReadOnly) shaderName += " (read-only)";
                                    GUIStyle shaderNameStyle = shader.isReadOnly ? labelDim : labelNormal;
                                    EditorGUILayout.LabelField(shaderName, shaderNameStyle);
                                    GUI.enabled = shader.hasSource;
                                    if (GUILayout.Button(new GUIContent("Locate", "Locates the shader in the project panel."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                        Shader theShader = AssetDatabase.LoadAssetAtPath<Shader>(shader.path);
                                        Selection.activeObject = theShader;
                                        EditorGUIUtility.PingObject(theShader);
                                    }
                                    GUI.enabled = true;
                                    if (GUILayout.Button(new GUIContent("Filter", "Show shaders using this keyword."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                        keywordFilter = kv.keyword.name;
                                        sortType = SortType.VariantsCount;
                                        GUIUtility.ExitGUI();
                                        return;
                                    }
                                    if (!shader.hasBackup)
                                        GUI.enabled = false;
                                    if (GUILayout.Button(new GUIContent("Restore", "Restores shader from the backup copy."), EditorStyles.miniButton)) {
                                        RestoreShader(shader);
                                        GUIUtility.ExitGUI();
                                        return;
                                    }
                                    GUI.enabled = true;
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                } else {
                    int shadersListedCount = 0;
                    bool usesShaderNameFilter = !string.IsNullOrEmpty(projectShaderNameFilter);
                    bool filterByKeywordName = !string.IsNullOrEmpty(keywordFilter);

                    for (int s = 0; s < shaderCount; s++) {
                        SCShader shader = shaders[s];
                        if (shader.keywordEnabledCount < minimumKeywordCount)
                            continue;
                        if (modifiedStatus == ModifiedStatus.OnlyModified && !shader.editedByShaderControl) continue;
                        if (modifiedStatus == ModifiedStatus.NonModified && shader.editedByShaderControl) continue;
                        if (filterByKeywordName || keywordScopeFilter != KeywordScopeFilter.Any || pragmaTypeFilter != PragmaTypeFilter.Any) {
                            int kwCount = shader.keywords.Count;
                            bool found = false;
                            for (int w = 0; w < kwCount; w++) {
                                found = true;
                                SCKeyword keyword = shader.keywords[w];
                                if (filterByKeywordName) {
                                    if (keyword.name.IndexOf(keywordFilter, StringComparison.InvariantCultureIgnoreCase) < 0) {
                                        found = false;
                                    }
                                }
                                if (keywordScopeFilter != KeywordScopeFilter.Any && ((keywordScopeFilter == KeywordScopeFilter.GlobalKeywords && !keyword.isGlobal) || (keywordScopeFilter == KeywordScopeFilter.LocalKeywords && keyword.isGlobal))) {
                                    found = false;
                                }
                                if (pragmaTypeFilter != PragmaTypeFilter.Any && ((pragmaTypeFilter == PragmaTypeFilter.MultiCompile && !keyword.isMultiCompile) || (pragmaTypeFilter == PragmaTypeFilter.ShaderFeature && keyword.isMultiCompile))) {
                                    found = false;
                                }
                                if (found) break;
                            }
                            if (!found)
                                continue;
                        }
                        if (usesShaderNameFilter && shader.name.IndexOf(projectShaderNameFilter, StringComparison.InvariantCultureIgnoreCase) < 0) {
                            continue;
                        }
                        if (showShadersNotUsedInBuild && totalBuildShaders > 0) {
                            bool included = false;
                            for (int sb = 0; sb < totalBuildShaders; sb++) {
                                if (shadersBuildInfo.shaders[sb].name.Equals(shader.fullName)) {
                                    included = true;
                                    break;
                                }
                            }
                            if (included) continue;
                        }

                        shadersListedCount++;
                        EditorGUILayout.BeginHorizontal();
                        string shaderName = shader.isReadOnly ? shader.name + " (read-only)" : shader.name;
                        if (shader.hasSource) {
                            shader.foldout = EditorGUILayout.Foldout(shader.foldout, new GUIContent(shaderName + " (" + shader.keywords.Count + " keywords, " + shader.keywordEnabledCount + " used, " + shader.actualBuildVariantCount + " variants)"), shader.editedByShaderControl ? foldoutBold : foldoutNormal);
                        } else {
                            shader.foldout = EditorGUILayout.Foldout(shader.foldout, new GUIContent(shaderName + " (" + shader.keywordEnabledCount + " keywords used by materials)"), foldoutDim);
                        }
                        EditorGUILayout.EndHorizontal();
                        if (shader.foldout) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.Width(15));
                            if (shader.hasSource) {
                                if (GUILayout.Button(new GUIContent("Locate", "Locate the shader in the project panel."), EditorStyles.miniButton)) {
                                    Shader theShader = AssetDatabase.LoadAssetAtPath<Shader>(shader.path);
                                    Selection.activeObject = theShader;
                                    EditorGUIUtility.PingObject(theShader);
                                }
                                if (!shader.isShaderGraph) {
                                    if (GUILayout.Button(new GUIContent("Open", "Open the shader with the system default editor."), EditorStyles.miniButton)) {
                                        EditorUtility.OpenWithDefaultApp(shader.path);
                                    }
                                    if (!shader.pendingChanges)
                                        GUI.enabled = false;
                                    if (GUILayout.Button(new GUIContent("Save", "Saves any keyword change to the shader file (disable/enable the keywords just clicking on the toggle next to the keywords shown below). A backup is created in the same folder."), EditorStyles.miniButton)) {
                                        UpdateShader(shader);
                                    }
                                    GUI.enabled = true;
                                }
                                if (!shader.hasBackup)
                                    GUI.enabled = false;
                                if (GUILayout.Button(new GUIContent("Restore", "Restores shader from the backup copy."), EditorStyles.miniButton)) {
                                    RestoreShader(shader);
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                                GUI.enabled = true;
                            } else {
                                EditorGUILayout.LabelField("(Shader source not available)");
                            }
                            if (showShadersNotUsedInBuild) {
                                if (redButtonStyle == null) {
                                    redButtonStyle = new GUIStyle(GUI.skin.button);
                                    redButtonStyle.normal.textColor = Color.red;
                                }
                                if (GUILayout.Button(new GUIContent("Delete", "Deletes shader file."), redButtonStyle)) {
                                    if (shader.isReadOnly) {
                                        EditorUtility.DisplayDialog("Error", "Shader file is read-only.", "Ok");
                                    } else if (EditorUtility.DisplayDialog("Delete Shader", "Are you sure you want to delete the shader " + shader.name + " at " + shader.path + "?\n\nThis operation is not reversible - make sure you have a backup or way to import the shader again if you need it later.", "Yes - Delete It", "Cancel")) {
                                        DeleteShader(shader);
                                        GUIUtility.ExitGUI();
                                        return;
                                    }
                                }
                            }
                            if (shader.materials.Count > 0) {
                                if (GUILayout.Button(new GUIContent(shader.showMaterials ? "Hide Materials" : "List Materials", "Show or hide the materials that use these keywords."), EditorStyles.miniButton)) {
                                    shader.showMaterials = !shader.showMaterials;
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                                if (shader.showMaterials) {
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("", GUILayout.Width(15));
                                    if (GUILayout.Button("Select Materials In Project", EditorStyles.miniButton)) {
                                        int matCount = shader.materials.Count;
                                        List<Material> allMaterials = new List<Material>();
                                        for (int m = 0; m < matCount; m++) {
                                            SCMaterial material = shader.materials[m];
                                            allMaterials.Add(material.unityMaterial);
                                        }
                                        if (allMaterials.Count > 0) {
                                            Selection.objects = allMaterials.ToArray();
                                            EditorGUIUtility.PingObject(allMaterials[0]);
                                        } else {
                                            EditorUtility.DisplayDialog("Select Materials In Project", "No matching materials found in project.", "Ok");
                                        }
                                    }
                                    if (GUILayout.Button("Select Materials In Scene", EditorStyles.miniButton)) {
                                        int matCount = shader.materials.Count;
                                        List<GameObject> gos = new List<GameObject>();
                                        Renderer[] rr = FindObjectsOfType<Renderer>();
                                        foreach (Renderer r in rr) {
                                            GameObject go = r.gameObject;
                                            if (go == null) continue;
                                            if ((go.hideFlags & HideFlags.NotEditable) != 0 || (go.hideFlags & HideFlags.HideAndDontSave) != 0)
                                                continue;

                                            Material[] mats = r.sharedMaterials;
                                            if (mats == null) continue;

                                            for (int m = 0; m < matCount; m++) {
                                                Material mat = shader.materials[m].unityMaterial;
                                                for (int sm = 0; sm < mats.Length; sm++) {
                                                    if (mats[sm] == mat) {
                                                        gos.Add(go);
                                                        m = matCount;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (gos.Count > 0) {
                                            Selection.objects = gos.ToArray();
                                        } else {
                                            EditorUtility.DisplayDialog("Select Materials In Scene", "No matching materials found in objects of this scene.", "Ok");
                                        }
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            for (int k = 0; k < shader.keywords.Count; k++) {
                                SCKeyword keyword = shader.keywords[k];
                                if (keyword.isUnderscoreKeyword)
                                    continue;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("", GUILayout.Width(15));
                                if (shader.hasSource && !shader.isShaderGraph) {
                                    bool prevState = keyword.enabled;
                                    keyword.enabled = EditorGUILayout.Toggle(prevState, GUILayout.Width(18));
                                    if (prevState != keyword.enabled) {
                                        shader.pendingChanges = true;
                                        shader.UpdateVariantCount();
                                        UpdateProjectStats();
                                        GUIUtility.ExitGUI();
                                        return;
                                    }
                                } else {
                                    EditorGUILayout.Toggle(true, GUILayout.Width(18));
                                }
                                EditorGUILayout.LabelField(keyword.verboseName);
                                if (keyword.enabled) {
                                    if (!shader.hasSource && GUILayout.Button(new GUIContent("Prune Keyword", "Removes the keyword from all materials that reference it."), EditorStyles.miniButton, GUILayout.Width(110))) {
                                        if (EditorUtility.DisplayDialog("Prune Keyword", "This option will disable the keyword " + keyword.name + " in all materials that use " + shader.name + " shader.\nDo you want to continue?", "Ok", "Cancel")) {
                                            PruneMaterials(keyword.name);
                                            UpdateProjectStats();
                                        }
                                    }
                                    if (!keyword.isMultiCompile && keyword.isGlobal) {
                                        if (GUILayout.Button("Convert To Local Keyword", EditorStyles.miniButtonRight, GUILayout.Width(190))) {
                                            if (EditorUtility.DisplayDialog("Convert global keyword to local", "Keyword " + keyword.name + " will be converted to local. This means the keyword won't count toward the maximum global keyword limit (256). Continue?", "Ok", "Cancel")) {
                                                ConvertToLocal(keyword);
                                                ScanProject();
                                                GUIUtility.ExitGUI();
                                            }
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                                if (shader.showMaterials) {
                                    // show materials using this shader
                                    int matCount = shader.materials.Count;
                                    for (int m = 0; m < matCount; m++) {
                                        SCMaterial material = shader.materials[m];
                                        if (material.ContainsKeyword(keyword.name)) {
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("", GUILayout.Width(30));
                                            EditorGUILayout.LabelField(matIcon, GUILayout.Width(18));
                                            EditorGUILayout.LabelField(material.unityMaterial.name);
                                            if (GUILayout.Button(new GUIContent("Locate", "Locates the material in the project panel."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                                Material theMaterial = AssetDatabase.LoadAssetAtPath<Material>(material.path) as Material;
                                                Selection.activeObject = theMaterial;
                                                EditorGUIUtility.PingObject(theMaterial);
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                }
                            }

                            if (shader.showMaterials) {
                                // show materials using this shader that does not use any keywords
                                bool first = true;
                                int matCount = shader.materials.Count;
                                for (int m = 0; m < matCount; m++) {
                                    SCMaterial material = shader.materials[m];
                                    if (material.keywords.Count == 0) {
                                        if (first) {
                                            first = false;
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("", GUILayout.Width(15));
                                            EditorGUILayout.LabelField("Materials using this shader and no keywords:");
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("", GUILayout.Width(15));
                                        EditorGUILayout.LabelField(matIcon, GUILayout.Width(18));
                                        EditorGUILayout.LabelField(material.unityMaterial.name);
                                        if (GUILayout.Button(new GUIContent("Locate", "Locates the material in the project panel."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                            Material theMaterial = AssetDatabase.LoadAssetAtPath<Material>(material.path) as Material;
                                            Selection.activeObject = theMaterial;
                                            EditorGUIUtility.PingObject(theMaterial);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                            }
                        }
                        EditorGUILayout.Separator();
                    }
                    if (shadersListedCount == 0 && !string.IsNullOrEmpty(projectShaderNameFilter)) {
                        EditorGUILayout.HelpBox("No shader matching '" + projectShaderNameFilter + "' name found. Either the shader doesn't exist in the project as source file or the shader does not use explicit keywords.", MessageType.Info);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }


    }

}