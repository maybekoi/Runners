/****************************************************************************
 *
 * Copyright (c) 2012 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;



/*JP
 * \brief CRI File System初期化パラメータ
 */
[System.Serializable]
public class CriFsConfig {
	/*JP デバイス性能読み込み速度のデフォルト値(bps) */
	public const int defaultAndroidDeviceReadBitrate = 50000000;

	/*JP ローダー数 */
	public int numberOfLoaders    = 16;
	/*JP バインダ数 */
	public int numberOfBinders    = 8;
	/*JP インストーラ数 */
	public int numberOfInstallers = 2;
	/*JP インストールバッファのサイズ */
	public int installBufferSize  = CriFsPlugin.defaultInstallBufferSize / 1024;
	/*JP パスの最大長 */
	public int maxPath            = 256;
	/*JP ユーザーエージェント文字列 */
	public string userAgentString = "";
	/*JP ファイルディスクリプタの節約モードフラグ */
	public bool minimizeFileDescriptorUsage = false;
	/*JP [Android] デバイス性能読み込み速度(bps) */
	public int androidDeviceReadBitrate = defaultAndroidDeviceReadBitrate;

}

/*JP
 * \brief CRI Atom初期化パラメータ
 */
[System.Serializable]
public class CriAtomConfig {
	/*JP ACFファイル名
	 *   \attention ACFファイルをStreamingAssetsフォルダに配置しておく必要あり。 */
	public string acfFileName = "";
	
	/*JP 標準ボイスプール作成パラメータ */
	[System.Serializable]
	public class StandardVoicePoolConfig {
		public int memoryVoices    = 16;
		public int streamingVoices = 8;
	}
	
	/*JP HCA-MXボイスプール作成パラメータ */
	[System.Serializable]
	public class HcaMxVoicePoolConfig {
		public int memoryVoices    = 0;
		public int streamingVoices = 0;
	}
	
	/*JP 最大バーチャルボイス数 */
	public int maxVirtualVoices = 32;
	/*JP 最大ボイスリミットグループ数 */
	public int maxVoiceLimitGroups = 32;
	/*JP 最大カテゴリ数 */
	public int maxCategories = 32;
	/*JP 標準ボイスプール作成パラメータ */
	public StandardVoicePoolConfig standardVoicePoolConfig = new StandardVoicePoolConfig();
	/*JP HCA-MXボイスプール作成パラメータ */
	public HcaMxVoicePoolConfig hcaMxVoicePoolConfig = new HcaMxVoicePoolConfig();
	/*JP 出力サンプリングレート */
	public int outputSamplingRate = 0;
	/*JP インゲームプレビューを使用するかどうか */
	public bool usesInGamePreview = false;
	/*JP サーバ周波数 */
	public float serverFrequency  = 60.0f;
	/*JP ASR出力チャンネル数 */
	public int asrOutputChannels  = 0;
	/*JP 乱数種に時間（System.DateTime.Now.Ticks）を使用するかどうか */
	public bool useRandomSeedWithTime = false;
	/*JP 再生単位でのカテゴリ参照数 */
	public int categoriesPerPlayback = 4;
	/*JP 最大バス数 */
	public int maxBuses = 8;
	/*JP 最大パラメータブロック数 */
	public int maxParameterBlocks = 1024;
    /*JP VR サウンド出力モードを使用するか否か */
    public bool vrMode = false;

    /*JP [PC] 出力バッファリング時間 */
    public int pcBufferingTime = 0;

	/*JP [iOS] 出力バッファリング時間(ミリ秒)*/
	public int  iosBufferingTime     = 50;
	/*JP [iOS] iPodの再生を上書きするか？ */
	public bool iosOverrideIPodMusic = false;

	/*JP [Android] 出力バッファリング時間 */
	public int androidBufferingTime      = 133;
	/*JP [Android] 再生開始バッファリング時間 */
	public int androidStartBufferingTime = 100;

