using System;
using UnityEngine;

public class GuiPanelPowersInfo : GuiPanel
{
    [Tooltip("reference to the panel's label in this scene")]
    public GuiLabel DescriptionLabel;
    private bool isPanelVisible;

    private void OnCloseButtonPushed()
    {
        this.PlayAnimation("Close");
        this.isPanelVisible = false;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.powersPanel.vfxSelect.SetActive(false);
        }
    }

    public override void Show(bool isVisible)
    {
        if (!this.Visible)
        {
            base.Show(isVisible);
        }
        if (isVisible && !this.isPanelVisible)
        {
            this.PlayAnimation("Open");
            this.isPanelVisible = true;
        }
        if (!isVisible && this.isPanelVisible)
        {
            this.PlayAnimation("Close");
            this.isPanelVisible = false;
        }
    }

    public string Text
    {
        set
        {
            this.DescriptionLabel.Text = value;
        }
    }
}

