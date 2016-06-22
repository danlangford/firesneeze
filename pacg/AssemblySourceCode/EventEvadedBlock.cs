using System;
using UnityEngine;

public class EventEvadedBlock : Event
{
    [Tooltip("the block to invoke")]
    public Block Block;
    [Tooltip("only fire this event when the Selector matches the card evaded")]
    public CardSelector Selector;

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!this.Selector.Match(card))
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCardEvaded(Card card)
    {
        if (this.IsEventValid(card) && (this.Block != null))
        {
            this.Block.Invoke();
        }
        base.OnCardEvaded(card);
    }

    public override EventType Type =>
        EventType.OnCardEvaded;
}

