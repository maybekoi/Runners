// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CriMana/SofdecPrimeRgbaAdditive"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[HideInInspector] _MovieTexture_ST ("MovieTexture_ST", Vector) = (1.0, 1.0, 0, 0)
		[HideInInspector] _TextureRGBA ("TextureRGBA", 2D) = "white" {}
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

			sampler2D _TextureRGBA;
			int _IsLinearColorSpace;
			//Temporary fix for Switch
			int _IsSwitch;

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 color = tex2D(_TextureRGBA, i.uv);
				color.rgb = (_IsLinearColorSpace == 1) ? pow(color.rgb, 2.2) : color.rgb;
				//Temporary fix for Switch
				color.rgb = (_IsSwitch) ? fixed3(color.b, color.g, color.r) : color.rgb;
				return color;
			}
			ENDCG
		}
	}
}
