using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AssetBundleLoadSceneTest : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(WaitLoad("http://web2/HikiData/Sonic_Runners/Soft/Asset/AssetBundles_Win/ResourcesCommonPrefabs.unity3d", 5, "ResourcesCommonPrefabs"));
		StartCoroutine(WaitLoad("http://web2/HikiData/Sonic_Runners/Soft/Asset/AssetBundles_Win/ResourcesCommonObject.unity3d", 5, "ResourcesCommonObject"));
	}

	private void Update()
	{
	}

	private IEnumerator WaitLoad(string url, int version, string sceneName)
	{
		UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint)version);
		yield return www.SendWebRequest();

		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogError(www.error);
			yield break;
		}

		AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
		if (bundle != null)
		{
			SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		}
		
		www.Dispose();
	}
}
