/****************************************************************************
 *
 * Copyright (c) 2011 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
using System;


public class CriAtomServer : MonoBehaviour {
	
	#region Internal Fields
	private static CriAtomServer _instance = null;
#if UNITY_EDITOR
	private bool isApplicationPaused = false;
	private bool isEditorPaused = false;
#endif
	#endregion
	
	public System.Action<bool> onApplicationPausePreProcess;
	public System.Action<bool> onApplicationPausePostProcess;
	
	public static CriAtomServer instance {
		get {
			CreateInstance();
			return _instance;
		}
	}
	
	public static void CreateInstance() {
		if (_instance == null) {
			CriWare.managerObject.AddComponent<CriAtomServer>();
		}
	}
	
	public static void DestroyInstance() {
		if (_instance != null) {
			UnityEngine.GameObject.Destroy(_instance);
		}
	}
	
	void Awake()
	{
		/* インスタンスは常に１つしか生成されないことを保証する */
		if (_instance == null) {
			_instance = this;
		} else {
			GameObject.Destroy(this);
		}
	}

	void OnEnable()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.playmodeStateChanged += OnPlaymodeStateChange;
#endif
	}

	void OnDisable()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.playmodeStateChanged -= OnPlaymodeStateChange;
#endif

		if (_instance == this) {
			_instance = null;
		}
	}

#if UNITY_EDITOR
	private void OnPlaymodeStateChange()
	{
		bool paused = UnityEditor.EditorApplication.isPaused;
		if (!isApplicationPaused && isEditorPaused != paused) {
			ProcessApplicationPause(paused);
			isEditorPaused = paused;
		}
	}
#endif

	void OnApplicationPause(bool appPause)
	{
#if UNITY_EDITOR
		if (!isEditorPaused && isApplicationPaused != appPause) {
			ProcessApplicationPause(appPause);
			isApplicationPaused = appPause;
		}
#else
		ProcessApplicationPause(appPause);
#endif
	}

	void ProcessApplicationPause(bool appPause)
	{
		if (onApplicationPausePreProcess != null) {
			onApplicationPausePreProcess(appPause);
		}
		CriAtomPlugin.Pause(appPause);
		if (onApplicationPausePostProcess != null) {
			onApplicationPausePostProcess(appPause);
		}
	}
}
