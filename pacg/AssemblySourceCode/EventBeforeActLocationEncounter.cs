using System;

public class EventBeforeActLocationEncounter : Event
{
    public bool DefeatAll;

    public override void OnBeforeAct()
    {
        if (Turn.Number == Turn.InitialCharacter)
        {
            if (this.DefeatAll)
            {
                Turn.EncounterType = EncounterType.LocationDefeat;
                Turn.Iterators.Start(TurnStateIteratorType.Defeat);
            }
            else
            {
                Turn.EncounterType = EncounterType.LocationEncounter;
                Turn.Iterators.Start(TurnStateIteratorType.Encounter);
            }
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

