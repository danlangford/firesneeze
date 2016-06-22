using System;
using UnityEngine;

public class PowerConditionBlessingDiscard : PowerCondition
{
    [Tooltip("if true, always return false if this card is the following magic card IDs")]
    public bool ExcludeMagicList = true;
    [Tooltip("Does the top card of the blessing pile match this selector?")]
    public CardSelector Selector;

    public override bool Evaluate(Card card)
    {
        if (Scenario.Current.Discard.Count <= 0)
        {
            return false;
        }
        if (this.ExcludeMagicList)
        {
            for (int i = 0; i < Constants.BlessingDiscardExclusion.Length; i++)
            {
                if (card.ID == Constants.BlessingDiscardExclusion[i])
                {
                    return false;
                }
            }
        }
        Card card2 = Scenario.Current.Discard[0];
        return this.Selector.Match(card2);
    }
}

