using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleTestNoCache : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(WaitLoad());
	}

	private IEnumerator WaitLoad()
	{
		UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(
			"http://web2/HikiData/Sonic_Runners/Soft/Asset/AssetBundles_Win/PrephabKnuckles.unity3d");
			
		yield return www.SendWebRequest();

		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogError(www.error);
			yield break;
		}

		AssetBundle myLoadedAssetBundle = DownloadHandlerAssetBundle.GetContent(www);
		Object asset = myLoadedAssetBundle.mainAsset;
		Object.Instantiate(asset);
		myLoadedAssetBundle.Unload(false);
		www.Dispose();
	}
}
