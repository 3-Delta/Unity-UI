/*
    https://blog.uwa4d.com/archives/USparkle_Shader.html
    https://zhuanlan.zhihu.com/p/78589597
    https://zhuanlan.zhihu.com/p/93194054
    https://zhuanlan.zhihu.com/p/47506575
*/

Shader "UI/Default"
{
    Properties
    {
        _Color ("Tint", Color) = (1, 1, 1, 1)
        [PerRendererData] _MainTex ("Sprite Texture(RGBA)", 2D) = "white" {}
        
        // 圆角裁切
        [Toggle]/* [IntRange]*/ [Enum(ESwitch)] _InvertAlphaMask("InvertAlphaMask", Range(0, 1)) = 0
        [NoScaleOffset] _AlphaMaskTex("AlphaMask(A)", 2D) = "white" {}
        
        [Header(Stencil)]
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("StencilComparison", Float) = 8
        _Stencil ("RefValue", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilPass ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilFail ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilZFail ("Stencil ZFail", Float) = 0

        [Header(Blend)]
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("BlendOp", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcColorFactor ("SrcColorFactor", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DestColorFactor ("DestColorFactor", Float) = 10
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaFactor ("SrcAlphaFactor", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DestAlphaFactor ("DestAlphaFactor", Float) = 10

        [Enum(Nothing, 0, A, 1, B, 2, G, 4, R, 8, RGB, 14, RGBA, 15)] _ColorMask ("ColorMask", Float) = 15
        // [Enum(EColorMask)] _ColorMask ("ColorMask", Float) = 15
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
        // (Off, 0, On, 1)
        [Toggle] /* [IntRange]*/ [Enum(ESwitch)] _ZWrite ("ZWrite", Range(0, 1)) = 0
        
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode ("CullMode", Range(0, 1)) = 0
        
        [HideInInspector] _ClipRect ("ClipRect", Vector) = (0, 0, 0, 0)

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        // 置灰
        [Toggle] /* [IntRange]*/ [Enum(ESwitch)] _IsGrey("IsGrey", Range(0, 1)) = 0
        
        // 旋转缩放
        _MeshScale ("MeshScale", float) = 0

        // https://zhuanlan.zhihu.com/p/84867268 shader中通过Marco实现纹理的wrapMode,而不是通过纹理的设置实现 
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Pass [_StencilPass]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }

        Lighting Off
        Cull [_CullMode]
        ZWrite [_ZWrite]
        ZTest [_ZTest] // [unity_GUIZTestMode]
        Blend [_SrcColorFactor] [_DestColorFactor], [_SrcAlphaFactor] [_DestAlphaFactor]
        ColorMask [_ColorMask]

        Pass
        {
            Name "UI_Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma enable_d3d11_debug_symbols
            
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _AlphaMaskTex;
            
            fixed4 _Color;
            fixed _IsGrey;
            half _MeshScale;
            fixed _InvertAlphaMask;
        
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                // https://edu.uwa4d.com/lesson-detail/197/1120/1?isPreview=0
                // 顶点坐标设置_MeshScale偏移[以mesh中心点为缩放中心]
                v.vertex.xy += ((v.texcoord - float2(0.5f, 0.5f)) * _MeshScale);
                
                // 设置不同的Pivot是否存在影响？
                // v.vertex.xy += ((v.vertex.xy - float2(0.5f, 0.5f)) * _MeshScale);
                
                // 这里只是一个LocalSpace，为什么充当WorldSPace? 关键结果还是正确的！
                // 因为本质上UGUI 使用Canvas作为一个整体的subMesh进行渲染，所以最终canvas上每个graphic都不会存在自己的localspace,都是以canvas这个submesh作为渲染单元进行的，然后
                // _CLipRect也是以canvas为基准，各个rectmask2d的的相对于canvas的一个偏移的rect,那么双方相当于都在一个 统一的以canvas为基准的坐标系中，自然可以进行合理的计算。
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {                
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                
                #ifdef UNITY_UI_CLIP_RECT
                    float2 pos = IN.worldPosition.xy;
                    color.a *= UnityGet2DClipping(pos, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    // 圆角裁切
                    _InvertAlphaMask = 1 - step(_InvertAlphaMask, 0.5);
                    half maskAlpha = tex2D(_AlphaMaskTex, IN.texcoord).a;
                    maskAlpha = lerp(maskAlpha, 1 - maskAlpha, _InvertAlphaMask);
                    color.a *= maskAlpha;
                    
                    // 最原始，是否需要删除？
                    clip (color.a - 0.001);
                #endif

                // 矫正[0, 1]为 0和1 两个数值
                _IsGrey = 1 - step(_IsGrey, 0.5);
                // 之前犯了愚蠢的错误，认为在ui层面这是gray就行，后来发现不行，因为gray是针对于每个像素进行的处理
                color.rgb = lerp(color.rgb, Luminance(color.rgb), _IsGrey);
                
                return color;
            }
        ENDCG
        }
    }
}
