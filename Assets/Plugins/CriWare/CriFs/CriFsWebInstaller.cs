/****************************************************************************
 *
 * Copyright (c) 2016 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID
	#define CRIFSWEBINSTALLER_SUPPORTED
#endif

using UnityEngine;
using System;
using System.Runtime.InteropServices;


/**
 * \addtogroup CRIFS_NATIVE_WRAPPER
 * @{
 */


/**
 * <summary>HTTP によるローカルストレージへのインストールを行うモジュールです。</summary>
 * \par 説明:
 * Web サーバ上のコンテンツをローカルストレージにインストールするために使用します。
 * \attention 
 * iOSでの本機能の動作要件は iOS7 以降になります。
 * \attention
 * ::CriFsWebInstaller のインスタンスを生成する前に、 ::CriFsWebInstaller::InitializeModule メソッド
 * でモジュールを初期化する必要があります。
 */
public class CriFsWebInstaller : IDisposable
{
	#region Data Types
	/**
	 * <summary>ステータス</summary>
	 * \sa CriFsWebInstaller::GetStatusInfo
	 */
	public enum Status : int
	{
		Stop,		/**< 停止中	*/
		Busy,		/**< 処理中	*/
		Complete,	/**< 完了	*/
		Error,		/**< エラー	*/
	}

	/**
	 * <summary>エラー種別</summary>
	 * \par 説明：
	 * インストーラハンドルのエラー種別を表します。<br>
	 * ::CriFsWebInstaller::GetStatusInfo 関数により取得できます。
	 * \sa CriFsWebInstaller::GetStatusInfo
	 */
	public enum Error : int
	{
		None,		/**< エラーなし	*/
		Timeout,	/**< タイムアウトエラー	*/
		Memory,		/**< メモリ確保失敗	*/
		LocalFs,	/**< ローカルファイルシステムエラー	*/
		DNS,		/**< DNSエラー	*/
		Connection,	/**< 接続エラー	*/
		SSL,		/**< SSLエラー	*/
		HTTP,		/**< HTTPエラー	*/
		Internal,	/**< 内部エラー	*/
	}

	/**
	 * <summary>ステータス情報</summary>
	 * \par 説明：
	 * ::CriFsWebInstaller::Status を含む詳細な状態を表します。<br>
	 * ::CriFsWebInstaller::GetStatusInfo 関数により取得できます。
	 * \sa CriFsWebInstaller::StatusInfo, CriFsWebInstaller::GetStatusInfo
	 */
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct StatusInfo
	{
		/*JP
		 * <summary>インストーラハンドルの状態</summary>
		 * \sa CriFsWebInstaller::Status
		 */
		public Status status;

		/**
		 * <summary>インストーラハンドルのエラー状態</summary>
		 * \par 説明：
		 * CriFsWebInstaller::StatusInfo.status != CriFsWebInstaller::Status.Error の際に、
		 * CriFsWebInstaller::Error.None 以外の値が格納されます。<br>
		 * エラー発生時には、エラー種別によって適切にエラーハンドリングを行なってください。
		 * \sa CriFsWebInstaller::Error
		 */
		public Error error;

		/**
		 * <summary>HTTPステータスコード</summary>
		 * \par 説明：
		 * 以下のどちらかの場合に HTTPステータスコードが格納されます。<br>
		 *   - CriFsWebInstaller::StatusInfo.status == CriFsWebInstaller::Status.Complete <br>
		 *   - CriFsWebInstaller::StatusInfo.status == CriFsWebInstaller::Status.Error かつ CriFsWebInstaller::StatusInfo.error == CriFsWebInstaller::Error.HTTP <br>
		 *
		 * その他の場合は、負値( CriFsWebInstaller.InvalidHttpStatusCode )が格納されます。
		 * \sa CriFsWebInstaller.InvalidHttpStatusCode
		 */
		public int httpStatusCode;

		/**
		 * <summary>インストール対象のサイズ(byte)</summary>
		 * \par 説明：
		 * インストール対象のサイズ(byte)が格納されます。<br>
		 * インストール対象のサイズが不明な場合は負値( CriFsWebInstaller.InvalidContentsSize ) が格納されます。<br>
		 * HTTP による転送が開始すると有効な値が格納されます。
		 * \sa CriFsWebInstaller.InvalidContentsSize, CriFsWebInstaller::StatusInfo.receivedSize
		 */
		public long contentsSize;

		/**
		 * \brief 受信済みのサイズ(byte)
		 * \sa CriFsWebInstaller::StatusInfo.contentsSize
		 */
		public long receivedSize;
	}

