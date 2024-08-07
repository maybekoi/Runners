/****************************************************************************
 *
 * Copyright (c) 2016 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

/*---------------------------
 * Asr Support Defines
 *---------------------------*/
#if !UNITY_PSP2
#define CRIWARE_SUPPORT_ASR
#endif

using System;
using System.Runtime.InteropServices;

/*==========================================================================
 *      CRI Atom Native Wrapper
 *=========================================================================*/
/**
 * \addtogroup CRIATOM_NATIVE_WRAPPER
 * @{
 */

/**
 * <summary>ASRラック</summary>
 */
public partial class CriAtomExAsrRack : IDisposable
{
	#region Data Types
	/**
	 * <summary>ASRラック作成用コンフィグ構造体</summary>
	 * \par 説明:
	 * CriAtomExAsrRack の動作仕様を指定するための構造体です。<br>
	 * モジュール作成時（::CriAtomExAsrRack::CriAtomExAsrRack 関数）に引数として本構造体を指定します。<br>
	 * \par 備考:
	 * ::CriAtomExAsrRack::defaultConfig で取得したデフォルトコンフィギュレーションを必要に応じて変更して
	 * ください。<br>
	 * \sa CriAtomExAsrRack::CriAtomExAsrRack, CriAtomExAsrRack::defaultConfig
	 */
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct Config
	{
		/**
		 * <summary>サーバ処理の実行頻度</summary>
		 * \par 説明：
		 * サーバ処理を実行する頻度を指定します。
		 * \attention
		 * CriWareInitializer に指定した CriAtomConfig::serverFrequency と同じ値を指定してください。
		 */
		public float serverFrequency;

		/**
		 * <summary>バス数</summary>
		 * \par 説明：
		 * ASRが作成するバスの数を指定します。<br>
		 * バスはサウンドのミックスや、DSPエフェクトの管理等を行います。
		 */
		public int numBuses;

		/**
		 * <summary>出力チャンネル数</summary>
		 * \par 説明：
		 * ASRラックの出力チャンネル数を指定します。<br>
		 * パン3Dもしくは3Dポジショニング機能を使用する場合は6ch以上を指定します。
		 */
		public int outputChannels;

		/**
		 * <summary>出力サンプリングレート</summary>
		 * \par 説明：
		 * ASRラックの出力および処理過程のサンプリングレートを指定します。<br>
		 * 通常、ターゲット機のサウンドデバイスのサンプリングレートを指定します。
		 * \par 備考：
		 * 低くすると処理負荷を下げることができますが音質が落ちます。
		 */
		public int outputSamplingRate;

		/**
		 * <summary>サウンドレンダラタイプ</summary>
		 * \par 説明：
		 * ASRラックの出力先サウンドレンダラの種別を指定します。<br>
		 * soundRendererType に CriAtomEx.SoundRendererType.Native を指定した場合、
		 * 音声データはデフォルト設定の各プラットフォームのサウンド出力に転送されます。
		 */
		public CriAtomEx.SoundRendererType soundRendererType;

		/**
		 * <summary>出力先ASRラックID</summary>
		 * \par 説明：
		 * ASRラックの出力先ASRラックIDを指定します。<br>
		 * soundRendererType に CriAtomEx.SoundRendererType.Asr を指定した場合のみ有効です。
		 */
		public int outputRackId;

		/**
		 * <summary>プラットフォーム固有のパラメータへのポインタ</summary>
		 * \par 説明：
		 * プラットフォーム固有のパラメータへのポインタを指定します。<br>
		 * CriAtomExAsrRack::CriAtomExAsrRack 関数の引数に用いる場合は、第二引数の 
		 * PlatformContext で上書きされるため、 IntPtr.Zero を指定してください。
		 */
		public IntPtr context;
	}

	/**
	 * <summary>ASRラック作成用プラットフォーム固有コンフィグ構造体</summary>
	 * \par 説明:
	 * CriAtomExAsrRack の動作仕様を指定するための構造体です。<br>
	 * モジュール作成時（::CriAtomExAsrRack::CriAtomExAsrRack 関数）に引数として本構造体を指定します。<br>
	 * 詳細についてはプラットフォーム毎のマニュアルを参照してください。
	 * \sa CriAtomExAsrRack::CriAtomExAsrRack
	 */
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct PlatformConfig
	{
	#if !UNITY_EDITOR && UNITY_PS4
		public int userId;
		public CriWarePS4.AudioPortType portType;
		public CriWarePS4.AudioPortAttribute portAttr;
	#else
		public byte reserved;
	#endif
	}
	#endregion

