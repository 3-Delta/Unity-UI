using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class StateAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(FSM.State);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor("", appdomain, instance);
    }

    //为了完整实现MonoBehaviour的所有特性，这个Adapter还得扩展，这里只抛砖引玉，只实现了最常用的Awake, Start和Update
    public class Adaptor : FSM.State, CrossBindingAdaptorType
    {
        private ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        private ILTypeInstance instance;
        // 缓存这个数组来避免调用时的GC Alloc
        object[] param1 = new object[1];

        public Adaptor(string name) : base(name) { }
        public Adaptor(string name, ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) : base(name)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }
        bool m_bget_fsmGot = false;
        IMethod m_get_fsm = null;
        public FSM.IStateMachine get_fsm()
        {
            if (!m_bget_fsmGot)
            {
                m_get_fsm = instance.Type.GetMethod("get_fsm", 0);
                m_bget_fsmGot = true;
            }
            if (m_get_fsm != null)
            {
                return (FSM.IStateMachine)appdomain.Invoke(m_get_fsm, instance, null);
            }
            else
            {
                return null;
            }
        }
        bool m_bset_fsmGot = false;
        IMethod m_set_fsm = null;
        public void set_fsm(FSM.IStateMachine arg0)
        {
            if (!m_bset_fsmGot)
            {
                m_set_fsm = instance.Type.GetMethod("set_fsm", 1);
                m_bset_fsmGot = true;
            }
            if (m_set_fsm != null)
            {
                appdomain.Invoke(m_set_fsm, instance, arg0);
            }
            else
            {

            }
        }
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
        bool m_bget_timeGot = false;
        IMethod m_get_time = null;
        public System.Single get_time()
        {
            if (!m_bget_timeGot)
            {
                m_get_time = instance.Type.GetMethod("get_time", 0);
                m_bget_timeGot = true;
            }
            if (m_get_time != null)
            {
                return (System.Single)appdomain.Invoke(m_get_time, instance, null);
            }
            else
            {
                return default(System.Single);
            }
        }

        bool m_bEnterGot = false;
        IMethod m_Enter = null;
        bool isEnterInvoking = false;
        public new virtual void Enter(FSM.IState arg0)
        {
            if (!m_bEnterGot)
            {
                m_Enter = instance.Type.GetMethod("Enter", 1);
                m_bEnterGot = true;
            }
            // 对于虚函数而言，必须设定一个标识位来确定是否当前已经在调用中，否则如果脚本类中调用base.TestVirtual()就会造成无限循环，最终导致爆栈
            if (m_Enter != null && !isEnterInvoking)
            {
                isEnterInvoking = true;
                appdomain.Invoke(m_Enter, instance, arg0);
                isEnterInvoking = false;
            }
            else
            {
                // 看情况是否调用基类的方案
                base.Enter(arg0);
            }
        }

        bool m_bExitGot = false;
        IMethod m_Exit = null;
        bool isExitInvoking = false;
        public new virtual void Exit(FSM.IState arg0)
        {
            if (!m_bExitGot)
            {
                m_Exit = instance.Type.GetMethod("Exit", 1);
                m_bExitGot = true;
            }
            if (m_Exit != null && !isExitInvoking)
            {
                isExitInvoking = true;
                appdomain.Invoke(m_Exit, instance, arg0);
                isExitInvoking = false;
            }
            else
            {
                // 看情况是否调用基类的方案
                base.Exit(arg0);
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

        bool m_bget_transitionsGot = false;
        IMethod m_get_transitions = null;
        public List<FSM.Transition> get_transitions()
        {
            if (!m_bget_transitionsGot)
            {
                m_get_transitions = instance.Type.GetMethod("get_transitions", 0);
                m_bget_transitionsGot = true;
            }
            if (m_get_transitions != null)
            {
                return (List<FSM.Transition>)appdomain.Invoke(m_get_transitions, instance, null);
            }
            else
            {
                return null;
            }
        }

        bool m_bAddTransitionGot = false;
        IMethod m_AddTransition = null;
        public new void AddTransition(FSM.ITransition arg0)
        {
            if (!m_bAddTransitionGot)
            {
                m_AddTransition = instance.Type.GetMethod("AddTransition", 1);
                m_bAddTransitionGot = true;
            }
            if (m_AddTransition != null)
            {
                appdomain.Invoke(m_AddTransition, instance, arg0);
            }
            else
            {

            }
        }
        bool m_bRemoveTransitionGot = false;
        IMethod m_RemoveTransition = null;
        public new void RemoveTransition(FSM.ITransition arg0)
        {
            if (!m_bRemoveTransitionGot)
            {
                m_RemoveTransition = instance.Type.GetMethod("RemoveTransition", 1);
                m_bRemoveTransitionGot = true;
            }
            if (m_RemoveTransition != null)
            {
                appdomain.Invoke(m_RemoveTransition, instance, arg0);
            }
            else
            {

            }
        }
    }
}
