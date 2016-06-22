using System;
using UnityEngine;

public class GuiPanelForfeit : GuiPanelBackStack
{
    [Tooltip("reference to the button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the button on this panel")]
    public GuiButton ForfeitButton;

    private void OnCloseButtonPushed()
    {
        this.Show(false);
        UI.Window.Pause(false);
    }

    private void OnForfeitButtonPushed()
    {
        this.Show(false);
        GameStateForfeit.Initialize();
        Turn.State = GameStateType.Forfeit;
    }

    public override void Rebind()
    {
        this.CloseButton.Rebind();
        this.ForfeitButton.Rebind();
    }

    public override bool Fullscreen =>
        true;
}

