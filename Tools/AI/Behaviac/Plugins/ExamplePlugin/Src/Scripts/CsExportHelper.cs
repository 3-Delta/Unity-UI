using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Behaviac.Design;
using ExamplePlugin.Properties;
using Behaviac.Design.Attributes;

namespace ExamplePlugin
{
    public static class CsExportHelper
    {
        //public static string ExportFloatValue(float value)
        //{
        //    return value + ".0f";
        //}
        //public static string ExportDoubleValue(double value)
        //{
        //    return value + ".0";
        //}
        //public static string ExportBoolValue(bool value)
        //{
        //    return value ? "true" : "false";
        //}
        //public static string ExportEnumValue<T>(T value)
        //{
        //    return Enum.GetName(typeof(T), value);
        //}
        // 支持原生类型
        // 如何拓展呢？
        public static string ExportValue<T>(T value)
        {
            string result = string.Empty;
            if (value is int)
            {
                result = value.ToString();
            }
            else if (value is float)
            {
                result = value + "f";
            }
            else if (value is double)
            {
                result = value + "";
            }
            else if (value is bool)
            {
                result = value.ToString().ToLower();
            }
            else if (value is Enum)
            {
                result = typeof(T).Name + "." + value;
            }
            else if (value is string)
            {
                result = "\"" + value.ToString() + "\"";
            }
            else
            {
                // 不支持的格式
                result = "\"\"";
            }
            return result;
        }
        public static string ExportValue(System.Object value)
        {
            string result = string.Empty;
            if (value is int)
            {
                result = value.ToString();
            }
            else if (value is float)
            {
                result = value + "f";
            }
            else if (value is double)
            {
                result = value + "";
            }
            else if (value is bool)
            {
                result = value.ToString().ToLower();
            }
            else if (value is Enum)
            {
                result = value.GetType().Name + "." + value;
            }
            else if (value is string)
            {
                result = "\"" + value.ToString() + "\"";
            }
            else
            {
                // 不支持的格式
                result = "\"\"";
            }
            return result;
        }
    }
}
