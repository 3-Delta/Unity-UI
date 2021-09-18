/// <summary>
/// Shader Control - (C) Copyright 2016-2020 Ramiro Oliva (Kronnect)
/// </summary>
/// 
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

namespace ShaderControl {

    public partial class SCWindow : EditorWindow {

        const string JSON_NODE_DATA = "JSONnodeData";
        const string JSON_KEYWORD_SCOPE = "m_KeywordScope";

        [Serializable]
        public struct SerializedKeywordData {
            public string typeInfo;
            public string JSONnodeData;
        }

        [Serializable]
        public struct SerializedKeywordProxy {
            public string m_Name;
            public string m_DefaultReferenceName;
            public int m_KeywordScope;
            public int m_KeywordDefinition;
        }

        [Serializable]
        public struct ShaderGraphProxy {
            public SerializedKeywordData[] m_SerializedKeywords;
        }


        void ScanShaderGraphV1(SCShader shader, string shaderContents) { 

            shaderContents = shaderContents.Replace("UnityEditor.ShaderGraph.ShaderKeyword", "ShaderControl.SCWindow.SerializedKeyword");
            ShaderGraphProxy graph = JsonUtility.FromJson<ShaderGraphProxy>(shaderContents);
            
            SCShaderPass currentPass = new SCShaderPass();
            if (graph.m_SerializedKeywords != null) {
                for (int k = 0; k < graph.m_SerializedKeywords.Length; k++) {

                    SerializedKeywordData skw = graph.m_SerializedKeywords[k];
                    if (string.IsNullOrEmpty(skw.JSONnodeData)) continue;

                    SerializedKeywordProxy kw = JsonUtility.FromJson<SerializedKeywordProxy>(skw.JSONnodeData);

                    PragmaType pragmaType = PragmaType.Unknown;
                    if (kw.m_KeywordDefinition == SHADER_GRAPH_KEYWORD_DEFINITION_MULTI_COMPILE && kw.m_KeywordScope == SHADER_GRAPH_KEYWORD_SCOPE_GLOBAL) {
                        pragmaType = PragmaType.MultiCompileGlobal;
                    } else if (kw.m_KeywordDefinition == SHADER_GRAPH_KEYWORD_DEFINITION_MULTI_COMPILE && kw.m_KeywordScope == SHADER_GRAPH_KEYWORD_SCOPE_LOCAL) {
                        pragmaType = PragmaType.MultiCompileLocal;
                    } else if (kw.m_KeywordDefinition == SHADER_GRAPH_KEYWORD_DEFINITION_SHADER_FEATURE && kw.m_KeywordScope == SHADER_GRAPH_KEYWORD_SCOPE_GLOBAL) {
                        pragmaType = PragmaType.FeatureGlobal;
                    } else if (kw.m_KeywordDefinition == SHADER_GRAPH_KEYWORD_DEFINITION_SHADER_FEATURE && kw.m_KeywordScope == SHADER_GRAPH_KEYWORD_SCOPE_LOCAL) {
                        pragmaType = PragmaType.FeatureLocal;
                    }

                    SCKeywordLine keywordLine = new SCKeywordLine();
                    keywordLine.pragmaType = pragmaType;

                    SCKeyword keyword = new SCKeyword(kw.m_DefaultReferenceName, kw.m_Name);
                    keywordLine.Add(keyword);
                    currentPass.Add(keywordLine);
                }
            }
            shader.Add(currentPass);
            shader.UpdateVariantCount();
        }

        void ConvertToLocalGraphV1(SCKeyword keyword, SCShader shader) {
            string contents = File.ReadAllText(shader.path, Encoding.UTF8);
            int i = contents.IndexOf("m_SerializedKeywords");
            if (i < 0) return;
            int j = contents.IndexOf("m_SerializedNodes");
            if (j < 0) j = contents.Length - 1;

            int pos = contents.IndexOf(keyword.name, i);
            bool changed = false;
            if (pos > i && pos < j) {
                int dataBlockPos = contents.LastIndexOf(JSON_NODE_DATA, pos);
                if (dataBlockPos > 0) {
                    int scopePos = contents.IndexOf(JSON_KEYWORD_SCOPE, dataBlockPos);
                    if (scopePos > dataBlockPos && scopePos < j) {
                        scopePos += JSON_KEYWORD_SCOPE.Length + 2;
                        int valuePos = contents.IndexOf("1", scopePos);
                        int safetyPos = contents.IndexOf("\"", scopePos);
                        if (valuePos > scopePos && valuePos < safetyPos && safetyPos > valuePos) {
                            contents = contents.Substring(0, valuePos) + "0" + contents.Substring(valuePos + 1);
                            changed = true;
                        }

                    }
                }
            }
            if (changed) {
                MakeBackup(shader);
                File.WriteAllText(shader.path, contents, Encoding.UTF8);
            }
        }


    }
}