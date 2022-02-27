using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEditor;

#if ENABLE_VSTU
using SyntaxTree.VisualStudio.Unity.Bridge;

// https://docs.microsoft.com/zh-cn/visualstudio/cross-platform/customize-project-files-created-by-vstu?view=vs-2019
[InitializeOnLoad]
public class BuildCoprocessor : AssetPostprocessor {
    // necessary for XLinq to save the xml project file in utf8
    public class Utf8StringWriter : StringWriter {
        public override Encoding Encoding => Encoding.UTF8;
    }

    static BuildCoprocessor() {
        ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) => {
            Debug.LogError(name + " ===============>>>  " + content);

            // parse the document and make some changes
            var document = XDocument.Parse(content);

            // save the changes using the Utf8StringWriter
            var str = new Utf8StringWriter();
            document.Save(str);

            return str.ToString();
        };
    }

    public static string OnGeneratedSlnSolution(string path, string content) {
        return content;
    }

    public static string OnGeneratedCSProject(string path, string content) {
        return content;
    }
}
#endif
