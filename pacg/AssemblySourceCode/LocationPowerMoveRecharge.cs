using System;

public class LocationPowerMoveRecharge : LocationPower
{
    public override void Activate()
    {
        base.PowerBegin();
        Turn.MarkPowerActive(this, true);
        TurnStateData data = new TurnStateData(ActionType.Recharge, 1) {
            MaxNumCards = Turn.Character.Hand.Count
        };
        Turn.SetStateData(data);
        Turn.BlackBoard.Set<int>("LocationPowerMoveRecharge_HandCount", Turn.Character.Hand.Count);
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerMoveRecharge_Shuffle"));
        Turn.State = GameStateType.Penalty;
    }

    public override bool IsValid()
    {
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        return (Turn.State == GameStateType.StartTurn);
    }

    private void LocationPowerMoveRecharge_Shuffle()
    {
        this.PowerEnd();
        int num = Turn.BlackBoard.Get<int>("LocationPowerMoveRecharge_HandCount") - Turn.Character.Hand.Count;
        if (num != 0)
        {
            VisualEffect.Shuffle(DeckType.Character);
            Turn.Character.Deck.Shuffle();
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            for (int i = 0; i < (num - 1); i++)
            {
                window.Draw(Turn.Character.Deck[0]);
            }
        }
        Turn.State = GameStateType.StartTurn;
    }
}

