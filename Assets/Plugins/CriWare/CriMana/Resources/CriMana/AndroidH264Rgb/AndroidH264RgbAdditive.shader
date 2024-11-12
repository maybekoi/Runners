Shader "CriMana/AndroidH264RgbAdditive"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[HideInInspector] _MovieTexture_ST ("MovieTexture_ST", Vector) = (1.0, 1.0, 0, 0)
		[HideInInspector] _TextureRGB ("TextureRGB", 2D) = "white" {}
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
			Blend One One

			GLSLPROGRAM
#version 100

#ifdef VERTEX
precision highp float;
attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _MovieTexture_ST;
uniform highp vec4 _MainTex_ST;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  xlv_TEXCOORD0 = (((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw) * _MovieTexture_ST.xy) + _MovieTexture_ST.zw;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
}
#endif

#ifdef FRAGMENT
#extension GL_OES_EGL_image_external : require
precision highp float;
uniform samplerExternalOES _TextureRGB;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  gl_FragData[0] = texture2D (_TextureRGB, xlv_TEXCOORD0);
}
#endif
			ENDGLSL
		}
	}
}
