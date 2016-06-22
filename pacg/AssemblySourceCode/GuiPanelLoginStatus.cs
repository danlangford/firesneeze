using System;
using UnityEngine;

public class GuiPanelLoginStatus : GuiPanelBackStack
{
    [Tooltip("reference to the quit button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CornerCloseButton;

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    public override void Rebind()
    {
        this.CornerCloseButton.Rebind();
        this.CloseButton.Rebind();
    }

    public override void Show(bool isVisible)
    {
        if (isVisible && Game.UI.NetworkTooltip.Visible)
        {
            Game.UI.NetworkTooltip.Show(false);
        }
        base.Show(isVisible);
        if (base.Owner != null)
        {
            base.Owner.Pause(isVisible);
        }
        else
        {
            UI.Window.Pause(isVisible);
        }
        if (!isVisible)
        {
            base.Owner = null;
        }
        else
        {
            this.Refresh();
            UI.Busy = false;
        }
    }

    public override bool Fullscreen =>
        true;
}

