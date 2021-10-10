using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using Unity.DebugWatch.WatchContext;

namespace Unity.DebugWatch.Editor
{

    [CustomEditor(typeof(DebugWatchRegistryComponent))]
    //[CanEditMultipleObjects]
    public class LookAtPointEditor : UnityEditor.Editor
    {
        SerializedProperty WatchRegistryProp;

        void OnEnable()
        {
            WatchRegistryProp = serializedObject.FindProperty("WatchRegistry");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (GUILayout.Button("Open watch list"))
            {

                var window = UnityEditor.EditorWindow.CreateWindow<DebugWatcher>();
                if (WatchRegistryProp.objectReferenceValue == null)
                {
                    WatchRegistryProp.objectReferenceValue = WatchRegistryContainer.CreateInstance<WatchRegistryContainer>();
                }
                var ctn = WatchRegistryProp.objectReferenceValue as WatchRegistryContainer;
                window.Init(ctn.WatchRegistry);
                window.Show();
            }
            EditorGUILayout.PropertyField(WatchRegistryProp);
            serializedObject.ApplyModifiedProperties();
        }
    }

    public class AutoComplete : EditorWindow
    {

    }
    public class DebugWatcher : EditorWindow
    {
        [MenuItem("Window/Analysis/Debug Watch", false)]
        static void CreateDefaultWatcher()
        {
            
            var window = UnityEditor.EditorWindow.CreateWindow<DebugWatcher>("Debug Watch (Prototype Version)");
            window.Init(WatchRegistry.Instance);
            window.Show();
        }
        [MenuItem("Window/Analysis/Debug Watch : Remove All Watches", false)]
        static void CreateDefaultWatcherClean()
        {
            WatchRegistry.Instance.Watches.Clear();
        }

        class DebuggerStyles
        {
            public GUIStyle ToolbarStyle;
            public GUIStyle SearchFieldStyle;
            public GUIStyle SearchFieldCancelButton;
            public GUIStyle SearchFieldCancelButtonEmpty;
            public int SearchFieldWidth;
            public GUIStyle ToolbarDropdownStyle;
            public GUIStyle ToolbarButtonStyle;
            public GUIStyle LabelStyle;
            public GUIStyle BoxStyle;
            public GUIStyle ToolbarLabelStyle;
            public GUIStyle LineEven = "OL EntryBackEven";
            public GUIStyle LineOdd = "OL EntryBackOdd";
            public GUIStyle background = "OL Box";
        }

        private static DebuggerStyles Styles;

        void InitStyles()
        {
            if (Styles == null)
            {
                Styles = new DebuggerStyles();
                Styles.ToolbarStyle = "Toolbar";
                Styles.SearchFieldStyle = "ToolbarSeachTextField";
                Styles.SearchFieldCancelButton = "ToolbarSeachCancelButton";
                Styles.SearchFieldCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
                Styles.SearchFieldWidth = 100;
                Styles.ToolbarDropdownStyle = "ToolbarDropDown";
                Styles.ToolbarButtonStyle = "toolbarbutton";
                Styles.LabelStyle = new GUIStyle(EditorStyles.label)
                {
                    margin = EditorStyles.boldLabel.margin,
                    richText = true
                };
                Styles.BoxStyle = new GUIStyle(GUI.skin.box)
                {
                    margin = new RectOffset(),
                    padding = new RectOffset(1, 0, 1, 0),
                    overflow = new RectOffset(0, 1, 0, 1)
                };
                
                Styles.ToolbarLabelStyle = new GUIStyle(Styles.ToolbarButtonStyle)
                {
                    richText = true,
                    alignment = TextAnchor.MiddleLeft
                };
                var styleState = Styles.ToolbarLabelStyle.normal;
                styleState.background = null;
                styleState.scaledBackgrounds = null;
            }
        }

