using App;
using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenu : UIDebugMenuTask
{
	private enum MenuType
	{
		GAME,
		TITLE,
		AHIEVEMENT,
		EVENT,
		SERVER,
		ACTIVE_DEBUG_SERVER,
		MIGRATION,
		USER_MOVE,
		DISCORD,
		NOTIFICATION,
		CAMPAIGN,
		SERVER1,
		SERVER2,
		SERVER3,
		SERVER4,
		NUM
	}

	private List<string> MenuObjName = new List<string>
	{
		"Game",
		"Title",
		"Achievement",
		"Event",
		"Server",
		"for Dev./ActiveDebugServer",
		"for Dev./migration",
		"for Dev./user_move",
		"Discord",
		"Notification",
		"Campaign",
#if DEBUG || UNITY_EDITOR
		"LOCAL2",
		"FP137TEST1",
		"FP137TEST2",
		"LEADERBOARDTEST"
#else
		"LOCAL1",
		"LOCAL2",
		"LOCAL3",
		"LOCAL4"
#endif
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(200f, 200f, 150f, 50f),
		new Rect(400f, 200f, 150f, 50f),
		new Rect(600f, 200f, 150f, 50f),
		new Rect(200f, 260f, 150f, 50f),
		new Rect(200f, 380f, 150f, 50f),
		new Rect(200f, 500f, 175f, 50f),
		new Rect(600f, 330f, 150f, 50f),
		new Rect(600f, 390f, 150f, 50f),
		new Rect(600f, 470f, 150f, 50f),
		new Rect(800f, 200f, 150f, 50f),
		new Rect(400f, 260f, 150f, 50f),
		new Rect(200f, 570f, 150f, 50f),
		new Rect(400f, 570f, 150f, 50f),
		new Rect(600f, 570f, 150f, 50f),
		new Rect(800f, 570f, 150f, 50f)
	};

	private UIDebugMenuButtonList m_buttonList;

	private UIDebugMenuTextField m_LoginIDField;

	private NetDebugLogin m_debugLogin;

	private UIDebugMenuTextField m_debugServerUrlField;

	private string m_clickedButtonName;

	protected override void OnStartFromTask()
	{
		m_buttonList = base.gameObject.AddComponent<UIDebugMenuButtonList>();
		for (int i = 0; i < 15; i++)
		{
			string name = MenuObjName[i];
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, name);
			if (!(gameObject == null))
			{
				string childName = MenuObjName[i];
				m_buttonList.Add(RectList, MenuObjName, base.gameObject);
				AddChild(childName, gameObject);
			}
		}
		m_LoginIDField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		string empty = string.Empty;
		string gameID = SystemSaveManager.GetGameID();
		empty = gameID;
		m_LoginIDField.Setup(new Rect(200f, 350f, 375f, 30f), "Enter Login ID.", empty);
		GameObject gameObject2 = GameObject.Find("DebugGameObject");
		if (gameObject2 != null)
		{
			gameObject2.AddComponent<LoadURLComponent>();
		}
		m_debugServerUrlField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		m_debugServerUrlField.Setup(new Rect(200f, 470f, 375f, 30f), "forDev: Debug Server URL (i.e. http://157.109.83.27/sdl/)");
		StartCoroutine(InitCoroutine());
		TransitionFrom();
	}

	protected override void OnTransitionTo()
	{
		if (m_buttonList != null)
		{
			m_buttonList.SetActive(false);
		}
		if (m_LoginIDField != null)
		{
			m_LoginIDField.SetActive(false);
		}
		if (m_debugServerUrlField != null)
		{
			m_debugServerUrlField.SetActive(false);
		}
	}

	protected override void OnTransitionFrom()
	{
		if (m_buttonList != null)
		{
			m_buttonList.SetActive(true);
		}
		if (m_LoginIDField != null)
		{
			m_LoginIDField.SetActive(true);
		}
		if (m_debugServerUrlField != null)
		{
			m_debugServerUrlField.SetActive(true);
		}
	}

	private void OnClicked(string name)
	{
		m_clickedButtonName = name;
		bool flag = true;
		if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER1] ||
			m_clickedButtonName == MenuObjName[(int)MenuType.SERVER2] ||
			m_clickedButtonName == MenuObjName[(int)MenuType.SERVER3] ||
			m_clickedButtonName == MenuObjName[(int)MenuType.SERVER4] ||
			m_clickedButtonName == MenuObjName[(int)MenuType.ACTIVE_DEBUG_SERVER] ||
			m_clickedButtonName == MenuObjName[(int)MenuType.TITLE] ||
			m_clickedButtonName == MenuObjName[(int)MenuType.DISCORD] ||
			m_clickedButtonName == MenuObjName[(int)MenuType.USER_MOVE])
		{
			flag = false;
		}
		if (flag)
		{
			ValidateSessionCallback(true);
		}
		else
		{
			ValidateSessionCallback(true);
		}
	}

	private void ValidateSessionCallback(bool isSuccess)
	{
#if DEBUG || UNITY_EDITOR
		if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER1])
		{
			Env.actionServerType = Env.ActionServerType.LOCAL2;
		}
		else if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER2])
		{
			Env.actionServerType = Env.ActionServerType.FP137TEST1;
		}
		else if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER3])
		{
			Env.actionServerType = Env.ActionServerType.FP137TEST2;
		}
		else if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER4])
		{
			Env.actionServerType = Env.ActionServerType.LEADERBOARDTEST;
		}
