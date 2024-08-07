/****************************************************************************
 *
 * Copyright (c) 2015 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_PSP2 || UNITY_ANDROID || UNITY_IOS || UNITY_TVOS || UNITY_WEBGL || UNITY_STANDALONE_LINUX

using UnityEngine;
using System;

namespace CriMana.Detail
{
	public static partial class AutoResisterRendererResourceFactories
	{
		[RendererResourceFactoryPriority(5000)]
		public class RendererResourceFactorySofdecPrimeYuv : RendererResourceFactory
		{
			public override RendererResource CreateRendererResource(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
			{
				bool isCodecSuitable = movieInfo.codecType == CodecType.SofdecPrime;
				bool isSuitable      = isCodecSuitable;
				return isSuitable
					? new RendererResourceSofdecPrimeYuv(playerId, movieInfo, additive, userShader)
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




	public class RendererResourceSofdecPrimeYuv : RendererResource
	{
		private int		width;
		private int		height;
		private bool	hasAlpha;
		private bool	additive;
		private bool	useUserShader;
		
		private Shader		shader;

		private Vector4		movieTextureST = Vector4.zero;

		private Texture2D	textureY;
		private Texture2D	textureU;
		private Texture2D	textureV;
		private Texture2D	textureA;
		private IntPtr[] 	nativeTextures = new IntPtr[4];

		private Int32 		playerID;


		public RendererResourceSofdecPrimeYuv(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_PSP2 || UNITY_PS4 || UNITY_WINRT || UNITY_WEBGL || UNITY_STANDALONE_LINUX
            width = Ceiling256((int)movieInfo.width);
			height = Ceiling16((int)movieInfo.height);
#elif UNITY_ANDROID || UNITY_IOS || UNITY_TVOS
			width  = NextPowerOfTwo(Ceiling64((int)movieInfo.width));
			height = NextPowerOfTwo(Ceiling16((int)movieInfo.height));
#else
	#error unsupported platform
#endif
			hasAlpha		= movieInfo.hasAlpha;
			this.additive	= additive;
			useUserShader	= userShader != null;

			if (useUserShader) {
				shader = userShader;
			} else {
				string shaderName = 
					hasAlpha	? additive	? "CriMana/SofdecPrimeYuvaAdditive"
											: "CriMana/SofdecPrimeYuva"
								: additive	? "CriMana/SofdecPrimeYuvAdditive"
											: "CriMana/SofdecPrimeYuv";
				shader = Shader.Find(shaderName);
			}
			
			UpdateMovieTextureST(movieInfo.dispWidth, movieInfo.dispHeight);
			
			textureY = new Texture2D(width, height, TextureFormat.Alpha8, false);
			textureY.wrapMode = TextureWrapMode.Clamp;
			textureU = new Texture2D(width / 2, height / 2, TextureFormat.Alpha8, false);
			textureU.wrapMode = TextureWrapMode.Clamp;
			textureV = new Texture2D(width / 2, height / 2, TextureFormat.Alpha8, false);
			textureV.wrapMode = TextureWrapMode.Clamp;
			nativeTextures[0] = textureY.GetNativeTexturePtr();
			nativeTextures[1] = textureU.GetNativeTexturePtr();
			nativeTextures[2] = textureV.GetNativeTexturePtr();
			if (hasAlpha) {
				textureA = new Texture2D(width, height, TextureFormat.Alpha8, false);
				textureA.wrapMode = TextureWrapMode.Clamp;
				nativeTextures[3] = textureA.GetNativeTexturePtr();
			}

			playerID = playerId;
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
			if (textureU != null) {
				Texture2D.Destroy(textureU);
				textureU = null;
			}
			if (textureV != null) {
				Texture2D.Destroy(textureV);
				textureV = null;
			}
			if (textureA != null) {
				Texture2D.Destroy(textureA);
				textureA = null;
			}
		}
		
		
		public override bool IsPrepared()
		{ return true; }
		
		
		public override bool ContinuePreparing()
		{ return true; }
		
		
		public override bool IsSuitable(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
			bool isCodecSuitable    = movieInfo.codecType == CodecType.SofdecPrime;
			bool isSizeSuitable     = (width >= (int)movieInfo.width) && (height >= (int)movieInfo.height);
			bool isAlphaSuitable    = hasAlpha == movieInfo.hasAlpha;
			bool isAdditiveSuitable = this.additive == additive;
			bool isShaderSuitable   = this.useUserShader ? (userShader == shader) : true;
			return isCodecSuitable && isSizeSuitable && isAlphaSuitable && isAdditiveSuitable && isShaderSuitable;
		}


		public override void AttachToPlayer(int playerId)
		{}


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
			material.shader = shader;
			material.SetTexture("_TextureY", textureY);
			material.SetTexture("_TextureU", textureU);
			material.SetTexture("_TextureV", textureV);
            material.SetInt("_IsLinearColorSpace", (QualitySettings.activeColorSpace == ColorSpace.Linear) ? 1 : 0);
			if (hasAlpha) {
				material.SetTexture("_TextureA", textureA);
			}
			material.SetVector("_MovieTexture_ST", movieTextureST);
			return true;
		}
		
		
		private void UpdateMovieTextureST(System.UInt32 dispWidth, System.UInt32 dispHeight)
		{
			float uScale = (dispWidth != width) ? (float)(dispWidth - 1) / width : 1.0f;
			float vScale = (dispHeight != height) ? (float)(dispHeight - 1) / height : 1.0f;
			movieTextureST.x = uScale;
			movieTextureST.y = -vScale;
			movieTextureST.z = 0.0f;
			movieTextureST.w = vScale;
		}

		public override void UpdateTextures()
		{
			int numTextures = 3;
			if (hasAlpha) {
				numTextures = 4;
			}

			criManaUnityPlayer_UpdateTextures(playerID, numTextures, nativeTextures);
		}
	}
}


#endif
