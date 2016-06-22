using System;
using UnityEngine;

public class EventUndefeatedBanish : Event
{
    [Tooltip("only cards of this type will be banished")]
    public CardType CardType;

    public override void OnCardUndefeated(Card card)
    {
        if (card.Type == this.CardType)
        {
            card.Disposition = DispositionType.Banish;
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

