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
    unsafe class ManualBindings_FUIBase {
        private static MethodBase method_OnLoaded;
        private static MethodBase method_OnTransfer;
        private static MethodBase method_OnOpen;
        private static MethodBase method_OnClose;
        private static MethodBase method_OnOpened;
        private static MethodBase method_OnShow;
        private static MethodBase method_OnHide;
        private static MethodBase method_ProcessEvent;
        private static MethodBase method_ProcessEventForShowHide;

        private static object[] args = new object[1];

        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app) {
            // 热更层子类会调用主工程基类的protected成员，所以这里需要加上BindingFlags.NonPublic
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.NonPublic;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::FUIBase);

            args = new Type[] { };
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

            args = new Type[] { typeof(System.Int32), typeof(global::FUIEntry) };
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_0);

            args = new Type[] { typeof(System.Boolean) };
            method = type.GetMethod("BlockRaycaster", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BlockRaycaster_1);

            args = new Type[] { typeof(System.Int32) };
            method = type.GetMethod("SetOrder", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetOrder_2);

            args = new Type[] { };
            method = type.GetMethod("CloseSelf", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CloseSelf_3);

            field = type.GetField("uiType", flag);
            app.RegisterCLRFieldGetter(field, get_uiType_0);
            app.RegisterCLRFieldSetter(field, set_uiType_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_uiType_0, AssignFromStack_uiType_0);

            field = type.GetField("cfg", flag);
            app.RegisterCLRFieldGetter(field, get_cfg_1);
            app.RegisterCLRFieldSetter(field, set_cfg_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_cfg_1, AssignFromStack_cfg_1);

            args = new Type[] { typeof(UnityEngine.Transform) };
            method_OnLoaded = method = type.GetMethod("OnLoaded", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _OnLoaded);

            args = new Type[] { typeof(System.Object) };
            method_OnTransfer = method = type.GetMethod("OnTransfer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _OnTransfer);

            args = new Type[] { typeof(System.Object) };
            method_OnOpen = method = type.GetMethod("OnTransfer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _OnOpen);

            args = new Type[] { };
            method_OnClose = method = type.GetMethod("OnClose", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _OnClose);

            args = new Type[] { };
            method_OnOpened = method = type.GetMethod("OnOpened", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _OnOpened);

            args = new Type[] { };
            method_OnShow = method = type.GetMethod("OnShow", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _OnShow);

            args = new Type[] { };
            method_OnHide = method = type.GetMethod("OnHide", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _OnHide);

            args = new Type[] { typeof(System.Boolean) };
            method_ProcessEvent = method = type.GetMethod("ProcessEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _ProcessEvent);

            args = new Type[] { typeof(System.Boolean) };
            method_ProcessEventForShowHide = method = type.GetMethod("ProcessEventForShowHide", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, _ProcessEventForShowHide);
        }

        static StackObject* Init_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::FUIEntry @cfg = (global::FUIEntry)typeof(global::FUIEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @uiType = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Init(@uiType, @cfg);

            return __ret;
        }

        static StackObject* BlockRaycaster_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @toBlcok = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BlockRaycaster(@toBlcok);

            return __ret;
        }

        static StackObject* SetOrder_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @order = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetOrder(@order);

            return __ret;
        }

        static StackObject* CloseSelf_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CloseSelf();

            return __ret;
        }

        static object get_uiType_0(ref object o) {
            return ((global::FUIBase)o).uiType;
        }

        static StackObject* CopyToStack_uiType_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack) {
            var result_of_this_method = ((global::FUIBase)o).uiType;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_uiType_0(ref object o, object v) {
            ((global::FUIBase)o).uiType = (System.Int32)v;
        }

        static StackObject* AssignFromStack_uiType_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @uiType = ptr_of_this_method->Value;
            ((global::FUIBase)o).uiType = @uiType;
            return ptr_of_this_method;
        }

        static object get_cfg_1(ref object o) {
            return ((global::FUIBase)o).cfg;
        }

        static StackObject* CopyToStack_cfg_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack) {
            var result_of_this_method = ((global::FUIBase)o).cfg;
            object obj_result_of_this_method = result_of_this_method;
            if (obj_result_of_this_method is CrossBindingAdaptorType) {
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_cfg_1(ref object o, object v) {
            ((global::FUIBase)o).cfg = (global::FUIEntry)v;
        }

        static StackObject* AssignFromStack_cfg_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::FUIEntry @cfg = (global::FUIEntry)typeof(global::FUIEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::FUIBase)o).cfg = @cfg;
            return ptr_of_this_method;
        }

        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new global::FUIBase();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* _OnLoaded(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Transform transform = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            args[0] = transform;
            method_OnLoaded.Invoke(instance_of_this_method, args);

            return __ret;
        }

        static StackObject* _OnTransfer(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Tuple<ulong, ulong, ulong, object> @o = (Tuple<ulong, ulong, ulong, object>)typeof(Tuple<ulong, ulong, ulong, object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            args[0] = @o;
            method_OnTransfer.Invoke(instance_of_this_method, args);

            return __ret;
        }

        static StackObject* _OnOpen(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Tuple<ulong, ulong, ulong, object> @o = (Tuple<ulong, ulong, ulong, object>)typeof(Tuple<ulong, ulong, ulong, object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            args[0] = @o;
            method_OnOpen.Invoke(instance_of_this_method, args);

            return __ret;
        }

        static StackObject* _ProcessEvent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            bool toListen = ((ptr_of_this_method->Value) != 0);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            method_ProcessEventForShowHide.Invoke(instance_of_this_method, null);

            return __ret;
        }

        static StackObject* _ProcessEventForShowHide(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            bool toListen = ((ptr_of_this_method->Value) != 0);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            method_ProcessEventForShowHide.Invoke(instance_of_this_method, null);

            return __ret;
        }

        static StackObject* _OnClose(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            method_OnClose.Invoke(instance_of_this_method, null);

            return __ret;
        }

        static StackObject* _OnOpened(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            method_OnOpened.Invoke(instance_of_this_method, null);

            return __ret;
        }

        static StackObject* _OnShow(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            method_OnShow.Invoke(instance_of_this_method, null);

            return __ret;
        }

        static StackObject* _OnHide(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::FUIBase instance_of_this_method = (global::FUIBase)typeof(global::FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            // 反射调用protected成员
            method_OnHide.Invoke(instance_of_this_method, null);

            return __ret;
        }
    }
}