	/**
	 * <summary>モジュールコンフィギュレーション</summary>
	 * \par 説明:
	 * CriFsWebInstaller 動作仕様を指定するための構造体です。<br>
	 * モジュール初期化時（::CriFsWebInstaller::InitializeModule 関数）に引数として本構造体を指定します。<br>
	 * \par 備考:
	 * ::CriFsWebInstaller::defaultModuleConfig で取得したデフォルトコンフィギュレーションを必要に応じて変更して
	 * ::CriFsWebInstaller::InitializeModule 関数に指定してください。<br>
	 * \sa CriFsWebInstaller::InitializeModule, CriFsWebInstaller::defaultModuleConfig
	 */
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct ModuleConfig
	{
		/**
		 * <summary>同時に使用するインストーラの最大数設定</summary>
		 * \par 説明：
		 * この数を越えて CriFsWebInstaller を同時に生成することは出来ません。
		 */
		public uint numInstallers;

		[MarshalAs(UnmanagedType.LPStr)]
		/**
		 * <summary>HTTP プロキシサーバホスト名設定</summary>
		 * \par 説明：
		 * CriFsWebInstaller で使用するプロキシサーバのホスト名を設定してください。<br>
		 * null が設定された場合は、プロキシサーバは使用されません。
		 */
		public string proxyHost;

		/**
		 * <summary>HTTP プロキシサーバポート設定</summary>
		 * \par 説明：
		 * CriFsWebInstaller で使用するプロキシサーバのポートを設定してください。<br>
		 * この値は、 CriFsWebInstaller::ModuleConfig.proxyHost != null の場合のみ効果があります。
		 */
		public ushort proxyPort;

		/**
		 * <summary>User-Agent 設定</summary>
		 * \par 説明：
		 * デフォルトの User-Agent を上書きする際に設定してください。
		 * null が設定された場合は、デフォルトの User-Agent が使用されます。
		 */
		[MarshalAs(UnmanagedType.LPStr)]
		public string userAgent;

		/**
		 * <summary>タイムアウト時間設定(秒単位)</summary>
		 * \par 説明：
		 * この時間の間、受信済みのサイズが変化しない場合にタイムアウトエラー( CriFsWebinstaller::Error.Timeout )が発生します。
		 * \sa CriFsWebInstaller::StatusInfo.error, CriFsWebinstaller::Error.Timeout
		 */
		public uint inactiveTimeoutSec;

		/**
		 * <summary>安全でない HTTPS 通信の許可設定</summary>
		 * \par 説明：
		 * true の場合、安全でない HTTPS 通信を許可します。<br>
		 * アプリケーション開発時に、有効なサーバ証明書を用意出来ない場合のみ true を設定してください。
		 * \attention
		 *   - Apple のプラットフォームにおいて安全でない HTTPS 通信を許可するためには、
		 *     このフラグを true にすることに加えて、 ATS(App Transport Security) を無効にするか、
		 *     例外設定を行なう必要があります。
		 */
		public bool allowInsecureSSL;

		/**
		 * <summary>プラットフォーム固有の設定</summary>
		 */
		public ModulePlatformConfig platformConfig;
	}

	#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
	public struct ModulePlatformConfig
	{
		public byte	reserved;

		public static ModulePlatformConfig defaultConfig {
			get {
				ModulePlatformConfig config;
				config.reserved = 0;
				return config;
			}
		}
	}
	#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
	public struct ModulePlatformConfig
	{
		public byte	reserved;

		public static ModulePlatformConfig defaultConfig {
			get {
				ModulePlatformConfig config;
				config.reserved = 0;
				return config;
			}
		}
	}
	#elif UNITY_IOS
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct ModulePlatformConfig
	{
		public byte	reserved;

		public static ModulePlatformConfig defaultConfig {
			get {
				ModulePlatformConfig config;
				config.reserved = 0;
				return config;
			}
		}
	}
	#elif UNITY_ANDROID
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct ModulePlatformConfig
	{
		public byte	reserved;

		public static ModulePlatformConfig defaultConfig {
			get {
				ModulePlatformConfig config;
				config.reserved = 0;
				return config;
			}
		}
	}
	#else
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct ModulePlatformConfig
	{
		public static ModulePlatformConfig defaultConfig {
			get {
				ModulePlatformConfig config;
				return config;
			}
		}
	}
	#endif
	#endregion

	#region Static Properties
	public static bool isInitialized { get; private set; }

