using System.Collections.Generic;

namespace Logic.Hotfix.HotReload
{
    public class UIEntryRegistry
    {
        public static void Inject()
        {
            for (int i = 0, length = registry.Count; i < length; ++i)
            {
                FUIEntryRegistry.Register(registry[i]);
            }

            registry.Clear();
            registry = null;
        }

        private static List<FUIEntry> registry = new List<FUIEntry>() {
            new UIEntry().Reset(EUIType.UIMain, "UIMain", typeof(UIMain), EUIOption.None, EUILayer.BasementStack),
            new UIEntry().Reset(EUIType.UILogin, "UILogin", typeof(UILogin), EUIOption.HideBefore | EUIOption.CheckGuide | EUIOption.CheckNetwork | EUIOption.CheckQuality | EUIOption.Disable3DCamera | EUIOption.DisableBeforeRaycaster | EUIOption.Mask),
            new UIEntry().Reset(EUIType.UIWairForNetwork, "UIWairForNetwork", typeof(UIWairForNetwork), EUIOption.None),
        };
    }
}
