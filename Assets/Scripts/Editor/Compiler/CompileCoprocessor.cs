using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEditor;

// 使用vs打开，该宏才会生效
#if ENABLE_VSTU
using SyntaxTree.VisualStudio.Unity.Bridge;

// https://docs.microsoft.com/zh-cn/visualstudio/cross-platform/customize-project-files-created-by-vstu?view=vs-2019
[InitializeOnLoad]
public class BuildCoprocessor
{
    // necessary for XLinq to save the xml project file in utf8
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    static BuildCoprocessor()
    {
        //ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) =>
        //{
        //    Debug.LogError(name + " ===============>>>  " + content);

        //    var document = XDocument.Parse(content);
        //    var str = new Utf8StringWriter();
        //    document.Save(str);

        //    return str.ToString();
        //};

        //ProjectFilesGenerator.SolutionFileGeneration += (string fileName, string fileContent) =>
        //{
        //    return fileContent;
        //};
    }
}
#endif
