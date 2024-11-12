/****************************************************************************
 *
 * Copyright (c) 2015 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

#if !UNITY_EDITOR && UNITY_IOS

using UnityEngine;
using System.Runtime.InteropServices;

namespace CriMana.Detail
{
	public static partial class AutoResisterRendererResourceFactories
	{
		[RendererResourceFactoryPriority(7000)]
		public class RendererResourceFactoryIOSH264Yuv : RendererResourceFactory
		{
			public override RendererResource CreateRendererResource(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
			{
				bool isCodecSuitable = movieInfo.codecType == CodecType.H264;
				bool isAlphaSuitable = !movieInfo.hasAlpha;	/* アルファムービは非対応 */
				bool isSuitable      = isCodecSuitable && isAlphaSuitable;
				return isSuitable
					? new RendererResourceIOSH264Yuv(playerId, movieInfo, additive, userShader)
					: null;
			}

			protected override void OnDisposeManaged()
			{
			}

			protected override void OnDisposeUnmanaged()
			{
			}
		}
	}


	public class RendererResourceIOSH264Yuv : RendererResource
	{
		private int		width;
		private int		height;
		private int 	playerId;
		private bool	hasAlpha;
		private bool	additive;
		private bool	useUserShader;
		private bool	isOpenGLES;

		private Shader			shader;

		private Vector4			movieTextureST = Vector4.zero;

		private Texture2D		textureY;
		private Texture2D		textureUV;
		private System.IntPtr	nativeTextureY;
		private System.IntPtr	nativeTextureUV;

		public RendererResourceIOSH264Yuv(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
			if (movieInfo.hasAlpha) {
				UnityEngine.Debug.LogError("[CRIWARE] H.264 with Alpha is unsupported");
			}
			this.width		= (int)movieInfo.width;
			this.height		= (int)movieInfo.height;
			this.playerId	= playerId;
			hasAlpha		= movieInfo.hasAlpha;
			this.additive	= additive;
			useUserShader	= userShader != null;

			if (userShader != null) {
				shader = userShader;
			} else {
				string shaderName = 
					hasAlpha	? additive	? "Diffuse"
											: "Diffuse"
								: additive	? "CriMana/IOSH264YuvAdditive"
											: "CriMana/IOSH264Yuv";
				shader = Shader.Find(shaderName);
			}

			UpdateMovieTextureST(movieInfo.dispWidth, movieInfo.dispHeight);

#if (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
			isOpenGLES = SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL");
#else
			isOpenGLES = (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 ||
							SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3);
#endif
		}


		protected override void OnDisposeManaged()
		{
		}


		protected override void OnDisposeUnmanaged()
		{
			if (textureY != null) {
				Texture2D.Destroy(textureY);
				textureY = null;
			}
			if (textureUV != null) {
				Texture2D.Destroy(textureUV);
				textureUV = null;
			}
		}


		public override bool IsPrepared()
		{ return true; }


		public override bool ContinuePreparing()

		{ return true; }		

		public override bool IsSuitable(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
			bool isCodecSuitable    = movieInfo.codecType == CodecType.H264;
			bool isAlphaSuitable    = hasAlpha == movieInfo.hasAlpha;
			bool isAdditiveSuitable = this.additive == additive;
			bool isShaderSuitable   = this.useUserShader ? (userShader == shader) : true;
			return isCodecSuitable && isAlphaSuitable && isAdditiveSuitable && isShaderSuitable;
		}


		public override void AttachToPlayer(int playerId)
		{
			// reset texture if exist
			OnDisposeUnmanaged();
		}


		public override bool UpdateFrame(int playerId, FrameInfo frameInfo)
		{
			bool isFrameUpdated = criManaUnityPlayer_UpdateFrame(playerId, 0, null, frameInfo);
			if (isFrameUpdated) {
				UpdateMovieTextureST(frameInfo.dispWidth, frameInfo.dispHeight);
			}
			return isFrameUpdated;
		}

		public override bool UpdateMaterial(Material material)
		{
			if (textureY != null) {
				material.shader = shader;
				material.SetTexture("_TextureY", textureY);
				material.SetTexture("_TextureUV", textureUV);
				material.SetInt("_IsLinearColorSpace", (QualitySettings.activeColorSpace == ColorSpace.Linear) ? 1 : 0);
				material.SetVector("_MovieTexture_ST", movieTextureST);
				return true;
			}
			return false;
		}


		private void UpdateMovieTextureST(System.UInt32 dispWidth, System.UInt32 dispHeight)
		{
			float uScale = (dispWidth != width) ? (float)(dispWidth - 0.5f) / width : 1.0f;
			float vScale = (dispHeight != height) ? (float)(dispHeight - 0.5f) / height : 1.0f;
			movieTextureST.x = uScale;
			movieTextureST.y = -vScale;
			movieTextureST.z = 0.0f;
			movieTextureST.w = vScale;
		}


		public override void UpdateTextures()
		{
			System.IntPtr[] nativePtrs = new System.IntPtr[2];
			bool isTextureUpdated = criManaUnityPlayer_UpdateTextures(playerId, 2, nativePtrs); // out textures
			if (isTextureUpdated && nativePtrs[0] != System.IntPtr.Zero) {
				if (isOpenGLES) {
					if (textureY == null) {
						textureY = Texture2D.CreateExternalTexture(width, height, TextureFormat.Alpha8, false, false, System.IntPtr.Zero);
						textureUV = Texture2D.CreateExternalTexture(width / 2, height / 2, TextureFormat.Alpha8, false, false, System.IntPtr.Zero);
					}
					Texture2D tmptextureY = Texture2D.CreateExternalTexture(textureY.width, textureY.height, TextureFormat.Alpha8, false, false, nativePtrs[0]);
					tmptextureY.wrapMode = TextureWrapMode.Clamp;
					textureY.UpdateExternalTexture(tmptextureY.GetNativeTexturePtr());
					Texture2D.Destroy(tmptextureY);
					Texture2D tmptextureUV = Texture2D.CreateExternalTexture(textureUV.width, textureUV.height, TextureFormat.Alpha8, false, false, nativePtrs[1]);
					tmptextureUV.wrapMode = TextureWrapMode.Clamp;
					textureUV.UpdateExternalTexture(tmptextureUV.GetNativeTexturePtr());
					Texture2D.Destroy(tmptextureUV);
				} else {
					if (textureY == null) {
						textureY = Texture2D.CreateExternalTexture(width, height, TextureFormat.Alpha8, false, false, nativePtrs[0]);
						textureUV = Texture2D.CreateExternalTexture(width / 2, height / 2, TextureFormat.Alpha8, false, false, nativePtrs[1]);
						textureY.wrapMode = TextureWrapMode.Clamp;
						textureUV.wrapMode = TextureWrapMode.Clamp;
					} else {
						textureY.UpdateExternalTexture(nativePtrs[0]);
						textureUV.UpdateExternalTexture(nativePtrs[1]);
					}
				}
			}
		}
	}

}

#endif
