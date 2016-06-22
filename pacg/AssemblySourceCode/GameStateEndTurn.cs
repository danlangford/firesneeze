using System;

public class GameStateEndTurn : GameState
{
    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        Turn.DestructiveActionsCount++;
        if (Turn.End)
        {
            Turn.DestructiveActionsCount++;
        }
        if (Turn.Owner.IsOverHandSize())
        {
            Turn.Discard = true;
        }
        if (Rules.IsAnyActionPossible())
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                base.Message(0x47);
                window.ShowProceedEndTurnButton(true);
                window.ShowCancelButton(false);
                window.ShowMapButton(true);
                window.ShowExploreButton(false);
            }
        }
        else
        {
            this.Proceed();
        }
    }

    public override void Exit(GameStateType nextState)
    {
        base.Message((string) null);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowMapButton(false);
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    public override void Proceed()
    {
        Turn.Card.OnEndOfEndTurn();
        Location.Current.OnEndOfEndTurn();
        Scenario.Current.OnEndOfEndTurn();
        Turn.State = GameStateType.Reset;
    }

    public override GameStateType Type =>
        GameStateType.EndTurn;
}

