using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CutsceneIntertitle : MonoBehaviour
{
    [Tooltip("delay before the intertitle")]
    public float Delay = 0.3f;
    [Tooltip("duration, in seconds")]
    public float Length = 2f;
    [Tooltip("the text to display")]
    public StrRefType Message;
    [Tooltip("prologue or epilogue?")]
    public CutsceneIntertitleType Type;

    public void Play()
    {
        GuiWindowCutscene window = UI.Window as GuiWindowCutscene;
        if (window != null)
        {
            window.panelIntertitle.Show(this);
        }
    }

    public bool Complete { get; set; }

    public float Duration =>
        this.Length;

    public string Text =>
        this.Message.ToString();
}

