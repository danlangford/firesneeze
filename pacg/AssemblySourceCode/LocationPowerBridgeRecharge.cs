using System;

public class LocationPowerBridgeRecharge : LocationPower
{
    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerBridgeRecharge_Cancel"));
        Turn.SetStateData(new TurnStateData(ActionType.Bury, 1));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerBridgeRecharge_Finish1"));
        Turn.State = GameStateType.Penalty;
        base.Activate();
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (!Location.Current.Closed)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (Turn.Character.Hand.Count < 1)
        {
            return false;
        }
        if (Turn.Character.Discard.Count < 1)
        {
            return false;
        }
        return (Turn.State == GameStateType.EndTurn);
    }

    private void LocationPowerBridgeRecharge_Cancel()
    {
        this.PowerEnd();
        Turn.State = GameStateType.EndTurn;
        base.Deactivate();
    }

    private void LocationPowerBridgeRecharge_Finish1()
    {
        Turn.SetStateData(new TurnStateData(ActionType.Discard, CardFilter.Empty, ActionType.Recharge, 1));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerBridgeRecharge_Finish2"));
        Turn.State = GameStateType.Pick;
    }

    private void LocationPowerBridgeRecharge_Finish2()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        Turn.State = GameStateType.EndTurn;
    }
}

