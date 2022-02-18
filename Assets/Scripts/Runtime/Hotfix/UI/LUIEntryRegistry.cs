using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUIType {
    UILogin = 1,
}

public class LUIEntryRegistry {
    public static readonly Dictionary<EUIType, UIEntry> entries = new Dictionary<EUIType, UIEntry>() {
        {EUIType.UILogin, new UIEntry((int) EUIType.UILogin, "UI/Login/UILogin.prefab", typeof(UILogin))}
    };

    public static void Inject() {
        foreach (var kvp in entries) {
            FUIEntryRegistry.Register(kvp.Value);
        }
    }
}
