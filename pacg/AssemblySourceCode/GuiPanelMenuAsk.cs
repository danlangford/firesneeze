using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelMenuAsk : GuiPanel
{
    [Tooltip("reference to the title message label in our hierarchy")]
    public GuiLabel MessageLabel;
    [Tooltip("reference to the no button in our hierarchy")]
    public GuiButton NoButton;
    [Tooltip("reference to the yes button in our hierarchy")]
    public GuiButton YesButton;

    private void Close()
    {
        this.Show(false);
        base.Owner = null;
        this.MessageText = null;
        this.YesButtonText = null;
        this.YesButtonCallback = null;
        this.NoButtonText = null;
        this.NoButtonCallback = null;
    }

    private void OnNoButtonPushed()
    {
        if (this.NoButtonCallback != null)
        {
            if (base.Owner != null)
            {
                base.Owner.SendMessage(this.NoButtonCallback, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                UI.Window.SendMessage(this.NoButtonCallback, SendMessageOptions.DontRequireReceiver);
            }
        }
        this.Close();
    }

    private void OnYesButtonPushed()
    {
        if (this.YesButtonCallback != null)
        {
            if (base.Owner != null)
            {
                base.Owner.SendMessage(this.YesButtonCallback, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                UI.Window.SendMessage(this.YesButtonCallback, SendMessageOptions.DontRequireReceiver);
            }
        }
        this.Close();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (base.Owner != null)
        {
            base.Owner.Pause(isVisible);
        }
        else
        {
            UI.Window.Pause(isVisible);
        }
        UI.Busy = isVisible;
    }

    public override bool Fullscreen =>
        true;

    public string MessageText
    {
        get => 
            this.MessageLabel.Text;
        set
        {
            this.MessageLabel.Text = value;
        }
    }

    public string NoButtonCallback { get; set; }

    public string NoButtonText
    {
        get => 
            this.NoButton.Text;
        set
        {
            this.NoButton.Text = value;
        }
    }

    public string YesButtonCallback { get; set; }

    public string YesButtonText
    {
        get => 
            this.YesButton.Text;
        set
        {
            this.YesButton.Text = value;
        }
    }
}

