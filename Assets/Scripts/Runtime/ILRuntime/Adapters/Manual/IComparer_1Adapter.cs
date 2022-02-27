using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IComparer_1Adapter : CrossBindingAdaptor
{
	public override Type BaseCLRType
	{
	    get
	    {
	        return typeof(System.Collections.Generic.IComparer<ILRuntime.Runtime.Intepreter.ILTypeInstance>);// 这是你想继承的那个类
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
	public class Adaptor : System.Collections.Generic.IComparer<ILRuntime.Runtime.Intepreter.ILTypeInstance>, CrossBindingAdaptorType
	{
	    ILTypeInstance instance;
	    ILRuntime.Runtime.Enviorment.AppDomain appdomain;

	    public Adaptor()
	    {
	
	    }
	    public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
	    {
	        this.appdomain = appdomain;
	        this.instance = instance;
	    }
	    public ILTypeInstance ILInstance { get { return instance; } }

		bool m_bCompareGot = false;
		bool m_bCompareGotVirtual = false;
		IMethod m_Compare = null;
		public System.Int32 Compare (ILRuntime.Runtime.Intepreter.ILTypeInstance arg0,ILRuntime.Runtime.Intepreter.ILTypeInstance arg1)
		{
		   if(!m_bCompareGot)
		   {
		       m_Compare = instance.Type.GetMethod("Compare", 2);
		       m_bCompareGot = true;
		   }
		   if(m_Compare != null)
			{
				var result = (System.Int32)appdomain.Invoke(m_Compare, instance, arg0,arg1);
 				return result;
			}
			else
			{
				return default(System.Int32);
			} 
		}
}
}