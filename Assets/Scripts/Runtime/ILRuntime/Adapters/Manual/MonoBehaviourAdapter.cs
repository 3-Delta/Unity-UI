using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class MonoBehaviourAdapter : CrossBindingAdaptor
{
	public override Type BaseCLRType
	{
	    get
	    {
	        return typeof(UnityEngine.MonoBehaviour);// 这是你想继承的那个类
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
	    return new Adaptor(appdomain, instance);// 创建一个新的实例
	}
	// 实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
	public class Adaptor : UnityEngine.MonoBehaviour, CrossBindingAdaptorType
	{
	    ILTypeInstance instance;
	    ILRuntime.Runtime.Enviorment.AppDomain appdomain;
	    
	    public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }
        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }
	    // 缓存这个数组来避免调用时的GC Alloc
	    
	    object[] param1 = new object[1];
	    public Adaptor()
	    {
	        // 因为是通过Addcomponent添加的，所以没有调用CreateCLRInstance，所以需要外部手动设置instance/appdomain
	    }
	    public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
	    {
	        this.appdomain = appdomain;
	        this.instance = instance;
	    }

	    private IMethod mAwakeMethod;
        private bool mAwakeMethodGot;
        public void Awake()
        {
            //Unity会在ILRuntime准备好这个实例前调用Awake，所以这里暂时先不掉用
            if (instance != null)
            {
                if (!mAwakeMethodGot)
                {
                    mAwakeMethod = instance.Type.GetMethod("Awake", 0);
                    mAwakeMethodGot = true;
                }

                if (mAwakeMethod != null)
                {
                    appdomain.Invoke(mAwakeMethod, instance, null);
                }
            }
        }

        private IMethod mStartMethod;
        private bool mStartMethodGot;
        void Start()
        {
            if (!mStartMethodGot)
            {
                mStartMethod = instance.Type.GetMethod("Start", 0);
                mStartMethodGot = true;
            }

            if (mStartMethod != null)
            {
                appdomain.Invoke(mStartMethod, instance, null);
            }
        }

        private IMethod mUpdateMethod;
        private bool mUpdateMethodGot;
        void Update()
        {
            if (!mUpdateMethodGot)
            {
                mUpdateMethod = instance.Type.GetMethod("Update", 0);
                mUpdateMethodGot = true;
            }

            if (mStartMethod != null)
            {
                appdomain.Invoke(mUpdateMethod, instance, null);
            }
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
            {
                return instance.Type.FullName;
            }
        }
    }
}
