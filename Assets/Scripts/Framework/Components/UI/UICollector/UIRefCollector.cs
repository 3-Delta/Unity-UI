using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;

[CustomPropertyDrawer(typeof(UIRefCollector.BindItem))]
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

[CustomEditor(typeof(UIRefCollector))]
public class UIRefCollectorInspector : Editor {
    private UIRefCollector owner;

    private SerializedProperty propertyStyle;
    private SerializedProperty codeStype;
    private ReorderableList reorderableList;

    private void OnEnable() {
        owner = target as UIRefCollector;
        propertyStyle = serializedObject.FindProperty("propertyStyle");
        codeStype = serializedObject.FindProperty("codeStype");

        var prop = serializedObject.FindProperty("bindList");
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

        reorderableList.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, string.Format("{0}  [-> {1} | {2} | {3} <-]", prop.displayName, "Component", "ToListen", "Name")); };
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(propertyStyle);
        EditorGUILayout.PropertyField(codeStype);
        reorderableList.DoLayoutList();

        if (GUILayout.Button("CHECK")) {
            owner.Check();
        }

        if (GUILayout.Button("COPY")) {
            string code = owner.Copy();
            GUIUtility.systemCopyBuffer = code;
            Debug.LogError(code);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

[DisallowMultipleComponent]
public class UIRefCollector : MonoBehaviour {
    [Serializable]
    public class BindItem {
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

    public List<BindItem> bindList = new List<BindItem>();

#if UNITY_EDITOR
    public enum ECodeStyle {
        CSharp,
        Lua,
    }

    [SerializeField] private ECodeStyle codeStype = ECodeStyle.CSharp;
    [SerializeField] private bool propertyStyle = true;
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

    public string Copy() {
        if (!Check()) {
            return null;
        }

        StringBuilder sb = new StringBuilder();

        if (codeStype == ECodeStyle.CSharp) {
            sb.AppendLine("public GameObject gameObject { get; private set; } = null;");
            sb.AppendLine("public Transform transform { get; private set; } = null;");
            sb.AppendLine();

            for (int i = 0, length = bindList.Count; i < length; ++i) {
                var item = bindList[i];
                sb.AppendFormat("// [{0}] Path: \"{1}\"", i.ToString(), item.GetComponentPath(this));
                sb.AppendLine();
                sb.AppendFormat("public {0} {1}", item.componentType, item.name);

                if (propertyStyle) {
                    sb.AppendLine(" { get; private set; } = null;");
                }
                else {
                    sb.AppendLine(" = null;");
                }
            }

            sb.AppendLine();
            sb.AppendLine("public void Bind(Transform transform) {");

            sb.Append(TAB);
            sb.AppendLine("this.transform = transform;");
            sb.Append(TAB);
            sb.AppendLine(@"this.gameObject = transform.gameObject;
}");

            sb.AppendLine();
            sb.AppendLine("// 后续想不热更prefab,只通过代码查找组件的时候，写另外一个partial Find即可");
            sb.AppendLine("public partial void Find() {");
            sb.Append(TAB);
            sb.AppendLine("var refCollector = transform.GetComponent<UIRefCollector>();");
            sb.Append(TAB);
            sb.AppendLine(@"if (refCollector == null) {
        return;
    }");

            sb.AppendLine();
            for (int i = 0, length = bindList.Count; i < length; ++i) {
                var item = bindList[i];
                sb.Append(TAB);
                sb.AppendFormat("this.{0} = refCollector.GetComponent<{1}>({2});", item.name, item.componentType, i.ToString());
                sb.AppendLine();
            }

            sb.AppendLine("}");

            sb.AppendLine();
            sb.AppendLine("public partial void Find() {");
            for (int i = 0, length = bindList.Count; i < length; ++i) {
                var item = bindList[i];
                sb.Append(TAB);
                var path = item.GetComponentPath(this);
                if (path == null) {
                    sb.AppendFormat("// this.{0} = this.transform.GetComponent<{1}>();", item.name, item.componentType);
                }
                else {
                    sb.AppendFormat("// this.{0} = this.transform.Find(\"{1}\").GetComponent<{2}>();", item.name, path, item.componentType);
                }

                sb.AppendLine();
            }

            sb.AppendLine("}");

            sb.AppendLine();
            sb.AppendLine(@"public interface IListener {");
            for (int i = 0, length = bindList.Count; i < length; ++i) {
                var item = bindList[i];
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
            for (int i = 0, length = bindList.Count; i < length; ++i) {
                var item = bindList[i];
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
        }
        else if (codeStype == ECodeStyle.Lua) { }

        return sb.ToString();
    }

    public bool Check() {
        bool rlt = true;
        HashSet<string> hashset = new HashSet<string>();
        for (int i = 0, length = bindList.Count; i < length; ++i) {
            var name = bindList[i].name;
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
                    var component = bindList[i].component;
                    if (component == null) {
                        Debug.LogErrorFormat("index: {0} component is null", i.ToString());
                        rlt = false;
                    }
                    else {
                        if (!hashset.Contains(name)) {
                            hashset.Add(name);
                        }
                        else {
                            Debug.LogErrorFormat("index: {0} has same name with alreaady item", i.ToString());
                            rlt = false;
                        }
                    }
                }
            }
        }

        return rlt;
    }
#endif

    public T GetComponent<T>(int index) where T : Component {
        if (index < 0 || index >= bindList.Count) {
            Debug.LogError("Index is out of range");
            return null;
        }

        T bindCom = bindList[index].component as T;
        if (bindCom == null) {
            Debug.LogErrorFormat("Index: {0} has invalid component", index.ToString());
            return null;
        }

        return bindCom;
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
