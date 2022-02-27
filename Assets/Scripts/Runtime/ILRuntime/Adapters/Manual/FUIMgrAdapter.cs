using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

public class FUIMgrAdapter : CrossBindingAdaptor {
    public override Type BaseCLRType {
        get {
            return typeof(FUIMgr); // 这是你想继承的那个类
        }
    }

    public override Type AdaptorType {
        get {
            return typeof(Adaptor); // 这是实际的适配器类
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
        return new Adaptor(); // 创建一个新的实例
    }

    // 实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    public class Adaptor : FUIMgr, CrossBindingAdaptorType {
        ILTypeInstance instance;

        public ILTypeInstance ILInstance {
            get { return instance; }
            set { instance = value; }
        }
    }
}
