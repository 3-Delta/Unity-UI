using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;

namespace ILRuntime.Runtime.Generated {
    public unsafe static class CLRManualRedirection {
        /// <summary>
        /// 最常用的一些方法这里注册
        /// 可以将appdomain中的关于Activitor/Type/Delegate等类型的重定向代码转移到这里进行注册,手写的，不是自动生成的
        ///  activitor的重定向代码不能依靠 绑定器的自动生成， 因为绑定器生成的自动代码都是没有区分是热更类型，还是框架类型，都是一股脑的用热更类型去处理的。
        ///  Getcomponent也需要手动注册，以来自动生成的绑定器也会存在Activitor不区分热更/框架的问题
        /// </summary>
        /// <param name="appDomain"></param>
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain appDomain) { }
    }
}