#else
		if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER1])
		{
			Env.actionServerType = Env.ActionServerType.LOCAL1;
		}
		else if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER2])
		{
			Env.actionServerType = Env.ActionServerType.LOCAL2;
		}
		else if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER3])
		{
			Env.actionServerType = Env.ActionServerType.LOCAL3;
		}
		else if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER4])
		{
			Env.actionServerType = Env.ActionServerType.LOCAL4;
		}
#endif
		if (m_clickedButtonName == MenuObjName[(int)MenuType.SERVER1] || m_clickedButtonName == MenuObjName[(int)MenuType.SERVER2] || m_clickedButtonName == MenuObjName[(int)MenuType.SERVER3] || m_clickedButtonName == MenuObjName[(int)MenuType.SERVER4])
		{
			NetBaseUtil.DebugServerUrl = null;
			string actionServerURL = NetBaseUtil.ActionServerURL;
		}
		else if (m_clickedButtonName == MenuObjName[(int)MenuType.ACTIVE_DEBUG_SERVER])
		{
			string actionServerURL = NetBaseUtil.DebugServerUrl = m_debugServerUrlField.text;
			DebugSaveServerUrl.SaveURL(actionServerURL);
		}
		else if (m_clickedButtonName == MenuObjName[(int)MenuType.TITLE])
		{
			Application.LoadLevel(TitleDefine.TitleSceneName);
		}
		else
		{
			TransitionToChild(m_clickedButtonName);
		}
	}

	protected override void OnGuiFromTask()
	{
		GUI.Label(new Rect(400f, 510f, 300f, 60f), "Current URL\n" + NetBaseUtil.ActionServerURL);

		string netState = "Unknown";
		switch(Application.internetReachability) {
			case NetworkReachability.NotReachable:
				netState = "Disconnected";
				break;
			case NetworkReachability.ReachableViaLocalAreaNetwork:
				netState = "Connected to Wi-Fi";
				break;
			case NetworkReachability.ReachableViaCarrierDataNetwork:
				netState = "Connected to mobile";
				break;
		}

		GUI.Label(new Rect(400f, 390f, 300f, 60f), "Network state\n"+netState);
	}

	private IEnumerator InitCoroutine()
	{
		yield return null;
		yield return null;
		if (DebugSaveServerUrl.Url != null)
		{
			m_debugServerUrlField.text = DebugSaveServerUrl.Url;
		}
		else
		{
			m_debugServerUrlField.text = NetBaseUtil.ActionServerURL;
		}
	}
}
