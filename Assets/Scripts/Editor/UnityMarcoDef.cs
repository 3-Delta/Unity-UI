using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public enum EMarcoType {
    Native,
    ReflReload,
    Refl,
    Ilr,
}

public class DefMarco {
    public const string __NATIVE__ = "__NATIVE__";
    public const string __REFL_RELOAD__ = "__REFL_RELOAD__";
    public const string __REFL__ = "__REFL__";
    public const string __ILR__ = "__ILR__";

    public static readonly List<string> allMarcos = new List<string>() {
        __NATIVE__,
        __REFL_RELOAD__,
        __REFL__,
        __ILR__,
    };
}

public static class UnityMarcoDef {
    public static void SetMarco(EMarcoType type) {
        switch (type) {
            case EMarcoType.Native:
                UnityMarco.ChangeMarco(DefMarco.allMarcos, DefMarco.__NATIVE__);
                break;
            case EMarcoType.ReflReload:
                UnityMarco.ChangeMarco(DefMarco.allMarcos, DefMarco.__REFL_RELOAD__);
                break;
            case EMarcoType.Refl:
                UnityMarco.ChangeMarco(DefMarco.allMarcos, DefMarco.__REFL__);
                break;
            case EMarcoType.Ilr:
                UnityMarco.ChangeMarco(DefMarco.allMarcos, DefMarco.__ILR__);
                break;
        }
    }
}
