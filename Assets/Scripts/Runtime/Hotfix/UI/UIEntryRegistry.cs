using System.Collections.Generic;

public class UIEntryRegistry {
    public static void Inject() {
        foreach (var kvp in registry) {
            FUIEntryRegistry.Register(kvp.Value);
        }

        registry.Clear();
        registry = null;
    }
    
    private static Dictionary<EUIType, FUIEntry> registry = new Dictionary<EUIType, FUIEntry>() {
        {
            EUIType.UIMain, new FUIEntry((int)EUIType.UIMain, "UIMain", typeof(UIMain), EUIOption.None, EUILayer.BasementStack)
        }, {
            EUIType.UILogin, new FUIEntry((int)EUIType.UILogin, "UILogin", typeof(UILogin),
                EUIOption.HideBefore | EUIOption.CheckGuide | EUIOption.CheckNetwork | EUIOption.CheckQuality | EUIOption.Disable3DCamera | EUIOption.DisableBeforeRaycaster | EUIOption.Mask)
        },
    };
}
