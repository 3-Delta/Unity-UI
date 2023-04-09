using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

#if UNITY_EDITOR
using System.IO;
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

    private SerializedProperty fieldStyle;
    private ReorderableList recorderableList;

    private void OnEnable() {
        owner = target as UIBindComponents;

        this.fieldStyle = serializedObject.FindProperty("fieldStyle");

        var prop = serializedObject.FindProperty("bindComponents");
        recorderableList = new ReorderableList(serializedObject, prop);
        recorderableList.elementHeight = 20;
        recorderableList.drawElementCallback = (rect, index, active, focused) => {
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

        recorderableList.onSelectCallback += rlist => { GUI.backgroundColor = Color.blue; };

        recorderableList.drawHeaderCallback = rect => {
            var oldColor = GUI.color;
            GUI.color = Color.green;
            EditorGUI.LabelField(rect, string.Format("[--> Index | {0} | {1} | {2} <--]", /*prop.displayName, */"Component", "Listen", "Name"));
            GUI.color = oldColor;
        };
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(fieldStyle);

        recorderableList.DoLayoutList();

        if (GUILayout.Button("命名检测")) {
            owner.Check();
        }

        if (GUILayout.Button("自动更名")) {
            owner.AutoName();
        }

        if (GUILayout.Button("复制")) {
            if (owner.Check()) {
                string code = owner.Copy("Layout", false, false);
                GUIUtility.systemCopyBuffer = code;
                Debug.LogError(code);
            }
        }

        if (GUILayout.Button("生成")) {
            if (owner.Check()) {
                string path = EditorUtility.SaveFilePanel("SaveFile", Application.dataPath, "XXX_Layout", "cs");
                if (string.IsNullOrWhiteSpace(path)) {
                    return;
                }

                string clsName = Path.GetFileNameWithoutExtension(path);
                string code = owner.Copy(clsName, true, true);
                GUIUtility.systemCopyBuffer = code;
                File.WriteAllText(path, code);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

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
    public enum ECSharpFieldStyle {
        Field,
        Property,
    }

    public ECSharpFieldStyle fieldStyle = ECSharpFieldStyle.Field;
    public const string TAB = "    ";

    // tuple形式，方便后续动态的add，进行拓展
    public static readonly Dictionary<Type, IList<ValueTuple<string, string>>> LISTEN_DESCS = new Dictionary<Type, IList<ValueTuple<string, string>>>() {
        {
            typeof(Button), new List<(string, string)>() {
                new ValueTuple<string, string>("void OnBtnClicked{0}();", "this.{0}.onClick.AddListener(listener.OnBtnClicked{1});"),
            }
        }, {
            typeof(Toggle), new List<(string, string)>() {
                new ValueTuple<string, string>("void OnValueChanged{0}(bool flag);", "this.{0}.onValueChanged.AddListener(listener.OnValueChanged{1});"),
            }
        }, {
            typeof(Slider), new List<(string, string)>() {
                new ValueTuple<string, string>("void OnValueChanged{0}(float currentValue);", "this.{0}.onValueChanged.AddListener(listener.OnValueChanged{1});"),
            }
        },
    };

    // https://github.com/scriban/scriban
    public string Copy(string clsName, bool includeNamespace, bool includeUsing = true) {
        string GenerateKls() {
            const string SYNC_BINDER = @"
#if UNITY_EDITOR
    [__TAB__]private void Reset() {
    [__TAB__]    this.hideFlags = HideFlags.DontSaveInBuild;
    [__TAB__]    this.FindByPath(transform, true);
    [__TAB__]}
    [__TAB__]
    [__TAB__][ContextMenu(nameof(SyncToBinder))]
    [__TAB__]private void SyncToBinder() {
    [__TAB__]    if (!transform.TryGetComponent<UIBindComponents>(out UIBindComponents r)) {
    [__TAB__]        r = transform.gameObject.AddComponent<UIBindComponents>();
    [__TAB__]    }
    [__TAB__]
    [__TAB__]    r.fieldStyle = UIBindComponents.ECSharpFieldStyle.Field;
    [__TAB__]    r.bindComponents = this.Collect();
    [__TAB__]}
    [__TAB__]
    [__TAB__]private List<UIBindComponents.BindComponent> Collect() {
    [__TAB__]    var fis = this.GetType().GetFields();
    [__TAB__]    List<UIBindComponents.BindComponent> array = new List<UIBindComponents.BindComponent>();
    [__TAB__]    foreach (var fi in fis) {
    [__TAB__]        if (!fi.FieldType.IsSubclassOf(typeof(UnityEngine.Component))) {
    [__TAB__]            continue;
    [__TAB__]        }
    [__TAB__]
    [__TAB__]        var cp = fi.GetValue(this) as UnityEngine.Component;
    [__TAB__]        var a = new UIBindComponents.BindComponent() {
    [__TAB__]            component = cp,
    [__TAB__]            toListen = true,
    [__TAB__]            name = fi.Name
    [__TAB__]        };
    [__TAB__]        array.Add(a);
    [__TAB__]    }
    [__TAB__]
    [__TAB__]    return array;
    [__TAB__]}
#endif
";
            const string FIELD_NOTE = "// [{0}] Path: \"{1}\"";
            const string FIELD_DEFINE = @"public {0} {1}";

            StringBuilder sb = new StringBuilder("");

            sb.AppendLine("[__TAB__]public partial class Layout : FUILayoutBase {");
            // sb.AppendLine(SYNC_BINDER);
            for (int i = 0, length = bindComponents.Count; i < length; ++i) {
                var item = bindComponents[i];
                sb.Append("[__TAB__]");
                sb.Append(MultiTab(1));
                sb.AppendFormat(FIELD_NOTE, i.ToString(), item.GetComponentPath(this));
                sb.AppendLine();

                sb.Append("[__TAB__]");
                sb.Append(MultiTab(1));
                sb.AppendFormat(FIELD_DEFINE, item.componentType, item.name);
                if (this.fieldStyle == ECSharpFieldStyle.Property) {
                    sb.AppendLine(" { get; private set; } = null;");
                }
                else if (this.fieldStyle == ECSharpFieldStyle.Field) {
                    sb.AppendLine(" = null;");
                }
            }

            sb.AppendLine();

            const string BINDER_FIND = @"this.{0} = binder.Find<{1}>({2});";
            sb.Append(MultiTab(1));
            sb.AppendLine("[__TAB__]protected override void FindByIndex(UIBindComponents binder) {");
            for (int i = 0, length = bindComponents.Count; i < length; ++i) {
                var item = bindComponents[i];
                sb.Append("[__TAB__]");
                sb.Append(MultiTab(2));
                sb.AppendFormat(BINDER_FIND, item.name, item.componentType, i.ToString());
                sb.AppendLine();
            }

            sb.Append(MultiTab(1));
            sb.AppendLine("[__TAB__]}");

            sb.AppendLine();
            sb.Append(MultiTab(1));
            sb.AppendLine("[__TAB__]// 后续想不热更prefab,只热更脚本的形式获取组件,再次函数内部添加查找逻辑即可");
            sb.Append(MultiTab(1));
            sb.AppendLine("[__TAB__]protected override void FindByPath(UnityEngine.Transform transform, bool check = false) {");

            sb.Append(MultiTab(2));
            sb.AppendLine("[__TAB__]if (!check) {");
            for (int i = 0, length = bindComponents.Count; i < length; ++i) {
                var item = bindComponents[i];
                sb.Append(MultiTab(3));
                var path = item.GetComponentPath(this);
                if (path == null) {
                    sb.AppendFormat("[__TAB__]this.{0} = transform.GetComponent<{1}>();", item.name, item.componentType);
                }
                else {
                    sb.AppendFormat("[__TAB__]this.{0} = transform.Find(\"{1}\").GetComponent<{2}>();", item.name, path, item.componentType);
                }

                sb.AppendLine();
            }

            sb.Append(MultiTab(2));
            sb.AppendLine("[__TAB__]}");

            sb.Append(MultiTab(2));
            sb.AppendLine("[__TAB__]else {");
            sb.Append(MultiTab(3));
            sb.AppendLine("[__TAB__]UnityEngine.Transform ___t = null;");
            for (int i = 0, length = bindComponents.Count; i < length; ++i) {
                var item = bindComponents[i];
                sb.Append(MultiTab(3));
                var path = item.GetComponentPath(this);
                if (path == null) {
                    sb.AppendFormat("[__TAB__]this.{0} = transform.GetComponent<{1}>();", item.name, item.componentType);
                }
                else {
                    sb.AppendFormat("[__TAB__]___t = transform.Find(\"{0}\");", path);
                    sb.AppendLine();
                    sb.Append(MultiTab(3));
                    sb.AppendFormat("[__TAB__]this.{0} = ___t != null ? ___t.GetComponent<{1}>() : null;", item.name, item.componentType);
                }

                sb.AppendLine();
            }

            sb.Append(MultiTab(2));
            sb.AppendLine("[__TAB__]}");

            sb.Append(MultiTab(1));
            sb.AppendLine("[__TAB__]}");

            if (NeedListener()) {
                sb.AppendLine();
                sb.Append(MultiTab(1));
                sb.AppendLine(@"[__TAB__]public interface IListener {");
                for (int i = 0, length = bindComponents.Count; i < length; ++i) {
                    var item = bindComponents[i];
                    if (item.component != null && item.toListen) {
                        var type = item.type;
                        if (type != null && LISTEN_DESCS.TryGetValue(type, out var descs)) {
                            foreach (var desc in descs) {
                                sb.Append(MultiTab(2));
                                sb.AppendFormat("[__TAB__]" + desc.Item1, item.name);
                                sb.AppendLine();
                            }
                        }
                    }
                }

                sb.Append(MultiTab(1));
                sb.AppendLine("[__TAB__]}");
                sb.AppendLine();

                sb.Append(MultiTab(1));
                sb.AppendLine("[__TAB__]public void Listen(IListener listener, bool toListen = true) {");
                for (int i = 0, length = bindComponents.Count; i < length; ++i) {
                    var item = bindComponents[i];
                    if (item.component != null && item.toListen) {
                        var type = item.type;
                        if (type != null && LISTEN_DESCS.TryGetValue(type, out var descs)) {
                            foreach (var desc in descs) {
                                sb.Append(MultiTab(2));
                                sb.AppendFormat("[__TAB__]" + desc.Item2, item.name, item.name);
                                sb.AppendLine();
                            }
                        }
                    }
                }

                sb.Append(MultiTab(1));
                sb.AppendLine("[__TAB__]}");
            }

            sb.AppendLine("[__TAB__]}");
            return sb.ToString();
        }

        string GenerateUsing() {
            StringBuilder sb = new StringBuilder("");

            const string USING = @"using System;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
#endif";
            if (includeUsing) {
                sb.AppendLine(USING);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        const string NAMESPACE = @"namespace Logic {
[__KLS__]}";

        string usingContent = GenerateUsing();
        StringBuilder final = new StringBuilder(usingContent);
        string klsContent = GenerateKls();
        if (includeNamespace) {
            final.Append(NAMESPACE);
            final = final.Replace("[__KLS__]", klsContent);
            final = final.Replace("[__TAB__]", TAB);
        }
        else {
            final.Append(klsContent);
            final = final.Replace("[__TAB__]", "");
        }

        return final.ToString();
    }

    // 是否需要生成listener
    public bool NeedListener() {
        bool need = false;
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            if (item.component != null && item.toListen) {
                var type = item.type;
                if (type != null && LISTEN_DESCS.TryGetValue(type, out var descs)) {
                    need = true;
                    break;
                }
            }
        }

        return need;
    }

    public void AutoName() {
        string FirstCharUpper(string target) {
            if (!string.IsNullOrWhiteSpace(target) && target.Length > 1) {
                return target[0].ToString().ToUpper() + target.Substring(1);
            }

            return null;
        }

        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var cp = bindComponents[i];
            if (cp != null && cp.component != null) {
                var targetName = cp.component.name;

                // 名字规则
                // 去除空格
                targetName = targetName?.Replace(" ", "");
                // 首字母大写
                targetName = FirstCharUpper(targetName);
                // 添加前缀
                var prefix = cp.componentType.ToLower();
                var splits = prefix.Split(new char[] { '.' });
                prefix = splits[splits.Length - 1];
                targetName = prefix + targetName;

                cp.name = targetName;
            }
        }
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

    public static bool IsOfType(Type toCheck, Type type, bool orInherited = true) {
        return type == toCheck || (orInherited && type.IsAssignableFrom(toCheck));
    }

    public static string MultiTab(int level, string TAB = "    ") {
        string append = "";
        for (int i = 0, length = level; i < length; ++i) {
            append += TAB;
        }

        return append;
    }

    // 给每行的行首添加TAB
    public static string AppendTabBeforeLine(string input, int tabCount) {
        string[] splites = input?.Split(new char[] { '\n' });
        if (splites.Length > 0) {
            string append = MultiTab(tabCount);
            StringBuilder sb = new StringBuilder();
            for (int i = 0, length = splites.Length - 1; i < length; ++i) {
                sb.Append(append);
                sb.Append(splites[i]);
                sb.Append("\n");
            }

            sb.Append(append);
            sb.Append(splites[splites.Length - 1]);
            return sb.ToString();
        }

        return "";
    }
}
