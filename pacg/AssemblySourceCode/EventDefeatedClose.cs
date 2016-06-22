using System;

public class EventDefeatedClose : Event
{
    public override void OnCardDefeated(Card card)
    {
        if (!Rules.IsCardSummons(card))
        {
            Turn.Close = true;
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

