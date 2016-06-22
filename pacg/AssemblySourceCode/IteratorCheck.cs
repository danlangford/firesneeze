using System;

public class IteratorCheck : TurnStateIterator
{
    public override void End()
    {
        base.End();
        Location.Load(Turn.Character.Location);
        if (base.CallBackType == TurnStateCallbackType.Card)
        {
            Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventRoll_End"));
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(base.CallBackType, "EventRoll_End"));
        }
        Turn.DamageTargetType = DamageTargetType.None;
        Turn.GotoStateDestination();
    }

    public override void Invoke()
    {
        Turn.Current = Turn.Number;
        Location.Load(Turn.Character.Location);
        base.RefreshLocationWindow();
        Turn.DamageReduction = true;
        if (base.CallBackType == TurnStateCallbackType.Card)
        {
            Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventRoll_Check"));
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(base.CallBackType, "EventRoll_Check"));
        }
        Turn.GotoStateDestination();
    }

    public override bool IsValid()
    {
        if (Turn.DamageTargetType == DamageTargetType.Location)
        {
            return (Location.CountCharactersAtLocation(Location.Current.ID) > 1);
        }
        return ((Turn.DamageTargetType == DamageTargetType.Party) && (Party.Characters.Count > 1));
    }

    public override bool Next() => 
        (((Turn.DamageTargetType == DamageTargetType.Party) && base.NextCharacterInParty()) || base.NextCharacterAtLocation(Location.Current.ID));

    public override void Start()
    {
        base.Start();
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Check;
}

