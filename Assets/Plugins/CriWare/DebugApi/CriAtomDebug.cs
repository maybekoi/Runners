/****************************************************************************
 *
 * Copyright (c) 2016 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/
using System;
using System.Runtime.InteropServices;


/**
 * \addtogroup CRIATOM_NATIVE_WRAPPER
 * @{
 */


/**
 * <summary>CriAtomEx のアプリケーションデバッグ向けの API をまとめたクラスクラスです。</summary>
 */
public static class CriAtomExDebug
{
	/**
	 * <summary>CriAtomEx 内部の各種リソースの状況</summary>
	 */
	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct ResourcesInfo
	{
		/** バーチャルボイスの使用状況 */
		public CriAtomEx.ResourceUsage virtualVoiceUsage;

		/** シーケンスの使用状況 */
		public CriAtomEx.ResourceUsage sequenceUsage;

		/** シーケンストラックの使用状況 */
		public CriAtomEx.ResourceUsage sequenceTrackUsage;

		/** シーケンストラックアイテムの使用状況 */
		public CriAtomEx.ResourceUsage sequenceTrackItemUsage;
	}

	/**
	 * <summary>CriAtomEx 内部の各種リソースの状況の取得</summary>
	 * <param name="resourcesInfo">CriAtomEx 内部の各種リソースの状況</param>
	 * \par 説明:
	 * CriAtomEx 内部の各種リソースの状況取得します。<br>
	 */
	public static void GetResourcesInfo (out ResourcesInfo resourcesInfo)
	{
		criAtomExDebug_GetResourcesInfo (out resourcesInfo);
	}

	#region Private

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExDebug_GetResourcesInfo (out ResourcesInfo resourcesInfo);

	#endregion
}


/**
 * <summary>CriAtomExAcf のアプリケーションデバッグ向けの API をまとめたクラスクラスです。</summary>
 */
public static class CriAtomExAcfDebug
{
	/**
	 * <summary>カテゴリ情報</summary>
	 */
	public struct CategoryInfo
	{
		/** グループ番号 */
		public uint groupNo;

		/** カテゴリID */
		public uint id;

		/** カテゴリ名 */
		public string name;

		/** キューリミット数 */
		public uint numCueLimits;

		/** ボリューム */
		public float volume;
	};

	/**
	 * <summary>DSPバス設定情報</summary>
	 */
	public struct DspBusInfo
	{
		/** 名前 */
		public string name;

		/** 音量 */
		public float volume;

		/** Pan3D 音量 */
		public float pan3dVolume;

		/** Pan3D 角度 */
		public float pan3dAngle;

