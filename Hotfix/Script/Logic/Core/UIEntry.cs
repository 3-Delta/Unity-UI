using System;
using System.Collections.Generic;

using Logic.Hotfix.HotReload;

namespace Logic.Hotfix
{
    [Serializable]
    public class UIEntry : FUIEntry
    {
        public FUIEntry Reset(EUIType uiType, string prefabPath, Type ui, EUIOption option = EUIOption.None, EUILayer layer = EUILayer.NormalStack)
        {
            return base.Reset((int)uiType, prefabPath, ui, option, layer) as FUIEntry;
        }

        public override FUIBase CreateInstance()
        {
            return Activator.CreateInstance(ui) as FUIBase;
        }
    }

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