        public WatchRegistry WatchRegistry;
        [SerializeField] private TreeViewState watchListState = new TreeViewState();
        WatchListView watchList;
        public void Init(WatchRegistry watchRegistry)
        {
            WatchRegistry = watchRegistry;
            watchList = new WatchListView(watchListState, WatchRegistry);
        }
        private int watchCountLastUpdate = -1;
        private readonly RepaintLimiter repaintLimiter = new RepaintLimiter();
        private void Update()
        {
            if (WatchRegistry == null) return;
            if (watchCountLastUpdate != WatchRegistry.Watches.Count
                || repaintLimiter.IsRepaintTime()
                )
            {
                Repaint();
            }
        }
        bool OnGUIProcessInput()
        {

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Delete)
            {
                var sel = watchList.GetSelection();
                if (sel != null)
                {
                    var indexToRemove = sel.ToArray().OrderByDescending(x => x);
                    foreach (var i in indexToRemove)
                    {
                        WatchRegistry.TryRemoveWatch(WatchRegistry.Watches[i]);
                    }
                }
                watchList.SelectNothing();
                Repaint();
                return true;
            }
            return false;
        }
        string addWatchText = ""; //"World[\"Editor World\"].Entity[9].Components[2].Value";
        bool hasFocusOnWatchField = false;
        AutoComplete AutoCompleteWind;


