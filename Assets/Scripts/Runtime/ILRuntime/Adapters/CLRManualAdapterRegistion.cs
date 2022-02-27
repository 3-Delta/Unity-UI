using System;

namespace ILRuntime.Runtime.Generated {
    public static class CLRManualAdapterRegistion {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
            appDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
            appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            appDomain.RegisterCrossBindingAdaptor(new ScriptableObjectAdapter());
            appDomain.RegisterCrossBindingAdaptor(new ExceptionAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IEnumerableAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());
        }
    }
}
