using System;
using UnityEngine;

public class LocationPowerBridgeEvade : LocationPower
{
    [Tooltip("if true, the location deck will be shuffled after evading")]
    public bool Shuffle = true;

    public override void Activate()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.CancelAllPowers(true, true);
        }
        base.PowerBegin();
        base.ShowDice(false);
        Turn.PushReturnState();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerBridgeEvade_Cancel"));
        Turn.SetStateData(new TurnStateData(ActionType.Discard, 2));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerBridgeEvade_Finish"));
        Turn.State = GameStateType.Penalty;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.Character.Hand.Count < 2)
        {
            return false;
        }
        return Rules.IsEvadePossible(Turn.Card);
    }

    private void LocationPowerBridgeEvade_Cancel()
    {
        this.PowerEnd();
        Turn.ReturnToReturnState();
    }

    private void LocationPowerBridgeEvade_Finish()
    {
        Turn.ReturnToReturnState();
        this.PowerEnd();
        base.ShowDice(false);
        Turn.Evade = true;
        if (this.Shuffle)
        {
            Turn.Card.Disposition = DispositionType.Shuffle;
        }
        else
        {
            Turn.Card.Disposition = DispositionType.Top;
        }
        Turn.Proceed();
    }

    public override PowerType Type =>
        PowerType.Evade;
}

