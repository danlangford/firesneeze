using System;
using UnityEngine;

public class GuiPanelCutsceneAlert : GuiPanel
{
    [Tooltip("reference to the text label on this panel")]
    public GuiLabel Text;

    private void OnCloseButtonPushed()
    {
        GuiWindowCutscene window = UI.Window as GuiWindowCutscene;
        if (window != null)
        {
            this.Show(false);
            UI.Window.Pause(false);
            window.Scene.Play();
        }
    }

    public void Show(CutsceneAlert alert)
    {
        UI.Window.Pause(true);
        this.Text.Text = alert.Text;
        alert.Complete = true;
        this.Show(true);
    }
}

