using System;
using UnityEngine;

public class EventUndefeatedDisposition : Event
{
    [Tooltip("the new disposition value")]
    public DispositionType Disposition;
    [Tooltip("true if the card will be face-up in the location deck")]
    public bool FaceUp;

    private BlockerType GetBlockerType(Card card)
    {
        CardPropertyBlocker component = card.GetComponent<CardPropertyBlocker>();
        if (component != null)
        {
            return component.GetBlockerType();
        }
        return BlockerType.None;
    }

    public override bool IsEventValid(Card card)
    {
        if (Rules.IsCardSummons(card))
        {
            return false;
        }
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCardUndefeated(Card card)
    {
        if (this.IsEventValid(card))
        {
            card.Disposition = this.Disposition;
            if (this.FaceUp)
            {
                card.Blocker = this.GetBlockerType(card);
            }
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

