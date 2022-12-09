using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;

public static class AdapterGenerater
{
    public static void Generate(List<Type> adapterList)
    {
        if (!Directory.Exists(HotfixSettings.AdaptorAnalysisFolderPath))
        {
            Directory.CreateDirectory(HotfixSettings.AdaptorAnalysisFolderPath);
        }

        foreach (var cls in adapterList)
        {
            GenAdapterFile(cls, HotfixSettings.AdaptorAnalysisFolderPath);
        }

        // GenAdapterRegisterFile(adapterList, ILRSettings.AdaptorAnalysisFilePath);

        UnityEngine.Debug.Log("Generate Ok");
        System.GC.Collect();
        AssetDatabase.Refresh();
    }

    private static void GenAdapterRegisterFile(List<Type> types, string dir)
    {
        string fileHeader = @"using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public static class AdapterRegister
{
    public static void RegisterCrossBindingAdaptor(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        // Manual
        domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

        // Auto";
        string lines = "\r\n";
        for(int i = 0, count = types.Count; i < count; ++ i)
        {
            var t = types[i];
            string line = "";
            if (i == count - 1)
            {
                line = "\t\tdomain.RegisterCrossBindingAdaptor(new " + t.Name.Replace("`", "_") + "Adapter());";
            }
            else
            {
                line = "\t\tdomain.RegisterCrossBindingAdaptor(new " + t.Name.Replace("`", "_") + "Adapter());\n";
            }
            
            lines += line;
        }

        string outputString = fileHeader + lines + @"
    }
}";

        using (FileStream file = new FileStream(PathService.Combine(dir, "AdapterRegister.cs"), FileMode.OpenOrCreate))
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.Write(outputString);
            }
        }
    }


    private static void GenAdapterFile(Type t, string dir)
    {
        string fileHeader = @"using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
";

        string className = t.Name.Replace("`", "_");
        string fullName = t.FullName.Replace("`", "_"); ;
        string publicNameStr = "public class " + className + "Adapter : CrossBindingAdaptor\r\n" +
"{\r\n";
        string BaseCLRTypeStr =
    "\tpublic override Type BaseCLRType\r\n" +
    "\t{\r\n" +
    "\t    get\r\n" +
    "\t    {\r\n" +
    "\t        return typeof(" + fullName + ");// 这是你想继承的那个类\r\n" +
    "\t    }\r\n" +
    "\t}\r\n" +

    "\tpublic override Type AdaptorType\r\n" +
    "\t{\r\n" +
    "\t    get\r\n" +
    "\t    {\r\n" +
    "\t        return typeof(Adaptor);// 这是实际的适配器类\r\n" +
    "\t    }\r\n" +
    "\t}\r\n" +

    "\tpublic override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)\r\n" +
    "\t{\r\n" +
    "\t    return new Adaptor(appdomain, instance);// 创建一个新的实例\r\n" +
    "\t}\r\n" +

    "\t// 实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口\r\n" +
    "\tpublic class Adaptor : " + fullName + ", CrossBindingAdaptorType\r\n" +
    "\t{\r\n" +
    "\t    ILTypeInstance instance;\r\n" +
    "\t    ILRuntime.Runtime.Enviorment.AppDomain appdomain;\r\n" +
    "\t    public Adaptor()\r\n" +
    "\t    {\r\n" +
    "\t\r\n" +
    "\t    }\r\n" +

    "\t    public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)\r\n" +
    "\t    {\r\n" +
    "\t        this.appdomain = appdomain;\r\n" +
    "\t        this.instance = instance;\r\n" +
    "\t    }\r\n" +

    "\t    public ILTypeInstance ILInstance { get { return instance; } }\r\n";

        //反射virtual的函数
        List<MethodInfo> methods = t.GetMethods().ToList().FindAll((_method) =>
        {
            return _method.IsPublic && _method.IsVirtual && _method.DeclaringType == t;
        });
        string methodsStr = "";
        foreach (var md in methods)
        {
            methodsStr += CreateOverrideMethod(md) + "\r\n";
        }

        string outputString = fileHeader + "\r\n" + publicNameStr + BaseCLRTypeStr + methodsStr + "}\r\n}";
        using (FileStream file = new FileStream(PathService.Combine(dir, className + "Adapter.cs"), FileMode.OpenOrCreate))
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.Write(outputString);
            }
        }
    }

    private static string CreateOverrideMethod(MethodInfo info)
    {
        string gotFieldStr = "m_b" + info.Name + "Got";
        string gotVirtualFieldStr = "m_b" + info.Name + "GotVirtual";
        string fieldStr = "m_" + info.Name;
        string returnTypeStr = "void";
        bool hasReturn = false;
        bool isVirtual = false;//info.IsVirtual;
        bool isInterface = info.ReturnType.IsInterface;
        if (info.ReturnType.Name != "Void")
        {
            hasReturn = true;
            returnTypeStr = info.ReturnType.FullName;
        }

        string paramsstr = "";
        int paramCount = 0;
        string paramarg = "null";
        string args = "";
        if (info.GetParameters() != null)
        {
            paramCount = info.GetParameters().Length;
            if (paramCount > 0)
            {
                paramarg = "";
            }
            int idx = 0;
            foreach (var _param in info.GetParameters())
            {
                string arg = "arg" + idx;
                paramarg += arg;
                paramsstr += _param.ParameterType.FullName + " " + arg;
                if (idx++ < info.GetParameters().Length - 1)
                {
                    paramsstr += ",";
                    paramarg += ",";
                    args += (arg + ", ");
                }
                else
                {
                    args += (arg);
                }
            }
        }

        string virtualCallIf = isVirtual ? " && !" + gotVirtualFieldStr : "";
        string virtualCallContent = "";
        if (isVirtual)
        {
            if (hasReturn)
            {
                virtualCallContent = "return base." + info.Name + "(" + args + ");";
            }
            else
            {
                virtualCallContent = "base." + info.Name + "(" + args + ");";
            }
        }
        else
        {
            if (hasReturn)
            {
                if (isInterface)
                {
                    virtualCallContent = "return null;";
                }
                else
                {
                    virtualCallContent = "return default(" + returnTypeStr + ");";
                }
            }
            else
            {
                virtualCallContent = "";
            }
        }
        string virtualCallSet1 = isVirtual ? "\t\t\t\t" + gotVirtualFieldStr + " = true;\r\n" : "";
        string virtualCallSet2 = isVirtual ? "\t\t\t\t" + gotVirtualFieldStr + " = false;\r\n" : "";
        string callmethod = "if(" + fieldStr + " != null" + virtualCallIf + ")\r\n" +
                            "\t\t\t{\r\n" + virtualCallSet1 +
                            "\t\t\t\t" + (hasReturn ? "var result = " : "") + (returnTypeStr == "void" ? "" : string.Format("({0})", returnTypeStr))
                                          + "appdomain.Invoke(" + fieldStr + ", instance, " + paramarg + ");\r\n " + 
                                          virtualCallSet2 +
                            "\t\t\t\t" + (hasReturn ? "return result;" : "") + "\r\n" +
                            "\t\t\t}\r\n" +
                            "\t\t\telse\r\n" +
                            "\t\t\t{\r\n" +
                            "\t\t\t\t" + virtualCallContent + "\r\n" +
                            "\t\t\t}";

        string descVirtual = isVirtual ? "override " : "new ";
        string gotmethod = "\n\t\tbool " + gotFieldStr + " = false;\r\n" +
                    "\t\tbool " + gotVirtualFieldStr + " = false;\r\n" +
                    "\t\tIMethod " + fieldStr + " = null;\r\n" +
                    "\t\tpublic " + descVirtual + returnTypeStr + " " + info.Name + " (" + paramsstr + ")\r\n" +
                    "\t\t{\r\n" +
                    "\t\t   if(!" + gotFieldStr + ")\r\n" +
                    "\t\t   {\r\n" +
                    "\t\t       " + fieldStr + " = instance.Type.GetMethod(\"" + info.Name + "\", " + paramCount + ");\r\n" +
                    "\t\t       " + gotFieldStr + " = true;\r\n" +
                    "\t\t   }\r\n" +
                    "\t\t   " + callmethod + " \r\n" +
                    "\t\t}";
        return gotmethod;
    }
}
