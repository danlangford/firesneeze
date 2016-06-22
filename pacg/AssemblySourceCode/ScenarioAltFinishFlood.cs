using System;
using System.Collections.Generic;

public class ScenarioAltFinishFlood : ScenarioAltFinishBase
{
    private int GetCardTypeCount(List<string> list, CardType type)
    {
        int num = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (CardTable.LookupCardType(list[i]) == type)
            {
                num++;
            }
        }
        return num;
    }

    public override bool IsScenarioOver()
    {
        if ((Scenario.Current.Blessings.Count > 0) && (Party.CountLivingMembers() > 0))
        {
            for (int i = 0; i < Scenario.Current.LocationCards.Count; i++)
            {
                if (Scenario.Current.GetCardCount(Scenario.Current.Locations[i].LocationName) > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override bool IsScenarioWon()
    {
        if (Party.CountLivingMembers() > 0)
        {
            if (!this.IsScenarioOver())
            {
                return false;
            }
            EffectAcquiredOutOfTotal effect = Scenario.Current.GetEffect(EffectType.AcquiredOutOfTotal) as EffectAcquiredOutOfTotal;
            EffectNameable nameable = Scenario.Current.GetEffect(EffectType.Nameable) as EffectNameable;
            if (nameable == null)
            {
                return false;
            }
            int cardTypeCount = this.GetCardTypeCount(nameable.sources, CardType.Ally);
            int num2 = (effect != null) ? effect.sources.Count : 0;
            if (num2 >= cardTypeCount)
            {
                base.GetComponent<RewardCardTotalAcquired>().Rewards = effect.sources;
                return true;
            }
        }
        return false;
    }

    public override void ScenarioCleanup()
    {
        if (!Scenario.Current.Complete)
        {
            EffectAcquiredOutOfTotal effect = Scenario.Current.GetEffect(EffectType.AcquiredOutOfTotal) as EffectAcquiredOutOfTotal;
            if (effect != null)
            {
                for (int i = 0; i < effect.sources.Count; i++)
                {
                    Campaign.Box.Add(CardTable.Create(effect.sources[i]), false);
                }
            }
        }
    }
}

