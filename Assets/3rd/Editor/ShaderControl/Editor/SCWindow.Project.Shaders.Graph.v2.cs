/// <summary>
/// Shader Control - (C) Copyright 2016-2020 Ramiro Oliva (Kronnect)
/// </summary>
/// 
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ShaderControl {

    public partial class SCWindow : EditorWindow {

        const string JSON_NODE_DATA_V2 = "JSONnodeData";
        const string JSON_KEYWORD_SCOPE_V2 = "m_KeywordScope";

        [Serializable]
        public struct ShaderGraphChunkDataV2 {
            public string m_Type;
            public string m_ObjectId;
            public string m_Name;
            public string m_DefaultReferenceName;
            public int m_KeywordScope;
            public int m_KeywordDefinition;
        }


        List<ShaderGraphChunkDataV2> graphKeywords;

        void ScanShaderGraphV2(SCShader shader, string shaderContents) {

            // Only extract info from first JSON chunk
            ExtractJSONChunks(shaderContents);

            SCShaderPass currentPass = new SCShaderPass();
            for (int k = 0; k < graphKeywords.Count; k++) {

                ShaderGraphChunkDataV2 kw = graphKeywords[k];

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

                SCKeyword keyword = new SCKeyword(kw.m_DefaultReferenceName, kw.m_Name, kw.m_ObjectId);
                keywordLine.Add(keyword);
                currentPass.Add(keywordLine);
            }

            shader.Add(currentPass);
            shader.UpdateVariantCount();
        }


        void ConvertToLocalGraphV2(SCKeyword keyword, SCShader shader) {
            string contents = File.ReadAllText(shader.path, Encoding.UTF8);

            int pos = contents.IndexOf("\"m_ObjectId\": \"" + keyword.shaderGraphObjectId);
            bool changed = false;
            if (pos > 0) {
                int scopePos = contents.IndexOf(JSON_KEYWORD_SCOPE, pos);
                if (scopePos > pos) {
                    scopePos += JSON_KEYWORD_SCOPE.Length + 2;
                    int valuePos = contents.IndexOf("1", scopePos);
                    int safetyPos = contents.IndexOf("\"", scopePos);
                    if (valuePos > scopePos && valuePos < safetyPos && safetyPos > valuePos) {
                        contents = contents.Substring(0, valuePos) + "0" + contents.Substring(valuePos + 1);
                        changed = true;
                    }

                }
            }

            if (changed) {
                MakeBackup(shader);
                File.WriteAllText(shader.path, contents, Encoding.UTF8);
            }
        }

        static readonly char[] jsonClosures = { '{', '}' };

        void ExtractJSONChunks(string json) {
            if (graphKeywords == null) {
                graphKeywords = new List<ShaderGraphChunkDataV2>();
            } else {
                graphKeywords.Clear();
            }
            int count = 0;
            int startIndex = 0, lastIndex = 0;
            do {
                int nextClosure = json.IndexOfAny(jsonClosures, lastIndex);
                if (nextClosure < 0) break;
                if (json[nextClosure] == '{') count++; else if (json[nextClosure] == '}') count--;
                lastIndex = nextClosure + 1;

                if (count == 0) {
                    string jsonChunk = json.Substring(startIndex, lastIndex - startIndex);
                    ShaderGraphChunkDataV2 chunk = JsonUtility.FromJson<ShaderGraphChunkDataV2>(jsonChunk);
                    if (chunk.m_Type.Equals("UnityEditor.ShaderGraph.ShaderKeyword")) {
                        graphKeywords.Add(chunk);
                    }
                    startIndex = lastIndex;
                }
            } while (true);
        }


    }
}