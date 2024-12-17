using Message;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleRequest
{
	public enum Type
	{
		GAMEOBJECT,
		TEXTURE,
		TEXT,
		SCENE,
		OTHER
	}

	private enum State
	{
		INVALID = -1,
		EXECUTING,
		SUCCEEDED,
		FAILED,
		RETRY
	}

	private bool mUseCache;

	private State mState;

	private string mPath;

	private string mFileName;

	private GameObject mReturnObject;

	private UnityWebRequest mWebRequest;

	private int mVersion;

	private uint mCRC;

	private Type mType;

	private string mURL;

	private int mTryCount;

	private int mMaxTryCount = 1;

	private bool mIsLoaded;

	private AssetBundleResult mAssetbundleResult;

	private GameObject mDownloaderObject;

	public static readonly float DefaultTimeOut = 60f;

	private float mTimeOut = DefaultTimeOut;

	private float mElapsedTime;

	private bool mCancel;

	public bool useCache
	{
		get
		{
			return mUseCache;
		}
	}

	public bool isCancel
	{
		get
		{
			return mCancel;
		}
	}

	public UnityWebRequest webRequest
	{
		get
		{
			return mWebRequest;
		}
	}

	public int version
	{
		get
		{
			return mVersion;
		}
	}

	public uint crc
	{
		get
		{
			return mCRC;
		}
	}

	public Type type
	{
		get
		{
			return mType;
		}
	}

	public string Url
	{
		get
		{
			return mURL;
		}
	}

	public Texture2D Texture
	{
		get
		{
			return mAssetbundleResult.Texture;
		}
	}

	public string path
	{
		get
		{
			return mPath;
		}
	}

	public string FileName
	{
		get
		{
			return mFileName;
		}
	}

	public AssetBundleResult assetbundleResult
	{
		get
		{
			return mAssetbundleResult;
		}
	}

	public GameObject returnObject
	{
		get
		{
			return mReturnObject;
		}
	}

	public bool IsTypeTexture
	{
		get
		{
			return (mType == Type.TEXTURE) ? true : false;
		}
	}

	public float TimeOut
	{
		get
		{
			return mTimeOut;
		}
		set
		{
			if (value > DefaultTimeOut * 3f)
			{
				value = DefaultTimeOut * 3f;
			}
			mTimeOut = value;
		}
	}

	public int TryCount
	{
		get
		{
			return mTryCount;
		}
	}

	public int MaxTryCount
	{
		get
		{
			return mMaxTryCount;
		}
	}

	public AssetBundleRequest(string path, int version, uint crc, Type type, GameObject returnObject)
		: this(path, version, crc, type, returnObject, false)
	{
	}

	public AssetBundleRequest(string path, int version, uint crc, Type type, GameObject returnObject, bool useCache)
	{
		mUseCache = useCache;
		mState = State.INVALID;
		mPath = path;
		mFileName = Path.GetFileNameWithoutExtension(path);
		mReturnObject = returnObject;
		mWebRequest = null;
		mVersion = version;
		mCRC = crc;
		mType = type;
		mTryCount = 0;
		mCancel = false;
		mAssetbundleResult = null;
		mIsLoaded = false;
	}

	public AssetBundleRequest(AssetBundleRequest request)
	{
		mUseCache = request.useCache;
		mState = State.INVALID;
		mPath = request.path;
		mFileName = request.mFileName;
		mReturnObject = request.returnObject;
		mWebRequest = null;
		mVersion = request.version;
		mCRC = request.crc;
		mType = request.type;
		mTryCount = 0;
		mCancel = false;
		mAssetbundleResult = null;
		mIsLoaded = false;
	}

	public void Load()
	{
		if (!IsExecuting() && mDownloaderObject == null)
		{
			Debug.Log($"AssetBundleRequest:{mFileName}");
			mDownloaderObject = new GameObject("AssetBundleAsyncDownloader");
			AssetBundleAsyncDownloader assetBundleAsyncDownloader = mDownloaderObject.AddComponent<AssetBundleAsyncDownloader>();
			assetBundleAsyncDownloader.SetBundleRequest(this);
			assetBundleAsyncDownloader.asyncLoadedCallback = LoadedCallback;
			mState = State.EXECUTING;
			mElapsedTime = 0f;
		}
	}

	public void Cancel()
	{
		mCancel = true;
	}

	public void Unload()
	{
		if (!AssetBundleManager.Instance.IsCancelRequest() && IsExecuting())
		{
			Debug.LogWarning("AssetBundleRequest.Unload : Now executing...");
			return;
		}
		if (mAssetbundleResult != null)
		{
			mAssetbundleResult.Clear();
			mAssetbundleResult = null;
		}
		if (mDownloaderObject != null)
		{
			UnityEngine.Object.Destroy(mDownloaderObject);
			mDownloaderObject = null;
		}
		if (mWebRequest != null)
		{
			mWebRequest.Dispose();
			mWebRequest = null;
		}
	}

	public bool IsInvalid()
	{
		if (mState == State.INVALID)
		{
			return true;
		}
		return false;
	}

	public bool IsRetry()
	{
		if (mState == State.RETRY)
		{
			return true;
		}
		return false;
	}

	public bool IsSucceeded()
	{
		if (mState == State.SUCCEEDED)
		{
			return true;
		}
		return false;
	}

	public bool IsFailed()
	{
		if (mState == State.FAILED)
		{
			return true;
		}
		return false;
	}

	public bool IsExecuting()
	{
		if (mState == State.EXECUTING)
		{
			return true;
		}
		return false;
	}

	private void LoadedCallback(UnityWebRequest www)
	{
		mWebRequest = www;
		mURL = mWebRequest.url;
		UnityEngine.Object.Destroy(mDownloaderObject);
		mDownloaderObject = null;
	}

	public bool IsLoaded()
	{
		if (mDownloaderObject != null)
		{
			return false;
		}
		if (mWebRequest == null)
		{
			return mIsLoaded;
		}
		return mWebRequest.isDone;
	}

	public bool IsLoading()
	{
		if (mDownloaderObject != null)
		{
			return true;
		}
		if (mWebRequest == null)
		{
			return !mIsLoaded;
		}
		return !mWebRequest.isDone;
	}

	public bool IsTimeOut()
	{
		if (mTimeOut <= mElapsedTime)
		{
			return true;
		}
		return false;
	}

	public float UpdateElapsedTime(float addElapsedTime)
	{
		mElapsedTime += addElapsedTime;
		return 0.1f;
	}

	private bool IsErrorTexture()
	{
		if (mAssetbundleResult == null)
		{
			return false;
		}
		Texture2D texture = mAssetbundleResult.Texture;
		if (null == texture)
		{
			return false;
		}
		if (string.Empty != texture.name)
		{
			return false;
		}
		if (texture.height != 8)
		{
			return false;
		}
		if (texture.width != 8)
		{
			return false;
		}
		if (texture.filterMode != FilterMode.Bilinear)
		{
			return false;
		}
		if (texture.anisoLevel != 1)
		{
			return false;
		}
		if (texture.wrapMode != 0)
		{
			return false;
		}
		if (texture.mipMapBias != 0f)
		{
			return false;
		}
		return true;
	}

	public Texture2D MakeTexture()
	{
		Texture2D result = null;
		if (mType == Type.TEXTURE && mWebRequest != null && !string.IsNullOrEmpty(mWebRequest.error))
		{
			result = DownloadHandlerTexture.GetContent(mWebRequest);
		}
		return result;
	}

	public string MakeText()
	{
		string result = null;
		if (mType == Type.TEXT && mWebRequest != null && !string.IsNullOrEmpty(mWebRequest.error))
		{
			result = mWebRequest.downloadHandler.text;
		}
		return result;
	}

	public void Result()
	{
		try
		{
			if (mCancel)
			{
				Debug.LogWarning($"!!! AssetBundleRequest Cancel : {mPath}", DebugTraceManager.TraceType.ASSETBUNDLE);
				if (mWebRequest != null)
				{
					mWebRequest.Dispose();
					mWebRequest = null;
				}
				mState = State.FAILED;
				return;
			}

			if (IsTimeOut())
			{
				Debug.Log($"AssetBundle TimeOut : {mPath}", DebugTraceManager.TraceType.ASSETBUNDLE);
				mState = State.FAILED;
				return;
			}

			if (mWebRequest != null && mWebRequest.isDone)
			{
				if (!string.IsNullOrEmpty(mWebRequest.error))
				{
					HandleError();
				}
				else
				{
					HandleSuccess();
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log($"AssetBundleRequest.Result() Exception:Message = {ex.Message} ToString() = {ex}");
		}
	}

	private void HandleError()
	{
		mTryCount++;
		if (mMaxTryCount > mTryCount)
		{
			Debug.LogWarning($"AssetBundle.Result : Retry[{mTryCount}/{mMaxTryCount}]", DebugTraceManager.TraceType.ASSETBUNDLE);
			mState = State.RETRY;
			if (mWebRequest != null)
			{
				mWebRequest.Dispose();
				mWebRequest = null;
			}
		}
		else
		{
			mState = State.FAILED;
		}
	}

	private void HandleSuccess()
	{
		if (mType == Type.GAMEOBJECT || mType == Type.TEXTURE || mType == Type.TEXT)
		{
			AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(mWebRequest);
			if (bundle != null)
			{
				// Handle the asset bundle...
				mState = State.SUCCEEDED;
			}
			else
			{
				mState = State.FAILED;
			}
		}
	}

	public AssetBundleResult CreateResult()
	{
		AssetBundleResult result = null;
		if (mWebRequest == null)
		{
			byte[] bytes = null;
			string empty = string.Empty;
			return new AssetBundleResult(mPath, bytes, empty);
		}
		try
		{
			string text = (mWebRequest != null && !string.IsNullOrEmpty(mWebRequest.error)) ? null : (mWebRequest.error.Clone() as string);
			AssetBundle assetBundle = (text != null) ? null : DownloadHandlerAssetBundle.GetContent(mWebRequest);
			switch (mType)
			{
			case Type.GAMEOBJECT:
				result = new AssetBundleResult(mPath, assetBundle, text);
				return result;
			case Type.TEXTURE:
			{
				Texture2D texture2D = null;
				result = new AssetBundleResult(texture: (!(assetBundle != null)) ? MakeTexture() : (assetBundle.mainAsset as Texture2D), path: mPath, assetBundle: assetBundle, err: text);
				return result;
			}
			case Type.TEXT:
			{
				string text2 = null;
				if (assetBundle != null)
				{
					TextAsset textAsset = assetBundle.mainAsset as TextAsset;
					if ((bool)textAsset)
					{
						text2 = textAsset.text;
					}
				}
				else
				{
					text2 = MakeText();
				}
				result = new AssetBundleResult(mPath, assetBundle, text2, text);
				return result;
			}
			case Type.SCENE:
				result = new AssetBundleResult(mPath, assetBundle, text);
				return result;
			default:
			{
				byte[] bytes2 = mWebRequest.downloadHandler.data.Clone() as byte[];
				result = new AssetBundleResult(mPath, bytes2, text);
				return result;
			}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("AssetBundleManager.CreateResult:Exception , Message = " + ex.Message + ", ToString() = " + ex.ToString());
			return result;
		}
	}
}
