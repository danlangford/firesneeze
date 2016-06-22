using System;
using UnityEngine;

public class EventDistributeEffectAcquiredOutOfTotal : EventDistributeCard
{
    [Tooltip("the effect message in the effects panel")]
    public StrRefType EffectMessage;
    [Tooltip("selector determines which ones we should keep track of out of total possible")]
    public CardSelector Selector;

    public override void DistributeBadGuys()
    {
        EffectAcquiredOutOfTotal effect = new EffectAcquiredOutOfTotal(null, Effect.DurationPermament, this.GetTotalOfType(), this.Selector.CardTypes[0], this.EffectMessage.id);
        Scenario.Current.ApplyEffect(effect);
    }

    private int GetTotalOfType()
    {
        int num = 0;
        for (int i = 0; i < this.Selector.CardTypes.Length; i++)
        {
            for (int j = 0; j < Scenario.Current.Locations.Length; j++)
            {
                if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[j].LocationName))
                {
                    num += Scenario.Current.GetCardCount(Scenario.Current.Locations[j].LocationName, this.Selector.CardTypes[i]);
                }
            }
        }
        EventDistributeFloodPile component = Scenario.Current.GetComponent<EventDistributeFloodPile>();
        if (component != null)
        {
            for (int k = 0; k < component.Types.Length; k++)
            {
                for (int m = 0; m < this.Selector.CardTypes.Length; m++)
                {
                    if (component.Types[k] == this.Selector.CardTypes[m])
                    {
                        num += Scenario.Current.GetNumLocations();
                    }
                }
            }
        }
        return num;
    }
}

