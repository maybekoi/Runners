/****************************************************************************
 *
 * Copyright (c) 2014 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
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
 * <summary>音声出力データ解析モジュール（プレーヤ/ソース単位)</summary>
 * \par 説明:
 * CriAtomSource/CriAtomExPlayerごとの音声出力の解析を行います。<br/>
 * レベルメータ機能などを提供します。<br/>
 * \attention 注意：
 * HCA-MXやプラットフォーム固有の音声圧縮コーデックを使用している場合は解析できません。<br />
 * HCAもしくはADXコーデックをご利用ください。
 */
public class CriAtomExPlayerOutputAnalyzer : IDisposable
{
	private bool disposed = false;
	
	public IntPtr nativeHandle {get {return this.handle;} }
	
	/**
	 * <summary>解析処理種別</summary>
	 * \par 説明：
	 * 解析モジュール作成時に指定する解析処理の種別を示す値です。
	 * \sa CriAtomExPlayerOutputAnalyzer
	 */
	public enum Type {
		LevelMeter = 0,			/**< レベルメーター(RMSレベル計測)	**/
		SpectrumAnalyzer = 1,	/**< スペクトルアナライザ **/
	}

    /**
     * <summary>スペクトラムアナライザの最大バンド数</summary>
     * \par 説明：
     * スペクトラムアナライザが出力できるバンド数の最大値です。
     */
    public const int MaximumSpectrumBands = 512;

	/**
	 * <summary>音声出力データ解析モジュールコンフィグ構造体</summary>
	 * \par 説明：
	 * 解析モジュール作成時に指定するコンフィグです。<br/>
	 * num_spectrum_analyzer_bands：スペクトルアナライザのバンド数<br/>
	 * \sa CriAtomExPlayerOutputAnalyzer
	 */
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct Config
	{
		public int num_spectrum_analyzer_bands;

		public Config(int num_spectrum_analyzer_bands = 8)
		{
			this.num_spectrum_analyzer_bands = num_spectrum_analyzer_bands;
		}
	};

	/**
	 * <summary>音声出力データ解析モジュールの作成</summary>
	 * <returns>音声出力データ解析モジュール</returns>
	 * \par 説明：
	 * CriAtomSource/CriAtomExPlayerの出力音声データの解析モジュールを作成します。<br/>
	 * 作成した解析モジュールは、CriAtomSourceまたはCriAtomExPlayerにアタッチして使用します。<br/>
	 * アタッチしている音声出力に対し、レベルメータなどの解析を行います。<br/>
	 * <code>
	 * // 解析モジュールの作成例
	 *
	 * // 種別にSpectrumAnalyzerを指定
	 * CriAtomExPlayerOutputAnalyzer.Type[] type = new CriAtomExPlayerOutputAnalyzer.Type[1];
	 * type[0] = CriAtomExPlayerOutputAnalyzer.Type.SpectrumAnalyzer;
	 *
	 * // コンフィグでバンド数を指定
	 * CriAtomExPlayerOutputAnalyzer.Config[] config = new CriAtomExPlayerOutputAnalyzer.Config[1];
	 * config[0] = new CriAtomExPlayerOutputAnalyzer.Config(8);
	 *
	 * // 出力データ解析モジュールを作成
	 * this.analyzer = new CriAtomExPlayerOutputAnalyzer(type, config);
	 * </code>
	 * \par 備考：
	 * 解析モジュールにアタッチ可能なCriAtomSource/CriAtomExPlayerは一つのみです。<br/>
	 * 解析モジュールを使いまわす場合は、デタッチを行ってください。<br/>
	 * \attention 注意：
	 * 音声出力データ解析モジュールの作成時には、アンマネージドなリソースが確保されます。<br/>
	 * 解析モジュールが不要になった際は、必ず CriAtomExPlayerOutputAnalyzer.Dispose メソッドを呼んでください。
	 */
	public CriAtomExPlayerOutputAnalyzer(Type[] types, Config[] configs = null)
	{
		/* ネイティブリソースの作成 */
		this.handle = criAtomExPlayerOutputAnalyzer_Create(types.Length, types, configs);
		if (this.handle == IntPtr.Zero) {
			throw new Exception("criAtomExPlayerOutputAnalyzer_Create() failed.");
		}

		/* コンフィグ指定の記憶 */
		if (configs != null) {
			this.numBands = configs[0].num_spectrum_analyzer_bands;
		}
	}

