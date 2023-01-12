using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderControl {

    [Serializable, ExecuteInEditMode]
    public class ShadersBuildInfo : ScriptableObject {

        [HideInInspector]
        public long creationDateTicks;

        [NonSerialized]
        public string creationDateString;

        public List<ShaderBuildInfo> shaders;

        [NonSerialized]
        public bool requiresBuild;

        [HideInInspector]
        public BuildViewSortType sortType = BuildViewSortType.ShaderKeywordCount;

        [HideInInspector]
        public BuildViewShaderOption viewType = BuildViewShaderOption.AllShaders;

        Dictionary<string, ShaderBuildInfo> shadersDict;

        private void OnEnable() {
            Refresh();
        }

        public void Clear() {
            creationDateString = "";
            creationDateTicks = 0;
            if (shaders != null) {
                shaders.Clear();
            }
            if (shadersDict != null) {
                shadersDict.Clear();
            }
        }

        public void Refresh() {
            creationDateString = new DateTime(creationDateTicks, DateTimeKind.Local).ToString();
            if (shadersDict == null) {
                shadersDict = new Dictionary<string, ShaderBuildInfo>();
            }
            if (shaders == null) {
                shaders = new List<ShaderBuildInfo>();
            }

            shadersDict.Clear();
            int count = shaders.Count;
            for (int k = 0; k < count; k++) {
                ShaderBuildInfo sb = shaders[k];
                shadersDict[sb.name] = sb;
            }
            Resort();
        }

        public void Add(ShaderBuildInfo sb) {
            if (shaders == null || shadersDict == null) {
                Refresh();
            }
            shaders.Add(sb);
            shadersDict[sb.name] = sb;
        }

        public bool ShaderIsExcluded(string shader) {
            ShaderBuildInfo sb = GetShader(shader);
            return sb != null ? !sb.includeInBuild : false;
        }

        public ShaderBuildInfo GetShader(string shader) {
            if (shadersDict == null) return null;
            ShaderBuildInfo sb;
            shadersDict.TryGetValue(shader, out sb);
            return sb;
        }

        public void Resort() {
            if (shaders == null) return;
            switch (sortType) {
                case BuildViewSortType.ShaderName:
                    shaders.Sort((t1, t2) => t1.name.CompareTo(t2.name));
                    break;
                case BuildViewSortType.ShaderKeywordCount:
                    shaders.Sort((t1, t2) => {
                        int kw1 = t1.keywords != null ? t1.keywords.Count : 0;
                        int kw2 = t2.keywords != null ? t2.keywords.Count : 0;
                        if (kw1 < kw2) {
                            return 1;
                        } else if (kw1 > kw2) {
                            return -1;
                        } else {
                            return 0;
                        }
                    }
                    );
                    break;
            }
        }

    }

    [Serializable]
    public class KeywordBuildSettings {
        public string keyword;
        public bool includeInBuild = true;
        public bool includeInVariant;
    }

    [Serializable]
    public class KeywordSet {
        public List<string> keywords = new List<string>();

        public bool Same(Shader shader, ShaderKeyword[] shaderKeywords) {
            if (shaderKeywords == null) return false;
            List<string> knames = new List<string>();
            for (int k = 0; k < shaderKeywords.Length; k++) {
                ShaderKeyword kw = shaderKeywords[k];
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
                knames.Add(kname);
            }
            return (keywords.All(knames.Contains) && keywords.Count == knames.Count);
        }
    }

    [Serializable]
    public class ShaderBuildInfo {
        public string name, simpleName;
        public bool isExpanded;
        public bool includeInBuild = true;
        public ShaderType type;
        public List<KeywordBuildSettings> keywords;
        public bool isInternal;
        public List<KeywordSet> variants = new List<KeywordSet>();

        public bool ContainsKeyword(string keyword, bool exact) {
            if (keywords == null) return false;
            int count = keywords.Count;
            for (int k = 0; k < count; k++) {
                if (keywords[k].keyword.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                    if (exact) {
                        return keywords[k].keyword == keyword;
                    } else {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ToggleIncludeKeyword(string keyword, bool includeInBuild) {
            if (keywords == null) return;
            int count = keywords.Count;
            for (int k = 0; k < count; k++) {
                if (keywords[k].keyword == keyword) {
                    keywords[k].includeInBuild = includeInBuild;
                }
            }
        }


        public bool KeywordsIsIncluded(string keyword) {
            if (keywords != null) {
                int count = keywords.Count;
                for (int k = 0; k < count; k++) {
                    KeywordBuildSettings kw = keywords[k];
                    if (kw.keyword == keyword) {
                        return kw.includeInBuild;
                    }
                }
            }
            AddKeyword(keyword);
            return true;
        }


        public void AddKeyword(string keyword) {
            if (keywords == null) {
                keywords = new List<KeywordBuildSettings>();
            }
            KeywordBuildSettings kb = new KeywordBuildSettings();
            kb.keyword = keyword;
            keywords.Add(kb);
        }
    }


}