	/**
	 * <summary>ASRラックの作成</summary>
	 * <param name="config">コンフィグ構造体</param>
	 * <param name="platformConfig">プラットフォーム固有パラメータ構造体</param>
	 * <returns>ASRラック</returns>
	 * \par 説明:
	 * ASRラックを作成します。<br>
	 * 本関数で作成したASRラックは、必ず Dispose 関数で破棄してください。
	 * */
	public CriAtomExAsrRack(Config config, PlatformConfig platformConfig)
	{
	#if CRIWARE_SUPPORT_ASR
		CriAtomPlugin.InitializeLibrary();

		this._rackId = criAtomUnityAsrRack_Create(ref config, ref platformConfig);
		if (config.context != IntPtr.Zero) {
			Marshal.FreeHGlobal(config.context);
		}
		if (this._rackId == -1) {
			throw new Exception("CriAtomExAsrRack() failed.");
		}
	#else
		this._rackId = -1;
	#endif
	}

	/**
	 * <summary>ASRラックの破棄</summary>
	 * \par 説明:
	 * ASRラックを破棄します。
	 * */
	public void Dispose()
	{
	#if CRIWARE_SUPPORT_ASR
		if (this._rackId == -1)
		{
			return;
		}

		criAtomExAsrRack_Destroy(this._rackId);
		this._rackId = -1;

        CriAtomPlugin.FinalizeLibrary();
	#endif
		GC.SuppressFinalize(this);
	}

	public int rackId {
		get { return this._rackId; }
	}

	#region Static Properties
	/**
	 * <summary>デフォルトコンフィギュレーション</summary>
	 * \par 説明:
	 * デフォルトコンフィグです。
	 * \par 備考:
	 * 本プロパティで取得したデフォルトコンフィギュレーションを必要に応じて変更して
	 * ::CriAtomExAsrRack::CriAtomExAsrRack 関数に指定してください。<br>
	 * \sa CriAtomExAsrRack::CriAtomExAsrRack
	 */
	public static Config defaultConfig {
		get {
			Config config;
			config.serverFrequency = 60.0f;
			config.numBuses = 8;
			config.soundRendererType = CriAtomEx.SoundRendererType.Native;
			config.outputRackId = 0;
			config.context = System.IntPtr.Zero;
	#if !UNITY_EDITOR && UNITY_PS4
			config.outputChannels = 8;
			config.outputSamplingRate = 48000;
	#elif !UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID
			config.outputChannels = 2;
			config.outputSamplingRate = 44100;
	#elif !UNITY_EDITOR && UNITY_VITA
			config.outputChannels = 2;
			config.outputSamplingRate = 48000;
	#else
			config.outputChannels = 6;
			config.outputSamplingRate = 48000;
	#endif
			return config;
		}
	}

	/**
	 * <summary>デフォルトASRラックID</summary>
	 * \par 説明:
	 * デフォルトのASRラックIDです。
	 * 通常出力に戻す場合や生成したASRラックを破棄する場合には、各種プレーヤに対して
	 * この定数を利用してASRラックIDの指定を行ってください。
	 * \sa CriAtomExPlayer::SetAsrRackId, CriMana::Player::SetAsrRackId
	 */
	public static int defaultRackId = 0;

	#endregion


	#region internal members
	~CriAtomExAsrRack()
	{
		this.Dispose();
	}

	private int _rackId = -1;
	#endregion

	#region DLL Import
	#if CRIWARE_SUPPORT_ASR
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomUnityAsrRack_Create([In] ref Config config, [In] ref PlatformConfig platformConfig);
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomExAsrRack_Destroy(Int32 rackId);
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomExAsrRack_AttachDspBusSetting(Int32 rackId, string setting, IntPtr work, Int32 workSize);
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomExAsrRack_DetachDspBusSetting(Int32 rackId);
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomExAsrRack_ApplyDspBusSnapshot(Int32 rackId, string snapshotName, Int32 timeMs);
	#endif
	#endregion
}

/**
 * @}
 */

/* --- end of file --- */
