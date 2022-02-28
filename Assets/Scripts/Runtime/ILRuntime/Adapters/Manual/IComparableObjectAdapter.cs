using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IComparableObjectAdapter : CrossBindingAdaptor {
    public override Type BaseCLRType {
        get {
            return typeof(IComparable<object>); // 这是你想继承的那个类
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
    public class Adaptor : IComparable<object>, CrossBindingAdaptorType {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        private object[] args = new object[1];
        
        public Adaptor() { }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance {
            get { return instance; }
        }

        bool m_bCompareGot = false;
        IMethod m_Compare = null;

        public int CompareTo(object arg0) {
            if (!m_bCompareGot) {
                m_Compare = instance.Type.GetMethod("CompareTo", 1);
                if (m_Compare == null) {
                    m_Compare = instance.Type.GetMethod("System.IComparable.CompareTo", 1);
                }

                m_bCompareGot = true;
            }

            if (m_Compare != null) {
                args[0] = arg0;
                var result = (System.Int32) appdomain.Invoke(m_Compare, instance, args);
                return result;
            }
            else {
                return default(System.Int32);
            }
        }
    }
}
