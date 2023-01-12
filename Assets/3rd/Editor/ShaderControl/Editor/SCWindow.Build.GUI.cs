/*
Shader Control - (C) Copyright 2016-2019 Ramiro Oliva (Kronnect)
*/

using UnityEngine;
using UnityEditor;
using System;

namespace ShaderControl {

    public enum BuildViewSortType {
        ShaderName = 0,
        ShaderKeywordCount = 1,
        Keyword = 2
    }

    public enum BuildViewShaderOption {
        AllShaders = 0,
        ProjectShaders = 1,
        UnityInternalShaders = 2
    }
    public partial class SCWindow : EditorWindow {

        string buildShaderNameFilter;
        GUIContent[] viewShaderTexts = { new GUIContent("All Shaders", "Show all shaders included in the build"), new GUIContent("Project Shaders", "Show shaders with source code available"), new GUIContent("Internal Shaders", "Show Unity internal shaders included in the build") };

        void DrawBuildGUI() {

            GUILayout.Box(new GUIContent("This tab shows all shaders compiled in your last build.\nHere you can exclude any number of shaders or keywords from future compilations. No file is modified, only excluded from the build.\nIf you have exceeded the maximum allowed keywords in your project, use the <b>Project View</b> tab to remove shaders or disable any unwanted keyword from the project."), titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Quick Build", "Forces a quick compilation to extract all shaders and keywords included in the build."))) {
                EditorUtility.DisplayDialog("Ready to analyze!", "Now make a build as normal (select 'File -> Build Settings -> Build').\n\nShader Control will detect the shaders and keywords from the build process and list that information here.\n\nImportant Note!\nTo make this special build faster, shaders won't be compiled (they will show in pink in the build). This is normal. To create a normal build, just build the project again without clicking 'Quick Build'.", "Ok");
                SetEditorPrefBool("QUICK_BUILD", true);
                nextQuickBuild = true;
                ClearBuildData();
            }
            if (GUILayout.Button("Help", GUILayout.Width(40))) {
                ShowHelpWindowBuildView();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (nextQuickBuild) {
                EditorGUILayout.HelpBox("Shader Control is ready to collect data during the next build.", MessageType.Info);
            }

            int shadersCount = shadersBuildInfo == null || shadersBuildInfo.shaders == null ? 0 : shadersBuildInfo.shaders.Count;

            if (shadersBuildInfo != null) {
                if (!nextQuickBuild) {
                    EditorGUILayout.LabelField("Last build: " + ((shadersBuildInfo.creationDateTicks != 0) ? shadersBuildInfo.creationDateString : "no data yet. Click 'Quick Build' for more details."), EditorStyles.boldLabel);
                }
                if (shadersBuildInfo.requiresBuild) {
                    EditorGUILayout.HelpBox("Project shaders have been modified. Do a 'Quick Build' again to ensure the data shown in this tab is accurate.", MessageType.Warning);
                }
            }

            if (shadersCount > 0) {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("View", GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();

                shadersBuildInfo.viewType = (BuildViewShaderOption)GUILayout.SelectionGrid((int)shadersBuildInfo.viewType, viewShaderTexts, 3);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(shadersBuildInfo);
                    RefreshBuildStats(true);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sort By", GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                shadersBuildInfo.sortType = (BuildViewSortType)EditorGUILayout.EnumPopup(shadersBuildInfo.sortType);
                if (EditorGUI.EndChangeCheck()) {
                    if (shadersBuildInfo != null) {
                        shadersBuildInfo.Resort();
                    }
                    EditorUtility.SetDirty(shadersBuildInfo);
                    GUIUtility.ExitGUI();
                    return;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Shader Name", GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                buildShaderNameFilter = EditorGUILayout.TextField(buildShaderNameFilter);
                if (GUILayout.Button(new GUIContent("Clear", "Clear filter."), EditorStyles.miniButton, GUILayout.Width(60))) {
                    buildShaderNameFilter = "";
                    GUIUtility.keyboardControl = 0;
                }
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck()) {
                    RefreshBuildStats(true);
                }

                EditorGUI.BeginChangeCheck();
                if (shadersBuildInfo.sortType != BuildViewSortType.Keyword) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Keywords >=", GUILayout.Width(100));
                    minimumKeywordCount = EditorGUILayout.IntSlider(minimumKeywordCount, 0, maxBuildKeywordsCountFound);
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
                if (EditorGUI.EndChangeCheck()) {
                    RefreshBuildStats(true);
                }

                EditorGUILayout.Separator();

                if (totalBuildShaders == 0 || totalBuildIncludedShaders == 0 || totalBuildKeywords == 0 || (totalBuildKeywords == totalBuildIncludedKeywords && totalBuildShaders == totalBuildIncludedShaders)) {
                    EditorGUILayout.HelpBox("Total Shaders: " + totalBuildShaders + "  Shaders Using Keywords: " + totalBuildShadersWithKeywords + "\nTotal Unique Keywords: " + totalBuildKeywords, MessageType.Info);
                } else {
                    int shadersPerc = totalBuildIncludedShaders * 100 / totalBuildShaders;
                    int shadersWithKeywordsPerc = totalBuildIncludedShadersWithKeywords * 100 / totalBuildIncludedShaders;
                    int keywordsPerc = totalBuildIncludedKeywords * 100 / totalBuildKeywords;
                    EditorGUILayout.HelpBox("Total Shaders: " + totalBuildIncludedShaders + " of " + totalBuildShaders + " (" + shadersPerc + "%" + "  Shaders Using Keywords: " + totalBuildIncludedShadersWithKeywords + " of " + totalBuildShadersWithKeywords + " (" + shadersWithKeywordsPerc + "%)\nTotal Unique Keywords: " + totalBuildIncludedKeywords + " of " + totalBuildKeywords + " (" + keywordsPerc.ToString() + "%)", MessageType.Info);
                }

                EditorGUILayout.Separator();

                scrollViewPosProject = EditorGUILayout.BeginScrollView(scrollViewPosProject);

                bool requireUpdate = false;

                if (shadersBuildInfo.sortType == BuildViewSortType.Keyword) {
                    if (buildKeywordView != null) {
                        int kvCount = buildKeywordView.Count;
                        for (int s = 0; s < kvCount; s++) {
                            BuildKeywordView kwv = buildKeywordView[s];
                            string keyword = kwv.keyword;
                            if (!string.IsNullOrEmpty(keywordFilter) && keyword.IndexOf(keywordFilter, StringComparison.InvariantCultureIgnoreCase) < 0)
                                continue;
                            EditorGUILayout.BeginHorizontal();
                            kwv.foldout = EditorGUILayout.Foldout(kwv.foldout, new GUIContent("Keyword #" + (s + 1) + " <b>" + kwv.keyword + "</b> found in " + kwv.shaders.Count + " shader(s)"), foldoutRTF);

                            if (!kwv.isInternal && GUILayout.Button("Show In Project View", EditorStyles.miniButton, GUILayout.Width(160))) {
                                sortType = SortType.EnabledKeywordsCount;
                                projectShaderNameFilter = "";
                                keywordFilter = kwv.keyword;
                                scanAllShaders = true;
                                if (shaders == null) ScanProject();
                                viewMode = ViewMode.Project;
                                GUIUtility.ExitGUI();
                            }

                            EditorGUILayout.EndHorizontal();
                            if (kwv.foldout) {
                                int kvShadersCount = kwv.shaders.Count;
                                for (int m = 0; m < kvShadersCount; m++) {
                                    ShaderBuildInfo sb = kwv.shaders[m];
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("", GUILayout.Width(30));
                                    EditorGUILayout.LabelField(shaderIcon, GUILayout.Width(18));
                                    EditorGUILayout.LabelField(sb.name);
                                    if (sb.isInternal) {
                                        GUILayout.Label("(Internal Shader)");
                                    } else {
                                        if (GUILayout.Button("Locate", EditorStyles.miniButton, GUILayout.Width(80))) {
                                            PingShader(sb.name);
                                        }
                                        if (GUILayout.Button("Show In Project View", EditorStyles.miniButton, GUILayout.Width(160))) {
                                            projectShaderNameFilter = sb.simpleName;
                                            keywordFilter = "";
                                            scanAllShaders = true;
                                            PingShader(sb.name);
                                            if (shaders == null) ScanProject();
                                            viewMode = ViewMode.Project;
                                            GUIUtility.ExitGUI();
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                } else {
                    for (int k = 0; k < shadersCount; k++) {
                        ShaderBuildInfo sb = shadersBuildInfo.shaders[k];
                        int kwCount = sb.keywords == null ? 0 : sb.keywords.Count;
                        if (kwCount < minimumKeywordCount) continue;
                        if (shadersBuildInfo.viewType == BuildViewShaderOption.ProjectShaders && sb.isInternal) continue;
                        if (shadersBuildInfo.viewType == BuildViewShaderOption.UnityInternalShaders && !sb.isInternal) continue;
                        if (!string.IsNullOrEmpty(keywordFilter) && !sb.ContainsKeyword(keywordFilter, false))
                            continue;
                        if (!string.IsNullOrEmpty(buildShaderNameFilter) && sb.name.IndexOf(buildShaderNameFilter, StringComparison.InvariantCultureIgnoreCase) < 0) continue;

                        GUI.enabled = sb.includeInBuild;
                        EditorGUILayout.BeginHorizontal();
                        string shaderName = (sb.isInternal && shadersBuildInfo.viewType != BuildViewShaderOption.UnityInternalShaders) ? sb.name + " (internal)" : sb.name;
                        sb.isExpanded = EditorGUILayout.Foldout(sb.isExpanded, shaderName + " (" + kwCount + " keyword" + (kwCount != 1 ? "s)" : ")"), sb.isInternal ? foldoutDim : foldoutNormal);
                        GUILayout.FlexibleSpace();
                        GUI.enabled = true;
                        if (sb.name != "Standard") {
                            EditorGUI.BeginChangeCheck();
                            sb.includeInBuild = EditorGUILayout.ToggleLeft("Include", sb.includeInBuild, GUILayout.Width(90));
                            if (EditorGUI.EndChangeCheck()) {
                                requireUpdate = true;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        if (sb.isExpanded) {
                            GUI.enabled = sb.includeInBuild;
                            EditorGUI.indentLevel++;
                            if (kwCount == 0) {
                                EditorGUILayout.LabelField("No keywords.");
                            } else {
                                if (!sb.isInternal) {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("", GUILayout.Width(15));
                                    if (GUILayout.Button("Locate", EditorStyles.miniButton, GUILayout.Width(80))) {
                                        PingShader(sb.name);
                                    }
                                    if (!sb.isInternal && GUILayout.Button("Show In Project View", EditorStyles.miniButton, GUILayout.Width(160))) {
                                        projectShaderNameFilter = sb.simpleName;
                                        scanAllShaders = true;
                                        PingShader(sb.name);
                                        if (shaders == null) ScanProject();
                                        viewMode = ViewMode.Project;
                                        GUIUtility.ExitGUI();
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                for (int j = 0; j < kwCount; j++) {
                                    KeywordBuildSettings kw = sb.keywords[j];
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField(kw.keyword);
                                    GUILayout.FlexibleSpace();
                                    EditorGUI.BeginChangeCheck();
                                    kw.includeInBuild = EditorGUILayout.ToggleLeft("Include", kw.includeInBuild, GUILayout.Width(90));
                                    if (EditorGUI.EndChangeCheck()) {
                                        requireUpdate = true;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            if (GUILayout.Button("Advanced...", GUILayout.Width(120))) {
                                SCWindowAdvanced.ShowWindow(sb);
                            }
                            int variantsCount = sb.variants != null ? sb.variants.Count : 0;
                            if (variantsCount > 0) {
                                GUILayout.Label("(Only building " + variantsCount + " variants)");
                            }
                            EditorGUILayout.EndHorizontal();


                            EditorGUI.indentLevel--;
                        }
                        GUI.enabled = true;
                    }
                }
                EditorGUILayout.EndScrollView();

                if (requireUpdate) {
                    RefreshBuildStats(true);
                    EditorUtility.SetDirty(shadersBuildInfo);
                    AssetDatabase.SaveAssets();
                }
            }

        }


    }

}