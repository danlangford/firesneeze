using System;

public class GameStateSelectType : GameState
{
    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        base.Message(0x53);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
            TurnStateData stateData = Turn.GetStateData();
            if ((stateData == null) || (stateData.Filter.CardTypes.Length == 0))
            {
                window.chooseTypePanel.Show(true);
            }
            else
            {
                window.chooseTypePanel.Show(stateData.Filter);
            }
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.chooseTypePanel.Show(false);
        }
        base.Message((string) null);
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        false;

    public override void Proceed()
    {
        Turn.GotoStateDestination();
    }

    public override void Refresh()
    {
    }

    public override GameStateType Type =>
        GameStateType.SelectType;
}