        bool showAutoComplete = false;
        Rect rectAutoComplete = Rect.zero;
        List<ContextMemberInfo> AutoCompleteSuggestions = new List<ContextMemberInfo>();
        string AutoCompleteValidString;
        IWatchContext AutoCompleteDeepestContext;
        WatchContext.GlobalContext AutoCompleteGlobalContext = new WatchContext.GlobalContext();
        void UpdateAutoComplete()
        {
            AutoCompleteSuggestions.Clear();
            RangeInt cursor = addWatchText.Range();
            if (AutoCompleteGlobalContext.TryParseDeepest(addWatchText, ref cursor, out AutoCompleteDeepestContext))
            {
                if (AutoCompleteDeepestContext != AutoCompleteGlobalContext)
                {
                    AutoCompleteSuggestions.Add(new ContextMemberInfo(ContextFieldInfo.Make(".."), ContextTypeInfo.Make(AutoCompleteDeepestContext.GetType())));
                }
                AutoCompleteValidString = addWatchText.Substring(0, cursor.start);
                AutoCompleteDeepestContext.VisitAllMembers((ContextMemberInfo info) =>
                {
                    AutoCompleteSuggestions.Add(info);
                    return true; 
                });
                //ParserUtils.TryParseAt(addWatchText, ref cursor, ".");
                //if (ctx.TryCreateWatch(addWatchText, cursor, out var w))
                //{
                //    WatchRegistry.AddWatch(w);
                //}
            }
        }
        bool UpdateAfterAutoCompleteSelect = false;
        void OnGUIAutoComplete()
        {


            if (showAutoComplete)
            {
                //Styles.LineEven.Draw
                //EditorGUI.DrawRect(rectAutoComplete, Color.black);
                //GUI.Box(rectAutoComplete, "", Styles.BoxStyle);
                if (Event.current.type == EventType.Repaint) Styles.background.Draw(rectAutoComplete, GUIContent.none, false, false, false, false);

                int lineH = EditorStyles.standardFont.lineHeight;
                int totH = AutoCompleteSuggestions.Count * lineH;

                AutoCompletePos = GUI.BeginScrollView(rectAutoComplete, AutoCompletePos, new Rect(0, 0, rectAutoComplete.width - 20, totH), false, true);
                var w = rectAutoComplete.width - 20;
                Rect curRect = new Rect(0, 0, w * 0.5f, lineH);
                Rect curRectType = new Rect(curRect.xMax, 0, w * 0.5f, lineH);
                bool even = true;
                //int i = 0;

                foreach (var s in AutoCompleteSuggestions)
                {
                    if (Event.current.type == EventType.Repaint)
                    {

                        //if (even) 
                        //    Styles.LineEven.Draw(curRect, GUIContent.none, false, false, false, false);
                        //else 
                        //    Styles.LineOdd.Draw(curRect, GUIContent.none, false, false, false, false);
                    }

                    if (curRect.Contains(Event.current.mousePosition))
                    {
                        EditorGUI.DrawRect(curRect, GUI.skin.settings.selectionColor);
                        if (Event.current.type == EventType.MouseDown)
                        {
                            if (s.FieldInfo.Name == "..")
                            {
                                var cursor = addWatchText.Range();
                                if (ParserUtils.TryExtractLastPathPart(addWatchText, ref cursor, out var lastPart))
                                {
                                    addWatchText = addWatchText.Substring(cursor);
                                }
                            }
                            else
                            {
                                // suggestion selected
                                if (AutoCompleteDeepestContext == AutoCompleteGlobalContext)
                                {
                                    addWatchText = s.AsLocal();
                                }
                                else
                                {
                                    addWatchText = s.AddTo(AutoCompleteValidString);
                                }
                            }
                            //UpdateAutoComplete();
                            showAutoComplete = false;
                            Debug.Log("Selected " + s);
                            UpdateAfterAutoCompleteSelect = true;
                            //GUIUtility.keyboardControl = controlId;
                        }
                    }
                    else
                    {
                    }

                    GUI.Label(curRect, s.FieldInfo.Name, Styles.LabelStyle);
                    string sInfo = "";
                    //if (s.FieldInfo.IsOperator) sInfo += "[Op]";
                    //if (s.TypeInfo.IsProperty) sInfo += "[Prop]";
                    //if (s.TypeInfo.IsCollection) sInfo += "[Coll[" + s.TypeInfo.CollectionLenth + "]]";
                    //if (s.TypeInfo.IsPropertyContainer) sInfo += "[PropCtnr]";
                    if (s.TypeInfo.ValueType != null)
                    {
                        sInfo += " " + s.TypeInfo.ValueType.Name;
                        if (s.TypeInfo.IsCollection) sInfo += " [" + s.TypeInfo.CollectionLenth + "]";
                    }
                    else
                    {
                        sInfo += " unknown type";
                    }
                    GUI.Label(curRectType, sInfo, Styles.LabelStyle);


                    curRect.y += lineH;
                    curRectType.y += lineH;
                    even = !even;
                    //++i;
                }
                GUI.EndScrollView();

            }
        }
        void OnGUI()
        {
            repaintLimiter.RecordRepaint();

            if (WatchRegistry == null || watchList == null)
            {
                Init(DebugWatch.WatchRegistry.Instance);
            }

            watchCountLastUpdate = WatchRegistry.Watches.Count;

            if (OnGUIProcessInput()) return;

            InitStyles();
            GUILayout.BeginVertical(Styles.BoxStyle);
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("WatchName");
            
            //int controlId = GUIUtility.GetControlID(FocusType.Passive) + 1;

            string addWatchTextNew = EditorGUILayout.TextField(addWatchText);
            if (addWatchTextNew != addWatchText)
            {
                addWatchText = addWatchTextNew;
                UpdateAutoComplete();
            }
            var type = typeof(EditorGUIUtility);
            var field = type.GetField("s_LastControlID", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var controlId = (int)field.GetValue(null);

            //GUIUtility.GetControlID(FocusType.Keyboard,
            //GUIContent content = new GUIContent("asdasdasd");
            //addWatchText = EditorGUILayout.TextField(content, addWatchText);
            //int controlId = GUIUtility.GetControlID(content, FocusType.Keyboard);

            //GUILayout.TextArea()
            //GUI.Button(new Rect(), content);
            //addWatchText = GUILayout.TextField(addWatchText, content);
            //GUIUtility.hotControl
            //if (GUI.GetNameOfFocusedControl() == "WatchName")
            if (UpdateAfterAutoCompleteSelect)
            {
                UpdateAfterAutoCompleteSelect = false;
                GUI.FocusControl("WatchName");
                
                GUIUtility.keyboardControl = controlId;
                //needs to be redefined after setting the FocusControl        
                //var controlIdPassive = GUIUtility.GetControlID(FocusType.Passive);
                //var controlIdKeyboard = GUIUtility.GetControlID(FocusType.Keyboard);
                ////var controlIdNative = GUIUtility.GetControlID(FocusType.Native);
                //var tePassive = GUIUtility.GetStateObject(typeof(TextEditor), controlIdPassive); // GUIUtility.keyboardControl);
                //var teKeyboard = GUIUtility.GetStateObject(typeof(TextEditor), controlIdKeyboard); // GUIUtility.keyboardControl);
                var teo = GUIUtility.GetStateObject(typeof(TextEditor), controlId); // GUIUtility.keyboardControl);
                var te = (TextEditor)teo;
                if (te != null)
                {
                    //if(te.controlID != controlId)
                    //{
                    //    te.controlID = controlId;
                    //}
                    te.SelectNone();
                    te.MoveLineEnd();
                }

            }
            if (Event.current.type == EventType.Repaint)
            {
                if (GUIUtility.keyboardControl == controlId)
                {
                    showAutoComplete = true;
                    rectAutoComplete = GUILayoutUtility.GetLastRect();
                    rectAutoComplete.y += rectAutoComplete.height;
                    rectAutoComplete.height = 100;

                    UpdateAutoComplete();
                    //if (!hasFocusOnWatchField)
                    //{
                    //    hasFocusOnWatchField = true;
                    //    var rect = GUILayoutUtility.GetLastRect();

                    //    rect = GUIUtility.GUIToScreenRect(rect);
                    //    rect.y += rect.height;
                    //    rect.height = 200;
                    //    Debug.Log("rect = " + rect);
                    //    if (AutoCompleteWind == null)
                    //    {
                    //        AutoCompleteWind = UnityEditor.EditorWindow.CreateWindow<AutoComplete>();
                    //        AutoCompleteWind.ShowUtility();
                    //        //AutoCompleteWind.ShowAsDropDown(rect, new Vector2(100, 100));
                    //        //AutoCompleteWind = UnityEditor.EditorWindow.GetWindowWithRect<AutoComplete>(rect, true);
                    //    }
                    //    AutoCompleteWind.position = rect;

                    //    //GUI.Window(0, rect, DoMyWindow, "Allllll");
                    //    //rect.y += rect.height;
                    //    //rect.height = 200;
                    //    //string[] l = new string[] { "allo", "allo2" };
                    //    //EditorGUI.Popup(rect, 0, l);
                    //}
                }
                else
                {
                    if (AutoCompleteWind != null) AutoCompleteWind.Close();

                    AutoCompleteWind = null;
                    hasFocusOnWatchField = false;
                    //Debug.Log("keyboardControl = " + GUIUtility.keyboardControl + ", controlId = " + controlId);
                }
            }
            //GUIUtility.
            //GUIUtility.hotControl
            if (GUILayout.Button("Add"))
            {
                if (AutoCompleteGlobalContext.TryCreateWatch(addWatchText, addWatchText.Range(), out var w))
                {

                    WatchRegistry.AddWatch(w);
                }
                else
                {
                    Debug.LogError($"Could not create watch at '{addWatchText}'");
                }
                //if (glb.TryParseDeepest(addWatchText, ref cursor, out var ctx))
                //{
                //    ParserUtils.TryParseAt(addWatchText, ref cursor, ".");
                //    if(ctx.TryCreateWatch(addWatchText, cursor, out var w))
                //    {
                //        WatchRegistry.AddWatch(w);
                //    }
                //}
            }

            if(Event.current.type == EventType.MouseDown)
            {
                OnGUIAutoComplete();
            }

            GUILayout.EndHorizontal();
            watchList.OnGUI(GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)));
            GUILayout.EndVertical();
            //OnGUIAutoComplete();

