using System;
using UnityEngine;

public class GuiPanelOutOfDate : GuiPanelBackStack
{
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CornerCloseButton;
    [Tooltip("reference to the description label on this panel")]
    public GuiLabel DescriptionLabel;
    [Tooltip("reference to the headline label on this panel")]
    public GuiLabel HeadlineLabel;
    [Tooltip("reference to the update now button on this panel")]
    public GuiButton UpdateNowButton;

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    private void OnUpdateNowButtonPushed()
    {
        if (!UI.Busy)
        {
            Application.OpenURL("http://play.google.com/store/apps/details?id=net.obsidian.pacg1");
        }
    }

    public override void Rebind()
    {
        this.CornerCloseButton.Rebind();
        this.CloseButton.Rebind();
        this.UpdateNowButton.Rebind();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.Refresh();
        }
        else
        {
            UI.Busy = false;
        }
    }

    public override bool Fullscreen =>
        true;
}

