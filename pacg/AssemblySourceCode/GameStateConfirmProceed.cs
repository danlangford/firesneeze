using System;

public class GameStateConfirmProceed : GameStateConfirm
{
    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(true);
            window.Refresh();
        }
        this.Refresh();
    }

    public override GameStateType Type =>
        GameStateType.ConfirmProceed;
}

