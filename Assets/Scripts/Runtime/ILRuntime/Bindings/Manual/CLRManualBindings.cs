namespace ILRuntime.Runtime.Generated {
    public static class CLRManualBindings {
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
            ManualBindings_Debug.Register(appDomain);

            ManualBindings_FUIEntry.Register(appDomain);
            ManualBindings_FUIBase.Register(appDomain);
        }
    }
}
