using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SortingLayerGenerter {
    public static string DefaultOutput {
        get { return Path.Combine(Application.dataPath, "Scripts/Framework/Utility/UnitySortingLayer.cs"); }
    }

    // [RuntimeInitializeOnLoadMethod]
    public static void Do(string outputFileWithExt) {
        if (string.IsNullOrWhiteSpace(outputFileWithExt)) {
            outputFileWithExt = DefaultOutput;
        }

        string enumName = "ESortingLayers";
        string className = "CSortingLayers";

        StringBuilder contentEnum = new StringBuilder();
        contentEnum.AppendLine(@"using System;");
        contentEnum.AppendLine();
        contentEnum.AppendFormat(CodeDef.enumHead, enumName);
        contentEnum.AppendLine();
        contentEnum.Append(CodeDef.segmentHead);

        StringBuilder contentClass = new StringBuilder();
        contentClass.AppendFormat(CodeDef.classStaticHead, className);
        contentClass.AppendLine();
        contentClass.Append(CodeDef.segmentHead);

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty it = tagManager.GetIterator();
        while (it.NextVisible(true)) {
            if (it.name == "m_SortingLayers") {
                for (int i = 0, count = it.arraySize; i < count; ++i) {
                    SerializedProperty serializedProperty = it.GetArrayElementAtIndex(i);
                    string stringValue = serializedProperty.displayName;
                    if (!string.IsNullOrWhiteSpace(stringValue)) {
                        stringValue = stringValue.Replace(" ", "");
                        contentEnum.Append("\t");
                        contentEnum.AppendFormat(CodeDef.enumMemberWithValue, stringValue, i.ToString());
                        contentEnum.AppendLine();

                        contentClass.Append("\t");
                        contentClass.AppendFormat(CodeDef.classConstPublicMemberWithValue, "string", stringValue, string.Format("\"{0}\"", stringValue));
                        contentClass.AppendLine();
                    }
                }

                break;
            }
        }

        contentEnum.AppendLine(CodeDef.segmentTail);
        contentEnum.AppendLine();

        contentClass.AppendLine(CodeDef.segmentTail);
        if (!File.Exists(outputFileWithExt)) {
            using (File.Create(outputFileWithExt)) { }
        }

        File.WriteAllText(outputFileWithExt, contentEnum.ToString() + contentClass.ToString());
        AssetDatabase.Refresh();
    }
}
