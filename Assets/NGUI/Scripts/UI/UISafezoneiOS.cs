using System.Diagnostics;
using UnityEngine;
using System.Collections;
using System;

public class UISafezoneiOS : MonoBehaviour {


	private GameObject m_uiroot;
	private GameObject m_pauseButton;
	private GameObject m_statusBar;
	private UIAnchor m_progressBar;
	private UIAnchor m_pauseoffset;

	void Start () {
	#if UNITY_IOS
	m_uiroot = GameObject.Find("UI Root (2D)");
		if (!(m_uiroot != null))
		{
			return;
		}

	m_pauseButton = GameObjectUtil.FindChildGameObject(m_uiroot, "Anchor_3_TR");
	m_statusBar = GameObjectUtil.FindChildGameObject(m_uiroot, "Anchor_8_BC");
		if((m_pauseButton != null))
		{
			m_pauseoffset = GameObjectUtil.FindGameObjectComponent<UIAnchor>("Anchor_3_TR");
			Debug.Log("hi " + m_pauseoffset);
			m_pauseoffset.relativeOffset.x = (Convert.ToSingle(-0.05));
		}
		if((m_statusBar != null))
		{
			m_progressBar = GameObjectUtil.FindGameObjectComponent<UIAnchor>("Anchor_8_BC");
			Debug.Log("hi " + m_progressBar);
			m_progressBar.relativeOffset.y = (Convert.ToSingle(+0.03));
			return;
		}
	#endif
	}
	
}