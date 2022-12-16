using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// 获取所有layer
// string[] layers = UnityEditorInternal.InternalEditorUtility.layers
public class LayerGenerter {
    public static void Do(string outputFileWithExt) {
        string enumName = "ELayers";
        string className = "CLayers";

        StringBuilder contentEnum = new StringBuilder();
        contentEnum.AppendLine(@"using System;");
        contentEnum.AppendLine();
        contentEnum.AppendLine("[System.Flags]");
        contentEnum.AppendFormat(CodeDef.enumHead, enumName);
        contentEnum.AppendLine();
        contentEnum.Append(CodeDef.segmentHead);

        StringBuilder contentClass = new StringBuilder();
        contentClass.AppendFormat(CodeDef.classStaticHead, className);
        contentClass.AppendLine();
        contentClass.Append(CodeDef.segmentHead);

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty it = tagManager.GetIterator();
        while (it.NextVisible(true))
        {
            if (it.name == "layers")
            {
                for (int i = 0, count = it.arraySize; i < count; ++i)
                {
                    SerializedProperty serializedProperty = it.GetArrayElementAtIndex(i);
                    string stringValue = serializedProperty.stringValue;
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        string intValue = i == 0 ? "0" : string.Format("1 << {0}", (i - 1).ToString());
                        int realIntValue = i == 0 ? 0 : 1 << (i - 1);
                        stringValue = stringValue.Replace(" ", "");
                        contentEnum.Append("\t");
                        contentEnum.AppendFormat(CodeDef.enumMemberWithValueWithNote, stringValue, intValue, realIntValue.ToString());
                        contentEnum.AppendLine();

                        contentClass.Append("\t");
                        contentClass.AppendFormat(CodeDef.classConstPublicMemberWithValue, "string", stringValue, string.Format("\"{0}\"", serializedProperty.stringValue));
                        contentClass.AppendLine();
                    }
                }
                break;
            }
        }

        contentEnum.AppendLine(CodeDef.segmentTail);
        contentEnum.AppendLine();

        contentClass.AppendLine(CodeDef.segmentTail);
        if (!File.Exists(outputFileWithExt))
        {
            using (File.Create(outputFileWithExt)) { }
        }
        File.WriteAllText(outputFileWithExt, contentEnum.ToString() + contentClass.ToString());
        AssetDatabase.Refresh();
    }
}
