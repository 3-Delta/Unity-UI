using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

public class FUIBaseAdapter : CrossBindingAdaptor {
    public override Type BaseCLRType {
        get {
            return typeof(FUIBase); // 这是你想继承的那个类
        }
    }

    public override Type AdaptorType {
        get {
            return typeof(Adaptor); // 这是实际的适配器类
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
        return new Adaptor(appdomain, instance); // 创建一个新的实例
    }

    // 实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    public class Adaptor : FUIBase, CrossBindingAdaptorType {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public ILTypeInstance ILInstance {
            get { return instance; }
            set { instance = value; }
        }

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain {
            get { return appdomain; }
            set { appdomain = value; }
        }
        // 缓存这个数组来避免调用时的GC Alloc

        object[] args = new object[1];

        public Adaptor() {
            // 因为是通过Addcomponent添加的，所以没有调用CreateCLRInstance，所以需要外部手动设置instance/appdomain
        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        bool hasCloseGot = false;
        bool hasVirtualCloseGot = false;
        IMethod _onClose = null;

        protected override void OnClose() {
            if (!hasCloseGot) {
                _onClose = instance.Type.GetMethod("OnCloses", 0);
                hasCloseGot = true;
            }

            if (_onClose != null && !hasVirtualCloseGot) {
                hasVirtualCloseGot = true;
                appdomain.Invoke(_onClose, instance, null);
                hasVirtualCloseGot = false;
            }
            else {
                base.OnClose();
            }
        }
        
        bool hasHideGot = false;
        bool hasVirtualHideGot = false;
        IMethod _onHide = null;

        protected override void OnHide() {
            if (!hasHideGot) {
                _onHide = instance.Type.GetMethod("OnHide", 0);
                hasHideGot = true;
            }

            if (_onHide != null && !hasVirtualHideGot) {
                hasVirtualHideGot = true;
                appdomain.Invoke(_onHide, instance, null);
                hasVirtualHideGot = false;
            }
            else {
                base.OnHide();
            }
        }

        bool hasLoadedGot = false;
        bool hasVirtualLoadedGot = false;
        IMethod _onLoaded = null;
        protected override void OnLoaded(Transform transform) {
            if (!hasLoadedGot) {
                _onLoaded = instance.Type.GetMethod("OnLoaded", 1);
                hasLoadedGot = true;
            }

            if (_onLoaded != null && !hasVirtualLoadedGot) {
                hasVirtualLoadedGot = true;
                args[0] = transform;
                appdomain.Invoke(_onLoaded, instance, args);
                hasVirtualLoadedGot = false;
            }
            else {
                base.OnLoaded(transform);
            }
        }

        bool hasOpenGot = false;
        bool hasVirtualOpenGot = false;
        IMethod _onOpen = null;
        protected override void OnOpen(Tuple<ulong, ulong, ulong, object> arg) {
            if (!hasOpenGot) {
                _onOpen = instance.Type.GetMethod("OnOpen", 1);
                hasOpenGot = true;
            }

            if (_onOpen != null && !hasVirtualOpenGot) {
                hasVirtualOpenGot = true;
                args[0] = arg;
                appdomain.Invoke(_onOpen, instance, arg);
                hasVirtualOpenGot = false;
            }
            else {
                base.OnOpen(arg);
            }
        }

        bool hasOpenedGot = false;
        bool hasVirtualOpenedGot = false;
        IMethod _onOpened = null;
        protected override void OnOpened() {
            if (!hasOpenedGot) {
                _onOpened = instance.Type.GetMethod("OnOpened", 0);
                hasOpenedGot = true;
            }

            if (_onOpened != null && !hasVirtualOpenedGot) {
                hasVirtualOpenedGot = true;
                appdomain.Invoke(_onOpened, instance, null);
                hasVirtualOpenedGot = false;
            }
            else {
                base.OnOpened();
            }
        }

        bool hasShowGot = false;
        bool hasVirtualShowGot = false;
        IMethod _onShow = null;
        protected override void OnShow() {
            if (!hasShowGot) {
                _onShow = instance.Type.GetMethod("OnShow", 0);
                hasShowGot = true;
            }

            if (_onShow != null && !hasVirtualShowGot) {
                hasVirtualShowGot = true;
                appdomain.Invoke(_onShow, instance, null);
                hasVirtualShowGot = false;
            }
            else {
                base.OnShow();
            }
        }

        bool hasTransferGot = false;
        bool hasVirtualTransferGot = false;
        IMethod _onTransfer = null;
        protected override void OnTransfer(Tuple<ulong, ulong, ulong, object> arg) {
            if (!hasTransferGot) {
                _onTransfer = instance.Type.GetMethod("OnTransfer", 1);
                hasTransferGot = true;
            }

            if (_onTransfer != null && !hasVirtualTransferGot) {
                hasVirtualTransferGot = true;
                args[0] = arg;
                appdomain.Invoke(_onTransfer, instance, args);
                hasVirtualTransferGot = false;
            }
            else {
                base.OnTransfer(arg);
            }
        }

        bool hasProcessEventGot = false;
        bool hasVirtualProcessEventGot = false;
        IMethod _ProcessEvent = null;
        protected override void ProcessEvent(bool toListen) {
            if (!hasProcessEventGot) {
                _ProcessEvent = instance.Type.GetMethod("ProcessEvent", 1);
                hasProcessEventGot = true;
            }

            if (_ProcessEvent != null && !hasVirtualProcessEventGot) {
                hasVirtualProcessEventGot = true;
                args[0] = toListen;
                appdomain.Invoke(_ProcessEvent, instance, args);
                hasVirtualProcessEventGot = false;
            }
            else {
                base.ProcessEvent(toListen);
            }
        }

        bool hasProcessEventForShowHideGot = false;
        bool hasVirtualProcessEventForShowHideGot = false;
        IMethod _ProcessEventForShowHide = null;
        protected override void ProcessEventForShowHide(bool toListen) {
            if (!hasProcessEventForShowHideGot) {
                _ProcessEventForShowHide = instance.Type.GetMethod("ProcessEventForShowHide", 1);
                hasProcessEventForShowHideGot = true;
            }

            if (_ProcessEventForShowHide != null && !hasVirtualProcessEventForShowHideGot) {
                hasVirtualProcessEventForShowHideGot = true;
                args[0] = toListen;
                appdomain.Invoke(_ProcessEventForShowHide, instance, args);
                hasVirtualProcessEventForShowHideGot = false;
            }
            else {
                base.ProcessEventForShowHide(toListen);
            }
        }
    }
}
