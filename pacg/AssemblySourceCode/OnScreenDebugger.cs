using System;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenDebugger : MonoBehaviour
{
    private GUIStyle currentStyle;
    private HashSet<string> m_exceptionCallstack = new HashSet<string>();
    private float m_lastExceptionCooldown = -1f;
    private float m_timer;
    private List<string> messages;
    private const float ON_SCREEN_TIME = 15f;

    private void Awake()
    {
        Application.logMessageReceived += new Application.LogCallback(this.OnUnityConsoleMessage);
        this.messages = new List<string>();
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    private void InitStyles(bool force)
    {
        if ((this.currentStyle == null) || force)
        {
            this.currentStyle = new GUIStyle(GUI.skin.box);
            this.currentStyle.normal.background = this.MakeTex(2, 2, new Color(0f, 0f, 0f, 0.5f));
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = col;
        }
        Texture2D textured = new Texture2D(width, height);
        textured.SetPixels(colors);
        textured.Apply();
        return textured;
    }

    private void OnGUI()
    {
        if (this.m_timer > 0f)
        {
            this.InitStyles(false);
            Vector2 position = new Vector2((float) (Screen.width / 8), (float) (Screen.height / 8));
            Vector2 size = new Vector2((float) ((6 * Screen.width) / 8), (float) ((6 * Screen.height) / 8));
            Rect rect = new Rect(position, size);
            GUI.Box(rect, "Error Logging - Recommend Restarting the Game - Disappearing in " + this.m_timer + " seconds", this.currentStyle);
            rect.x += 5f;
            rect.y += 15f;
            for (int i = this.messages.Count - 1; i >= 0; i--)
            {
                GUI.Label(rect, this.messages[i]);
                rect.y += 15f;
            }
        }
    }

    private void OnUnityConsoleMessage(string error, string stackTrace, LogType type)
    {
        if (((type == LogType.Exception) || (type == LogType.Error)) && !this.m_exceptionCallstack.Contains(stackTrace))
        {
            this.messages.Add(error + "\n" + stackTrace);
            this.m_exceptionCallstack.Add(stackTrace);
            this.m_lastExceptionCooldown = 10f;
            this.m_timer = 15f;
        }
    }

    private void Update()
    {
        if (this.m_lastExceptionCooldown > 0f)
        {
            this.m_lastExceptionCooldown -= Time.deltaTime;
            if (this.m_lastExceptionCooldown < 0f)
            {
                this.m_exceptionCallstack.Clear();
            }
        }
        if (this.m_timer > 0f)
        {
            this.m_timer -= Time.deltaTime;
        }
        else
        {
            this.messages = new List<string>();
        }
    }
}

