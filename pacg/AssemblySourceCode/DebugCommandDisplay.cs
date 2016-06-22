using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugCommandDisplay : MonoBehaviour
{
    private float aSliderValue = 0.5f;
    private float aSliderValue_old;
    private const float BOX_HEIGHT = 100f;
    private const float BOX_WIDTH = 500f;
    private List<TurnStateCallback> cancels;
    private GameStateType currentState;
    private GUIStyle currentStyle;
    private List<TurnStateCallback> destinations;
    private const float LINE_HEIGHT = 25f;
    private float m_stateDisplayChangeTimer = 3f;
    private List<GameStateType> returns;
    private const float STATE_DISPLAY_CHANGE_TIME = 3f;
    private List<GameStateType> states = new List<GameStateType>();

    protected string Accent(string s) => 
        ("<color=yellow>" + s + "</color>");

    private void AppendHeader(StringBuilder sb, string header)
    {
        sb.AppendLine("----------Start " + header + "----------");
    }

    private void AppendStack(StringBuilder sb, List<GameStateType> list, string start, string format)
    {
        if (list.Count > 0)
        {
            this.AppendHeader(sb, start);
        }
        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(string.Format(format, i.ToString(), list[i].ToString()));
        }
    }

    private void AppendStack(StringBuilder sb, List<TurnStateCallback> list, string start, string format)
    {
        if (list.Count > 0)
        {
            this.AppendHeader(sb, start);
        }
        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(string.Format(format, i.ToString(), list[i]));
        }
    }

    private void BuildButtons()
    {
        Vector2 position = new Vector2(Screen.width - 600f, (float) (Screen.height / 4));
        position.x += 5f;
        position.y += 35f;
        float x = 60f;
        float y = 15f;
        if (GUI.Button(new Rect(position, new Vector2(x, y)), "Peon"))
        {
            if (Settings.Debug.PeonMode)
            {
                Settings.Debug.PeonMode = false;
            }
            else
            {
                Settings.Debug.PeonMode = true;
                Settings.Debug.GodMode = false;
            }
        }
        position.y += 15f;
        if (GUI.Button(new Rect(position, new Vector2(x, y)), "God"))
        {
            if (Settings.Debug.GodMode)
            {
                Settings.Debug.GodMode = false;
            }
            else
            {
                Settings.Debug.GodMode = true;
                Settings.Debug.PeonMode = false;
            }
        }
        position.y += 15f;
        if (GUI.Button(new Rect(position, new Vector2(x, y)), "Normal"))
        {
            Settings.Debug.GodMode = false;
            Settings.Debug.PeonMode = false;
        }
    }

    protected string GreenOnRedOff(string s)
    {
        if (s.Length == 2)
        {
            return ("<color=green>" + s + "</color>");
        }
        return ("<color=red>" + s + "</color>");
    }

    private void InitStyles(bool force)
    {
        if ((this.currentStyle == null) || force)
        {
            this.currentStyle = new GUIStyle(GUI.skin.box);
            this.currentStyle.normal.background = this.MakeTex(2, 2, new Color(0f, 0f, 0f, this.aSliderValue));
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
        this.UpdateCurrentState();
        this.InitStyles(false);
        this.UpdateSliderValues();
        this.BuildButtons();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("State: " + this.Accent(this.currentState.ToString()) + (!UI.Busy ? string.Empty : " [Busy]"));
        sb.AppendLine("                 " + this.GreenOnRedOff(!Settings.Debug.PeonMode ? "Off" : "On"));
        sb.AppendLine("                 " + this.GreenOnRedOff(!Settings.Debug.GodMode ? "Off" : "On"));
        sb.AppendLine("                 " + this.GreenOnRedOff((Settings.Debug.PeonMode || Settings.Debug.GodMode) ? "Off" : "On"));
        if (Turn.DamageTraits.Count > 0)
        {
            this.AppendHeader(sb, "Turn Traits");
            for (int i = 0; i < Turn.DamageTraits.Count; i++)
            {
                sb.AppendLine(i + "Trait: " + Turn.DamageTraits[i]);
            }
        }
        if (Turn.Checks != null)
        {
            this.AppendHeader(sb, "Combat Skill");
            sb.AppendLine(Turn.CombatSkill.ToString());
        }
        this.AppendHeader(sb, "EmptyLayoutDecks");
        sb.AppendLine(Turn.EmptyLayoutDecks.ToString());
        char[] separator = new char[] { '\n' };
        int length = sb.ToString().Split(separator).Length;
        Vector2 position = new Vector2(Screen.width - 600f, (float) (Screen.height / 4));
        Vector2 size = new Vector2(500f, 25f * (length + 2));
        Rect rect = new Rect(position, size);
        GUI.Box(rect, "Debug Information", this.currentStyle);
        rect.x += 5f;
        rect.y += 15f;
        GUI.Label(rect, sb.ToString());
    }

    private void Start()
    {
    }

    private void UpdateCurrentState()
    {
        if (((this.currentState == GameStateType.None) || (this.states == null)) || (this.states.Count == 0))
        {
            this.states = new List<GameStateType>();
            this.states.Add(Turn.State);
            this.currentState = this.states[0];
        }
        if ((this.currentState != Turn.State) && (!this.states.Contains(Turn.State) || (((GameStateType) this.states[this.states.Count - 1]) != Turn.State)))
        {
            this.states.Add(Turn.State);
        }
        if (this.m_stateDisplayChangeTimer > 0f)
        {
            this.m_stateDisplayChangeTimer -= Time.deltaTime;
        }
        else if (this.states.Count > 1)
        {
            this.states.RemoveAt(0);
            this.currentState = this.states[0];
            this.m_stateDisplayChangeTimer = 3f;
        }
    }

    private void UpdateSliderValues()
    {
        Rect position = new Rect(Screen.width - 600f, (float) (Screen.height / 4), 500f, 15f);
        position.y += 100f;
        this.aSliderValue = GUI.HorizontalSlider(position, this.aSliderValue, 0f, 1f);
        if (this.aSliderValue != this.aSliderValue_old)
        {
            this.aSliderValue_old = this.aSliderValue;
            this.InitStyles(true);
        }
    }
}

