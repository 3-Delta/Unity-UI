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
    unsafe class FUIEntry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::FUIEntry);

            field = type.GetField("ui", flag);
            app.RegisterCLRFieldGetter(field, get_ui_0);
            app.RegisterCLRFieldSetter(field, set_ui_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ui_0, AssignFromStack_ui_0);


        }



        static object get_ui_0(ref object o)
        {
            return ((global::FUIEntry)o).ui;
        }

        static StackObject* CopyToStack_ui_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::FUIEntry)o).ui;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ui_0(ref object o, object v)
        {
            ((global::FUIEntry)o).ui = (System.Type)v;
        }

        static StackObject* AssignFromStack_ui_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Type @ui = (System.Type)typeof(System.Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::FUIEntry)o).ui = @ui;
            return ptr_of_this_method;
        }



    }
}
