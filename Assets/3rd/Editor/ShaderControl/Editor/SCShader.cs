using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace ShaderControl {

    public enum PragmaType {
        Unknown,
        MultiCompileGlobal,
        MultiCompileLocal,
        FeatureGlobal,
        FeatureLocal
    }

    public class SCShader {

        public string fullName = "";
        public string name = "";
        public string path = "";
        public int GUID;
        public List<SCShaderPass> passes = new List<SCShaderPass>();
        public List<SCKeyword> keywords = new List<SCKeyword>();
        public List<SCMaterial> materials = new List<SCMaterial>();
        public int totalVariantCount, actualBuildVariantCount;
        public int keywordEnabledCount;
        public bool foldout;
        public bool showMaterials;
        public bool pendingChanges;
        public bool editedByShaderControl;
        public bool hasBackup;
        public bool isReadOnly;
        public bool isShaderGraph;
        public int shaderGraphVersion = 1;

        public void Add(SCShaderPass pass) {
            passes.Add(pass);
            UpdateKeywords();
        }

        public void AddKeywordsByName(string[] names) {
            bool changes = false;
            for (int k = 0; k < names.Length; k++) {
                string kwName = names[k];
                if (string.IsNullOrEmpty(kwName)) continue;
                // is it already included?
                int kwCount = keywords.Count;
                bool repeated = false;
                for (int j = 0; j < kwCount; j++) {
                    if (keywords[j].name.Equals(kwName)) {
                        repeated = true;
                        break;
                    }
                }
                if (repeated) continue;
                SCKeyword keyword = new SCKeyword(kwName);
                keywords.Add(keyword);
                changes = true;
            }
            if (changes) {
                keywords.Sort(delegate (SCKeyword k1, SCKeyword k2) { return k1.name.CompareTo(k2.name); });
            }
        }

        public void RemoveKeyword(string name) {
            for (int k = 0; k < keywords.Count; k++) {
                SCKeyword keyword = keywords[k];
                if (keyword.name.Equals(name)) {
                    if (keyword.enabled) keywordEnabledCount--;
                    keywords.Remove(keyword);
                    return;
                }
            }
        }

        public void EnableKeywords() {
            keywords.ForEach((SCKeyword keyword) => keyword.enabled = true);
        }

        public List<string> enabledKeywords {
            get {
                List<string> kk = new List<string>(keywords.Count);
                keywords.ForEach(kw => { if (kw.enabled) kk.Add(kw.name); });
                return kk;
            }
        }

        public bool hasSource {
            get { return path.Length > 0; }
        }

        void UpdateKeywords() {
            passes.ForEach((SCShaderPass pass) => {
                for (int l = 0; l < pass.keywordLines.Count; l++) {
                    SCKeywordLine line = pass.keywordLines[l];
                    for (int k = 0; k < line.keywords.Count; k++) {
                        SCKeyword keyword = line.keywords[k];
                        if (!keywords.Contains(keyword)) {
                            keywords.Add(keyword);
                        }
                    }
                }
            });
        }

        public void UpdateVariantCount() {
            totalVariantCount = 0;
            actualBuildVariantCount = 0;
            passes.ForEach((SCShaderPass pass) => {
                int matCount = materials.Count;
                int passCount = 1;
                int passBuildCount = 1;
                for (int l = 0; l < pass.keywordLines.Count; l++) {
                    SCKeywordLine line = pass.keywordLines[l];
                    int kLineEnabledCount = line.hasUnderscoreVariant ? 1 : 0;
                    int kLineCount = kLineEnabledCount + line.keywords.Count;
                    for (int k = 0; k < line.keywords.Count; k++) {
                        SCKeyword keyword = line.keywords[k];
                        if (keyword.enabled) {
                            // if this is a shader feature, check if there's at least one material using it
                            if (line.pragmaType == PragmaType.FeatureGlobal || line.pragmaType == PragmaType.FeatureLocal) {
                                for (int m = 0; m < matCount; m++) {
                                    if (materials[m].ContainsKeyword(keyword.name)) {
                                        kLineEnabledCount++;
                                        break;
                                    }
                                }
                            } else {
                                kLineEnabledCount++;
                            }
                        }
                    }
                    if (kLineEnabledCount > 0) {
                        passBuildCount *= kLineEnabledCount;
                    }
                    passCount *= kLineCount;
                }
                totalVariantCount += passCount;
                actualBuildVariantCount += passBuildCount;
            });

            keywordEnabledCount = 0;
            int keywordCount = keywords.Count;
            for (int k = 0; k < keywordCount; k++) {
                if (keywords[k].enabled)
                    keywordEnabledCount++;
            }

        }

        public SCKeyword GetKeyword(string name) {
            int kCount = keywords.Count;
            for (int k = 0; k < kCount; k++) {
                SCKeyword keyword = keywords[k];
                if (keyword.name.Equals(name))
                    return keyword;
            }
            return new SCKeyword(name);
        }

        public static string GetSimpleName(string longName) {
            int k = longName.LastIndexOf("/");
            if (k >= 0) {
                return longName.Substring(k + 1);
            }
            return longName;
        }
    }

    public class SCShaderPass {
        public int pass;
        public List<SCKeywordLine> keywordLines = new List<SCKeywordLine>();
        public int keywordCount;

        public void Add(SCKeywordLine keywordLine) {
            keywordLines.Add(keywordLine);
            UpdateKeywordCount();
        }

        public void Add(List<SCKeywordLine> keywordLines) {
            this.keywordLines.AddRange(keywordLines);
            UpdateKeywordCount();
        }

        void UpdateKeywordCount() {
            keywordCount = 0;
            keywordLines.ForEach((SCKeywordLine obj) => keywordCount += obj.keywordCount);
        }

        public void Clear() {
            keywordCount = 0;
            keywordLines.Clear();
        }
    }

    public class SCKeywordLine {

        public const string PRAGMA_MULTICOMPILE_GLOBAL = "#pragma multi_compile ";
        public const string PRAGMA_MULTICOMPILE_LOCAL = "#pragma multi_compile_local ";
        public const string PRAGMA_FEATURE_GLOBAL = "#pragma shader_feature ";
        public const string PRAGMA_FEATURE_LOCAL = "#pragma shader_feature_local ";


        public List<SCKeyword> keywords = new List<SCKeyword>();
        public bool hasUnderscoreVariant;
        public PragmaType pragmaType;

        public string GetPragma() {
            switch (pragmaType) {
                case PragmaType.MultiCompileGlobal: return PRAGMA_MULTICOMPILE_GLOBAL;
                case PragmaType.MultiCompileLocal: return PRAGMA_MULTICOMPILE_LOCAL;
                case PragmaType.FeatureGlobal: return PRAGMA_FEATURE_GLOBAL;
                case PragmaType.FeatureLocal: return PRAGMA_FEATURE_LOCAL;
            }
            return "";
        }

        public SCKeyword GetKeyword(string name) {
            int kc = keywords.Count;
            for (int k = 0; k < kc; k++) {
                SCKeyword keyword = keywords[k];
                if (keyword.name.Equals(name)) {
                    return keyword;
                }
            }
            return null;
        }

        public void Add(SCKeyword keyword) {
            if (GetKeyword(keyword.name) != null)
                return;
            // ignore underscore keywords
            bool goodKeyword = false;
            for (int k = 0; k < keyword.name.Length; k++) {
                if (keyword.name[k] != '_') {
                    goodKeyword = true;
                    break;
                }
            }
            keyword.isMultiCompile = pragmaType == PragmaType.MultiCompileGlobal || pragmaType == PragmaType.MultiCompileLocal;
            if (goodKeyword) {
                keyword.isGlobal = pragmaType != PragmaType.FeatureLocal && pragmaType != PragmaType.MultiCompileLocal && pragmaType != PragmaType.Unknown;
                keyword.verboseName = keyword.name + " (";
                if (!string.IsNullOrEmpty(keyword.shaderGraphName)) {
                    keyword.verboseName += keyword.shaderGraphName + ", ";
                }
                keyword.verboseName += keyword.isMultiCompile ? "multi_compile" : "shader_feature";
                keyword.verboseName += keyword.isGlobal ? ", global)" : ", local)";
                keywords.Add(keyword);
            } else {
                keyword.isUnderscoreKeyword = true;
                hasUnderscoreVariant = true;
            }
        }

        public void DisableKeywords() {
            keywords.ForEach((SCKeyword obj) => obj.enabled = false);
        }

        public void Clear() {
            keywords.Clear();
        }

        public int keywordCount {
            get {
                return keywords.Count;
            }
        }

        public int keywordsEnabledCount {
            get {
                int kCount = keywords.Count;
                int enabledCount = 0;
                for (int k = 0; k < kCount; k++) {
                    if (keywords[k].enabled)
                        enabledCount++;
                }
                return enabledCount;
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < keywords.Count; k++) {
                if (k > 0)
                    sb.Append(" ");
                sb.Append(keywords[k].name);
            }
            return sb.ToString();
        }

    }

    public class SCKeyword {
        public string name, verboseName;
        public string shaderGraphName;
        public string shaderGraphObjectId;
        public bool enabled;
        public bool isUnderscoreKeyword;
        public bool isGlobal = true;
        public bool isMultiCompile = true;

        public SCKeyword(string name, string shaderGraphName = null, string shaderGraphObjectId = null) {
            this.name = name;
            this.verboseName = name;
            this.shaderGraphName = shaderGraphName;
            this.shaderGraphObjectId = shaderGraphObjectId;
            enabled = true;
        }

        public override bool Equals(object obj) {
            if (obj is SCKeyword) {
                SCKeyword other = (SCKeyword)obj;
                return name.Equals(other.name);
            }
            return false;
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }

        public override string ToString() {
            return name;
        }

    }

}