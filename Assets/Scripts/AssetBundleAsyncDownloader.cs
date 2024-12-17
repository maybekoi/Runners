using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleAsyncDownloader : MonoBehaviour
{
	private AssetBundleRequest mAbRquest;

	private AsyncDownloadCallback mAsyncDownloadCallback;

	private bool mDownloading;

	public AsyncDownloadCallback asyncLoadedCallback
	{
		get
		{
			return mAsyncDownloadCallback;
		}
		set
		{
			mAsyncDownloadCallback = value;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		if (mAbRquest != null)
		{
			try
			{
				StartCoroutine(Load());
			}
			catch (Exception ex)
			{
				Debug.Log("AssetBundleAsyncDownloader.Start() ExceptionMessage = " + ex.Message + "ToString() = " + ex.ToString());
			}
		}
	}

	public void SetBundleRequest(AssetBundleRequest request)
	{
		mAbRquest = request;
	}

	private void Update()
	{
	}

	private IEnumerator Load()
	{
		UnityWebRequest www = null;
		if (!mAbRquest.useCache)
		{
			www = UnityWebRequest.Get(mAbRquest.path);
		}
		else
		{
			if (mAbRquest.crc == 0)
			{
				www = UnityWebRequestAssetBundle.GetAssetBundle(mAbRquest.path, (uint)mAbRquest.version);
			}
			else
			{
				www = UnityWebRequestAssetBundle.GetAssetBundle(mAbRquest.path, (uint)mAbRquest.version, mAbRquest.crc);
			}
		}

		yield return www.SendWebRequest();

		if (mAsyncDownloadCallback != null)
		{
			mAsyncDownloadCallback(www);
		}
	}
}
