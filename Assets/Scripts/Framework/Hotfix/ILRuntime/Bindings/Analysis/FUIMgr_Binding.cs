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
    unsafe class FUIMgr_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::FUIMgr);
            args = new Type[]{};
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_0);
            args = new Type[]{typeof(System.Int32), typeof(System.Tuple<System.UInt64, System.UInt64, System.UInt64, System.Object>)};
            method = type.GetMethod("Open", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Open_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Close", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Close_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Show", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Show_3);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Hide", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Hide_4);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean)};
            method = type.GetMethod("CloseUtil", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CloseUtil_5);


        }


        static StackObject* Init_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::FUIMgr.Init();

            return __ret;
        }

        static StackObject* Open_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Tuple<System.UInt64, System.UInt64, System.UInt64, System.Object> @arg = (System.Tuple<System.UInt64, System.UInt64, System.UInt64, System.Object>)typeof(System.Tuple<System.UInt64, System.UInt64, System.UInt64, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @uiType = ptr_of_this_method->Value;


            global::FUIMgr.Open(@uiType, @arg);

            return __ret;
        }

        static StackObject* Close_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @uiType = ptr_of_this_method->Value;


            global::FUIMgr.Close(@uiType);

            return __ret;
        }

        static StackObject* Show_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @uiType = ptr_of_this_method->Value;


            global::FUIMgr.Show(@uiType);

            return __ret;
        }

        static StackObject* Hide_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @uiType = ptr_of_this_method->Value;


            global::FUIMgr.Hide(@uiType);

            return __ret;
        }

        static StackObject* CloseUtil_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @onlyJudgeOwnerStack = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @uiType = ptr_of_this_method->Value;


            global::FUIMgr.CloseUtil(@uiType, @onlyJudgeOwnerStack);

            return __ret;
        }



    }
}
