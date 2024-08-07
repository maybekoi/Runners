/****************************************************************************
 *
 * Copyright (c) 2015 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

#if !UNITY_EDITOR && UNITY_ANDROID

using UnityEngine;
using System.Runtime.InteropServices;


namespace CriMana.Detail
{
	public static partial class AutoResisterRendererResourceFactories
	{
		[RendererResourceFactoryPriority(7000)]
		public class RendererResourceFactoryAndroidH264Rgb : RendererResourceFactory
		{
			public override RendererResource CreateRendererResource(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
			{
				bool isCodecSuitable = movieInfo.codecType == CodecType.H264;
				bool isAlphaSuitable = !movieInfo.hasAlpha;	/* アルファムービは非対応 */
				bool isSuitable      = isCodecSuitable && isAlphaSuitable;
				return isSuitable
					? new RendererResourceAndroidH264Rgb(playerId, movieInfo, additive, userShader)
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




	public class RendererResourceAndroidH264Rgb : RendererResource
	{
		private int 	playerId;
		private bool	attached = false;
		
		private int		width;
		private int		height;
		private bool	hasAlpha;
		private bool	additive;
		private bool	useUserShader;

		private Shader			shader;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		private float[]			movieTextureSTRaw = new float[4];

		private Vector4			movieTextureST = Vector4.zero;

		private System.UInt32	oesTexture;
		private Texture2D		texture;


		public RendererResourceAndroidH264Rgb(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
			if (movieInfo.hasAlpha) {
				UnityEngine.Debug.LogError("[CRIWARE] H.264 with Alpha is unsupported");
			}

			this.playerId	= playerId;
			width 			= (int)movieInfo.width;
			height			= (int)movieInfo.height;
			hasAlpha		= movieInfo.hasAlpha;
			this.additive	= additive;
			useUserShader	= userShader != null;

			if (userShader != null) {
				shader = userShader;
			} else {
				string shaderName = 
					hasAlpha	? additive	? "Diffuse"
											: "Diffuse"
								: additive	? "CriMana/AndroidH264RgbAdditive"
											: "CriMana/AndroidH264Rgb";
				shader = Shader.Find(shaderName);
			}

			oesTexture	= criManaUnity_MediaCodecCreateTexture_ANDROID();
			texture		= Texture2D.CreateExternalTexture(width, height, TextureFormat.ARGB32, false, false, (System.IntPtr)(oesTexture));
		}
		
		
		protected override void OnDisposeManaged()
		{
		}
		
		
		protected override void OnDisposeUnmanaged()
		{
			if (texture != null) {
				Texture2D.Destroy(texture);
				if (attached) {
					criManaUnityPlayer_MediaCodecDetachTexture_ANDROID(playerId, oesTexture);
					attached = false;
				}
				criManaUnity_MediaCodecDeleteTexture_ANDROID(oesTexture);
				oesTexture = 0;
			}
			texture	= null;
			movieTextureSTRaw = null;
		}
		
		
		public override bool IsPrepared()
		{ return true; }
		
		
		public override bool ContinuePreparing()
		{ return true; }
		
		
		public override bool IsSuitable(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
			bool isPlayerSuitable   = playerId == this.playerId;
			bool isCodecSuitable    = movieInfo.codecType == CodecType.H264;
			bool isSizeSuitable     = (width == (int)movieInfo.width) && (height == (int)movieInfo.height);
			bool isAlphaSuitable    = hasAlpha == movieInfo.hasAlpha;
			bool isAdditiveSuitable = this.additive == additive;
			bool isShaderSuitable   = this.useUserShader ? (userShader == shader) : true;
			return isPlayerSuitable && isCodecSuitable && isSizeSuitable && isAlphaSuitable && isAdditiveSuitable && isShaderSuitable;
		}


		public override void AttachToPlayer(int playerId)
		{
			if (this.playerId == playerId) {
				criManaUnityPlayer_MediaCodecAttachTexture_ANDROID(playerId, oesTexture);
				attached = true;
			} else {
				Debug.LogError("[CRIWARE] Internal logic error");
			}
		}


		public override bool UpdateFrame(int playerId, FrameInfo frameInfo)
		{
			bool isFrameUpdated = criManaUnityPlayer_MediaCodecUpdateTexture_ANDROID(
				playerId,
				frameInfo,
				movieTextureSTRaw
				);
			if (isFrameUpdated) {
				UpdateMovieTextureST();
			}
			return isFrameUpdated;
		}


		public override bool UpdateMaterial(Material material)
		{
			material.shader = shader;
			material.mainTexture = texture;
			material.SetTexture("_TextureRGB", texture);
			material.SetVector("_MovieTexture_ST", movieTextureST);
			return true;
		}


		private void UpdateMovieTextureST()
		{
			movieTextureST.x = movieTextureSTRaw[0];
			movieTextureST.y = movieTextureSTRaw[1];
			movieTextureST.z = movieTextureSTRaw[2];
			movieTextureST.w = movieTextureSTRaw[3];
		}

		public override void UpdateTextures()
		{

		}


		#region Native API Definitions
		[DllImport(CriWare.pluginName)]
		private static extern System.UInt32 criManaUnity_MediaCodecCreateTexture_ANDROID();
		
		[DllImport(CriWare.pluginName)]
		private static extern void criManaUnity_MediaCodecDeleteTexture_ANDROID(System.UInt32 oes_texture);

		[DllImport(CriWare.pluginName)]
		private static extern bool criManaUnityPlayer_MediaCodecAttachTexture_ANDROID(int player_id, System.UInt32 oes_texture);

		[DllImport(CriWare.pluginName)]
		private static extern void criManaUnityPlayer_MediaCodecDetachTexture_ANDROID(int player_id, System.UInt32 oes_texture);

		[DllImport(CriWare.pluginName)]
		private static extern bool criManaUnityPlayer_MediaCodecUpdateTexture_ANDROID(
			int player_id,
			[In, Out] FrameInfo frame_info,
			[MarshalAs(UnmanagedType.LPArray)] float[] movieTextureST
			);
		#endregion
	}
}


#endif
