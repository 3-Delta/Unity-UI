using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IComparerObjectAdapter : CrossBindingAdaptor {
    public override Type BaseCLRType {
        get {
            return typeof(IComparer<object>); // 这是你想继承的那个类
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
    public class Adaptor : IComparer<object>, CrossBindingAdaptorType {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        private object[] args = new object[2];

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

        public int Compare(object left, object right) {
            if (!m_bCompareGot) {
                m_Compare = instance.Type.GetMethod("Compare", 2);
                if (m_Compare == null) {
                    m_Compare = instance.Type.GetMethod("System.Collections.Generic.IComparer.Compare", 2);
                }

                m_bCompareGot = true;
            }

            if (m_Compare != null) {
                args[0] = left;
                args[1] = right;
                var result = (System.Int32) appdomain.Invoke(m_Compare, instance, args);
                return result;
            }
            else {
                return default(System.Int32);
            }
        }
    }
}
