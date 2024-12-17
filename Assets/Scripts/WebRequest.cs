using System;
using UnityEngine;
using UnityEngine.Networking;

internal class WebRequest
{
    private UnityWebRequest m_request;
    private bool m_checkTime;
    private float m_startTime;
    private bool m_isEnd;
    private bool m_isTimeOut;
    public static readonly float DefaultConnectTime = 60f;
    private float m_connectTime = DefaultConnectTime;

    public WebRequest(string url, bool checkTime = false)
    {
        m_request = UnityWebRequest.Get(url);
        m_request.SendWebRequest();
        m_startTime = Time.realtimeSinceStartup;
        m_checkTime = checkTime;
    }

    public void SetConnectTime(float connectTime)
    {
        if (connectTime > 180f)
        {
            connectTime = 180f;
        }
        m_connectTime = connectTime;
    }

    public void Remove()
    {
        try
        {
            if (m_request != null)
            {
                m_request.Dispose();
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"WebRequest.Remove():Exception->Message = {ex.Message},ToString() = {ex}");
        }
        m_request = null;
    }

    public void Cancel()
    {
        if (m_request != null)
        {
            m_request.Abort();
        }
        Remove();
    }

    public void Update()
    {
        if (!m_isTimeOut)
        {
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            float num = realtimeSinceStartup - m_startTime;
            if (num >= m_connectTime)
            {
                m_isTimeOut = true;
            }
        }
        if (m_isEnd)
        {
            return;
        }
        if (m_request != null)
        {
            if (m_request.isDone)
            {
                m_isEnd = true;
            }
        }
        else
        {
            m_isEnd = true;
        }
        if (m_checkTime)
        {
            float realtimeSinceStartup2 = Time.realtimeSinceStartup;
            Debug.Log($"LS:Load File: {m_request.url} Time is {realtimeSinceStartup2 - m_startTime}");
        }
    }

    public bool IsEnd()
    {
        return m_isEnd;
    }

    public bool IsTimeOut()
    {
        return m_isTimeOut;
    }

    public string GetError()
    {
        if (m_request != null)
        {
            return m_request.error;
        }
        return null;
    }

    public byte[] GetResult()
    {
        if (m_request != null)
        {
            return m_request.downloadHandler.data;
        }
        return null;
    }

    public string GetResultString()
    {
        if (m_request != null)
        {
            return m_request.downloadHandler.text;
        }
        return null;
    }

    public int GetResultSize()
    {
        if (m_request != null && m_request.downloadHandler != null)
        {
            return m_request.downloadHandler.data.Length;
        }
        return 0;
    }
} 