	/*JP [Android] 低遅延再生用ボイスプール作成パラメータ */
	[System.Serializable]
	public class AndroidLowLatencyStandardVoicePoolConfig {
		public int memoryVoices    = 0;
		public int streamingVoices = 0;
	}
	/*JP [Android] 低遅延再生用ボイスプール作成パラメータ */
	public AndroidLowLatencyStandardVoicePoolConfig androidLowLatencyStandardVoicePoolConfig = new AndroidLowLatencyStandardVoicePoolConfig();
    /*JP [Android] Android OS の Fast Mixer を使用して、音声再生時の発音遅延を短縮するかどうか。ASR/NSR の発音遅延や、遅延推測機能の結果に影響する */
    public bool androidUsesAndroidFastMixer = true;

	/*JP [PSVita] ATRAC9用ボイスプール作成パラメータ */
	[System.Serializable]
	public class VitaAtrac9VoicePoolConfig {
		public int memoryVoices    = 0;
		public int streamingVoices = 0;
	}
	/*JP [PSVita] ATRAC9用ボイスプール作成パラメータ */
	public VitaAtrac9VoicePoolConfig vitaAtrac9VoicePoolConfig = new VitaAtrac9VoicePoolConfig();

	/*JP [PS4] ATRAC9用ボイスプール作成パラメータ */
	[System.Serializable]
	public class Ps4Atrac9VoicePoolConfig {
		public int memoryVoices    = 0;
		public int streamingVoices = 0;
	}
	/*JP [PS4] ATRAC9用ボイスプール作成パラメータ */
	public Ps4Atrac9VoicePoolConfig ps4Atrac9VoicePoolConfig = new Ps4Atrac9VoicePoolConfig();

	/*JP [PS4] Audio3D用ボイスプール作成パラメータ */
	[System.Serializable]
	public class Ps4Audio3dConfig {
		/*JP [PS4] Audio3D機能を使用するかどうか */
		public bool useAudio3D = false;

		/*JP [PS4] Audio3D用ボイスプール作成パラメータ */
		[System.Serializable]
		public class VoicePoolConfig {
			public int memoryVoices    = 0;
			public int streamingVoices = 0;
		}
		public VoicePoolConfig voicePoolConfig = new VoicePoolConfig();

	}
	public Ps4Audio3dConfig ps4Audio3dConfig = new Ps4Audio3dConfig();

	/*JP [WebGL] WebAudioボイスプール作成パラメータ */
	[System.Serializable]
	public class WebGLWebAudioVoicePoolConfig {
		public int voices    = 16;
	}
	public WebGLWebAudioVoicePoolConfig webglWebAudioVoicePoolConfig = new WebGLWebAudioVoicePoolConfig();
}

/*JP
 * \brief CRI Mana初期化パラメータ
 */
[System.Serializable]
public class CriManaConfig {
	/*JP デコーダー数 */
	public int  numberOfDecoders   = 8;
	/*JP 連結再生エントリー数 */
	public int  numberOfMaxEntries = 4;
	/*JP GL.IssuePluginEventを用いたマルチスレッドでのテクスチャ描画処理を有効にするかどうか */
	public readonly bool graphicsMultiThreaded = true; // always true.

	/*JP [PSVita] H.264 ムービ再生の設定 */
	[System.Serializable]
	public class VitaH264PlaybackConfig {
		/*JP H.264 H.264 ムービ再生の機能を使用するか */
		public bool useH264Playback = false;
		/*JP 再生する H.264 ムービの幅 (最大) */
		public int maxWidth = 960;
		/*JP 再生する H.264 ムービの高さ (最大) */
		public int maxHeight = 544;
        public bool getMemoryFromTexture = false;
    }
	public VitaH264PlaybackConfig vitaH264PlaybackConfig = new VitaH264PlaybackConfig();
}

/*JP
 * \brief CRI Ware Decrypter初期化パラメータ
 */
[System.Serializable]
public class CriWareDecrypterConfig {
	/*JP 暗号化キー */
	public string key = "";
	/*JP 復号化認証ファイルのパス */
	public string authenticationFile = "";
	/*JP CRI Atomの復号化を有効にするかどうか */
	public bool enableAtomDecryption = true;
	/*JP CRI Manaの復号化を有効にするかどうか */
	public bool enableManaDecryption = true;
}

/// \addtogroup CRIWARE_UNITY_COMPONENT
/// @{

