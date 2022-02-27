using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IEquatable_1Adapter : CrossBindingAdaptor
{
	public override Type BaseCLRType
	{
	    get
	    {
	        return typeof(System.IEquatable<ILRuntime.Runtime.Intepreter.ILTypeInstance>);// 这是你想继承的那个类
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
	public class Adaptor : System.IEquatable<ILRuntime.Runtime.Intepreter.ILTypeInstance>, CrossBindingAdaptorType
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

		bool m_bEqualsGot = false;
		bool m_bEqualsGotVirtual = false;
		IMethod m_Equals = null;
		public System.Boolean Equals (ILRuntime.Runtime.Intepreter.ILTypeInstance arg0)
		{
		   if(!m_bEqualsGot)
		   {
		       m_Equals = instance.Type.GetMethod("Equals", 1);
		       m_bEqualsGot = true;
		   }
		   if(m_Equals != null)
			{
				var result = (System.Boolean)appdomain.Invoke(m_Equals, instance, arg0);
 				return result;
			}
			else
			{
				return default(System.Boolean);
			} 
		}
}
}