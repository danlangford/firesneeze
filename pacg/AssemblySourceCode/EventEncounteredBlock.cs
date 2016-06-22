using System;
using UnityEngine;

public class EventEncounteredBlock : Event
{
    [Tooltip("the block to invoke when a card is encountered")]
    public Block BlockPenalty;

    public override void OnCardEncountered(Card card)
    {
        if (base.IsConditionValid(card) && (this.BlockPenalty != null))
        {
            this.BlockPenalty.Invoke();
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardEncountered;
}

