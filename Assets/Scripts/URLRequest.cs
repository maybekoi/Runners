using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class URLRequest
{
	private bool mEmulation;

	private string mURL;

	private List<URLRequestParam> mParamList;

	private Action mDelegateRequest;

	private Action<UnityWebRequest> mDelegateSuccess;

	private Action<UnityWebRequest, bool, bool> mDelegateFailure;

	private bool mCompleted;

	private bool mNotReachability;

	private float mElapsedTime;

	private UnityWebRequest mWebRequest;

	private string mFormString;

	public bool Completed
	{
		get
		{
			return mCompleted;
		}
	}

	public float ElapsedTime
	{
		get
		{
			return mElapsedTime;
		}
	}

	public List<URLRequestParam> ParamList
	{
		get
		{
			return mParamList;
		}
	}

	public string FormString
	{
		get
		{
			return mFormString;
		}
	}

	public string url
	{
		get
		{
			return mURL;
		}
		set
		{
			mURL = value;
		}
	}

	public Action beginRequest
	{
		get
		{
			return mDelegateRequest;
		}
		set
		{
			mDelegateRequest = value;
		}
	}

	public Action<UnityWebRequest> success
	{
		get
		{
			return mDelegateSuccess;
		}
		set
		{
			mDelegateSuccess = value;
		}
	}

	public Action<UnityWebRequest, bool, bool> failure
	{
		get
		{
			return mDelegateFailure;
		}
		set
		{
			mDelegateFailure = value;
		}
	}

	public float TimeOut
	{
		get
		{
			return NetUtil.ConnectTimeOut;
		}
		private set
		{
		}
	}

	public bool Emulation
	{
		get
		{
			return mEmulation;
		}
		set
		{
			mEmulation = value;
		}
	}

	public URLRequest()
		: this(string.Empty, null, null, null)
	{
	}

	public URLRequest(string url)
		: this(url, null, null, null)
	{
	}

	public URLRequest(string url, Action begin, Action<UnityWebRequest> success, Action<UnityWebRequest, bool, bool> failure)
	{
		mEmulation = URLRequestManager.Instance.Emulation;
		mURL = url;
		mParamList = new List<URLRequestParam>();
		mDelegateRequest = begin;
		mDelegateSuccess = success;
		mDelegateFailure = failure;
		mCompleted = false;
		mNotReachability = false;
		mElapsedTime = 0f;
	}

	public void AddParam(string propertyName, string value)
	{
		URLRequestParam item = new URLRequestParam(propertyName, value);
		mParamList.Add(item);
	}

	public void Add1stParam(string propertyName, string value)
	{
		URLRequestParam item = new URLRequestParam(propertyName, value);
		mParamList.Insert(0, item);
	}

	public WWWForm CreateWWWForm()
	{
		if (mParamList.Count == 0)
		{
			return null;
		}
		WWWForm wWWForm = new WWWForm();
		int count = mParamList.Count;
		for (int i = 0; i < count; i++)
		{
			URLRequestParam uRLRequestParam = mParamList[i];
			if (uRLRequestParam != null)
			{
				wWWForm.AddField(uRLRequestParam.propertyName, uRLRequestParam.value);
			}
		}
		return wWWForm;
	}

	public void DidReceiveSuccess(UnityWebRequest www)
	{
		if (mDelegateSuccess != null)
		{
			mDelegateSuccess(www);
		}
	}

	public void DidReceiveFailure(UnityWebRequest www)
	{
		if (mDelegateFailure != null)
		{
			mDelegateFailure(www, IsTimeOut(), IsNotReachability());
		}
	}

	public void PreBegin()
	{
		if (mDelegateRequest != null)
		{
			mDelegateRequest();
		}
	}

	public void Begin()
	{
		PreBegin();
		mElapsedTime = 0f;

		if (mParamList.Count == 0)
		{
			mWebRequest = UnityWebRequest.Get(mURL);
			Debug.Log($"URLRequestManager.ExecuteRequest:{UriDecode(mURL)}", DebugTraceManager.TraceType.SERVER);
			mFormString = null;
		}
		else
		{
			WWWForm form = CreateWWWForm();
			mWebRequest = UnityWebRequest.Post(mURL, form);
			mFormString = Encoding.ASCII.GetString(form.data);
			Debug.Log($"URLRequestManager.ExecuteRequest:{UriDecode(mURL)}  params:{UriDecode(mFormString)}", DebugTraceManager.TraceType.SERVER);
		}

		mWebRequest.SendWebRequest();
	}

	public float UpdateElapsedTime(float addElapsedTime)
	{
		mElapsedTime += addElapsedTime;
		return 0.1f;
	}

	public bool IsDone()
	{
		return mWebRequest.isDone;
	}

	public bool IsTimeOut()
	{
		if (NetUtil.ConnectTimeOut <= mElapsedTime)
		{
			return true;
		}
		return false;
	}

	public bool IsNotReachability()
	{
		return mNotReachability;
	}

	public void Result()
	{
		if (IsTimeOut())
		{
			Debug.Log($"Request : TimeOut : {mURL}", DebugTraceManager.TraceType.SERVER);
			DidReceiveFailure(null);
			mWebRequest.Dispose();
			mWebRequest = null;
			return;
		}

		if (!mWebRequest.isDone)
		{
			Debug.Log("WebRequest doesn't begin yet.", DebugTraceManager.TraceType.SERVER);
		}

		if (string.IsNullOrEmpty(mWebRequest.error))
		{
			DidReceiveSuccess(mWebRequest);
		}
		else
		{
			DidReceiveFailure(mWebRequest);
		}
	}

	private static string UriDecode(string stringToUnescape)
	{
		return Uri.UnescapeDataString(stringToUnescape.Replace("+", "%20"));
	}
}
