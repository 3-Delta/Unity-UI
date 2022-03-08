namespace ILRuntime.Runtime.Generated {
    public static class CLRManualAdapterRegister {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
            // appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

            appDomain.RegisterCrossBindingAdaptor(new ScriptableObjectAdapter());
            appDomain.RegisterCrossBindingAdaptor(new ExceptionAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new IDisposableAdapter());

            appDomain.RegisterCrossBindingAdaptor(new IEnumerableAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IComparableObjectAdapter());

            appDomain.RegisterCrossBindingAdaptor(new IComparerIntAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IComparerObjectAdapter());

            appDomain.RegisterCrossBindingAdaptor(new IEqualityComparerIntAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IEqualityComparerObjectAdapter());
            
            appDomain.RegisterCrossBindingAdaptor(new IMessageAdapter());
            
            appDomain.RegisterCrossBindingAdaptor(new FUIBaseAdapter());
            appDomain.RegisterCrossBindingAdaptor(new FUIEntryAdapter());
            appDomain.RegisterCrossBindingAdaptor(new FUIMgrAdapter());
        }
    }
}