            if (Event.current.type != EventType.MouseDown)
            {
                OnGUIAutoComplete();
            }

            //if (showAutoComplete)
            //{
            //    //Styles.LineEven.Draw
            //    //EditorGUI.DrawRect(rectAutoComplete, Color.black);
            //    //GUI.Box(rectAutoComplete, "", Styles.BoxStyle);
            //    if (Event.current.type == EventType.Repaint) Styles.background.Draw(rectAutoComplete, GUIContent.none, false, false, false, false);
            //    
            //    int lineH = EditorStyles.standardFont.lineHeight;
            //    int totH = AutoCompleteSuggestions.Count * lineH;
            //
            //    AutoCompletePos = GUI.BeginScrollView(rectAutoComplete, AutoCompletePos, new Rect(0, 0, rectAutoComplete.width-20, totH), false, true);
            //    var w = rectAutoComplete.width - 20;
            //    Rect curRect = new Rect(0, 0, w*0.5f, lineH);
            //    Rect curRectType = new Rect(curRect.xMax, 0, w * 0.5f, lineH);
            //    bool even = true;
            //    //int i = 0;
            //
            //    foreach (var s in AutoCompleteSuggestions)
            //    {
            //        if (Event.current.type == EventType.Repaint)
            //        {
            //            
            //            //if (even) 
            //            //    Styles.LineEven.Draw(curRect, GUIContent.none, false, false, false, false);
            //            //else 
            //            //    Styles.LineOdd.Draw(curRect, GUIContent.none, false, false, false, false);
            //        }
            //
            //        if (curRect.Contains(Event.current.mousePosition))
            //        {
            //            EditorGUI.DrawRect(curRect, GUI.skin.settings.selectionColor);
            //            if (Event.current.type == EventType.MouseDown)
            //            {
            //                if (s.FieldInfo.Name == "..")
            //                {
            //                    var cursor = addWatchText.Range();
            //                    if (ParserUtils.TryExtractLastPathPart(addWatchText, ref cursor, out var lastPart))
            //                    {
            //                        addWatchText = addWatchText.Substring(cursor);
            //                    }
            //                }
            //                else
            //                {
            //                    // suggestion selected
            //                    if (AutoCompleteDeepestContext == AutoCompleteGlobalContext)
            //                    {
            //                        addWatchText = s.AsLocal();
            //                    }
            //                    else
            //                    {
            //                        addWatchText = s.AddTo(AutoCompleteValidString);
            //                    }
            //                }
            //                //UpdateAutoComplete();
            //                showAutoComplete = false;
            //                Debug.Log("Selected " + s);
            //                UpdateAfterAutoCompleteSelect = true;
            //                //GUIUtility.keyboardControl = controlId;
            //            }
            //        }
            //        else
            //        {
            //        }
            //        
            //        GUI.Label(curRect, s.FieldInfo.Name, Styles.LabelStyle);
            //        string sInfo ="";
            //        //if (s.FieldInfo.IsOperator) sInfo += "[Op]";
            //        //if (s.TypeInfo.IsProperty) sInfo += "[Prop]";
            //        //if (s.TypeInfo.IsCollection) sInfo += "[Coll[" + s.TypeInfo.CollectionLenth + "]]";
            //        //if (s.TypeInfo.IsPropertyContainer) sInfo += "[PropCtnr]";
            //        if (s.TypeInfo.ValueType != null)
            //        {
            //            sInfo += " " + s.TypeInfo.ValueType.Name;
            //            if (s.TypeInfo.IsCollection) sInfo += " [" + s.TypeInfo.CollectionLenth + "]";
            //        } else
            //        {
            //            sInfo += " unknown type";
            //        }
            //        GUI.Label(curRectType, sInfo, Styles.LabelStyle);
            //        
            //        
            //        curRect.y += lineH;
            //        curRectType.y += lineH;
            //        even = !even;
            //        //++i;
            //    }
            //    GUI.EndScrollView();
            //    
            //}
            if (Event.current.type == EventType.MouseDown)
            {
                if (!rectAutoComplete.Contains(Event.current.mousePosition))
                {
                    showAutoComplete = false;
                }
            }
        }
        Vector2 AutoCompletePos = Vector2.zero;
    }
}
