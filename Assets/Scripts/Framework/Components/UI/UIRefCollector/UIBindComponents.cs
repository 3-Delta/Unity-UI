using System;
using System.Collections.Generic;
using UnityEngine;

#region Editor内容
#if UNITY_EDITOR
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;

[CustomPropertyDrawer(typeof(UIBindComponents.BindComponent))]
public class BindItemDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        using (new EditorGUI.PropertyScope(position, label, property)) {
            EditorGUIUtility.labelWidth = 120;
            position.height = EditorGUIUtility.singleLineHeight;

            Rect componentRect = new Rect(position) {
                x = position.x + 30,
                width = 130
            };
            var component = property.FindPropertyRelative("component");
            EditorGUI.PropertyField(componentRect, component, GUIContent.none);

            Rect listenRect = new Rect(componentRect) {
                x = componentRect.x + 135,
                width = 30
            };
            var listen = property.FindPropertyRelative("toListen");
            EditorGUI.PropertyField(listenRect, listen, GUIContent.none);

            Rect nameRect = new Rect(listenRect) {
                x = listenRect.x + 20,
                width = 130
            };
            var name = property.FindPropertyRelative("name");
            name.stringValue = EditorGUI.TextField(nameRect, "", name.stringValue);
        }
    }
}

[CustomEditor(typeof(UIBindComponents))]
public class UIBindComponentsInspector : Editor {
    private UIBindComponents owner;

    private SerializedProperty csharpFieldStyle;
    private SerializedProperty codeStyle;
    private ReorderableList reorderableList;

