using System;

public class EventUndefeatedEndTurn : Event
{
    public override void OnCardUndefeated(Card card)
    {
        Turn.End = true;
        Turn.EndReason = GameReasonType.MonsterForced;
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