		/** Pan3D インテリア距離 */
		public float pan3dDistance;

		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
		/** DSP FXインデックス配列 */
		public ushort[] fxIndexes;

		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 64)]
		/** DSPバスリンクインデックス配列 */
		public ushort[] busLinkIndexes;

		/** セッティング内DSPバス番号 */
		public ushort busNo;

		/** DSP FX数 */
		public byte numFxes;

		/** DSPバスリンク数 */
		public byte numBusLinks;
	};
	
	/**
	 * <summary>カテゴリ数の取得</summary>
	 * <returns>カテゴリ数</returns>
	 * \par 説明:
	 * 登録されたACFに含まれるカテゴリの数を取得します。
	 */
	public static int GetNumCategories ()
	{
		return criAtomExAcf_GetNumCategories ();
	}
	
	/**
	 * <summary>カテゴリ情報の取得（インデックス指定）<summary>
	 * <param name="index">カテゴリインデックス</param>
	 * <param name="categoryInfo">カテゴリ情報</param>
	 * <returns>情報が取得出来たかどうか</returns>
	 * \par 説明:
	 * カテゴリインデックスからカテゴリ情報を取得します。<br>
	 * 指定したインデックスのカテゴリが存在しない場合、 false が返ります。
	 */
	public static bool GetCategoryInfoByIndex (ushort index, out CategoryInfo categoryInfo)
	{
		CategoryInfoForMarshaling x;
		bool result = criAtomExAcf_GetCategoryInfo (index, out x) == 1;
		x.Convert (out categoryInfo);
		return result;
	}

	/**
	 * <summary>カテゴリ情報の取得（名前指定）<summary>
	 * <param name="name">カテゴリ名</param>
	 * <param name="categoryInfo">カテゴリ情報</param>
	 * <returns>情報が取得出来たかどうか</returns>
	 * \par 説明:
	 * カテゴリインデックスからカテゴリ情報を取得します。<br>
	 * 指定したインデックスのカテゴリが存在しない場合、 false が返ります。
	 */
	public static bool GetCategoryInfoByName (string name, out CategoryInfo categoryInfo)
	{
		CategoryInfoForMarshaling x;
		bool result = criAtomExAcf_GetCategoryInfoByName (name, out x) == 1;
		x.Convert (out categoryInfo);
		return result;
	}
	
	/**
	 * <summary>カテゴリ情報の取得（ID指定）<summary>
	 * <param name="id">カテゴリID</param>
	 * <param name="categoryInfo">カテゴリ情報</param>
	 * <returns>情報が取得出来たかどうか</returns>
	 * \par 説明:
	 * カテゴリインデックスからカテゴリ情報を取得します。<br>
	 * 指定したインデックスのカテゴリが存在しない場合、 false が返ります。
	 */
	public static bool GetCategoryInfoById (uint id, out CategoryInfo categoryInfo)
	{
		CategoryInfoForMarshaling x;
		bool result = criAtomExAcf_GetCategoryInfoById (id, out x) == 1;
		x.Convert (out categoryInfo);
		return result;
	}
	
	/**
	 * <summary>DSPバスの取得</summary>
	 * <param name="index">バスインデックス</param>
	 * <param name="dspBusInfo">バス情報</param>
	 * <returns>カテゴリ情報が取得出来たかどうか</returns>
	 * \par 説明:
	 * インデックスを指定してDSPバス情報を取得します。<br>
	 * 指定したインデックス名のDSPバスが存在しない場合、 false が返ります。
	 */
	public static bool GetDspBusInformation (ushort index, out DspBusInfo dspBusInfo)
	{
		DspBusInfoForMarshaling x;
		bool result = criAtomExAcf_GetDspBusInformation (index, out x) == 1;
		x.Convert (out dspBusInfo);
		return result;
	}

	/**
	 * <summary>AISACコントロールIDの取得（AISACコントロール名指定）</summary>
	 * <param name="name">AISACコントロール名</param>
	 * <returns>AISACコントロールID</returns>
	 * \par 説明:
	 * AISACコントロール名からAISACコントロールIDを取得します。<br>
	 * ACFが登録されていない、または指定したAISACコントロール名のAISACコントロールが存在しない場合、CriAtomEx.InvalidAisacControlId が返ります。
	 */
	public static uint GetAisacControlIdByName (string name)
	{
		return criAtomExAcf_GetAisacControlIdByName (name);
	}
	
	/**
	 * <summary>AISACコントロール名の取得（AISACコントロールID指定）</summary>
	 * <param name="id">AISACコントロールID</param>
	 * <returns>AISACコントロールID</returns>
	 * \par 説明:
	 * AISACコントロールIDからAISACコントロール名を取得します。<br>
	 * ACFが登録されていない、または指定したAISACコントロールIDのAISACコントロールが存在しない場合、null が返ります。
	 */
	public static string GetAisacControlNameById (uint id)
	{
		IntPtr namePtr = criAtomExAcf_GetAisacControlNameById (id);
		return CriAtomDebugDetail.Utility.PtrToStringAutoOrNull (namePtr);
	}

	#region Private

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	private struct CategoryInfoForMarshaling
	{
		public uint groupNo;
		public uint id;
		public IntPtr namePtr;
		public uint numCueLimits;
		public float volume;

		public void Convert (out CategoryInfo x)
		{
			x.groupNo = groupNo;
			x.id = id;
			x.name = CriAtomDebugDetail.Utility.PtrToStringAutoOrNull (namePtr);
			x.numCueLimits = numCueLimits;
			x.volume = volume;
		}
	};

	private struct DspBusInfoForMarshaling
	{
		public IntPtr namePtr;
		public float volume;
		public float pan3dVolume;
		public float pan3dAngle;
		public float pan3dDistance;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
		public ushort[] fxIndexes;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 64)]
		public ushort[] busLinkIndexes;
		public ushort busNo;
		public byte numFxes;
		public byte numBusLinks;

		public void Convert (out DspBusInfo x)
		{
			x.name = CriAtomDebugDetail.Utility.PtrToStringAutoOrNull (namePtr);
			x.volume = volume;
			x.pan3dVolume = pan3dVolume;
			x.pan3dAngle = pan3dAngle;
			x.pan3dDistance = pan3dDistance;
			x.fxIndexes = fxIndexes;
			x.busLinkIndexes = busLinkIndexes;
			x.busNo = busNo;
			x.numFxes = numFxes;
			x.numBusLinks = numBusLinks;
		}
	};

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExAcf_GetNumCategories ();

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExAcf_GetCategoryInfo (ushort index, out CategoryInfoForMarshaling categoryInfo);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExAcf_GetCategoryInfoByName (string name, out CategoryInfoForMarshaling categoryInfo);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExAcf_GetCategoryInfoById (uint id, out CategoryInfoForMarshaling categoryInfo);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExAcf_GetDspBusInformation (ushort index, out DspBusInfoForMarshaling dspBusInfo);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern uint criAtomExAcf_GetAisacControlIdByName (string name);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern IntPtr criAtomExAcf_GetAisacControlNameById (uint id);

	#endregion
}


