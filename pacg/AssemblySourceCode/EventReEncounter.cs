using System;

public class EventReEncounter : Event
{
    public override void OnLocationExplored(Card card)
    {
        Turn.EncounterType = EncounterType.ReEncounter;
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnLocationExplored;
}

