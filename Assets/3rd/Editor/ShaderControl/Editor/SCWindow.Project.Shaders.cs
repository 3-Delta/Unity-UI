/// <summary>
/// Shader Control - (C) Copyright 2016-2020 Ramiro Oliva (Kronnect)
/// </summary>
/// 
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ShaderControl {

    public partial class SCWindow : EditorWindow {

        class KeywordView {
            public SCKeyword keyword;
            public List<SCShader> shaders;
            public bool foldout;
        }

        const string PRAGMA_COMMENT_MARK = "// Edited by Shader Control: ";
        const string PRAGMA_DISABLED_MARK = "// Disabled by Shader Control: ";
        const string BACKUP_SUFFIX = "_backup";
        const string PRAGMA_UNDERSCORE = "__ ";

        List<SCShader> shaders;
        Dictionary<int, SCShader> shadersDict;
        int minimumKeywordCount;
        int totalShaderCount;
        int maxKeywordsCountFound = 0;
        int totalKeywords, totalGlobalKeywords, totalVariants, totalUsedKeywords, totalBuildVariants, totalGlobalShaderFeatures;
        int plusBuildKeywords;
        Dictionary<string, List<SCShader>> uniqueKeywords, uniqueEnabledKeywords;
        Dictionary<string, SCKeyword> keywordsDict;
        List<KeywordView> keywordView;

        #region Shader handling

        void ScanProject() {
            try {
                if (shaders == null) {
                    shaders = new List<SCShader>();
                } else {
                    shaders.Clear();
                }
                // Add shaders from Resources folder
                string[] guids = AssetDatabase.FindAssets("t:Shader");
                totalShaderCount = guids.Length;
                for (int k = 0; k < totalShaderCount; k++) {
                    string guid = guids[k];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path != null) {
                        string pathUpper = path.ToUpper();
                        if (scanAllShaders || pathUpper.Contains("\\RESOURCES\\") || pathUpper.Contains("/RESOURCES/")) {   // this shader will be included in build
                            Shader unityShader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                            if (unityShader != null) {
                                SCShader shader = new SCShader();
                                shader.fullName = unityShader.name;
                                shader.name = SCShader.GetSimpleName(shader.fullName); //  Path.GetFileNameWithoutExtension(path);
                                shader.path = path;
                                shader.isReadOnly = path.Contains("Packages/com.unity") || IsFileReadonly(path);
                                shader.GUID = unityShader.GetInstanceID();
                                ScanShader(shader);
                                if (shader.keywords.Count > 0) {
                                    shaders.Add(shader);
                                }
                            }
                        }
                    }
                }

                // Load and reference materials
                if (shadersDict == null) {
                    shadersDict = new Dictionary<int, SCShader>(shaders.Count);
                } else {
                    shadersDict.Clear();
                }
                shaders.ForEach(shader => {
                    shadersDict[shader.GUID] = shader;
                });
                string[] matGuids = AssetDatabase.FindAssets("t:Material");
                if (projectMaterials == null) {
                    projectMaterials = new List<SCMaterial>();
                } else {
                    projectMaterials.Clear();
                }

                for (int k = 0; k < matGuids.Length; k++) {
                    string matGUID = matGuids[k];
                    string matPath = AssetDatabase.GUIDToAssetPath(matGUID);
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    if (mat.shader == null)
                        continue;
                    SCMaterial scMat = new SCMaterial(mat, matPath, matGUID);
                    scMat.SetKeywords(mat.shaderKeywords);

                    if (mat.shaderKeywords != null && mat.shaderKeywords.Length > 0) {
                        projectMaterials.Add(scMat);
                    }

                    string path = AssetDatabase.GetAssetPath(mat.shader);
                    int shaderGUID = mat.shader.GetInstanceID();
                    SCShader shader;
                    if (!shadersDict.TryGetValue(shaderGUID, out shader)) {
                        if (mat.shaderKeywords == null || mat.shaderKeywords.Length == 0)
                            continue;
                        Shader shad = AssetDatabase.LoadAssetAtPath<Shader>(path);
                        // add non-sourced shader
                        shader = new SCShader();
                        shader.isReadOnly = path.Contains("Packages/com.unity") || IsFileReadonly(path);
                        shader.GUID = shaderGUID;
                        if (shad != null) {
                            shader.fullName = shad.name;
                            shader.name = SCShader.GetSimpleName(shader.fullName);
                            shader.path = path;
                            ScanShader(shader);
                        } else {
                            shader.fullName = mat.shader.name;
                            shader.name = SCShader.GetSimpleName(shader.fullName);
                        }
                        shaders.Add(shader);
                        shadersDict[shaderGUID] = shader;
                        totalShaderCount++;
                    }
                    shader.materials.Add(scMat);
                    shader.AddKeywordsByName(mat.shaderKeywords);
                }

                // sort materials by name
                projectMaterials.Sort(CompareMaterialsName);

                // refresh variant and keywords count due to potential additional added keywords from materials (rogue keywords) and shader features count
                maxKeywordsCountFound = 0;
                shaders.ForEach((SCShader shader) => {
                    if (shader.keywordEnabledCount > maxKeywordsCountFound) {
                        maxKeywordsCountFound = shader.keywordEnabledCount;
                    }
                    shader.UpdateVariantCount();
                });

                switch (sortType) {
                    case SortType.VariantsCount:
                        shaders.Sort((SCShader x, SCShader y) => {
                            return y.actualBuildVariantCount.CompareTo(x.actualBuildVariantCount);
                        });
                        break;
                    case SortType.EnabledKeywordsCount:
                        shaders.Sort((SCShader x, SCShader y) => {
                            return y.keywordEnabledCount.CompareTo(x.keywordEnabledCount);
                        });
                        break;
                    case SortType.ShaderFileName:
                        shaders.Sort((SCShader x, SCShader y) => {
                            return x.name.CompareTo(y.name);
                        });
                        break;
                }
                UpdateProjectStats();
            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while scanning project: " + ex.Message);
            }
        }

        static int CompareMaterialsName(SCMaterial m1, SCMaterial m2) {
            return m1.unityMaterial.name.CompareTo(m2.unityMaterial.name);
        }

        void ScanShader(SCShader shader) {

            // Inits shader
            shader.passes.Clear();
            shader.keywords.Clear();
            shader.hasBackup = File.Exists(shader.path + BACKUP_SUFFIX);
            shader.pendingChanges = false;
            shader.editedByShaderControl = shader.hasBackup;

            if (shader.path.EndsWith(".shadergraph")) {
                shader.isShaderGraph = true;
                try {
                    ScanShaderGraph(shader);
                } catch (Exception ex) {
                    Debug.LogError("Couldn't analyze shader graph at " + shader.path + ". Error found: " + ex.ToString());
                }
            } else {
                try {
                    ScanShaderNonGraph(shader);
                } catch (Exception ex) {
                    Debug.LogError("Couldn't analyze shader at " + shader.path + ". Error found: " + ex.ToString());
                }
            }
        }


        void UpdateProjectStats() {
            totalKeywords = 0;
            totalGlobalKeywords = 0;
            totalUsedKeywords = 0;
            totalVariants = 0;
            totalBuildVariants = 0;
            totalGlobalShaderFeatures = 0;

            if (shaders == null)
                return;

            if (keywordsDict == null) {
                keywordsDict = new Dictionary<string, SCKeyword>();
            } else {
                keywordsDict.Clear();
            }
            if (uniqueKeywords == null) {
                uniqueKeywords = new Dictionary<string, List<SCShader>>();
            } else {
                uniqueKeywords.Clear();
            }
            if (uniqueEnabledKeywords == null) {
                uniqueEnabledKeywords = new Dictionary<string, List<SCShader>>();
            } else {
                uniqueEnabledKeywords.Clear();
            }

            int shadersCount = shaders.Count;
            for (int k = 0; k < shadersCount; k++) {
                SCShader shader = shaders[k];
                int keywordsCount = shader.keywords.Count;
                for (int w = 0; w < keywordsCount; w++) {
                    SCKeyword keyword = shader.keywords[w];
                    List<SCShader> shadersWithThisKeyword;
                    if (!uniqueKeywords.TryGetValue(keyword.name, out shadersWithThisKeyword)) {
                        shadersWithThisKeyword = new List<SCShader>();
                        uniqueKeywords[keyword.name] = shadersWithThisKeyword;
                        totalKeywords++;
                        if (keyword.isGlobal) totalGlobalKeywords++;
                        if (keyword.isGlobal && !keyword.isMultiCompile && keyword.enabled) totalGlobalShaderFeatures++;
                        keywordsDict[keyword.name] = keyword;
                    }
                    shadersWithThisKeyword.Add(shader);
                    if (keyword.enabled) {
                        List<SCShader> shadersWithThisKeywordEnabled;
                        if (!uniqueEnabledKeywords.TryGetValue(keyword.name, out shadersWithThisKeywordEnabled)) {
                            shadersWithThisKeywordEnabled = new List<SCShader>();
                            uniqueEnabledKeywords[keyword.name] = shadersWithThisKeywordEnabled;
                            totalUsedKeywords++;
                        }
                        shadersWithThisKeywordEnabled.Add(shader);
                    }
                }
                totalVariants += shader.totalVariantCount;
                totalBuildVariants += shader.actualBuildVariantCount;
            }

            if (keywordView == null) {
                keywordView = new List<KeywordView>();
            } else {
                keywordView.Clear();
            }
            foreach (KeyValuePair<string, List<SCShader>> kvp in uniqueEnabledKeywords) {
                SCKeyword kw;
                if (!keywordsDict.TryGetValue(kvp.Key, out kw)) continue;
                KeywordView kv = new KeywordView { keyword = kw, shaders = kvp.Value };
                keywordView.Add(kv);
            }
            keywordView.Sort(delegate (KeywordView x, KeywordView y) {
                return y.shaders.Count.CompareTo(x.shaders.Count);
            });

            // Compute which keywords in build are not present in project
            plusBuildKeywords = 0;
            if (buildKeywordView != null) {
                int count = buildKeywordView.Count;
                for (int k = 0; k < count; k++) {
                    BuildKeywordView bkv = buildKeywordView[k];
                    if (!uniqueKeywords.ContainsKey(bkv.keyword)) {
                        plusBuildKeywords++;
                    }
                }
            }
        }

        bool IsFileReadonly(string path) {
            FileStream stream = null;

            try {
                FileAttributes fileAttributes = File.GetAttributes(path);
                if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    return true;
                }
                FileInfo file = new FileInfo(path);
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            } catch (IOException) {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            } finally {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        void MakeBackup(SCShader shader) {
            string backupPath = shader.path + BACKUP_SUFFIX;
            if (!File.Exists(backupPath)) {
                AssetDatabase.CopyAsset(shader.path, backupPath);
                shader.hasBackup = true;
            }
        }

        void UpdateShader(SCShader shader) {
            if (shader.isReadOnly) {
                EditorUtility.DisplayDialog("Locked file", "Shader file " + shader.name + " is read-only.", "Ok");
                return;
            }
            try {
                // Create backup
                MakeBackup(shader);

                if (shader.isShaderGraph) {
                    UpdateShaderGraph(shader);
                } else {
                    UpdateShaderNonGraph(shader);
                }

                // Also update materials
                CleanMaterials(shader);

                ScanShader(shader); // Rescan shader

                // do not include in build (sync with Build View)
                BuildUpdateShaderKeywordsState(shader);

            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while updating shader: " + ex.Message);
            }
        }

        void RestoreShader(SCShader shader) {
            try {
                string shaderBackupPath = shader.path + BACKUP_SUFFIX;
                if (!File.Exists(shaderBackupPath)) {
                    EditorUtility.DisplayDialog("Restore shader", "Shader backup is missing!", "OK");
                    return;
                }
                File.Copy(shaderBackupPath, shader.path, true);
                File.Delete(shaderBackupPath);
                if (File.Exists(shaderBackupPath + ".meta"))
                    File.Delete(shaderBackupPath + ".meta");
                AssetDatabase.Refresh();

                ScanShader(shader); // Rescan shader
                UpdateProjectStats();
            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while restoring shader: " + ex.Message);
            }
        }


        void DeleteShader(SCShader shader) {
            try {
                if (File.Exists(shader.path)) {
                    File.Delete(shader.path);
                } else {
                    EditorUtility.DisplayDialog("Error", "Shader file was not found at " + shader.path + "!?", "Weird");
                    return;
                }
                File.Delete(shader.path);
                if (File.Exists(shader.path + ".meta")) {
                    File.Delete(shader.path + ".meta");
                }
                AssetDatabase.Refresh();
                ScanProject();
            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while deleting shader: " + ex.Message);
            }
        }

        void ConvertToLocal(SCKeyword keyword) {
            List<SCShader> shaders;
            if (!uniqueKeywords.TryGetValue(keyword.name, out shaders)) return;
            if (shaders == null) return;
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < shaders.Count; k++) {
                SCShader shader = shaders[k];
                if (shader.isReadOnly) {
                    if (sb.Length > 0) {
                        sb.Append(", ");
                    }
                    sb.Append(shader.name);
                } else {
                    ConvertToLocal(keyword, shaders[k]);
                }
            }
            if (sb.Length > 0) {
                EditorUtility.DisplayDialog("Converting " + keyword.name + " To Local Keyword", "The following shaders couldn't be modified because they're read-only:\n" + sb.ToString(), "Ok");
            }
            AssetDatabase.Refresh();
        }

        void ConvertToLocal(SCKeyword keyword, SCShader shader) {

            // Check total local keyword does not exceed 64 limit
            int potentialCount = 0;
            int kwCount = shader.keywords.Count;
            for (int k = 0; k < kwCount; k++) {
                SCKeyword kw = shader.keywords[k];
                if (!kw.isMultiCompile) potentialCount++;
            }
            if (potentialCount > 64) return;

            if (shader.isReadOnly) {
                EditorUtility.DisplayDialog("Convert To Local", "The keyword " + keyword.name + " can't be converted to local in shader " + shader.name + " at " + shader.path + " because file is readonly.", "Ok");
                return;
            }
            if (shader.isShaderGraph) {
                ConvertToLocalGraph(keyword, shader);
            } else {
                ConvertToLocalNonGraph(keyword, shader);
            }
        }

        void ConvertToLocalAll() {
            int kvCount = keywordView.Count;
            for (int s = 0; s < kvCount; s++) {
                SCKeyword keyword = keywordView[s].keyword;
                if (keyword.isGlobal && !keyword.isMultiCompile) {
                    ConvertToLocal(keyword);
                }
            }
            AssetDatabase.Refresh();
        }

        #endregion

    }

}