using System;
using UnityEngine;

public class EventDefeatedBlock : Event
{
    [Tooltip("block to invoke when not matched")]
    public Block BlockNo;
    [Tooltip("block to invoke when matched")]
    public Block BlockYes;
    [Tooltip("determines which cards trigger the block. Null selector means everything")]
    public CardSelector Selector;

    public override bool IsEventValid(Card card) => 
        base.IsConditionValid(card);

    public override void OnCardDefeated(Card card)
    {
        if (this.IsEventValid(card))
        {
            if ((this.Selector == null) || this.Selector.Match(card))
            {
                if (this.BlockYes != null)
                {
                    this.BlockYes.Invoke();
                }
            }
            else if (this.BlockNo != null)
            {
                this.BlockNo.Invoke();
            }
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

