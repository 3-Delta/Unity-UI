using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class ExceptionAdapter : CrossBindingAdaptor
{
	public override Type BaseCLRType
	{
	    get
	    {
	        return typeof(System.Exception);// 这是你想继承的那个类
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
	public class Adaptor : System.Exception, CrossBindingAdaptorType
	{
	    ILTypeInstance instance;
	    ILRuntime.Runtime.Enviorment.AppDomain appdomain;
	    // 缓存这个数组来避免调用时的GC Alloc
	    object[] param1 = new object[1];
	    public Adaptor()
	    {
	
	    }
	    public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
	    {
	        this.appdomain = appdomain;
	        this.instance = instance;
	    }
	    public ILTypeInstance ILInstance { get { return instance; } }

		bool m_bget_MessageGot = false;
		bool m_bget_MessageGotVirtual = false;
		IMethod m_get_Message = null;
		public new System.String get_Message ()
		{
		   if(!m_bget_MessageGot)
		   {
		       m_get_Message = instance.Type.GetMethod("get_Message", 0);
		       m_bget_MessageGot = true;
		   }
		   if(m_get_Message != null)
			{
				var result = (System.String)appdomain.Invoke(m_get_Message, instance, null);
 				return result;
			}
			else
			{
				return default(System.String);
			} 
		}

		bool m_bget_DataGot = false;
		bool m_bget_DataGotVirtual = false;
		IMethod m_get_Data = null;
		public new System.Collections.IDictionary get_Data ()
		{
		   if(!m_bget_DataGot)
		   {
		       m_get_Data = instance.Type.GetMethod("get_Data", 0);
		       m_bget_DataGot = true;
		   }
		   if(m_get_Data != null)
			{
				var result = (System.Collections.IDictionary)appdomain.Invoke(m_get_Data, instance, null);
 				return result;
			}
			else
			{
				return null;
			} 
		}

		bool m_bGetBaseExceptionGot = false;
		bool m_bGetBaseExceptionGotVirtual = false;
		IMethod m_GetBaseException = null;
		public new System.Exception GetBaseException ()
		{
		   if(!m_bGetBaseExceptionGot)
		   {
		       m_GetBaseException = instance.Type.GetMethod("GetBaseException", 0);
		       m_bGetBaseExceptionGot = true;
		   }
		   if(m_GetBaseException != null)
			{
				var result = (System.Exception)appdomain.Invoke(m_GetBaseException, instance, null);
 				return result;
			}
			else
			{
				return default(System.Exception);
			} 
		}

		bool m_bget_InnerExceptionGot = false;
		bool m_bget_InnerExceptionGotVirtual = false;
		IMethod m_get_InnerException = null;
		public new System.Exception get_InnerException ()
		{
		   if(!m_bget_InnerExceptionGot)
		   {
		       m_get_InnerException = instance.Type.GetMethod("get_InnerException", 0);
		       m_bget_InnerExceptionGot = true;
		   }
		   if(m_get_InnerException != null)
			{
				var result = (System.Exception)appdomain.Invoke(m_get_InnerException, instance, null);
 				return result;
			}
			else
			{
				return default(System.Exception);
			} 
		}

		bool m_bget_TargetSiteGot = false;
		bool m_bget_TargetSiteGotVirtual = false;
		IMethod m_get_TargetSite = null;
		public new System.Reflection.MethodBase get_TargetSite ()
		{
		   if(!m_bget_TargetSiteGot)
		   {
		       m_get_TargetSite = instance.Type.GetMethod("get_TargetSite", 0);
		       m_bget_TargetSiteGot = true;
		   }
		   if(m_get_TargetSite != null)
			{
				var result = (System.Reflection.MethodBase)appdomain.Invoke(m_get_TargetSite, instance, null);
 				return result;
			}
			else
			{
				return default(System.Reflection.MethodBase);
			} 
		}

		bool m_bget_StackTraceGot = false;
		bool m_bget_StackTraceGotVirtual = false;
		IMethod m_get_StackTrace = null;
		public new System.String get_StackTrace ()
		{
		   if(!m_bget_StackTraceGot)
		   {
		       m_get_StackTrace = instance.Type.GetMethod("get_StackTrace", 0);
		       m_bget_StackTraceGot = true;
		   }
		   if(m_get_StackTrace != null)
			{
				var result = (System.String)appdomain.Invoke(m_get_StackTrace, instance, null);
 				return result;
			}
			else
			{
				return default(System.String);
			} 
		}

		bool m_bget_HelpLinkGot = false;
		bool m_bget_HelpLinkGotVirtual = false;
		IMethod m_get_HelpLink = null;
		public new System.String get_HelpLink ()
		{
		   if(!m_bget_HelpLinkGot)
		   {
		       m_get_HelpLink = instance.Type.GetMethod("get_HelpLink", 0);
		       m_bget_HelpLinkGot = true;
		   }
		   if(m_get_HelpLink != null)
			{
				var result = (System.String)appdomain.Invoke(m_get_HelpLink, instance, null);
 				return result;
			}
			else
			{
				return default(System.String);
			} 
		}

		bool m_bset_HelpLinkGot = false;
		bool m_bset_HelpLinkGotVirtual = false;
		IMethod m_set_HelpLink = null;
		public new void set_HelpLink (System.String arg0)
		{
		   if(!m_bset_HelpLinkGot)
		   {
		       m_set_HelpLink = instance.Type.GetMethod("set_HelpLink", 1);
		       m_bset_HelpLinkGot = true;
		   }
		   if(m_set_HelpLink != null)
			{
				appdomain.Invoke(m_set_HelpLink, instance, arg0);
 				
			}
			else
			{
				
			} 
		}

		bool m_bget_SourceGot = false;
		bool m_bget_SourceGotVirtual = false;
		IMethod m_get_Source = null;
		public new System.String get_Source ()
		{
		   if(!m_bget_SourceGot)
		   {
		       m_get_Source = instance.Type.GetMethod("get_Source", 0);
		       m_bget_SourceGot = true;
		   }
		   if(m_get_Source != null)
			{
				var result = (System.String)appdomain.Invoke(m_get_Source, instance, null);
 				return result;
			}
			else
			{
				return default(System.String);
			} 
		}

		bool m_bset_SourceGot = false;
		bool m_bset_SourceGotVirtual = false;
		IMethod m_set_Source = null;
		public new void set_Source (System.String arg0)
		{
		   if(!m_bset_SourceGot)
		   {
		       m_set_Source = instance.Type.GetMethod("set_Source", 1);
		       m_bset_SourceGot = true;
		   }
		   if(m_set_Source != null)
			{
				appdomain.Invoke(m_set_Source, instance, arg0);
 				
			}
			else
			{
				
			} 
		}

		bool m_bToStringGot = false;
		bool m_bToStringGotVirtual = false;
		IMethod m_ToString = null;
		public new System.String ToString ()
		{
		   if(!m_bToStringGot)
		   {
		       m_ToString = instance.Type.GetMethod("ToString", 0);
		       m_bToStringGot = true;
		   }
		   if(m_ToString != null)
			{
				var result = (System.String)appdomain.Invoke(m_ToString, instance, null);
 				return result;
			}
			else
			{
				return default(System.String);
			} 
		}

		bool m_bGetObjectDataGot = false;
		bool m_bGetObjectDataGotVirtual = false;
		IMethod m_GetObjectData = null;
		public new void GetObjectData (System.Runtime.Serialization.SerializationInfo arg0,System.Runtime.Serialization.StreamingContext arg1)
		{
		   if(!m_bGetObjectDataGot)
		   {
		       m_GetObjectData = instance.Type.GetMethod("GetObjectData", 2);
		       m_bGetObjectDataGot = true;
		   }
		   if(m_GetObjectData != null)
			{
				appdomain.Invoke(m_GetObjectData, instance, arg0,arg1);
 				
			}
			else
			{
				
			} 
		}

		bool m_bGetTypeGot = false;
		bool m_bGetTypeGotVirtual = false;
		IMethod m_GetType = null;
		public new System.Type GetType ()
		{
		   if(!m_bGetTypeGot)
		   {
		       m_GetType = instance.Type.GetMethod("GetType", 0);
		       m_bGetTypeGot = true;
		   }
		   if(m_GetType != null)
			{
				var result = (System.Type)appdomain.Invoke(m_GetType, instance, null);
 				return result;
			}
			else
			{
				return default(System.Type);
			} 
		}
}
}