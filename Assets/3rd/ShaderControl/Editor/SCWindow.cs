/// <summary>
/// Shader Control - (C) Copyright 2016-2018 Ramiro Oliva (Kronnect)
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

        enum ViewMode {
            Build,
            Project
        }

        const string EDITOR_PREFIX = "ShaderControl_";

        ViewMode viewMode;
        string[] viewModeTexts = { "Build View", "Project View" };

        #region Unity events

        void OnEnable() {
            // Setup styles
            Color backColor = EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.7f, 0.7f, 0.7f);
            Texture2D _blackTexture;
            _blackTexture = MakeTex(4, 4, backColor);
            _blackTexture.hideFlags = HideFlags.DontSave;
            blackStyle = new GUIStyle();
            blackStyle.normal.background = _blackTexture;
            firstTime = !GetEditorPrefBool("FIRST_TIME", true);
            RefreshBuildStats(false);
        }

        Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static void ShowWindow() {
            GetWindow<SCWindow>(false, "Shader Control", true);
        }

        void OnGUI() {

            // Detect changes
            if (issueRefresh == 0) {
                shadersBuildInfo.Refresh();
                RefreshBuildStats(false);
            }

            if (commentStyle == null) {
                commentStyle = new GUIStyle(EditorStyles.label);
            }
            commentStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.62f, 0.76f, 0.9f) : new Color(0.32f, 0.36f, 0.42f);
            if (disabledStyle == null) {
                disabledStyle = new GUIStyle(EditorStyles.label);
            }
            disabledStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.8f) : new Color(0.32f, 0.32f, 0.32f);
            if (foldoutRTF == null) {
                foldoutRTF = new GUIStyle(EditorStyles.foldout);
            }
            foldoutRTF.richText = true;
            if (foldoutBold == null) {
                foldoutBold = new GUIStyle(EditorStyles.foldout);
                foldoutBold.fontStyle = FontStyle.Bold;
            }
            if (foldoutNormal == null) {
                foldoutNormal = new GUIStyle(EditorStyles.foldout);
            }
            if (foldoutDim == null) {
                foldoutDim = new GUIStyle(EditorStyles.foldout);
                foldoutDim.fontStyle = FontStyle.Italic;
            }
            if (matIcon == null) {
                matIcon = EditorGUIUtility.IconContent("PreMatSphere");
                if (matIcon == null)
                    matIcon = new GUIContent();
            }
            if (shaderIcon == null) {
                shaderIcon = EditorGUIUtility.IconContent("Shader Icon");
                if (shaderIcon == null)
                    matIcon = new GUIContent();
            }
            if (titleStyle == null) {
                titleStyle = new GUIStyle(GUI.skin.box);
                titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                titleStyle.richText = true;
                titleStyle.alignment = TextAnchor.MiddleLeft;
            }
            if (foldoutNormal == null) {
                foldoutNormal = new GUIStyle(EditorStyles.foldout);
            }
            if (labelNormal == null) {
                labelNormal = new GUIStyle(EditorStyles.label);
            }
            if (labelDim == null) {
                labelDim = new GUIStyle(EditorStyles.label);
                labelDim.fontStyle = FontStyle.Italic;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(blackStyle);

            ViewMode prevMode = viewMode;
            viewMode = (ViewMode)GUILayout.SelectionGrid((int)viewMode, viewModeTexts, 2);
            if (viewMode == ViewMode.Project) {
                DrawProjectGUI();
            } else {
                if (prevMode != viewMode) {
                    RefreshBuildStats(false);
                }
                DrawBuildGUI();
            }

        }

        void OnInspectorUpdate() {
            if (firstTime)
                ShowHelpWindowFirstTime();

        }

        void ShowHelpWindowFirstTime() {
            if (firstTime) {
                SetEditorPrefBool("FIRST_TIME", true);
                firstTime = false;
            }
            EditorUtility.DisplayDialog("About Shader Control", "Shader Control is a powerful tool that scans your project or build and lists any shader that could be adding unnecessary code to your build, increasing its size and compilation time.\n\nThe 'Project View' lists all shaders existing in your project. It's useful to locate, remove or disable keywords in order to reduce the total keywords used in your project.\n\nThe 'Build View' shows all the shaders and keywords compiled in the build (including shaders in the project plus all internal Unity shaders). It allows you to specify which shaders or keywords must be excluded from the next build.\n\nImportant: a disabled or excluded keyword will deactivate a shader feature. Make sure the shaders and keywords you're disabling or excluding from your build are not being used! You may need to review the shader documentation, its source code for comments or contact the author concerning the keywords that can be safely disabled before applying any change.\n\nAlthough Shader Control will make a backup copy of any shader being modified, it's recommended to make a backup copy of your project before applying automatic changes to your shaders from the Project View (any change in the Build View won't affect your files).\n\nPlease contact us on kronnect.com support forum for any question or suggestions. Thanks!", "Ok");
        }

        void ShowHelpWindowBuildView() {
            EditorUtility.DisplayDialog("About the Build View", "In this tab you decide which shaders (and keywords) can be compiled in next build.\n\nThe data shown here is collected from the last build since Shader Control was installed.\n\nThis tab lists ALL shaders and keywords used during the build, including Unity internal shaders which cannot be modified by Shader Control (for example to remove a keyword) but still can be excluded here from the next build, saving time and disk space.", "Ok");
        }

        void ShowHelpWindowProjectView() {
            EditorUtility.DisplayDialog("About the Project View", "This tab lets you browse all shaders and keywords available in the files of your project. It does NOT show Unity internal shaders or keywords, unless they're referenced by a material.\n\nUnity has a limit of 256 total keywords (including internal shaders and keywords which cannot be modified). If you have exceeded this limit, you can:\n\n1.- Locate and remove shaders or keywords from the list below. To disable a keyword, uncheck it and press Save. Shader Control will modify the shader file for you!\n2.- Convert a keyword to local. This operation only can be used safely with keywords of type 'shader_feature'. Select 'Sort by Keyword' in the filters below and Shader Control will show a button 'Convert To Local' next to the keywords that can be converted.\n\nImportant: the total keyword count is the value shown in the Build View which includes Unity internal shaders and private keywords.", "Ok");
        }


        public static void SetEditorPrefBool(string param, bool value) {
            EditorPrefs.SetBool(EDITOR_PREFIX + param, value);
        }

        public static bool GetEditorPrefBool(string param, bool defaultValue) {
            return EditorPrefs.GetBool(EDITOR_PREFIX + param, defaultValue);
        }

        #endregion

        public static void PingShader(string name) {
            Shader shader = Shader.Find(name);
            if (shader != null) {
                Selection.activeObject = shader;
                EditorGUIUtility.PingObject(shader);
            }

        }
    }

}