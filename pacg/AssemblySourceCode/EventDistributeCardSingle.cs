using System;
using UnityEngine;

public class EventDistributeCardSingle : EventDistributeCard
{
    [Tooltip("the card ID to distribute")]
    public string ID;
    [Tooltip("whether the players know this card is distributed")]
    public bool Known = true;
    [Tooltip("where in the deck should the card be placed?")]
    public DeckPositionType Position = DeckPositionType.Shuffle;

    public override void DistributeBadGuys()
    {
        for (int i = 0; i < Scenario.Current.Locations.Length; i++)
        {
            if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[i].LocationName))
            {
                CardIdentity card = Campaign.Box.Pull(this.ID);
                if (card != null)
                {
                    Location.Distribute(Scenario.Current.Locations[i].LocationName, card, this.Position, this.Known);
                }
            }
        }
    }
}

