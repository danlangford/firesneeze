using System;

public class GameStateAskClose : GameState
{
    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.Card.Show(false);
            window.closeLocationPanel.Show(CloseType.Temporary);
            window.Pause(true);
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.closeLocationPanel.Show(false);
            window.Pause(false);
        }
        base.Message((string) null);
    }

    public override void Proceed()
    {
        Turn.Iterators.Next(TurnStateIteratorType.Close);
    }

    public override GameStateType Type =>
        GameStateType.AskClose;
}