	/**
	 * <summary>AtomExプレーヤ出力データ解析モジュールの破棄</summary>
	 * \par 説明:
	 * AtomExプレーヤ出力データ解析モジュールを破棄します。<br/>
	 * 本関数を実行した時点で、AtomExプレーヤ出力データ解析モジュール作成時にプラグイン内で確保されたリソースが全て解放されます。<br/>
	 * メモリリークを防ぐため、出力データ解析モジュールが不要になった時点で本メソッドを呼び出してください。<br/>
	 * \attention
	 * 本関数は完了復帰型の関数です。<br/>
	 * アタッチ済みのAtomExプレーヤがある場合、本関数内でデタッチが行われます。<br/>
	 * 対象のAtomExプレーヤが再生中の音声は強制的に停止しますのでご注意ください。<br/>
	 * \sa CriAtomExPlayerOutputAnalyzer::CriAtomExPlayerOutputAnalyzer
	 */
	public void Dispose()
	{
		this.Dispose(true);
	}

	private void Dispose(bool disposing)
	{
		if (disposed) {
			return;
		}
		
		if (this.handle == IntPtr.Zero) {
			return;
		}
		
		/* アタッチ済みのプレーヤがあればデタッチ */
		this.DetachExPlayer();
		
		/* ネイティブリソースの破棄 */
		criAtomExPlayerOutputAnalyzer_Destroy(this.handle);
		
		if (disposing) {
			GC.SuppressFinalize(this);
		}
		
		disposed = true;
	}

	/**
	 * <summary>AtomExプレーヤのアタッチ</summary>
	 * <returns>アタッチが成功したかどうか（成功：true、失敗：false）</returns>
	 * \par 説明:
	 * 出力データ解析を行うAtomExプレーヤをアタッチします。<br/>
	 * 複数のAtomExプレーヤをアタッチすることは出来ません。
	 * アタッチ中に別のAtomExプレーヤをアタッチした場合、アタッチ中のAtomExプレーヤはデタッチされます。<br/>
	 * <br/>
	 * CriAtomSourceをアタッチする場合、CriAtomSource::AttachToOutputAnalyzerを使用してください。
	 * \attention
	 * アタッチは再生開始前に行う必要があります。再生開始後のアタッチは失敗します。<br/>
	 * <br/>
	 * 本関数でアタッチしたAtomExプレーヤをデタッチする前に破棄した場合、
	 * デタッチ時にアクセス違反が発生します。<br/>
	 * 必ず先にデタッチを行ってからAtomExプレーヤを破棄してください。<br/>
	 * \sa CriAtomExPlayerOutputAnalyzer::DetachExPlayer, CriAtomSource::AttachToOutputAnalyzer
	 */
	public bool AttachExPlayer(CriAtomExPlayer player)
	{
		if (player == null || this.handle == IntPtr.Zero) {
			return false;
		}

		/* アタッチ済みのプレーヤがあればデタッチ */
		this.DetachExPlayer();

		/* プレーヤの状態をチェック */
		CriAtomExPlayer.Status status = player.GetStatus();
		if (status != CriAtomExPlayer.Status.Stop) {
			return false;
		}

		criAtomExPlayerOutputAnalyzer_AttachExPlayer(this.handle, player.nativeHandle);
		this.player = player;

		return true;
	}

	/**
	 * <summary>AtomExプレーヤのデタッチ</summary>
	 * \par 説明:
	 * 出力データ解析を行うAtomExプレーヤをデタッチします。<br/>
	 * デタッチを行うと、以降の解析処理は行われなくなります。
	 * \attention
	 * アタッチ済みのプレーヤが音声を再生している状態で本関数を呼び出した場合、
	 * 強制的に再生を停止した上でデタッチが行われます。<br/>
	 * <br/>
	 * アタッチしたAtomExプレーヤが既に破棄されていた場合はアクセス違反が発生します。<br/>
	 * 必ず本関数、またはCriAtomExPlayerOutputAnalyzer::Disposeを呼び出してから、
	 * AtomExプレーヤを破棄するようにしてください。<br/>
	 * \sa CriAtomExPlayerOutputAnalyzer::AttachExPlayer, CriAtomExPlayerOutputAnalyzer::Dispose
	 */
	public void DetachExPlayer()
	{
		if (this.player == null || this.handle == IntPtr.Zero) {
			return;
		}

		CriAtomExPlayer.Status status = this.player.GetStatus();
		if (status != CriAtomExPlayer.Status.Stop) {
			/* 音声再生中にデタッチは行えないため、強制的に停止 */
			Debug.LogWarning("[CRIWARE] Warning: CriAtomExPlayer is forced to stop for detaching CriAtomExPlayerOutputAnalyzer.");
			this.player.StopWithoutReleaseTime();
		}

		criAtomExPlayerOutputAnalyzer_DetachExPlayer(this.handle, this.player.nativeHandle);
		this.player = null;
	}

