using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class TransitionAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(FSM.Transition);// 这是你想继承的那个类
        }
    }
    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);// 这是实际的适配器类
        }
    }
    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor("", null, null, appdomain, instance);// 创建一个新的实例
    }
    // 实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    public class Adaptor : FSM.Transition, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        // 缓存这个数组来避免调用时的GC Alloc
        object[] param1 = new object[1];
        public Adaptor(string name, FSM.IState from, FSM.IState to) : base(name, from, to)
        {

        }
        public Adaptor(string name, FSM.IState from, FSM.IState to, ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) : base(name, from, to)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }
        public ILTypeInstance ILInstance { get { return instance; } }

        bool m_bget_nameGot = false;
        IMethod m_get_name = null;
        public System.String get_name()
        {
            if (!m_bget_nameGot)
            {
                m_get_name = instance.Type.GetMethod("get_name", 0);
                m_bget_nameGot = true;
            }
            if (m_get_name != null)
            {
                return (System.String)appdomain.Invoke(m_get_name, instance, null);
            }
            else
            {
                return null;
            }
        }

        bool m_bget_fromGot = false;
        IMethod m_get_from = null;
        public FSM.IState get_from()
        {
            if (!m_bget_fromGot)
            {
                m_get_from = instance.Type.GetMethod("get_from", 0);
                m_bget_fromGot = true;
            }
            if (m_get_from != null)
            {
                return (FSM.IState)appdomain.Invoke(m_get_from, instance, null);
            }
            else
            {
                return null;
            }
        }

        bool m_bget_toGot = false;
        IMethod m_get_to = null;
        public FSM.IState get_to()
        {
            if (!m_bget_toGot)
            {
                m_get_to = instance.Type.GetMethod("get_to", 0);
                m_bget_toGot = true;
            }
            if (m_get_to != null)
            {
                return (FSM.IState)appdomain.Invoke(m_get_to, instance, null);
            }
            else
            {
                return null;
            }
        }

        bool m_bOnTransitionProgressGot = false;
        IMethod m_OnTransitionProgress = null;
        public new System.Single OnTransitionProgress()
        {
            if (!m_bOnTransitionProgressGot)
            {
                m_OnTransitionProgress = instance.Type.GetMethod("OnTransitionProgress", 0);
                m_bOnTransitionProgressGot = true;
            }
            if (m_OnTransitionProgress != null)
            {
                return (System.Single)appdomain.Invoke(m_OnTransitionProgress, instance, null);
            }
            else
            {
                return default(System.Single);
            }
        }

        bool m_bEnterGot = false;
        IMethod m_Enter = null;
        bool isEnterInvoking = false;
        public new virtual void Enter()
        {
            if (!m_bEnterGot)
            {
                m_Enter = instance.Type.GetMethod("Enter", 0);
                m_bEnterGot = true;
            }
            if (m_Enter != null && !isEnterInvoking)
            {
                isEnterInvoking = true;
                appdomain.Invoke(m_Enter, instance, null);
                isEnterInvoking = false;
            }
            else
            {
                // 看情况是否调用基类的方案
                base.Enter();
            }
        }

        bool m_bExitGot = false;
        IMethod m_Exit = null;
        bool isExitInvoking = false;
        public new virtual void Exit()
        {
            if (!m_bExitGot)
            {
                m_Exit = instance.Type.GetMethod("Exit", 0);
                m_bExitGot = true;
            }
            if (m_Exit != null && !isExitInvoking)
            {
                isExitInvoking = true;
                appdomain.Invoke(m_Exit, instance, null);
                isExitInvoking = false;
            }
            else
            {
                // 看情况是否调用基类的方案
                base.Exit();
            }
        }

        bool m_bUpdateGot = false;
        IMethod m_Update = null;
        bool isUpdateInvoking = false;
        public new virtual void Update(System.Single arg0)
        {
            if (!m_bUpdateGot)
            {
                m_Update = instance.Type.GetMethod("Update", 1);
                m_bUpdateGot = true;
            }
            if (m_Update != null && !isUpdateInvoking)
            {
                isUpdateInvoking = true;
                appdomain.Invoke(m_Update, instance, arg0);
                isUpdateInvoking = false;
            }
            else
            {
                // 看情况是否调用基类的方案
                base.Update(arg0);
            }
        }

        bool m_bShouldBeginGot = false;
        IMethod m_ShouldBegin = null;
        bool isShouldBeginInvoking = false;
        public new virtual System.Boolean ShouldBegin()
        {
            if (!m_bShouldBeginGot)
            {
                m_ShouldBegin = instance.Type.GetMethod("ShouldBegin", 0);
                m_bShouldBeginGot = true;
            }
            if (m_ShouldBegin != null && !isShouldBeginInvoking)
            {
                isShouldBeginInvoking = true;
                bool result = (System.Boolean)appdomain.Invoke(m_ShouldBegin, instance, null);
                isShouldBeginInvoking = false;
                return result;
            }
            else
            {
                // 看情况是否调用基类的方案
                return base.ShouldBegin();
            }
        }

        bool m_bShouleEndGot = false;
        IMethod m_ShouleEnd = null;
        bool isShouleEndInvoking = false;
        public new virtual System.Boolean ShouleEnd()
        {
            if (!m_bShouleEndGot)
            {
                m_ShouleEnd = instance.Type.GetMethod("ShouleEnd", 0);
                m_bShouleEndGot = true;
            }
            if (m_ShouleEnd != null && !isShouleEndInvoking)
            {
                isShouleEndInvoking = true;
                bool result = (System.Boolean)appdomain.Invoke(m_ShouleEnd, instance, null);
                isShouleEndInvoking = false;
                return result;
            }
            else
            {
                // 看情况是否调用基类的方案
                return base.ShouleEnd();
            }
        }
    }
}