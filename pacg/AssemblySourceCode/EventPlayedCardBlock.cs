using System;
using UnityEngine;

public class EventPlayedCardBlock : Event
{
    [Tooltip("block to run when a card is played")]
    public Block Block;

    public override void OnCardPlayed(Card card)
    {
        if (this.IsEventValid(card) && (this.Block != null))
        {
            this.Block.Invoke();
        }
        Event.Done();
    }

    public override bool Stateless =>
        ((this.Block == null) || this.Block.Stateless);

    public override EventType Type =>
        EventType.OnCardPlayed;
}