/*JP
 * \brief CRIWARE初期化コンポーネント
 * \par 説明:
 * CRIWAREライブラリの初期化を行うためのコンポーネントです。<br>
 */
[AddComponentMenu("CRIWARE/Library Initializer")]
public class CriWareInitializer : MonoBehaviour {
	
	/*JP CRI File Systemライブラリを初期化するかどうか */
	public bool initializesFileSystem = true;
	
	/*JP CRI File Systemライブラリ初期化設定 */
	public CriFsConfig fileSystemConfig = new CriFsConfig();
	
	/*JP CRI Atomライブラリを初期化するかどうか */
	public bool initializesAtom = true;
	
	/*JP CRI Atomライブラリ初期化設定 */
	public CriAtomConfig atomConfig = new CriAtomConfig();
	
	/*JP CRI Manaライブラリを初期化するかどうか */
	public bool initializesMana = true;
	
	/*JP CRI Manaライブラリ初期化設定 */
	public CriManaConfig manaConfig = new CriManaConfig();

	/*JP CRI Ware Decrypterを使用するかどうか */
	public bool useDecrypter = false;
	
	/*JP CRI Ware Decrypter設定 */
	public CriWareDecrypterConfig decrypterConfig = new CriWareDecrypterConfig();

	/*JP Awake時にライブラリを初期化するかどうか */
	public bool dontInitializeOnAwake = false;

	/*JP シーンチェンジ時にライブラリを終了するかどうか */
	public bool dontDestroyOnLoad = false;

	/* オブジェクト作成時の処理 */
	void Awake() {
		/* 現在のランタイムのバージョンが正しいかチェック */
		CriWare.CheckBinaryVersionCompatibility();

		if (dontInitializeOnAwake)
		{
        	/* フラグが立っていた場合はAwakeでは初期化を行わない */
			return;
		}

		/* プラグインの初期化 */
		this.Initialize();
	}
	
