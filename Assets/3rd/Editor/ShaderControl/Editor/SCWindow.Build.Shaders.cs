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

        class BuildKeywordView {
            public string keyword;
            public List<ShaderBuildInfo> shaders;
            public bool foldout, isInternal;
        }

        ShadersBuildInfo shadersBuildInfo;
        int totalBuildKeywords, totalBuildIncludedKeywords, totalBuildShadersWithKeywords, totalBuildShaders, totalBuildIncludedShaders, totalBuildIncludedShadersWithKeywords;
        int maxBuildKeywordsCountFound = 0;
        bool nextQuickBuild;
        Dictionary<string, List<ShaderBuildInfo>> uniqueBuildKeywords, uniqueIncludedBuildKeywords;
        public static int issueRefresh;
        List<BuildKeywordView> buildKeywordView;

        void RefreshBuildStats(bool quick) {
            issueRefresh = 1;
            nextQuickBuild = GetEditorPrefBool("QUICK_BUILD", false);
            shadersBuildInfo = ShaderDebugBuildProcessor.CheckShadersBuildStore(shadersBuildInfo);
            totalBuildKeywords = totalBuildIncludedKeywords = totalBuildShadersWithKeywords = totalBuildShaders = totalBuildIncludedShaders = totalBuildIncludedShadersWithKeywords = 0;
            shadersBuildInfo = Resources.Load<ShadersBuildInfo>("BuiltShaders");
            if (shadersBuildInfo == null || shadersBuildInfo.shaders == null) return;

            if (uniqueBuildKeywords == null) {
                uniqueBuildKeywords = new Dictionary<string, List<ShaderBuildInfo>>();
            } else {
                uniqueBuildKeywords.Clear();
            }
            if (uniqueIncludedBuildKeywords == null) {
                uniqueIncludedBuildKeywords = new Dictionary<string, List<ShaderBuildInfo>>();
            } else {
                uniqueIncludedBuildKeywords.Clear();
            }

            int count = shadersBuildInfo.shaders.Count;
            totalBuildShaders = 0;
            maxBuildKeywordsCountFound = 0;

            for (int k = 0; k < count; k++) {
                ShaderBuildInfo sb = shadersBuildInfo.shaders[k];
                int kwCount = sb.keywords != null ? sb.keywords.Count : 0;
                if (shadersBuildInfo.sortType != BuildViewSortType.Keyword) {
                    if (minimumKeywordCount > 0 && kwCount < minimumKeywordCount) continue;
                    if (!string.IsNullOrEmpty(keywordFilter) && !sb.ContainsKeyword(keywordFilter, false)) continue;
                }
                if (shadersBuildInfo.viewType == BuildViewShaderOption.ProjectShaders && sb.isInternal) continue;
                if (shadersBuildInfo.viewType == BuildViewShaderOption.UnityInternalShaders && !sb.isInternal) continue;
                if (!string.IsNullOrEmpty(buildShaderNameFilter) && sb.name.IndexOf(buildShaderNameFilter, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
                totalBuildShaders++;

                // Check shaders exist
                if (!quick && Shader.Find(sb.name) == null) {
                    shadersBuildInfo.shaders.RemoveAt(k);
                    k--;
                    totalBuildShaders--;
                    count--;
                    continue;
                }
                if (sb.includeInBuild) {
                    totalBuildIncludedShaders++;
                }
                if (kwCount > 0) {
                    if (kwCount > maxBuildKeywordsCountFound) {
                        maxBuildKeywordsCountFound = kwCount;
                    }
                    //totalBuildKeywords += kwCount;
                    totalBuildShadersWithKeywords++;
                    if (sb.includeInBuild) {
                        totalBuildIncludedShadersWithKeywords++;
                        for (int j = 0; j < kwCount; j++) {
                            List<ShaderBuildInfo> shaderList;
                            if (!uniqueBuildKeywords.TryGetValue(sb.keywords[j].keyword, out shaderList)) {
                                totalBuildKeywords++;
                                shaderList = new List<ShaderBuildInfo>();
                                uniqueBuildKeywords[sb.keywords[j].keyword] = shaderList;
                            }
                            shaderList.Add(sb);
                            if (sb.keywords[j].includeInBuild) {
                                List<ShaderBuildInfo> includedList;
                                if (!uniqueIncludedBuildKeywords.TryGetValue(sb.keywords[j].keyword, out includedList)) {
                                    totalBuildIncludedKeywords++;
                                    includedList = new List<ShaderBuildInfo>();
                                    uniqueIncludedBuildKeywords[sb.keywords[j].keyword] = includedList;
                                }
                                includedList.Add(sb);
                            }
                        }
                    }
                }
            }

            if (buildKeywordView == null) {
                buildKeywordView = new List<BuildKeywordView>();
            } else {
                buildKeywordView.Clear();
            }
            foreach (KeyValuePair<string, List<ShaderBuildInfo>> kvp in uniqueBuildKeywords) {
                BuildKeywordView kv = new BuildKeywordView { keyword = kvp.Key, shaders = kvp.Value };
                buildKeywordView.Add(kv);
            }
            buildKeywordView.Sort(delegate (BuildKeywordView x, BuildKeywordView y) {
                return y.shaders.Count.CompareTo(x.shaders.Count);
            });
            // Annotate which keywords are used in project
            int bkwCount = buildKeywordView.Count;
            for (int k = 0; k < bkwCount; k++) {
                BuildKeywordView bkv = buildKeywordView[k];
                bool isInternal = true;
                int shadersCount = bkv.shaders.Count;
                for (int j = 0; j < shadersCount; j++) {
                    if (!bkv.shaders[j].isInternal) {
                        isInternal = false;
                        break;
                    }
                }
                bkv.isInternal = isInternal;
            }

            UpdateProjectStats();
        }

        void ClearBuildData() {
            shadersBuildInfo = ShaderDebugBuildProcessor.CheckShadersBuildStore(shadersBuildInfo);
            if (shadersBuildInfo != null) {
                shadersBuildInfo.Clear();
            }
        }

        void BuildUpdateShaderKeywordsState(SCShader shader) {
            if (shader == null || shader.keywords == null) return;
            if (shadersBuildInfo == null) return;
            int shadersCount = shadersBuildInfo.shaders.Count;
            for (int s = 0; s < shadersCount; s++) {
                ShaderBuildInfo sb = shadersBuildInfo.shaders[s];
                if (sb != null && sb.name.Equals(shader.fullName)) {
                    for (int k = 0; k < shader.keywords.Count; k++) {
                        SCKeyword keyword = shader.keywords[k];
                        sb.ToggleIncludeKeyword(keyword.name, keyword.enabled);
                    }
                }
            }
            shadersBuildInfo.requiresBuild = true;
        }


    }

}