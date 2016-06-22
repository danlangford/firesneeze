using System;

public class EventExamineAnyLocationAugury : Event
{
    public override void OnExamineAnyLocation()
    {
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "SpellAugury_Examine"));
        Turn.GotoStateDestination();
        Event.Done();
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnExamineAnyLocation;
}

