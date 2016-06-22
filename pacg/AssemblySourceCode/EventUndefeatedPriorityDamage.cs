using System;
using UnityEngine;

public class EventUndefeatedPriorityDamage : Event
{
    [Tooltip("force discards of this type during damage")]
    public CardType CardType = CardType.Blessing;

    public override bool IsEventValid(Card card)
    {
        if (Turn.LastCombatResult != CombatResultType.Lose)
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCombatResolved()
    {
        if (this.IsEventValid(Turn.Card))
        {
            Turn.PriorityCardType = this.CardType;
        }
        base.OnCombatResolved();
    }

    public override EventType Type =>
        EventType.OnCombatResolved;
}

