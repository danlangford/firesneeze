using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CutsceneAlert : MonoBehaviour
{
    [Tooltip("the text to display")]
    public StrRefType Message;

    public void Play()
    {
        GuiWindowCutscene window = UI.Window as GuiWindowCutscene;
        if (window != null)
        {
            window.panelAlert.Show(this);
        }
    }

    public bool Complete { get; set; }

    public string Text =>
        this.Message.ToString();
}

