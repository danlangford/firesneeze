using System;

public class GameStatePreEnd : GameState
{
    public override void Enter()
    {
        Event.Done();
        Turn.EmptyLayoutDecks = true;
        base.Enter();
        if (Location.Current.StopsVillainCorner)
        {
            if (Location.Current.Keepers == null)
            {
                for (int i = Location.Current.Deck.Count - 1; i >= 1; i--)
                {
                    Campaign.Box.Add(Location.Current.Deck[i], false);
                }
            }
            Turn.PushStateDestination(GameStateType.End);
            if (!Location.Current.Closed)
            {
                Location.Current.Closed = true;
            }
            if (Rules.IsAnyActionPossible())
            {
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    base.Message(StringTableManager.GetUIText(0x113));
                    window.ShowProceedButton(true);
                    window.ShowCancelButton(false);
                }
            }
            else if (!Game.Events.ContainsStatefulEvent() && !Game.Events.IsEventRunning)
            {
                Turn.State = GameStateType.Post;
            }
        }
        else
        {
            this.Proceed();
        }
    }

    public override void Proceed()
    {
        Turn.State = GameStateType.End;
    }

    public override GameStateType Type =>
        GameStateType.PreEnd;
}