/**
 * <summary>CriAtomExAcb のアプリケーションデバッグ向けの API をまとめたクラスクラスです。</summary>
 */
public static class CriAtomExAcbDebug
{
	/**
	 * <summary>ACB情報</summary>
	 * \par 説明:
	 * ACBデータの各種情報です。
	 */
	public struct AcbInfo
	{
		/** 名前 */
		public string name;

		/** サイズ */
		public uint size;

		/** ACBバージョン */
		public uint version;

		/** 文字コード */
		public CriAtomEx.CharacterEncoding characterEncoding;

		/** キューシートボリューム */
		public float volume;

		/** キュー数 */
		public int numCues;
	};
	
	/**
	 * <summary>ACB情報の取得</summary>
	 * <param name="acb">ACB</param>
	 * <param name="acbInfo">ACB情報</param>
	 * <returns>情報が取得できたか</returns>
	 * \par 説明:
	 * ACBデータの各種情報を取得します。
	 */
	public static bool GetAcbInfo (CriAtomExAcb acb, out AcbInfo acbInfo)
	{
		AcbInfoForMarshaling x;
		bool result = criAtomExAcb_GetAcbInfo (acb.nativeHandle, out x) == 1;
		x.Convert (out acbInfo);
		return result;
	}

	#region Private

	[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	private struct AcbInfoForMarshaling
	{
		public IntPtr namePtr;
		public uint size;
		public uint version;
		public CriAtomEx.CharacterEncoding characterEncoding;
		public float volume;
		public int numCues;

		public void Convert (out AcbInfo x)
		{
			x.name = CriAtomDebugDetail.Utility.PtrToStringAutoOrNull (namePtr);
			x.size = size;
			x.version = version;
			x.characterEncoding = characterEncoding;
			x.volume = volume;
			x.numCues = numCues;
		}
	};

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExAcb_GetAcbInfo (IntPtr acbHn, out AcbInfoForMarshaling acbInfo);

	#endregion
}


/**
 * <summary>CriAtomExPlayback のアプリケーションデバッグ向けの API をまとめたクラスクラスです。</summary>
 */
public static class CriAtomExPlaybackDebug
{
	/**
	 * <summary>パラメータの取得</summary>
	 * <param name="playback">CriAtomExPlayback</param>
	 * <param name="parameterId">パラメータID</param>
	 * <param name="value">パラメータの値(出力)</param>
	 * <returns>パラメータの取得に成功したか</returns>
	 * \par 説明:
	 * CriAtomExPlayback の各種パラメータの値を取得します。<br>
	 * パラメータが取得できた場合、本関数は true を返します。<br>
	 * 指定したボイスが既に消去されている場合等には、本関数は false を返します。<br>
	 */
	public static bool GetParameter (CriAtomExPlayback playback, CriAtomEx.Parameter parameterId, out float value)
	{
		return criAtomExPlayback_GetParameterFloat32 (playback.id, (int)parameterId, out value) == 1;
	}

	/**
	 * <summary>パラメータの取得</summary>
	 * <param name="playback">CriAtomExPlayback</param>
	 * <param name="parameterId">パラメータID</param>
	 * <param name="value">パラメータの値(出力)</param>
	 * <returns>パラメータの取得に成功したか</returns>
	 * \par 説明:
	 * CriAtomExPlayback の各種パラメータの値を取得します。<br>
	 * パラメータが取得できた場合、本関数は true を返します。<br>
	 * 指定したボイスが既に消去されている場合等には、本関数は false を返します。<br>
	 */
	public static bool GetParameter (CriAtomExPlayback playback, CriAtomEx.Parameter parameterId, out uint value)
	{
		return criAtomExPlayback_GetParameterUint32 (playback.id, (int)parameterId, out value) == 1;
	}

