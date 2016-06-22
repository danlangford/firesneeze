using System;
using UnityEngine;

public class CharacterPowerChangeCardDice : CharacterPower
{
    [Tooltip("whenever an appropriate card is played use this dice type")]
    public DiceType ReplaceWith;
    [Tooltip("the Selector determines which cards this power will effect")]
    public CardSelector Selector;

    public DiceType GetModifiedDice(Card card, DiceType originalDice)
    {
        if (this.IsValid(card))
        {
            return this.ReplaceWith;
        }
        return originalDice;
    }

    private bool IsValid(Card card)
    {
        if ((this.Selector != null) && !this.Selector.Match(card))
        {
            return false;
        }
        return base.IsConditionValid(card);
    }
}

