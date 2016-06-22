using System;
using UnityEngine;

public class GuiPanelPopup : GuiPanel
{
    [Tooltip("reference to a text label contained in our hierarchy")]
    public GuiLabel MessageTextLabel;

    public override void Initialize()
    {
        this.MessageTextLabel.GetComponent<Renderer>().sortingLayerID = 2;
        this.MessageTextLabel.GetComponent<Renderer>().sortingOrder = 1;
    }

    private void OnMessageContinueButtonPushed()
    {
        UI.Modal = false;
        this.MessageTextLabel.Clear();
        this.Show(false);
    }

    public void Show(string text)
    {
        base.Show(true);
        this.MessageTextLabel.Text = text;
        UI.Modal = true;
    }
}