	/* Execution Order の設定を確実に有効にするために OnEnable をオーバーライド */
	void OnEnable() {
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	/**
	 * <summary>プラグインの初期化（手動初期化用）</summary> 
	 * \par 説明:
	 * プラグインの初期化を行います。<br/>
	 * デフォルトでは本関数はAwake関数内で自動的に呼び出されるので、アプリケーションが直接呼び出す必要はありません。<br/>
	 * <br/>
	 * 初期化パラメタをスクリプトから動的に変更して初期化を行いたい場合、本関数を使用してください。<br/>
	 * \par 注意：
	 * 本関数を使用する場合は、 自動初期化を無効にするため、 ::CriWareInitializer::dontInitializeOnAwake プロパティをインスペクタ上でチェックしてください。<br/>
	 * また、本関数を呼び出すタイミングは全てのプラグインAPIよりも前に呼び出す必要があります。Script Execution Orderが高いスクリプト上で行ってください。
	 * 
	 */
	public void Initialize() {
		/* 初期化カウンタの更新 */
		initializationCount++;
		if (initializationCount != 1) {
			/* CriWareInitializer自身による多重初期化は許可しない */
			GameObject.Destroy(this);
			return;
		}	

		/* CRI File Systemライブラリの初期化 */
		if (initializesFileSystem) {
			CriFsPlugin.SetConfigParameters(
				fileSystemConfig.numberOfLoaders,
				fileSystemConfig.numberOfBinders,
				fileSystemConfig.numberOfInstallers,
				(fileSystemConfig.installBufferSize * 1024),
				fileSystemConfig.maxPath,
				fileSystemConfig.minimizeFileDescriptorUsage
				);
			{
				/* Ver.2.03.03 以前は 0 がデフォルト値だったことの互換性維持のための処理 */
				if (fileSystemConfig.androidDeviceReadBitrate == 0) {
					fileSystemConfig.androidDeviceReadBitrate = CriFsConfig.defaultAndroidDeviceReadBitrate;
				}
			}
			CriFsPlugin.SetConfigAdditionalParameters_ANDROID(fileSystemConfig.androidDeviceReadBitrate);
			CriFsPlugin.InitializeLibrary();
			if (fileSystemConfig.userAgentString.Length != 0) {
				CriFsUtility.SetUserAgentString(fileSystemConfig.userAgentString);
			}
		}
		
		/* CRI Atomライブラリの初期化 */
		if (initializesAtom) {
			/* 初期化処理の実行 */
			CriAtomPlugin.SetConfigParameters(
				(int)Math.Max(atomConfig.maxVirtualVoices, CriAtomPlugin.GetRequiredMaxVirtualVoices(atomConfig)),
				atomConfig.maxVoiceLimitGroups,
				atomConfig.maxCategories,
				atomConfig.standardVoicePoolConfig.memoryVoices,
				atomConfig.standardVoicePoolConfig.streamingVoices,
				atomConfig.hcaMxVoicePoolConfig.memoryVoices,
				atomConfig.hcaMxVoicePoolConfig.streamingVoices,
				atomConfig.outputSamplingRate,
				atomConfig.asrOutputChannels,
				atomConfig.usesInGamePreview,
				atomConfig.serverFrequency,
				atomConfig.maxParameterBlocks,
				atomConfig.categoriesPerPlayback,
				atomConfig.maxBuses,
                atomConfig.vrMode);

			CriAtomPlugin.SetConfigAdditionalParameters_PC(
				atomConfig.pcBufferingTime
				);

			CriAtomPlugin.SetConfigAdditionalParameters_IOS(
				(uint)Math.Max(atomConfig.iosBufferingTime, 16),
				atomConfig.iosOverrideIPodMusic
				);
            /* Android 固有の初期化パラメータを登録 */
            {
				/* Ver.2.03.03 以前は 0 がデフォルト値だったことの互換性維持のための処理 */
				if (atomConfig.androidBufferingTime == 0) {
					atomConfig.androidBufferingTime = (int)(4 * 1000.0 / atomConfig.serverFrequency);
				}
				if (atomConfig.androidStartBufferingTime == 0) {
					atomConfig.androidStartBufferingTime = (int)(3 * 1000.0 / atomConfig.serverFrequency);
				}
                IntPtr android_context = IntPtr.Zero;
#if !UNITY_EDITOR && UNITY_ANDROID
		        if (atomConfig.androidUsesAndroidFastMixer) {
			        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
			        android_context = activity.GetRawObject();
		        }
#endif
                CriAtomPlugin.SetConfigAdditionalParameters_ANDROID(
				    atomConfig.androidLowLatencyStandardVoicePoolConfig.memoryVoices,
				    atomConfig.androidLowLatencyStandardVoicePoolConfig.streamingVoices,
				    atomConfig.androidBufferingTime,
				    atomConfig.androidStartBufferingTime,
                    android_context);
            }
            CriAtomPlugin.SetConfigAdditionalParameters_VITA(
				atomConfig.vitaAtrac9VoicePoolConfig.memoryVoices,
				atomConfig.vitaAtrac9VoicePoolConfig.streamingVoices,
				(initializesMana) ? manaConfig.numberOfDecoders : 0);
            {
                /* VR Mode が有効なときも useAudio3D を True にする */
                atomConfig.ps4Audio3dConfig.useAudio3D |= atomConfig.vrMode;
                CriAtomPlugin.SetConfigAdditionalParameters_PS4(
				    atomConfig.ps4Atrac9VoicePoolConfig.memoryVoices,
				    atomConfig.ps4Atrac9VoicePoolConfig.streamingVoices,
				    atomConfig.ps4Audio3dConfig.useAudio3D,
				    atomConfig.ps4Audio3dConfig.voicePoolConfig.memoryVoices,
				    atomConfig.ps4Audio3dConfig.voicePoolConfig.streamingVoices);
            }
			CriAtomPlugin.SetConfigAdditionalParameters_WEBGL(
				atomConfig.webglWebAudioVoicePoolConfig.voices);
			
			CriAtomPlugin.InitializeLibrary();

			if (atomConfig.useRandomSeedWithTime == true) {
				/* 時刻を乱数種に設定 */
				CriAtomEx.SetRandomSeed((uint)System.DateTime.Now.Ticks);
			}

			/* ACFファイル指定時は登録 */
			if (atomConfig.acfFileName.Length != 0) {
				string acfPath = atomConfig.acfFileName;
				if (CriWare.IsStreamingAssetsPath(acfPath))
				{
					acfPath = Path.Combine(CriWare.streamingAssetsPath, acfPath);
				}

				CriAtomEx.RegisterAcf(null, acfPath);
			}
		}

#if UNITY_EDITOR || (!UNITY_PS3)
        /* CRI Manaライブラリの初期化 */
        if (initializesMana) {
            CriManaPlugin.SetConfigParameters(manaConfig.graphicsMultiThreaded, manaConfig.numberOfDecoders, manaConfig.numberOfMaxEntries);
            CriManaPlugin.SetConfigAdditonalParameters_ANDROID(true);
#if UNITY_PSP2
            CriWareVITA.EnableManaH264Playback(manaConfig.vitaH264PlaybackConfig.useH264Playback);
            CriWareVITA.SetManaH264DecoderMaxSize(manaConfig.vitaH264PlaybackConfig.maxWidth,
                                                     manaConfig.vitaH264PlaybackConfig.maxHeight);
            CriWareVITA.EnableManaH264DecoderGetDisplayMemoryFromUnityTexture(manaConfig.vitaH264PlaybackConfig.getMemoryFromTexture);
#endif
            CriManaPlugin.InitializeLibrary();
        }
#endif
		
		/*JP< CRI Ware Decrypterの設定 */
		if (useDecrypter) {
			ulong decryptionKey = (decrypterConfig.key.Length == 0) ? 0 : Convert.ToUInt64(decrypterConfig.key);
			string authenticationPath = decrypterConfig.authenticationFile;
			if (CriWare.IsStreamingAssetsPath(authenticationPath))
			{
				authenticationPath = Path.Combine(CriWare.streamingAssetsPath, authenticationPath);
			}
#if !UNITY_EDITOR && UNITY_WEBGL
			CriWare.criWareUnity_SetDecryptionKey_EMSCRIPTEN(
				decryptionKey
				);
#else
			CriWare.criWareUnity_SetDecryptionKey(
				decryptionKey,
				authenticationPath,
				decrypterConfig.enableAtomDecryption,
				decrypterConfig.enableManaDecryption
				);
#endif
		} else {
#if !UNITY_EDITOR && UNITY_WEBGL
			CriWare.criWareUnity_SetDecryptionKey_EMSCRIPTEN(0);
#else
			CriWare.criWareUnity_SetDecryptionKey(0, "", false, false);
#endif
		}

		/* シーンチェンジ後もオブジェクトを維持するかどうかの設定 */
		if (dontDestroyOnLoad) {
			DontDestroyOnLoad(transform.gameObject);
			DontDestroyOnLoad(CriWare.managerObject);
		}
	}

	void OnDestroy() {
		/* 初期化カウンタの更新 */
        if (initializationCount == 0) {
            return;
        }

		initializationCount--;
		if (initializationCount != 0) {
			return;
		}
		
#if UNITY_EDITOR || (!UNITY_PS3)
		/* CRI Manaライブラリの終了 */
		if (initializesMana) {
			CriManaPlugin.FinalizeLibrary();
		}
#endif
		
		/* CRI Atomライブラリの終了 */
		if (initializesAtom) {
            /* EstimatorがStop状態になるまでFinalize */
            while (CriAtomExLatencyEstimator.GetCurrentInfo().status != CriAtomExLatencyEstimator.Status.Stop) {
                CriAtomExLatencyEstimator.FinalizeModule();
            }
            
            /* 終了処理の実行 */
            CriAtomPlugin.FinalizeLibrary();
        }
		
		/* CRI File Systemライブラリの終了 */
		if (initializesFileSystem) {
			CriFsPlugin.FinalizeLibrary();
		}
	}
	
	/* 初期化カウンタ */
	private static int initializationCount = 0;

	/* 初期化実行チェック関数 */
	public static bool IsInitialized() {
		if (initializationCount > 0) {
			return true;
		} else {
			/* 現在のランタイムのバージョンが正しいかチェック */
			CriWare.CheckBinaryVersionCompatibility();
			return false;
		}
	}
} // end of class

/// @}

/* --- end of file --- */
