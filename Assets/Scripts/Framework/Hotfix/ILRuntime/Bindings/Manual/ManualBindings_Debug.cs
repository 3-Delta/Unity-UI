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

namespace ILRuntime.Runtime.Generated {
    unsafe class ManualBindings_Debug {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app) {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.Debug);
            args = new Type[] {typeof(System.Object)};
            method = type.GetMethod("LogError", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogError_0);
            args = new Type[] {typeof(System.String), typeof(System.Object[])};
            method = type.GetMethod("LogErrorFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogErrorFormat_1);
            args = new Type[] {typeof(System.Object)};
            method = type.GetMethod("Log", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Log_2);
            args = new Type[] {typeof(System.String), typeof(System.Object[])};
            method = type.GetMethod("LogFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogFormat_3);
            args = new Type[] {typeof(System.Object)};
            method = type.GetMethod("LogWarning", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogWarning_4);
            args = new Type[] {typeof(System.String), typeof(System.Object[])};
            method = type.GetMethod("LogWarningFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogWarningFormat_5);
            args = new Type[] { };
            method = type.GetMethod("DebugBreak", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DebugBreak_6);
        }

        static StackObject* LogError_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @message = (System.Object) typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            UnityEngine.Debug.LogError(@message + " \n" + stackTrace);

            return __ret;
        }

        static StackObject* LogErrorFormat_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[]) typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @format = (System.String) typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            string output = string.Format(@format, @args) + " \n" + stackTrace;
            UnityEngine.Debug.LogError(output);

            return __ret;
        }

        static StackObject* Log_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @message = (System.Object) typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            UnityEngine.Debug.Log(@message + " \n" + stackTrace);

            return __ret;
        }

        static StackObject* LogFormat_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[]) typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @format = (System.String) typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            string output = string.Format(@format, @args) + " \n" + stackTrace;
            UnityEngine.Debug.Log(output);

            return __ret;
        }

        static StackObject* LogWarning_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @message = (System.Object) typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            UnityEngine.Debug.LogWarning(@message + " \n" + stackTrace);

            return __ret;
        }

        static StackObject* LogWarningFormat_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[]) typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @format = (System.String) typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags) 0);
            __intp.Free(ptr_of_this_method);

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            string output = string.Format(@format, @args) + " \n" + stackTrace;
            UnityEngine.Debug.LogWarning(output);

            return __ret;
        }

        static StackObject* DebugBreak_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            UnityEngine.Debug.DebugBreak();

            return __ret;
        }
    }
}
