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

        
        void ScanShaderNonGraph(SCShader shader) {

            // Reads shader
            string[] shaderLines = File.ReadAllLines(shader.path);
            string[] separator = new string[] { " " };
            SCShaderPass currentPass = new SCShaderPass();
            SCShaderPass basePass = null;
            int pragmaControl = 0;
            int pass = -1;
            bool blockComment = false;
            SCKeywordLine keywordLine = new SCKeywordLine();
            for (int k = 0; k < shaderLines.Length; k++) {
                string line = shaderLines[k].Trim();
                if (line.Length == 0)
                    continue;

                int lineCommentIndex = line.IndexOf("//");
                int blocCommentIndex = line.IndexOf("/*");
                int endCommentIndex = line.IndexOf("*/");
                if (blocCommentIndex > 0 && (lineCommentIndex > blocCommentIndex || lineCommentIndex < 0)) {
                    blockComment = true;
                }
                if (endCommentIndex > blocCommentIndex && (lineCommentIndex > endCommentIndex || lineCommentIndex < 0)) {
                    blockComment = false;
                }
                if (blockComment)
                    continue;

                string lineUPPER = line.ToUpper();
                if (lineUPPER.Equals("PASS") || lineUPPER.StartsWith("PASS ")) {
                    if (pass >= 0) {
                        currentPass.pass = pass;
                        if (basePass != null)
                            currentPass.Add(basePass.keywordLines);
                        shader.Add(currentPass);
                    } else if (currentPass.keywordCount > 0) {
                        basePass = currentPass;
                    }
                    currentPass = new SCShaderPass();
                    pass++;
                    continue;
                }
                int j = line.IndexOf(PRAGMA_COMMENT_MARK);
                if (j >= 0) {
                    pragmaControl = 1;
                } else {
                    j = line.IndexOf(PRAGMA_DISABLED_MARK);
                    if (j >= 0)
                        pragmaControl = 3;
                }
                if (lineCommentIndex == 0 && pragmaControl != 1 && pragmaControl != 3) {
                    continue; // do not process lines commented by user
                }

                PragmaType pragmaType = PragmaType.Unknown;
                int offset = 0;
                j = line.IndexOf(SCKeywordLine.PRAGMA_MULTICOMPILE_GLOBAL);
                if (j >= 0) {
                    pragmaType = PragmaType.MultiCompileGlobal;
                    offset = SCKeywordLine.PRAGMA_MULTICOMPILE_GLOBAL.Length;
                } else {
                    j = line.IndexOf(SCKeywordLine.PRAGMA_FEATURE_GLOBAL);
                    if (j >= 0) {
                        pragmaType = PragmaType.FeatureGlobal;
                        offset = SCKeywordLine.PRAGMA_FEATURE_GLOBAL.Length;
                    } else {
                        j = line.IndexOf(SCKeywordLine.PRAGMA_MULTICOMPILE_LOCAL);
                        if (j >= 0) {
                            pragmaType = PragmaType.MultiCompileLocal;
                            offset = SCKeywordLine.PRAGMA_MULTICOMPILE_LOCAL.Length;
                        } else {
                            j = line.IndexOf(SCKeywordLine.PRAGMA_FEATURE_LOCAL);
                            if (j >= 0) {
                                pragmaType = PragmaType.FeatureLocal;
                                offset = SCKeywordLine.PRAGMA_FEATURE_LOCAL.Length;
                            }
                        }
                    }
                }
                if (j >= 0) {
                    if (pragmaControl != 2) {
                        keywordLine = new SCKeywordLine();
                    }
                    keywordLine.pragmaType = pragmaType;
                    // exclude potential comments inside the #pragma line
                    int lastStringPos = line.IndexOf("//", j + offset);
                    if (lastStringPos < 0) {
                        lastStringPos = line.Length;
                    }
                    int length = lastStringPos - j - offset;
                    string[] kk = line.Substring(j + offset, length).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // Sanitize keywords
                    for (int i = 0; i < kk.Length; i++) {
                        kk[i] = kk[i].Trim();
                    }
                    // Act on keywords
                    switch (pragmaControl) {
                        case 1: // Edited by Shader Control line
                            shader.editedByShaderControl = true;
                            // Add original keywords to current line
                            for (int s = 0; s < kk.Length; s++) {
                                keywordLine.Add(shader.GetKeyword(kk[s]));
                            }
                            pragmaControl = 2;
                            break;
                        case 2:
                            // check enabled keywords
                            keywordLine.DisableKeywords();
                            for (int s = 0; s < kk.Length; s++) {
                                SCKeyword keyword = keywordLine.GetKeyword(kk[s]);
                                if (keyword != null)
                                    keyword.enabled = true;
                            }
                            currentPass.Add(keywordLine);
                            pragmaControl = 0;
                            break;
                        case 3: // disabled by Shader Control line
                            shader.editedByShaderControl = true;
                            // Add original keywords to current line
                            for (int s = 0; s < kk.Length; s++) {
                                SCKeyword keyword = shader.GetKeyword(kk[s]);
                                keyword.enabled = false;
                                keywordLine.Add(keyword);
                            }
                            currentPass.Add(keywordLine);
                            pragmaControl = 0;
                            break;
                        case 0:
                            // Add keywords to current line
                            for (int s = 0; s < kk.Length; s++) {
                                keywordLine.Add(shader.GetKeyword(kk[s]));
                            }
                            currentPass.Add(keywordLine);
                            break;
                    }
                }
            }
            currentPass.pass = Mathf.Max(pass, 0);
            if (basePass != null)
                currentPass.Add(basePass.keywordLines);
            shader.Add(currentPass);
            shader.UpdateVariantCount();
        }


        void UpdateShaderNonGraph(SCShader shader) {
            // Reads and updates shader from disk
            string[] shaderLines = File.ReadAllLines(shader.path);
            string[] separator = new string[] { " " };
            StringBuilder sb = new StringBuilder();
            int pragmaControl = 0;
            shader.editedByShaderControl = false;
            SCKeywordLine keywordLine = new SCKeywordLine();
            bool blockComment = false;
            for (int k = 0; k < shaderLines.Length; k++) {

                int lineCommentIndex = shaderLines[k].IndexOf("//");
                int blocCommentIndex = shaderLines[k].IndexOf("/*");
                int endCommentIndex = shaderLines[k].IndexOf("*/");
                if (blocCommentIndex > 0 && (lineCommentIndex > blocCommentIndex || lineCommentIndex < 0)) {
                    blockComment = true;
                }
                if (endCommentIndex > blocCommentIndex && (lineCommentIndex > endCommentIndex || lineCommentIndex < 0)) {
                    blockComment = false;
                }

                int j = -1;
                PragmaType pragmaType = PragmaType.Unknown;
                if (!blockComment) {
                    j = shaderLines[k].IndexOf(PRAGMA_COMMENT_MARK);
                    if (j >= 0) {
                        pragmaControl = 1;
                    }
                    j = shaderLines[k].IndexOf(SCKeywordLine.PRAGMA_MULTICOMPILE_GLOBAL);
                    if (j >= 0) {
                        pragmaType = PragmaType.MultiCompileGlobal;
                    } else {
                        j = shaderLines[k].IndexOf(SCKeywordLine.PRAGMA_FEATURE_GLOBAL);
                        if (j >= 0) {
                            pragmaType = PragmaType.FeatureGlobal;
                        } else {
                            j = shaderLines[k].IndexOf(SCKeywordLine.PRAGMA_MULTICOMPILE_LOCAL);
                            if (j >= 0) {
                                pragmaType = PragmaType.MultiCompileLocal;
                            } else {
                                j = shaderLines[k].IndexOf(SCKeywordLine.PRAGMA_FEATURE_LOCAL);
                                if (j >= 0) {
                                    pragmaType = PragmaType.FeatureLocal;
                                }

                            }
                        }
                    }
                    if (pragmaControl != 1 && lineCommentIndex == 0 && shaderLines[k].IndexOf(PRAGMA_DISABLED_MARK) < 0) {
                        // do not process a commented line
                        j = -1;
                    }
                }
                if (j >= 0) {
                    if (pragmaControl != 2) {
                        keywordLine.Clear();
                    }
                    keywordLine.pragmaType = pragmaType;
                    j = shaderLines[k].IndexOf(' ', j + 20) + 1; // first space after pragma declaration
                    if (j >= shaderLines[k].Length) continue;
                    // exclude potential comments inside the #pragma line
                    int lastStringPos = shaderLines[k].IndexOf("//", j);
                    if (lastStringPos < 0) {
                        lastStringPos = shaderLines[k].Length;
                    }
                    int length = lastStringPos - j;
                    string[] kk = shaderLines[k].Substring(j, length).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // Sanitize keywords
                    for (int i = 0; i < kk.Length; i++) {
                        kk[i] = kk[i].Trim();
                    }
                    // Act on keywords
                    switch (pragmaControl) {
                        case 1:
                            // Read original keywords
                            for (int s = 0; s < kk.Length; s++) {
                                SCKeyword keyword = shader.GetKeyword(kk[s]);
                                keywordLine.Add(keyword);
                            }
                            pragmaControl = 2;
                            break;
                        case 0:
                        case 2:
                            if (pragmaControl == 0) {
                                for (int s = 0; s < kk.Length; s++) {
                                    SCKeyword keyword = shader.GetKeyword(kk[s]);
                                    keywordLine.Add(keyword);
                                }
                            }
                            int kCount = keywordLine.keywordCount;
                            int kEnabledCount = keywordLine.keywordsEnabledCount;
                            if (kEnabledCount < kCount) {
                                // write original keywords
                                if (kEnabledCount == 0) {
                                    sb.Append(PRAGMA_DISABLED_MARK);
                                } else {
                                    sb.Append(PRAGMA_COMMENT_MARK);
                                }
                                shader.editedByShaderControl = true;
                                sb.Append(keywordLine.GetPragma());
                                if (keywordLine.hasUnderscoreVariant)
                                    sb.Append(PRAGMA_UNDERSCORE);
                                for (int s = 0; s < kCount; s++) {
                                    SCKeyword keyword = keywordLine.keywords[s];
                                    sb.Append(keyword.name);
                                    if (s < kCount - 1)
                                        sb.Append(" ");
                                }
                                sb.AppendLine();
                            }

                            if (kEnabledCount > 0) {
                                // Write actual keywords
                                sb.Append(keywordLine.GetPragma());
                                if (keywordLine.hasUnderscoreVariant)
                                    sb.Append(PRAGMA_UNDERSCORE);
                                for (int s = 0; s < kCount; s++) {
                                    SCKeyword keyword = keywordLine.keywords[s];
                                    if (keyword.enabled) {
                                        sb.Append(keyword.name);
                                        if (s < kCount - 1)
                                            sb.Append(" ");
                                    }
                                }
                                sb.AppendLine();
                            }
                            pragmaControl = 0;
                            break;
                    }
                } else {
                    sb.AppendLine(shaderLines[k]);
                }
            }

            // Writes modified shader
            File.WriteAllText(shader.path, sb.ToString());
            AssetDatabase.Refresh();
        }

        void ConvertToLocalNonGraph(SCKeyword keyword, SCShader shader) {

            string path = shader.path;
            if (!File.Exists(path)) return;
            string[] lines = File.ReadAllLines(path);
            bool changed = false;
            for (int k = 0; k < lines.Length; k++) {
                // Just convert to local shader_features for now since multi_compile global keywords can be nabled using the Shader global API
                if (lines[k].IndexOf(SCKeywordLine.PRAGMA_FEATURE_GLOBAL, StringComparison.InvariantCultureIgnoreCase) >= 0 && lines[k].IndexOf(keyword.name, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                    lines[k] = lines[k].Replace(SCKeywordLine.PRAGMA_FEATURE_GLOBAL, SCKeywordLine.PRAGMA_FEATURE_LOCAL);
                    lines[k] = lines[k].Replace(SCKeywordLine.PRAGMA_FEATURE_GLOBAL.ToUpper(), SCKeywordLine.PRAGMA_FEATURE_LOCAL);
                    changed = true;
                }
            }
            if (changed) {
                MakeBackup(shader);
                File.WriteAllLines(path, lines, Encoding.UTF8);
            }
        }
    }

}