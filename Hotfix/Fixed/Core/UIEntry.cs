using System;
using System.Collections.Generic;

namespace Logic.Hotfix.Fixed
{
    [Serializable]
    public class UIEntry : FUIEntry
    {
        public FUIEntry Reset(EUIType uiType, string prefabPath, EUIOption option = EUIOption.None, EUILayer layer = EUILayer.NormalStack)
        {
            string uiTypeWithNamespace = Enum.GetName(typeof(EUIType), uiType);
            uiTypeWithNamespace = "Logic.Hotfix.HotReload." + uiTypeWithNamespace;
            return base.Reset((int)uiType, prefabPath, uiTypeWithNamespace, option, layer) as FUIEntry;
        }

        public override FUIBase CreateInstance()
        {
            return Activator.CreateInstance("Logic.Hotfix.HotReload", " Logic.Hotfix.HotReload." + uiType.ToString()).Unwrap() as FUIBase;
        }
    }

    public class UIEntryRegistry
    {
        public static void Inject()
        {
            FUIEntryRegistry.Clear();
            for (int i = 0, length = registry.Count; i < length; ++i)
            {
                FUIEntryRegistry.Register(registry[i]);
            }
        }

        private static List<FUIEntry> registry = new List<FUIEntry>() {
            new UIEntry().Reset(EUIType.UIMain, "UIMain", EUIOption.None, EUILayer.BasementStack),
            new UIEntry().Reset(EUIType.UILogin, "UILogin", EUIOption.HideBefore | EUIOption.CheckGuide | EUIOption.CheckNetwork | EUIOption.CheckQuality | EUIOption.Disable3DCamera | EUIOption.DisableBeforeRaycaster | EUIOption.Mask),
            new UIEntry().Reset(EUIType.UIWairForNetwork, "UIWairForNetwork", EUIOption.None),
        };
    }
}
