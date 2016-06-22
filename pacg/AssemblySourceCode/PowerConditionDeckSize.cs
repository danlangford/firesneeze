using System;
using UnityEngine;

public class PowerConditionDeckSize : PowerCondition
{
    [Tooltip("which deck to use")]
    public DeckType DeckType = DeckType.Hand;
    [Tooltip("deck size to check for")]
    public int Number;
    [Tooltip("comparison operator")]
    public MetaCompareOperator Operator = MetaCompareOperator.More;

    public override bool Evaluate(Card card)
    {
        Deck deck = this.DeckType.GetDeck();
        if (deck == null)
        {
            return false;
        }
        int count = deck.Count;
        if (this.DeckType == DeckType.Hand)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                if (deck[i].Displayed)
                {
                    count--;
                }
            }
        }
        return this.Operator.Evaluate(count, this.Number);
    }
}

