using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hook
{
    static class HookLog
    {
        public static void LogFormat(string format,params object[] args)
        {
            format = "<color=#00F85F>[Hook] " + format + "</color>\n" + StackTraceUtility.ExtractStackTrace();
            
            Debug.LogFormat(format, args);
        }
    }
}
