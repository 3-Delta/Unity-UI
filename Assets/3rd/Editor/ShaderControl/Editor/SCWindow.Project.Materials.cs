/// <summary>
/// Shader Control - (C) Copyright 2016-2020 Ramiro Oliva (Kronnect)
/// </summary>
/// 
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ShaderControl {

    public partial class SCWindow : EditorWindow {

        List<SCMaterial> projectMaterials;

        #region Materials handling

        void CleanMaterials(SCShader shader) {
            // Updates any material using this shader
            Shader shad = (Shader)AssetDatabase.LoadAssetAtPath<Shader>(shader.path);
            if (shad != null) {
                bool requiresSave = false;
                string[] matGUIDs = AssetDatabase.FindAssets("t:Material");
                foreach (string matGUID in matGUIDs) {
                    string matPath = AssetDatabase.GUIDToAssetPath(matGUID);
                    Material mat = (Material)AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    if (mat != null && mat.shader.name.Equals(shad.name)) {
                        foreach (SCKeyword keyword in shader.keywords) {
                            foreach (string matKeyword in mat.shaderKeywords) {
                                if (matKeyword.Equals(keyword.name)) {
                                    if (!keyword.enabled && mat.IsKeywordEnabled(keyword.name)) {
                                        mat.DisableKeyword(keyword.name);
                                        EditorUtility.SetDirty(mat);
                                        requiresSave = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                if (requiresSave) {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        void CleanAllMaterials() {
            if (!EditorUtility.DisplayDialog("Clean All Materials", "This option will scan all materials and will prune any disabled keywords. This option is provided to ensure no materials are referencing a disabled shader keyword.\n\nRemember: to disable keywords, first expand any shader from the list and uncheck the unwanted keywords (press 'Save' to modify the shader file and to clean any existing material that uses that specific shader).\n\nDo you want to continue?", "Yes", "Cancel")) {
                return;
            }
            try {
                for (int k = 0; k < shaders.Count; k++) {
                    CleanMaterials(shaders[k]);
                }
                ScanProject();
                Debug.Log("Cleaning finished.");
            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while cleaning materials: " + ex.Message);
            }
        }

        void PruneMaterials(string keywordName) {
            try {
                bool requiresSave = false;
                for (int s = 0; s < shaders.Count; s++) {
                    SCShader shader = shaders[s];
                    int materialCount = shader.materials.Count;
                    for (int k = 0; k < materialCount; k++) {
                        SCMaterial material = shader.materials[k];
                        if (material.ContainsKeyword(keywordName)) {
                            Material theMaterial = AssetDatabase.LoadAssetAtPath<Material>(shader.materials[k].path);
                            if (theMaterial == null)
                                continue;
                            theMaterial.DisableKeyword(keywordName);
                            EditorUtility.SetDirty(theMaterial);
                            material.RemoveKeyword(keywordName);
                            shader.RemoveKeyword(keywordName);
                            requiresSave = true;
                        }
                    }
                }
                if (requiresSave) {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            } catch (Exception ex) {
                Debug.Log("Unexpected exception caught while pruning materials: " + ex.Message);
            }

        }


        #endregion
    }

}