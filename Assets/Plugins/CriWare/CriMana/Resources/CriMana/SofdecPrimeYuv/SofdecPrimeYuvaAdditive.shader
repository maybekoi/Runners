﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CriMana/SofdecPrimeYuvaAdditive"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[HideInInspector] _MovieTexture_ST ("MovieTexture_ST", Vector) = (1.0, 1.0, 0, 0)
		[HideInInspector] _TextureY ("TextureY", 2D) = "white" {}
		[HideInInspector] _TextureU ("TextureU", 2D) = "white" {}
		[HideInInspector] _TextureV ("TextureV", 2D) = "white" {}
		[HideInInspector] _TextureA ("TextureA", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType"="Plane"
		}

		Pass
		{
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#if defined(SHADER_API_PSP2) || defined(SHADER_API_PS3)
			// seems that ARB_precision_hint_fastest is not supported on these platforms.
			#else
			#pragma fragmentoption ARB_precision_hint_fastest
			#endif

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex   : POSITION;
				half2  texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4   pos : SV_POSITION;
				half2     uv : TEXCOORD0;
			};

			float4 _MainTex_ST;
			float4 _MovieTexture_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv  = (TRANSFORM_TEX(v.texcoord, _MainTex) * _MovieTexture_ST.xy) + _MovieTexture_ST.zw;
				return o;
			}

			static const fixed3x3 yuv_to_rgb = {
				{1.16438,      0.0,  1.59603},
				{1.16438, -0.39176, -0.81297},
				{1.16438,  2.01723,      0.0}
				};

			sampler2D _TextureY;
			sampler2D _TextureU;
			sampler2D _TextureV;
			sampler2D _TextureA;
			int _IsLinearColorSpace;

			fixed4 frag(v2f i) : COLOR
			{
				fixed3 yuv = {
					(tex2D(_TextureY, i.uv).a - 0.06275),
					(tex2D(_TextureU, i.uv).a - 0.50196),
					(tex2D(_TextureV, i.uv).a - 0.50196),
					};
				fixed4 color;
				color.rgb = mul(yuv_to_rgb, yuv);
				color.rgb = (_IsLinearColorSpace == 1) ? pow(color.rgb, 2.2) : color.rgb;
				color.a   = tex2D(_TextureA, i.uv).a;
				return color;
			}
			ENDCG
		}
	}
}
