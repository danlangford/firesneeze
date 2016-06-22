using System;

public class GameStateClosePrompt : GameState
{
    public override void Enter()
    {
        base.Enter();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.closeLocationPanel.Show(CloseType.Permanent);
        }
    }

    public override void Exit(GameStateType nextState)
    {
        base.Exit(nextState);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && (nextState != GameStateType.ClosePrompt))
        {
            window.locationPanel.GlowLocationClosePossible(true, false);
        }
        base.Message((string) null);
    }

    public override void Proceed()
    {
        Turn.ReturnToReturnState();
        Turn.CloseType = CloseType.None;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.closeLocationPanel.Show(CloseType.None);
        }
    }

    public override GameStateType Type =>
        GameStateType.ClosePrompt;
}

