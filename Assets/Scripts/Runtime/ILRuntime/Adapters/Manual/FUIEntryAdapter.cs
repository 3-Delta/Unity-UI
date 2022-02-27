using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

public class FUIEntryAdapter : CrossBindingAdaptor {
    public override Type BaseCLRType {
        get {
            return typeof(FUIEntry); // 这是你想继承的那个类
        }
    }

    public override Type AdaptorType {
        get {
            return typeof(Adaptor); // 这是实际的适配器类
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
        return new Adaptor(appdomain, instance); // 创建一个新的实例
    }

    // 实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    public class Adaptor : FUIEntry, CrossBindingAdaptorType {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public ILTypeInstance ILInstance {
            get { return instance; }
            set { instance = value; }
        }

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain {
            get { return appdomain; }
            set { appdomain = value; }
        }
        // 缓存这个数组来避免调用时的GC Alloc

        object[] args = new object[1];

        public Adaptor() {
            // 因为是通过Addcomponent添加的，所以没有调用CreateCLRInstance，所以需要外部手动设置instance/appdomain
        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        bool hasCreateInstanceGot = false;
        bool hasVirtualCreateInstanceGot = false;
        IMethod _createInstance = null;

        public override FUIBase CreateInstance() {
            if (!hasCreateInstanceGot) {
                _createInstance = instance.Type.GetMethod("CreateInstance", 0);
                hasCreateInstanceGot = true;
            }

            if (_createInstance != null && !hasVirtualCreateInstanceGot) {
                hasVirtualCreateInstanceGot = true;
                var rlt = appdomain.Invoke(_createInstance, instance, null);
                hasVirtualCreateInstanceGot = false;
                return rlt as FUIBase;
            }
            else {
                var rlt = base.CreateInstance();
                return rlt;
            }
        }
    }
}
