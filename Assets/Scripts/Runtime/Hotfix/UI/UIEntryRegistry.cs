using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUIType {
    UILogin = 1,
}

public class UIEntryRegistry {
    public static readonly Dictionary<EUIType, FUIEntry> registry = new Dictionary<EUIType, FUIEntry>() {
        {EUIType.UILogin, new FUIEntry((int) EUIType.UILogin, "UILogin", typeof(UILogin))}
    };
    
    public static void Inject() {
        foreach (var kvp in registry) {
            FUIEntryRegistry.Register(kvp.Value);
        }
    }
}
