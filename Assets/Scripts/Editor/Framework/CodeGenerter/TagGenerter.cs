using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TagGenerter {
    public static void Do(string outputFileWithExt) {
        string enumName = "ETags";
        string className = "CTags";

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
        while (it.NextVisible(true))
        {
            if (it.name == "tags")
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
                        contentEnum.AppendFormat(CodeDef.enumMember, stringValue);
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