	/**
	 * <summary>パラメータの取得</summary>
	 * <param name="playback">CriAtomExPlayback</param>
	 * <param name="parameterId">パラメータID</param>
	 * <param name="value">パラメータの値(出力)</param>
	 * <returns>パラメータの取得に成功したか</returns>
	 * \par 説明:
	 * CriAtomExPlayback の各種パラメータの値を取得します。<br>
	 * パラメータが取得できた場合、本関数は true を返します。<br>
	 * 指定したボイスが既に消去されている場合等には、本関数は false を返します。<br>
	 */
	public static bool GetParameter (CriAtomExPlayback playback, CriAtomEx.Parameter parameterId, out int value)
	{
		return criAtomExPlayback_GetParameterSint32 (playback.id, (int)parameterId, out value) == 1;
	}

	/**
	 * <summary>AISACコントロール値の取得（コントロールID指定）</summary>
	 * <param name="playback">Playback</param>
	 * <param name="controlId">コントロールID</param>
	 * <param name="value">コントロール値（0.0f～1.0f）、未設定時は-1.0f</param>
	 * <returns>AISACコントロール値が取得できたか</returns>
	 * \par 説明:
	 * ::CriAtomExPlayer::Start で再生された音声に設定されているAISACコントロール値を、コントロールID指定で取得します。<br>
	 * AISACコントロール値が取得できた場合（未設定時も「-1.0fが取得できた」と扱われます）、本関数は true を返します。<br>
	 * 指定したボイスが既に消去されている場合等には、本関数は false を返します。<br>
	 * \attention
	 * 本関数は、音声再生中のみAISACコントロール値を取得可能です。<br>
	 * 再生終了後や、発音数制御によりボイスが消去された場合には、AISACコントロール値の取得に失敗します。
	 */
	public static bool GetAisacControl (CriAtomExPlayback playback, uint controlId, out float value)
	{
		return criAtomExPlayback_GetAisacControlById (playback.id, controlId, out value) == 1;
	}

	/**
	 * <summary>AISACコントロール値の取得（コントロール名指定）</summary>
	 * <param name="playback">Playback</param>
	 * <param name="controlName">コントロール名</param>
	 * <param name="value">コントロール値（0.0f～1.0f）、未設定時は-1.0f</param>
	 * <returns>AISACコントロール値が取得できたか</returns>
	 * \par 説明:
	 * ::CriAtomExPlayer::Start で再生された音声に設定されているAISACコントロール値を、コントロールID指定で取得します。<br>
	 * AISACコントロール値が取得できた場合（未設定時も「-1.0fが取得できた」と扱われます）、本関数は true を返します。<br>
	 * 指定したボイスが既に消去されている場合等には、本関数は false を返します。<br>
	 * \attention
	 * 本関数は、音声再生中のみAISACコントロール値を取得可能です。<br>
	 * 再生終了後や、発音数制御によりボイスが消去された場合には、AISACコントロール値の取得に失敗します。
	 */
	public static bool GetAisacControl (CriAtomExPlayback playback, string controlName, out float value)
	{
		return criAtomExPlayback_GetAisacControlByName (playback.id, controlName, out value) == 1;
	}

	#region Private

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExPlayback_GetParameterFloat32 (uint id, int parameterId, out float value);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExPlayback_GetParameterUint32 (uint id, int parameterId, out uint value);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExPlayback_GetParameterSint32 (uint id, int parameterId, out int value);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExPlayback_GetAisacControlById (uint id, uint controlId, out float value);

	[DllImport (CriWare.pluginName, CallingConvention = CriWare.pluginCallingConvention)]
	private static extern int criAtomExPlayback_GetAisacControlByName (uint id, string controlName, out float value);

	#endregion
}


namespace CriAtomDebugDetail
{
	public class Utility
	{
		public static string PtrToStringAutoOrNull(IntPtr stringPtr)
		{
#if !UNITY_EDITOR && UNITY_WINRT
            return (stringPtr == IntPtr.Zero) ? null : Marshal.PtrToStringUni(stringPtr);
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return (stringPtr == IntPtr.Zero) ? null : Marshal.PtrToStringAnsi(stringPtr);
#else
            return (stringPtr == IntPtr.Zero) ? null : Marshal.PtrToStringAuto(stringPtr);
#endif
		}
	}
}


/**
 * @}
 */
