Shader "ShaderControl/ChromaScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
#pragma multi_compile __ ENABLE_RED_CHANNEL
#pragma multi_compile __ ENABLE_GREEN_CHANNEL
#pragma multi_compile __ ENABLE_BLUE_CHANNEL

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				#if !ENABLE_RED_CHANNEL
					col.r = 0;
				#endif
				#if !ENABLE_GREEN_CHANNEL
					col.g = 0;
				#endif
				#if !ENABLE_BLUE_CHANNEL
					col.b = 0;
				#endif
				return col;
			}
			ENDCG
		}
	}
}
