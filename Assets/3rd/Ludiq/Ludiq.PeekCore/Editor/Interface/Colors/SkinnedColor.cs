using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public struct SkinnedColor
	{
		private static readonly bool isProSkin = EditorGUIUtility.isProSkin;

		public SkinnedColor(Color personalColor, Color proColor)
		{
			this.personalColor = personalColor;
			this.proColor = proColor;
		}

		public SkinnedColor(Color personalAndProColor)
		{
			personalColor = personalAndProColor;
			proColor = personalAndProColor;
		}

		private readonly Color personalColor;
		private readonly Color proColor;

		public Color color => isProSkin ? proColor : personalColor;

		public static implicit operator Color(SkinnedColor skinnedColor)
		{
			return skinnedColor.color;
		}

		public static implicit operator SkinnedColor(Color color)
		{
			return new SkinnedColor(color);
		}

		public string ToHexString()
		{
			return color.ToHexString();
		}

		public override string ToString()
		{
			return ToHexString();
		}

		public SkinnedColor WithAlpha(float alpha)
		{
			return new SkinnedColor(personalColor.WithAlpha(alpha), proColor.WithAlpha(alpha));
		}

		public SkinnedColor WithAlpha(float personalAlpha, float proAlpha)
		{
			return new SkinnedColor(personalColor.WithAlpha(personalAlpha), proColor.WithAlpha(proAlpha));
		}

		public SkinnedColor WithAlphaMultiplied(float alphaMultiplier)
		{
			return new SkinnedColor(personalColor.WithAlphaMultiplied(alphaMultiplier), proColor.WithAlphaMultiplied(alphaMultiplier));
		}

		public SkinnedColor WithAlphaMultiplied(float personalAlphaMultiplier, float proAlphaMultiplier)
		{
			return new SkinnedColor(personalColor.WithAlphaMultiplied(personalAlphaMultiplier), proColor.WithAlphaMultiplied(proAlphaMultiplier));
		}
	}
}