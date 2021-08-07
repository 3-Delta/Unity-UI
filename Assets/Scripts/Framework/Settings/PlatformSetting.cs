using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class Platform {
    public static string RuntimePlatformName {
        get {
            switch (Application.platform) {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                default:
                    return null;
            }
        }
    }
}
