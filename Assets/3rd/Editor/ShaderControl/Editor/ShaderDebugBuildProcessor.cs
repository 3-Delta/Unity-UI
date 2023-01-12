using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderControl {

    class ShaderDebugBuildProcessor : IPreprocessShaders, IPostprocessBuildWithReport {

        ShadersBuildInfo shadersBuildInfo;

        public static ShadersBuildInfo CheckShadersBuildStore(ShadersBuildInfo shadersBuildInfo) {

            if (shadersBuildInfo == null) {
                string filename = GetStoredDataPath();
                shadersBuildInfo = AssetDatabase.LoadAssetAtPath<ShadersBuildInfo>(filename);
                if (shadersBuildInfo != null) {
                    return shadersBuildInfo;
                }
            }

            // Check if scriptable object exists
            string path = GetStoredDataPath();
            if (!File.Exists(path)) {
                string dir = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dir);
                shadersBuildInfo = ScriptableObject.CreateInstance<ShadersBuildInfo>();
                AssetDatabase.CreateAsset(shadersBuildInfo, path);
                AssetDatabase.SaveAssets();
            }
            return shadersBuildInfo;
        }


        public void OnPostprocessBuild(BuildReport report) {
            SaveResults();
        }

        public int callbackOrder { get { return 0; } }

        static string GetStoredDataPath() {
            // Locate shader control path
            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int k = 0; k < paths.Length; k++) {
                if (paths[k].EndsWith("/ShaderControl/Editor", StringComparison.InvariantCultureIgnoreCase)) {
                    return paths[k] + "/Resources/BuiltShaders.asset";
                }
            }
            return null;
        }

        void SaveResults() {

            SCWindow.SetEditorPrefBool("QUICK_BUILD", false);

            if (shadersBuildInfo != null) {
                shadersBuildInfo.creationDateTicks = DateTime.Now.Ticks;
                EditorUtility.SetDirty(shadersBuildInfo);
                string filename = GetStoredDataPath();
                if (filename == null) {
                    Debug.LogError("Shader Control path not found.");
                } else {
                    AssetDatabase.SaveAssets();
                }
            }
            SCWindow.issueRefresh = 0;
        }

        public void OnProcessShader(
            Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderCompilerData) {

            try {
                bool skipCompilation = false;
                if (SCWindow.GetEditorPrefBool("QUICK_BUILD", false)) {
                    skipCompilation = true;
                }

                if (shadersBuildInfo == null) {
                    string filename = GetStoredDataPath();
                    shadersBuildInfo = AssetDatabase.LoadAssetAtPath<ShadersBuildInfo>(filename);
                    if (shadersBuildInfo == null) {
                        return;
                    }
                }

                ShaderBuildInfo sb = shadersBuildInfo.GetShader(shader.name);
                if (sb == null) {
                    sb = new ShaderBuildInfo();
                    sb.name = shader.name;
                    sb.simpleName = SCShader.GetSimpleName(sb.name);
                    sb.type = snippet.shaderType;
                    string path = AssetDatabase.GetAssetPath(shader);
                    sb.isInternal = string.IsNullOrEmpty(path) || !File.Exists(path);
                    shadersBuildInfo.Add(sb);
                    EditorUtility.SetDirty(shadersBuildInfo);
                } else if (!sb.includeInBuild) {
                    skipCompilation = true;
                }

                int count = shaderCompilerData.Count;
                for (int i = 0; i < count; ++i) {
                    ShaderKeywordSet ks = shaderCompilerData[i].shaderKeywordSet;
                    ShaderKeyword[] shaderKeywords = ks.GetShaderKeywords();

                    // Check if variants are allowed
                    if (shaderKeywords.Length > 0 && sb.variants != null && sb.variants.Count > 0) {
                        bool includedVariant = false;
                        foreach (var variant in sb.variants) {
                            if (variant.Same(shader, shaderKeywords)) {
                                includedVariant = true;
                                break;
                            }
                        }
                        if (!includedVariant) {
                            shaderCompilerData.RemoveAt(i);
                            count--;
                            i--;
                            continue; // for
                        }
                    }

                    // Check if keywords are allowed
                    foreach (ShaderKeyword kw in shaderKeywords) {
#if UNITY_2019_3_OR_NEWER
                    string kname = ShaderKeyword.GetKeywordName(shader, kw);
#elif UNITY_2018_4_OR_NEWER
                        string kname = kw.GetKeywordName();
#else
                        string kname = kw.GetName();
#endif
                        if (string.IsNullOrEmpty(kname)) {
                            continue;
                        }
                        if (!sb.KeywordsIsIncluded(kname)) {
                            shaderCompilerData.RemoveAt(i);
                            count--;
                            i--;
                            break;
                        } else {
                            EditorUtility.SetDirty(shadersBuildInfo);
                        }
                    }
                }

                if (skipCompilation) {
                    shaderCompilerData.Clear();
                    return;
                }
            } catch (Exception ex) {
                Debug.LogWarning("Shader Control detected an error during compilation of one shader: " + ex.ToString());
            }

        }

    }
}