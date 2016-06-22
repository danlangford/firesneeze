using System;
using UnityEngine;

public class GuiPanelDeath : GuiPanel
{
    [Tooltip("reference to the message label in our hierarchy")]
    public GuiLabel Message;
    [Tooltip("reference to the portrait in our hierarchy")]
    public GuiImage Portrait;

    private void OnDeathContinueButtonPushed()
    {
        this.Show(false);
        Turn.Proceed();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.Message.Text = Turn.Character.DisplayName + " has fallen.";
            this.Portrait.Image = Turn.Character.PortraitAvatar;
            UI.Window.Pause(true);
        }
        else
        {
            this.Message.Clear();
            this.Portrait.Clear();
            UI.Window.Pause(false);
        }
    }
}

