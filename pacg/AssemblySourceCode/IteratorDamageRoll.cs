using System;

public class IteratorDamageRoll : IteratorDamage
{
    public override void Invoke()
    {
        Turn.Current = Turn.Number;
        Location.Load(Turn.Character.Location);
        base.RefreshLocationWindow();
        Turn.DamageReduction = true;
        Turn.State = GameStateType.Null;
        if (Turn.DamageTargetType == DamageTargetType.Party)
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "RollDamage"));
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "RollDamage"));
        }
        Turn.GotoStateDestination();
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.DamageRoll;
}

