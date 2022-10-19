using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.AddressableAssets.Initialization
{
    /// <summary>
    /// Supports the evaluation of embedded runtime variables in addressables locations
    /// </summary>
    public static class AddressablesRuntimeProperties
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0 && ENABLE_DOTNET
        static Assembly[] GetAssemblies()
        {
            //Not supported on UWP platforms
            return new Assembly[0];
        }

#else
        static Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

#endif
        
        static Dictionary<string, string> s_cancheDict = new Dictionary<string, string>();

        internal static int GetCachedValueCount()
        {
            return s_cancheDict.Count;
        }

        /// <summary>
        /// Predefine a runtime property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="val">The property value.</param>
        public static void SetPropertyValue(string name, string val)
        {
            s_cancheDict[name] = val;
        }

        /// <summary>
        /// This will clear all PropertyValues that have been cached.  This includes all values set by
        /// <see cref="SetPropertyValue"/> as well as any reflection-evaluated properties.
        /// </summary>
        public static void ClearCachedPropertyValues()
        {
            s_cancheDict.Clear();
        }

        /// <summary>
        /// Evaluates a named property using cached values and static public fields and properties.  Be aware that a field or property may be stripped if not referenced anywhere else.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The value of the property.  If not found, the name is returned.</returns>
        public static string EvaluateProperty(string name)
        {
            // 构建 class.prop到dict中，key为class.prop， value为prop/field的数值
            Debug.Assert(s_cancheDict != null, "ResourceManagerConfig.GetGlobalVar - s_cachedValues == null.");

            if (string.IsNullOrEmpty(name))
                return string.Empty;

            string cachedValue;
            if (s_cancheDict.TryGetValue(name, out cachedValue))
                return cachedValue;

            int i = name.LastIndexOf('.');
            if (i < 0)
                return name;

            var className = name.Substring(0, i);
            var propName = name.Substring(i + 1);
            foreach (var a in GetAssemblies())
            {
                Type t = a.GetType(className, false, false);
                if (t == null)
                    continue;
                try
                {
                    var pi = t.GetProperty(propName, BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                    if (pi != null)
                    {
                        var v = pi.GetValue(null, null);
                        if (v != null)
                        {
                            s_cancheDict.Add(name, v.ToString());
                            return v.ToString();
                        }
                    }
                    var fi = t.GetField(propName, BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                    if (fi != null)
                    {
                        var v = fi.GetValue(null);
                        if (v != null)
                        {
                            s_cancheDict.Add(name, v.ToString());
                            return v.ToString();
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return name;
        }

        /// <summary>
        /// Evaluates all tokens deliminated by '{' and '}' in a string and evaluates them with the EvaluateProperty method.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The evaluated string after resolving all tokens.</returns>
        public static string EvaluateString(string input)
        {
            return EvaluateString(input, '{', '}', EvaluateProperty);
        }

        /// <summary>
        /// Evaluates all tokens deliminated by the specified delimiters in a string and evaluates them with the supplied method.
        /// </summary>
        /// <param name="inputString">The string to evaluate.</param>
        /// <param name="startDelimiter">The start token delimiter.</param>
        /// <param name="endDelimiter">The end token delimiter.</param>
        /// <param name="varFunc">Func that has a single string parameter and returns a string.</param>
        /// <returns>The evaluated string.</returns>
        public static string EvaluateString(string inputString, char startDelimiter, char endDelimiter, Func<string, string> varFunc)
        {
            if (string.IsNullOrEmpty(inputString))
                return string.Empty;

            // 最初角色如果是嵌套的{{}}结构，肯定会出现问题
            // 后来发现这里是循环，而且{{}}这种即使中途不匹配也不影响最终输出
            while (true)
            {
                int i = inputString.IndexOf(startDelimiter);
                if (i < 0)
                    return inputString;
                int e = inputString.IndexOf(endDelimiter, i + 1);
                if (e < i)
                    return inputString;
                // 获取包含在{}的的内容
                var token = inputString.Substring(i + 1, e - i - 1);
                // 使用varFunc对内容解析
                var tokenVal = varFunc == null ? string.Empty : varFunc(token);
                // 重新拼接{之前的内容 和 上述内容 和 }之后的内容
                inputString = inputString.Substring(0, i) + tokenVal + inputString.Substring(e + 1);
            }
        }
    }
}