	/**
	 * <summary>アタッチ中の音声出力のRMSレベルの取得</summary>
	 * <param name="channel">チャンネル番号</param>
	 * <returns>RMSレベル</returns>
	 * \par 説明:
	 * アタッチ中の音声出力のRMSレベルを取得します。<br/>
	 * \sa CriAtomExPlayerOutputAnalyzer::AttachExPlayer
	 */
	public float GetRms(int channel)
	{
		if (this.player == null || this.handle == IntPtr.Zero) {
			return 0.0f;
		}

		/* プレーヤが再生状態でなければレベルを落とす */
		if (this.player.GetStatus() != CriAtomExPlayer.Status.Playing && 
			this.player.GetStatus() != CriAtomExPlayer.Status.Prep) {
			return 0.0f;
		}

		return criAtomExPlayerOutputAnalyzer_GetRms(this.handle, channel);
	}

	/**
	 * <summary>スペクトル解析結果の取得</summary>
	 * <param name="levels">解析結果(帯域毎の振幅値)</param>
	 * \par 説明：
	 * スペクトルアナライザによって解析された帯域ごとの振幅値を取得します。<br/>
	 * 配列の要素数はモジュールの作成時に指定したバンド数です。<br/>
	 * 解析結果を市販のスペクトルアナライザのように表示させたい場合、
	 * 本関数が返す値をデシベル値に変換する必要があります。<br/>
	 * \par 例：
	 * \code
	 * // 例：スペクトル解析結果を取得するコンポーネント
	 * public class SpectrumLevelMeter : MonoBehaviour {
	 *	private CriAtomExPlayerOutputAnalyzer analyzer;
	 *	void Start() {
	 *		// 引数 type, config については省略。モジュールの作成時に指定したバンド数は 8 とする
	 *		this.analyzer = new CriAtomExPlayerOutputAnalyzer(type, config);
	 *		// CriAtomExPlayer のアタッチについては省略
	 *	}
	 *
	 *	void Update() {
	 *		// 音声再生中の実行
	 *		float[] levels = new float[8];
	 *		analyzer.GetSpectrumLevels (ref levels);
	 *		// levelsの0帯域目の振幅値をデシベル値に変換
	 *		float db = 20.0f * Mathf.Log10(levels[0]);
	 *		Debug.Log (db);
	 *	}
	 * }
	 * \endcode
	 * \sa CriAtomExPlayerOutputAnalyzer::CriAtomExPlayerOutputAnalyzer,  CriAtomExPlayerOutputAnalyzer::AttachExPlayer
	 */
	public void GetSpectrumLevels(ref float[] levels)
	{
		if (this.player == null || this.handle == IntPtr.Zero) {
			return;
		}

		if (levels == null || levels.Length < numBands) {
			levels = new float[numBands];
		}
		
		IntPtr ret = criAtomExPlayerOutputAnalyzer_GetSpectrumLevels(this.handle);
		Marshal.Copy(ret, levels, 0, numBands);
	}

	#region Internal Members
	~CriAtomExPlayerOutputAnalyzer()
	{
		this.Dispose(false);
	}

	private IntPtr handle = IntPtr.Zero;
	private CriAtomExPlayer player = null;
	private int numBands = 8;
	#endregion

	#region DLL Import
	
	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern IntPtr criAtomExPlayerOutputAnalyzer_Create(int numTypes, Type[] types, Config[] configs);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern IntPtr criAtomExPlayerOutputAnalyzer_Destroy(IntPtr analyzer);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomExPlayerOutputAnalyzer_AttachExPlayer(IntPtr analyzer, IntPtr player);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern void criAtomExPlayerOutputAnalyzer_DetachExPlayer(IntPtr analyzer, IntPtr player);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern float criAtomExPlayerOutputAnalyzer_GetRms(IntPtr analyzer, int channel);

	[DllImport(CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern IntPtr criAtomExPlayerOutputAnalyzer_GetSpectrumLevels(IntPtr analyzer);

	#endregion
}

/**
 * @}
 */

/* --- end of file --- */