	/**
	 * <summary>デフォルトモジュールコンフィギュレーション</summary>
	 * \par 説明:
	 * デフォルトモジュールコンフィグです。
	 * \par 備考:
	 * 本プロパティで取得したデフォルトコンフィギュレーションを必要に応じて変更して
	 * ::CriFsWebInstaller::InitializeModule 関数に指定してください。<br>
	 * \sa CriFsWebInstaller::InitializeModule
	 */
	public static ModuleConfig defaultModuleConfig {
		get {
			ModuleConfig config;
			config.numInstallers		= 2;
			config.proxyHost			= null;
			config.proxyPort			= 0;
			config.userAgent			= null;
			config.inactiveTimeoutSec	= 300;
			config.allowInsecureSSL		= false;
			config.platformConfig		= ModulePlatformConfig.defaultConfig;
			return config;
		}
	}
	#endregion

	#region Constant Variables
	/**
	 * <summary>無効なHTTPステータスコード</summary>
	 * \par 説明:
	 * 無効なHTTPステータスコードを表わす定数です。<br>
	 * HTTP以外の原因でインストールに失敗した場合にセットされます。<br>
	 * この値は負値であることが保証されます。
	 * \sa CriFsWebInstaller::StatusInfo.httpStatusCode
	 */
	public const int	InvalidHttpStatusCode	= -1;

	/**
	 * <summary>無効なコンテンツサイズ</summary>
	 * \par 説明:
	 * インストール対象のサイズが取得出来ていない場合にセットされます。<br>
	 * この値は負値であることが保証されます。
	 * \sa CriFsWebInstaller::StatusInfo.contentsSize
	 */
	public const long	InvalidContentsSize		= -1;
	#endregion


	#if CRIFSWEBINSTALLER_SUPPORTED
	#region Private Variables
	private bool	disposed	= false;
	private IntPtr	handle		= IntPtr.Zero;
	#endregion

	public CriFsWebInstaller()
	{
		criFsWebInstaller_Create(out this.handle, IntPtr.Zero);
		if (this.handle == IntPtr.Zero)
		{
			throw new Exception("criFsWebInstaller_Create() failed.");
		}
	}

	~CriFsWebInstaller()
	{
		this.Dispose(false);
	}

	/**
	 * <summary>インストーラを破棄します。</summary>
	 * \attention
	 * インストール処理中にインストーラを破棄した場合、
	 * 本関数内で処理が長時間ブロックされる可能性があります。<br>
	 */
	public void Dispose()
	{
		this.Dispose(true);
		System.GC.SuppressFinalize(this);
	}


	/**
	 * <summary>ファイルをインストールします。</summary>
	 * <param name="url">インストール元URL</param>
	 * <param name="dstPath">インストール先ファイルパス名</param>
	 * \par 説明:
	 * ファイルのインストールを開始します。<br>
	 * 本関数は即時復帰関数です。<br>
	 * コピーの完了状態を取得するには ::CriFsWebInstaller::GetStatusInfo 関数を使用してください。
	 * \attention
	 *   - インストール先のファイルが存在する場合はエラー CriFsWebInstaller.Error.LocalFs が発生します。
	 *   - インストール先のフォルダが存在しない場合はエラー  CriFsWebInstaller.Error.LocalFs が発生します。
	 * \sa CriFsWebInstaller::GetStatusInfo
	 */
	public void Copy(string url, string dstPath)
	{
		criFsWebInstaller_Copy(this.handle, url, dstPath);
	}

	/**
	 * <summary>インストール処理を停止します。</summary>
	 * \par 説明:
	 * 処理を停止します。<br>
	 * 本関数は即時復帰関数です。<br>
	 * 停止の完了状態を取得するには ::CriFsWebInstaller::GetStatusInfo 関数を使用してください。
	 * \sa
	 * CriFsInstaller::GetStatusInfo
	 */
	public void Stop()
	{
		criFsWebInstaller_Stop(this.handle);
	}

	/**
	 * <summary>ステータス情報を取得します。</summary>
	 * <returns>ステータス情報</returns>
	 * \sa CriFsWebInstaller::StatusInfo
	 */
	public StatusInfo GetStatusInfo()
	{
		StatusInfo statusInfo;
		criFsWebInstaller_GetStatusInfo(this.handle, out statusInfo);
		return statusInfo;
	}

	#region Static Methods
	/**
	 * <summary>CriFsWebInstaller モジュールの初期化</summary>
	 * <param name="config">コンフィギュレーション</param>
	 * \par 説明:
	 * CriFsWebInstaller モジュールを初期化します。<br>
	 * モジュールの機能を利用するには、必ずこの関数を実行する必要があります。<br>
	 * （モジュールの機能は、本関数を実行後、 ::CriFsWebInstaller::FinalizeModule 関数を実行するまでの間、利用可能です。）<br>
	 * \attention
	 * 本関数を実行後、必ず対になる ::CriFsWebInstaller::FinalizeModule 関数を実行してください。<br>
	 * また、 ::CriFsWebInstaller::FinalizeModule 関数を実行するまでは、本関数を再度実行することはできません。<br>
	 * \sa CriFsWebInstaller::ModuleConfig, CriFsWebInstaller::FinalizeModule
	 */
	public static void InitializeModule(ModuleConfig config)
	{
		if (isInitialized) {
			UnityEngine.Debug.LogError("[CRIWARE] CriFsWebInstaller module is already initialized.");
			return;
		}
		CriFsPlugin.InitializeLibrary();
		criFsWebInstaller_Initialize(ref config);
		isInitialized = true;
	}

