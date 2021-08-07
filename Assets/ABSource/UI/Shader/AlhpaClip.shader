// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlhpaClip"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "black" {}
		_AlphaTex("AlphaMask", 2D) = "white" {}
		_Greyed("IsGrey", Range(0, 1)) = 0.0
		_OutSide("IsOutSide", Range(0, 1)) = 0.0
		_MaskMultiply("MaskMultiply", float) = 1.0
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float4 _MainTex_ST;
			float _Greyed;
			float _OutSide;
			float _MaskMultiply;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f o;
			v2f vert(appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				// 纹理颜色 与 Mesh顶点颜色[即比如Image的Color属性的颜色] 相乘
				fixed4 ret = tex2D(_MainTex, IN.texcoord) * IN.color;
				fixed maskAlpha = tex2D(_AlphaTex, IN.texcoord).a;
				if (_OutSide > 0.5)
				{
					ret.a *= (1 - maskAlpha);
				}
				else
				{
					ret.a *= maskAlpha;
				}

				if (_Greyed > 0.5)
				{
					fixed greyColor = ret.r * 0.299 + ret.g * 0.587 + ret.b * 0.114;
					ret.rgb = greyColor;
				}
				return ret;
			}
			ENDCG
		}
	}			
}
