using System;

public class EventDefeatedExplore : Event
{
    public override bool IsEventValid(Card card) => 
        base.IsConditionValid(card);

    public override void OnCardDefeated(Card card)
    {
        if (this.IsEventValid(card))
        {
            Turn.Explore = true;
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