    private void OnEnable() {
        owner = target as UIBindComponents;

        codeStyle = serializedObject.FindProperty("codeStyle");
        csharpFieldStyle = serializedObject.FindProperty("csharpFieldStyle");

        var prop = serializedObject.FindProperty("bindComponents");
        reorderableList = new ReorderableList(serializedObject, prop);
        reorderableList.elementHeight = 20;
        reorderableList.drawElementCallback = (rect, index, active, focused) => {
            var element = prop.GetArrayElementAtIndex(index);

            Rect itemRect = new Rect(rect) {
                x = rect.x,
                width = 20
            };
            EditorGUI.LabelField(itemRect, string.Format("[{0}]", index.ToString()));

            rect.height -= 4;
            rect.y += 2;
            EditorGUI.PropertyField(rect, element);
        };

        reorderableList.onSelectCallback += rlist => { GUI.backgroundColor = Color.blue; };

        reorderableList.drawHeaderCallback = rect => {
            var oldColor = GUI.color;
            GUI.color = Color.green;
            EditorGUI.LabelField(rect, string.Format("[--> Index | {0} | {1} | {2} <--]", /*prop.displayName, */"Component", "Listen", "Name"));
            GUI.color = oldColor;
        };
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(codeStyle);
        if (codeStyle.enumValueIndex == (int)UIBindComponents.ECodeStyle.CSharp) {
            EditorGUILayout.PropertyField(csharpFieldStyle);
        }

        reorderableList.DoLayoutList();

        if (GUILayout.Button("CHECK")) {
            owner.Check();
        }

        if (GUILayout.Button("COPY")) {
            if (owner.Check()) {
                string code = owner.Copy();
                GUIUtility.systemCopyBuffer = code;
                Debug.LogError(code);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion

[DisallowMultipleComponent]
public class UIBindComponents : MonoBehaviour {
    [Serializable]
    public class BindComponent {
        public Component component;
#if UNITY_EDITOR
        // 将name只在editor序列化，否则会占用运行时内存
        public string name;
        public bool toListen = false;

        public string componentType => type?.ToString();

        public Type type {
            get {
                if (component != null) {
                    return component.GetType();
                }

                return null;
            }
        }

        public string GetComponentPath(Component end) {
            return GetPath(component, end);
        }
#endif
    }

    public List<BindComponent> bindComponents = new List<BindComponent>();

#if UNITY_EDITOR
    public enum ECodeStyle {
        CSharp,
        Lua,
    }

    public enum ECSharpFieldStyle {
        Field,
        Property,
    }

    [SerializeField] private ECodeStyle codeStyle = ECodeStyle.CSharp;
    [SerializeField] private ECSharpFieldStyle csharpFieldStyle = ECSharpFieldStyle.Property;
    public static readonly string TAB = "    ";

    // tuple形式，方便后续动态的add，进行拓展
    public static readonly Dictionary<Type, ValueTuple<string, string>> csharpListenDescs = new Dictionary<Type, ValueTuple<string, string>>() {
        {
            typeof(Button), new ValueTuple<string, string>("void OnBtnClicked{0}();",
                "this.{0}.onClick.AddListener(listener.OnBtnClicked{1});")
        }, {
            typeof(Toggle), new ValueTuple<string, string>("void OnValueChanged{0}(bool flag);",
                "this.{0}.onValueChanged.AddListener(listener.OnValueChanged{1});")
        }, {
            typeof(Slider), new ValueTuple<string, string>("void OnValueChanged{0}(float currentValue);",
                "this.{0}.onValueChanged.AddListener(listener.OnValueChanged{1});")
        },
    };

    private string LuaCopy() {
        return "暂未实现";
    }

    private string CSharpCopy() {
        StringBuilder sb = new StringBuilder();

        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            sb.AppendFormat("// [{0}] Path: \"{1}\"", i.ToString(), item.GetComponentPath(this));
            sb.AppendLine();
            sb.AppendFormat("public {0} {1}", item.componentType, item.name);

            if (csharpFieldStyle == ECSharpFieldStyle.Property) {
                sb.AppendLine(" { get; private set; } = null;");
            }
            else if (csharpFieldStyle == ECSharpFieldStyle.Field) {
                sb.AppendLine(" = null;");
            }
        }

        sb.AppendLine();
        sb.AppendLine("public void Find(Transform transform) {");
        sb.Append(TAB);
        sb.AppendLine("var binds = transform.GetComponent<UIBindComponents>();");
        sb.Append(TAB);
        sb.AppendLine(@"if (binds == null) {
        return;
    }");

        sb.AppendLine();
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            sb.Append(TAB);
            sb.AppendFormat("this.{0} = binds.Find<{1}>({2});", item.name, item.componentType, i.ToString());
            sb.AppendLine();
        }

        sb.AppendLine();
        sb.Append(TAB);
        sb.AppendLine("this.FindByPath(transform);");

        sb.AppendLine("}");

        sb.AppendLine();
        sb.AppendLine("// 后续想不热更prefab,只热更脚本的形式获取组件,再次函数内部添加查找逻辑即可");
        sb.AppendLine("public void FindByPath(Transform transform) {");
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            sb.Append(TAB);
            var path = item.GetComponentPath(this);
            if (path == null) {
                sb.AppendFormat("// this.{0} = transform.GetComponent<{1}>();", item.name, item.componentType);
            }
            else {
                sb.AppendFormat("// this.{0} = transform.Find(\"{1}\").GetComponent<{2}>();", item.name, path, item.componentType);
            }

            sb.AppendLine();
        }

        sb.AppendLine("}");

        sb.AppendLine();
        sb.AppendLine(@"public interface IListener {");
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            if (item.component != null && item.toListen) {
                var type = item.type;
                if (type != null && csharpListenDescs.TryGetValue(type, out var desc)) {
                    sb.Append(TAB);
                    sb.AppendFormat(desc.Item1, item.name);
                    sb.AppendLine();
                }
            }
        }

        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendLine("public void Listen(IListener listener, bool toListen = true) {");
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            if (item.component != null && item.toListen) {
                var type = item.type;
                if (type != null && csharpListenDescs.TryGetValue(type, out var desc)) {
                    sb.Append(TAB);
                    sb.AppendFormat(desc.Item2, item.name, item.name);
                    sb.AppendLine();
                }
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public string Copy() {
        if (codeStyle == ECodeStyle.CSharp) {
            return CSharpCopy();
        }
        else if (codeStyle == ECodeStyle.Lua) {
            return LuaCopy();
        }

        return "暂未实现";
    }

    // 重名检测
    // 命名合理性检测
    // 组件null检测
    public bool Check() {
        bool rlt = true;
        HashSet<string> hashset = new HashSet<string>();
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var name = bindComponents[i].name;
            if (string.IsNullOrEmpty(name)) {
                Debug.LogErrorFormat("index: {0} has empty name", i.ToString());
                rlt = false;
            }
            else {
                name = name.Trim();
                // https://stackoverflow.com/questions/6372318/c-sharp-string-starts-with-a-number-regex
                if (Regex.IsMatch(name, @"^\d")) {
                    Debug.LogErrorFormat("index: {0} start with number", i.ToString());
                }
                else {
                    var component = bindComponents[i].component;
                    if (component == null) {
                        Debug.LogErrorFormat("index: {0} component is null", i.ToString());
                        rlt = false;
                    }
                    else {
                        if (!hashset.Contains(name)) {
                            hashset.Add(name);
                        }
                        else {
                            Debug.LogErrorFormat("index: {0} has same name with already item", i.ToString());
                            rlt = false;
                        }
                    }
                }
            }
        }

        return rlt;
    }
#endif

    public T Find<T>(int index) where T : Component {
        if (index < 0 || index >= bindComponents.Count) {
            Debug.LogErrorFormat("Index: {0} is out of range", index.ToString());
            return null;
        }

        T component = bindComponents[index].component as T;
        if (component == null) {
            Debug.LogErrorFormat("Index: {0} has invalid component {1}", index.ToString(), typeof(T));
            return null;
        }

        return component;
    }

    public static string GetPath(Component component, Component end = null) {
        if (component == null) {
            return null;
        }

        string totalPath = null;
        List<string> paths = new List<string>();
        if (end == null) {
            var cp = component;
            while (cp.transform.parent != null) {
                paths.Add(cp.transform.name);
                cp = cp.transform.parent;
            }
        }
        else {
            var cp = component;
            while (cp.gameObject != end.gameObject) {
                paths.Add(cp.transform.name);
                cp = cp.transform.parent;
            }
        }

        if (paths.Count > 1) {
            paths.Reverse();
            totalPath = string.Join("/", paths);
        }
        else if (paths.Count > 0) {
            totalPath = paths[0];
        }

        return totalPath;
    }
}
