using System;

public class GameStatePopup : GameState
{
    public override void Cancel()
    {
    }

    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(false);
            window.ShowProceedButton(false);
            window.Popup.Show(true);
            window.dicePanel.Show(false);
            window.dicePanel.ShowCheck(false);
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Show(false);
        }
        base.Message((string) null);
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        false;

    public override void Proceed()
    {
    }

    public override void Refresh()
    {
    }

    public override GameStateType Type =>
        GameStateType.Popup;
}

