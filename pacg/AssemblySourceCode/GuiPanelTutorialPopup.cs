using System;
using UnityEngine;

public class GuiPanelTutorialPopup : GuiPanel
{
    [Tooltip("reference to the close button in this panel (can be null)")]
    public GuiButton CloseButton;
    [Tooltip("reference to the message label in this panel (can be null)")]
    public GuiLabel MessageLabel;

    public virtual void Display(int id)
    {
        this.Show(true);
        if (this.MessageLabel != null)
        {
            this.MessageLabel.Text = StringTableManager.Get("Tutorial", id);
        }
    }

    public virtual void Display(string msg)
    {
        this.Show(true);
        if (this.MessageLabel != null)
        {
            this.MessageLabel.Text = msg;
        }
    }

    protected virtual void OnCloseButtonClicked()
    {
        this.Show(false);
        Tutorial.Notify(TutorialEventType.TutorialMessageClosed);
    }

    public override void Rebind()
    {
        if (this.CloseButton != null)
        {
            this.CloseButton.Rebind();
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible && (this.CloseButton != null))
        {
            this.CloseButton.Rebind();
        }
        if (!isVisible && (this.MessageLabel != null))
        {
            this.MessageLabel.Text = null;
        }
    }
}

