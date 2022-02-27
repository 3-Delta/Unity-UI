using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class AwakeDestroy_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::AwakeDestroy);

            field = type.GetField("onTrigger", flag);
            app.RegisterCLRFieldGetter(field, get_onTrigger_0);
            app.RegisterCLRFieldSetter(field, set_onTrigger_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onTrigger_0, AssignFromStack_onTrigger_0);


        }



        static object get_onTrigger_0(ref object o)
        {
            return ((global::AwakeDestroy)o).onTrigger;
        }

        static StackObject* CopyToStack_onTrigger_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::AwakeDestroy)o).onTrigger;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onTrigger_0(ref object o, object v)
        {
            ((global::AwakeDestroy)o).onTrigger = (System.Action<System.Boolean>)v;
        }

        static StackObject* AssignFromStack_onTrigger_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Boolean> @onTrigger = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::AwakeDestroy)o).onTrigger = @onTrigger;
            return ptr_of_this_method;
        }



    }
}
