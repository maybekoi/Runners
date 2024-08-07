/****************************************************************************
 *
 * Copyright (c) 2015 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using UnityEngine;
using System.Runtime.InteropServices;
namespace CriMana.Detail
{
	public static partial class AutoResisterRendererResourceFactories
	{
		[RendererResourceFactoryPriority(7000)]
		public class RendererResourceFactoryOSXH264Yuv : RendererResourceFactory
		{
			public override RendererResource CreateRendererResource(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
			{
				bool isCodecSuitable = movieInfo.codecType == CodecType.H264;
				bool isAlphaSuitable = !movieInfo.hasAlpha;	/* アルファムービは非対応 */
				bool isSuitable      = isCodecSuitable && isAlphaSuitable;
				return isSuitable
					? new RendererResourceOSXH264Yuv(playerId, movieInfo, additive, userShader)
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

	public class RendererResourceOSXH264Yuv : RendererResource
	{
		private int		width;
		private int		height;
		private int 	playerId;
		private bool	hasAlpha;
		private bool	additive;
		private bool	useUserShader;
		
		private Shader			shader;
		
		private Vector4			movieTextureST = Vector4.zero;

		private Texture2D		textureYUV;

		public RendererResourceOSXH264Yuv(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
			if (movieInfo.hasAlpha) {
				UnityEngine.Debug.LogError("[CRIWARE] H.264 with Alpha is unsupported");
			}
			this.width		= (int)movieInfo.width;
			this.height		= (int)movieInfo.height;
			this.playerId	= playerId;
			this.hasAlpha	= movieInfo.hasAlpha;
			this.additive	= additive;
			this.useUserShader	= userShader != null;
			
			if (userShader != null) {
				shader = userShader;
			} else {
				string shaderName = 
					hasAlpha	? additive	? "Diffuse"
											: "Diffuse"
								: additive	? "CriMana/SofdecPrimeRgbAdditive"
											: "CriMana/SofdecPrimeRgb";
				shader = Shader.Find(shaderName);
			}

			UpdateMovieTextureST(movieInfo.dispWidth, movieInfo.dispHeight);
		}

		protected override void OnDisposeManaged()
		{
		}
		
		protected override void OnDisposeUnmanaged()
		{
			if (textureYUV != null) {
				Texture2D.Destroy(textureYUV);
				textureYUV = null;
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
			if (textureYUV != null) {
				material.shader = shader;
				material.SetTexture("_TextureRGBA", textureYUV);
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
			System.IntPtr[] nativePtrs = new System.IntPtr[1];
			bool isTextureUpdated = criManaUnityPlayer_UpdateTextures(playerId, 1, nativePtrs);
			if (isTextureUpdated && nativePtrs[0] != System.IntPtr.Zero) {
				if (textureYUV == null) {
					textureYUV = Texture2D.CreateExternalTexture(width, height, TextureFormat.BGRA32, false, false, nativePtrs[0]);
					textureYUV.wrapMode = TextureWrapMode.Clamp;
				} else {
					textureYUV.UpdateExternalTexture(nativePtrs[0]);
				}
			}
		}
	}
}
#endif