	/**
	 * <summary>CriFsWebInstaller モジュールの終了</summary>
	 * \par 説明:
	 * CriFsWebInstaller モジュールを終了します。<br>
	 * \attention
	 *   - ::CriFsWebInstaller::InitializeModule 関数実行前に本関数を実行することはできません。<br>
	 *   - 全ての ::CriFsWebInstaller が破棄されている必要があります。
	 * \sa CriFsWebInstaller::InitializeModule
	 */
	public static void FinalizeModule()
	{
		if (!isInitialized) {
			UnityEngine.Debug.LogError("[CRIWARE] CriFsWebInstaller module is not initialized.");
			return;
		}
		criFsWebInstaller_Finalize();
		CriFsPlugin.FinalizeLibrary();
		isInitialized = false;
	}

	/**
	 * <summary>サーバ処理の実行</summary>
	 * \par 説明:
	 * サーバ処理を実行します。定期的に実行する必要があります。<br>
	 */
	public static void ExecuteMain()
	{
		criFsWebInstaller_ExecuteMain();
	}
	#endregion

	#region Private Methods
	private void Dispose(bool disposing)
	{
		if (disposed) {
			return;
		}

		if (this.handle != IntPtr.Zero) {
			var statusInfo = this.GetStatusInfo();
			if (statusInfo.status != Status.Stop) {
				this.Stop();
				while (true) {
					ExecuteMain();
					statusInfo = this.GetStatusInfo();
					if (statusInfo.status == Status.Stop) {
						break;
					}
					System.Threading.Thread.Sleep(1);
				}
			}
			criFsWebInstaller_Destroy(this.handle);
			this.handle = IntPtr.Zero;
		}
	}
	#endregion

	#region Native API Definitions
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criFsWebInstaller_Initialize([In] ref ModuleConfig config);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criFsWebInstaller_Finalize();

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criFsWebInstaller_ExecuteMain();

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criFsWebInstaller_Create(out IntPtr installer, IntPtr option);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criFsWebInstaller_Destroy(IntPtr installer);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criFsWebInstaller_Copy(IntPtr installer, string url, string dstPath);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criFsWebInstaller_Stop(IntPtr installer);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criFsWebInstaller_GetStatusInfo(IntPtr installer, out StatusInfo status);
	#endregion

	#else
	#region Internal Variables
	private bool disposed = false;
	private bool errorOccured = false;
	#endregion

	public CriFsWebInstaller()
	{
	}

	~CriFsWebInstaller()
	{
		this.Dispose(false);
	}

	public void Dispose()
	{
		this.Dispose(true);
		System.GC.SuppressFinalize(this);
	}

	public void Copy(string url, string dstPath)
	{
		Debug.LogError("[CRIWARE] CriWebInstaller does not support this platform.");
		errorOccured = true;
	}

	public void Stop()
	{
		errorOccured = false;
	}

	public StatusInfo GetStatusInfo()
	{
		StatusInfo statusInfo;
		if (errorOccured) {
			statusInfo.status	= Status.Error;
			statusInfo.error	= Error.None;
		} else {
			statusInfo.status	= Status.Stop;
			statusInfo.error	= Error.Internal;
		}
		statusInfo.httpStatusCode	= InvalidHttpStatusCode;
		statusInfo.contentsSize		= InvalidContentsSize;
		statusInfo.receivedSize		= 0;
		return statusInfo;
	}

	#region Static Methods
	public static void InitializeModule(ModuleConfig config)
	{
		if (isInitialized) {
			UnityEngine.Debug.LogError("[CRIWARE] CriFsWebInstaller module is already initialized.");
			return;
		}
		CriFsPlugin.InitializeLibrary();
		isInitialized = true;
	}

	public static void FinalizeModule()
	{
		if (!isInitialized) {
			UnityEngine.Debug.LogError("[CRIWARE] CriFsWebInstaller module is not initialized.");
			return;
		}
		CriFsPlugin.FinalizeLibrary();
		isInitialized = false;
	}

	public static void ExecuteMain()
	{
	}
	#endregion

	#region Private Methods
	private void Dispose(bool disposing)
	{
		if (disposed) {
			return;
		}
	}

	private void UnsupportedError()
	{
	}
	#endregion
	#endif
}


/**
 * @}
 */
