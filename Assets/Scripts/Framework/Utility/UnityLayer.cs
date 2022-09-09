using System;

[System.Flags]
public enum ELayers
{
	All = -1, // -1

	Default = 0, // 0
	TransparentFX = 1 << 0, // 1
	IgnoreRaycast = 1 << 1, // 2
	Water = 1 << 3, // 8
	UI = 1 << 4, // 16
	UITop = 1 << 7, // 128
}

public static class CLayers
{
	public const string Default = "Default";
	public const string TransparentFX = "TransparentFX";
	public const string IgnoreRaycast = "IgnoreRaycast";
	public const string Water = "Water";
	public const string UI = "UI";
	public const string UITop = "UITop";
}
