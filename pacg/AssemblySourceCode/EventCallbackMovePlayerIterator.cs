using System;

public class EventCallbackMovePlayerIterator : EventCallbackMovePlayer
{
    private void AdvanceIterator()
    {
        if (Turn.State != GameStateType.Penalty)
        {
            if (Turn.IsIteratorInProgress())
            {
                Turn.Iterators.Next();
            }
            else
            {
                Turn.State = GameStateType.Combat;
            }
        }
    }

    public override void OnCallback()
    {
        if (this.IsEventValid(Turn.Card))
        {
            Turn.PushCancelDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Iterator_Next"));
            Turn.PushReturnState(GameStateType.Done);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Turn.Target = Turn.Current;
                Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "MapMove_FinishMove"));
                window.mapPanel.Center(base.LocationSelector.Random());
                Turn.State = GameStateType.Move;
            }
        }
        else
        {
            this.AdvanceIterator();
        }
    }
}

