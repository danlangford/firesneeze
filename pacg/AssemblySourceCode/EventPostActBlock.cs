using System;
using UnityEngine;

public class EventPostActBlock : Event
{
    [Tooltip("the block to invoke")]
    public Block Block;
    [Tooltip("the cards to match this post act block to")]
    public CardSelector Selector;

    public override bool IsEventValid(Card card) => 
        base.IsConditionValid(card);

    public override void OnPostAct()
    {
        if ((this.IsEventValid(Turn.Card) && ((this.Selector == null) || this.Selector.Match())) && (this.Block != null))
        {
            this.Block.Invoke();
        }
        Event.Done();
    }

    public override bool Stateless =>
        ((this.Block == null) || this.Block.Stateless);

    public override EventType Type =>
        EventType.OnPostAct;
}

