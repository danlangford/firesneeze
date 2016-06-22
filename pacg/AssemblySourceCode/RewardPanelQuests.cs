using System;

public class RewardPanelQuests : GuiPanel
{
    private void OnCloseButtonPushed()
    {
        this.Show(false);
        UI.Window.SendMessage("RewardSequenceController");
    }

    public override bool Fullscreen =>
        true;
}

