using System;

using UnityEngine;

public enum EABVariantQuality {
    High = 0,
    Mid = 1,
    Low = 2,
}

public enum EABPriority {
    // 登录之前必须下载的
    Necessary = 0,
    UnNecessary,
    Max,
}

public class GlobalSetting {
    public static readonly Version AppVersion = new Version(0, 0, 0, 2);

    public const float ResolutionWidth = 2436f;
    public const float ResolutionHeight = 1125f;
    public const float ResolutionRatio = ResolutionWidth / ResolutionHeight;

    // ab变体实现高中低热更资源配置
    public static readonly EABVariantQuality ABVariantQuality = EABVariantQuality.High;
    public static readonly string[] ABVariantQualityNames = new string[] { "H", "M", "L" };

    public const string ABExtension = "unity3d";
    public static SystemLanguage Lan = SystemLanguage.ChineseSimplified;

    public static readonly long[] ByteSizes = { 1024 * 1024 * 1024, 1024 * 1024 * 1024, 1024 * 1024, 1024, 1 };
    public static readonly string[] ByteNames = { "GB", "MB", "KB", "B" };

    public static string FormatBytes(long bytes) {
        string sizeDesc = "0 B";
        if (bytes > 0) {
            for (int index = 0; index < ByteSizes.Length; ++index) {
                long edgeSize = ByteSizes[index];
                if (bytes >= edgeSize) {
                    sizeDesc = $"{bytes / edgeSize:##.##} {ByteNames[index]}";
                    break;
                }
            }
        }

        return sizeDesc;
    }
}
