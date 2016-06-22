using System;
using UnityEngine;

public class EventCheckModifierBlessing : Event
{
    [Tooltip("modifier amount")]
    public int Amount = 3;
    [Tooltip("event only triggers if the top blessing discard matches")]
    public CardSelector Selector;

    public override int GetCheckModifier()
    {
        if (((this.Selector != null) && (Scenario.Current.Discard.Count > 0)) && this.Selector.Match(Scenario.Current.Discard[0]))
        {
            return this.Amount;
        }
        return 0;
    }
}

