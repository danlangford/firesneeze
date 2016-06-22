using System;
using UnityEngine;

public class EventCheckModifierCard : Event
{
    [Tooltip("amount to add to the checks")]
    public int Amount = 1;
    [Tooltip("specifies the type of cards that this applies to")]
    public Selector Selector;

    public override int GetCheckModifier()
    {
        if (this.IsEventValid(Turn.Card))
        {
            return this.Amount;
        }
        return 0;
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if ((this.Selector != null) && !this.Selector.Match())
        {
            return false;
        }
        return base.IsEventValid(card);
    }
}

