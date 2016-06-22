using System;
using UnityEngine;

public class GuiPanelExit : GuiPanelBackStack
{
    private void OnCloseButtonPushed()
    {
        this.Show(false);
    }

    private void OnNoButtonPushed()
    {
        this.Show(false);
    }

    private void OnYesButtonPushed()
    {
        this.Show(false);
        Application.Quit();
    }

    public override bool Fullscreen =>
        true;
}

