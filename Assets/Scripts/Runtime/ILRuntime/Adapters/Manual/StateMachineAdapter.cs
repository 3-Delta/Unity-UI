using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class StateMachineAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(FSM.StateMachine);// 这是你想继承的那个类
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
        return new Adaptor("", null, appdomain, instance);// 创建一个新的实例
    }
    // 实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    public class Adaptor : FSM.StateMachine, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        // 缓存这个数组来避免调用时的GC Alloc
        object[] param1 = new object[1];
        public Adaptor(string name, FSM.IState defaultState) : base(name, defaultState)
        {

        }
        public Adaptor(string name, FSM.IState defaultState, ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) : base(name, defaultState)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }
        bool m_bget_statesGot = false;
        IMethod m_get_states = null;
        public Dictionary<string, FSM.IState> get_states()
        {
            if (!m_bget_statesGot)
            {
                m_get_states = instance.Type.GetMethod("get_states", 0);
                m_bget_statesGot = true;
            }
            if (m_get_states != null)
            {
                return (Dictionary<string, FSM.IState>) appdomain.Invoke(m_get_states, instance,null);
            }
           else
           {
               return null;
           } 
        }

        bool m_bget_currentStateGot = false;
        IMethod m_get_currentState = null;
        public FSM.IState get_currentState()
        {
            if (!m_bget_currentStateGot)
            {
                m_get_currentState = instance.Type.GetMethod("get_currentState", 0);
                m_bget_currentStateGot = true;
            }
            if (m_get_currentState != null)
            {
                return (FSM.IState)appdomain.Invoke(m_get_currentState, instance, null);
            }
            else
            {
                return null;
            }
        }

        bool m_bget_defaultStateGot = false;
        IMethod m_get_defaultState = null;
        public FSM.IState get_defaultState()
        {
            if (!m_bget_defaultStateGot)
            {
                m_get_defaultState = instance.Type.GetMethod("get_defaultState", 0);
                m_bget_defaultStateGot = true;
            }
            if (m_get_defaultState != null)
            {
                return (FSM.IState)appdomain.Invoke(m_get_defaultState, instance, null);
            }
            else
            {
                return null;
            }
        }

        bool m_bset_defaultStateGot = false;
        IMethod m_set_defaultState = null;
        public void set_defaultState(FSM.IState arg0)
        {
            if (!m_bset_defaultStateGot)
            {
                m_set_defaultState = instance.Type.GetMethod("set_defaultState", 1);
                m_bset_defaultStateGot = true;
            }
            if (m_set_defaultState != null)
            {
                appdomain.Invoke(m_set_defaultState, instance, arg0);
            }
            else
            {

            }
        }
        bool m_bget_isInTransitionGot = false;
        IMethod m_get_isInTransition = null;
        public System.Boolean get_isInTransition()
        {
            if (!m_bget_isInTransitionGot)
            {
                m_get_isInTransition = instance.Type.GetMethod("get_isInTransition", 0);
                m_bget_isInTransitionGot = true;
            }
            if (m_get_isInTransition != null)
            {
                return (System.Boolean)appdomain.Invoke(m_get_isInTransition, instance, null);
            }
            else
            {
                return default(System.Boolean);
            }
        }

        bool m_bget_currentTransitionGot = false;
        IMethod m_get_currentTransition = null;
        public FSM.ITransition get_currentTransition()
        {
            if (!m_bget_currentTransitionGot)
            {
                m_get_currentTransition = instance.Type.GetMethod("get_currentTransition", 0);
                m_bget_currentTransitionGot = true;
            }
            if (m_get_currentTransition != null)
            {
                return (FSM.ITransition)appdomain.Invoke(m_get_currentTransition, instance, null);
            }
            else
            {
                return null;
            }
        }

        bool m_bResetGot = false;
        IMethod m_Reset = null;
        public new void Reset()
        {
            if (!m_bResetGot)
            {
                m_Reset = instance.Type.GetMethod("Reset", 0);
                m_bResetGot = true;
            }
            if (m_Reset != null)
            {
                appdomain.Invoke(m_Reset, instance, null);
            }
            else
            {

            }
        }

        bool m_bAddGot = false;
        IMethod m_Add = null;
        public new void Add(System.String arg0, FSM.IState arg1)
        {
            if (!m_bAddGot)
            {
                m_Add = instance.Type.GetMethod("Add", 2);
                m_bAddGot = true;
            }
            if (m_Add != null)
            {
                appdomain.Invoke(m_Add, instance, arg0, arg1);
            }
            else
            {

            }
        }

        bool m_bRemoveGot = false;
        IMethod m_Remove = null;
        public new void Remove(System.String arg0)
        {
            if (!m_bRemoveGot)
            {
                m_Remove = instance.Type.GetMethod("Remove", 1);
                m_bRemoveGot = true;
            }
            if (m_Remove != null)
            {
                appdomain.Invoke(m_Remove, instance, arg0);
            }
            else
            {

            }
        }

        bool m_bSwitchToGot1 = false;
        IMethod m_SwitchTo1 = null;
        public new void SwitchTo(FSM.IState arg0, System.Boolean arg1)
        {
            if (!m_bSwitchToGot1)
            {
                m_SwitchTo1 = instance.Type.GetMethod("SwitchTo", 2);
                m_bSwitchToGot1 = true;
            }
            if (m_SwitchTo1 != null)
            {
                appdomain.Invoke(m_SwitchTo1, instance, arg0, arg1);
            }
            else
            {

            }
        }
        bool m_bSwitchToGot = false;
        IMethod m_SwitchTo = null;
        public new void SwitchTo(System.String arg0, System.Boolean arg1)
        {
            if (!m_bSwitchToGot)
            {
                m_SwitchTo = instance.Type.GetMethod("SwitchTo", 2);
                m_bSwitchToGot = true;
            }
            if (m_SwitchTo != null)
            {
                appdomain.Invoke(m_SwitchTo, instance, arg0, arg1);
            }
            else
            {

            }
        }

        bool m_bGetGot = false;
        IMethod m_Get = null;
        public new FSM.IState Get(System.String arg0)
        {
            if (!m_bGetGot)
            {
                m_Get = instance.Type.GetMethod("Get", 1);
                m_bGetGot = true;
            }
            if (m_Get != null)
            {
                return (FSM.IState)appdomain.Invoke(m_Get, instance, arg0);
            }
            else
            {
                return null;
            }
        }

        bool m_bContainsGot = false;
        IMethod m_Contains = null;
        public new System.Boolean Contains(System.String arg0)
        {
            if (!m_bContainsGot)
            {
                m_Contains = instance.Type.GetMethod("Contains", 1);
                m_bContainsGot = true;
            }
            if (m_Contains != null)
            {
                return (System.Boolean)appdomain.Invoke(m_Contains, instance, arg0);
            }
            else
            {
                return default(System.Boolean);
            }
        }

        bool m_bEnterGot = false;
        IMethod m_Enter = null;
        public override void Enter(FSM.IState arg0)
        {
            if (!m_bEnterGot)
            {
                m_Enter = instance.Type.GetMethod("Enter", 1);
                m_bEnterGot = true;
            }
            if (m_Enter != null)
            {
                appdomain.Invoke(m_Enter, instance, arg0);
            }
            else
            {
                // 看情况是否调用基类的方案
                base.Enter(arg0);
            }
        }

        bool m_bExitGot = false;
        IMethod m_Exit = null;
        public override void Exit(FSM.IState arg0)
        {
            if (!m_bExitGot)
            {
                m_Exit = instance.Type.GetMethod("Exit", 1);
                m_bExitGot = true;
            }
            if (m_Exit != null)
            {
                appdomain.Invoke(m_Exit, instance, arg0);
            }
            else
            {
                // 看情况是否调用基类的方案
                base.Exit(arg0);
            }
        }

        bool m_bUpdateGot = false;
        IMethod m_Update = null;
        public override void Update(System.Single arg0)
        {
            if (!m_bUpdateGot)
            {
                m_Update = instance.Type.GetMethod("Update", 1);
                m_bUpdateGot = true;
            }
            if (m_Update != null)
            {
                appdomain.Invoke(m_Update, instance, arg0);
            }
            else
            {
                // 看情况是否调用基类的方案
                base.Update(arg0);
            }
        }

        bool m_bDoTransitionGot = false;
        IMethod m_DoTransition = null;
        public new void DoTransition(FSM.ITransition arg0)
        {
            if (!m_bDoTransitionGot)
            {
                m_DoTransition = instance.Type.GetMethod("DoTransition", 1);
                m_bDoTransitionGot = true;
            }
            if (m_DoTransition != null)
            {
                appdomain.Invoke(m_DoTransition, instance, arg0);
            }
            else
            {

            }
        }

        bool m_bStopGot = false;
        IMethod m_Stop = null;
        public new void Stop()
        {
            if (!m_bStopGot)
            {
                m_Stop = instance.Type.GetMethod("Stop", 0);
                m_bStopGot = true;
            }
            if (m_Stop != null)
            {
                appdomain.Invoke(m_Stop, instance, null);
            }
            else
            {

            }
        }
    }
}