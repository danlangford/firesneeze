using System;
using UnityEngine;

public class EventDistributeCardType : EventDistributeCard
{
    [Tooltip("the card type to distribute to locations")]
    public CardType DistributableType;

    public override void DistributeBadGuys()
    {
        for (int i = 0; i < Scenario.Current.Locations.Length; i++)
        {
            if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[i].LocationName))
            {
                CardIdentity card = Campaign.Box.Pull(this.DistributableType);
                if (card != null)
                {
                    Location.Distribute(Scenario.Current.Locations[i].LocationName, card, DeckPositionType.Shuffle, true);
                }
            }
        }
    }
}

