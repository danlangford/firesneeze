using System;
using System.Collections.Generic;
using UnityEngine;

public class EventDistributeFloodPile : EventDistributeCard
{
    [Tooltip("unique cards that should be added to the pile")]
    public string[] CardIDS;
    [Tooltip("the position the Pile cards should go")]
    public DeckPositionType Position = DeckPositionType.Shuffle;
    [Tooltip("the type of cards that should be added to the pile")]
    public CardType[] Types;

    public override void DistributeBadGuys()
    {
        int num = this.Types.Length + this.CardIDS.Length;
        int numLocations = Scenario.Current.GetNumLocations();
        List<CardIdentity> list = new List<CardIdentity>(numLocations * num);
        for (int i = 0; i < numLocations; i++)
        {
            for (int k = 0; k < this.Types.Length; k++)
            {
                list.Add(Campaign.Box.Pull(this.Types[k]));
            }
            for (int m = 0; m < this.CardIDS.Length; m++)
            {
                list.Add(Campaign.Box.Pull(this.CardIDS[m]));
            }
        }
        list.Shuffle<CardIdentity>();
        for (int j = 0; j < Scenario.Current.Locations.Length; j++)
        {
            if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[j].LocationName))
            {
                for (int n = 0; n < num; n++)
                {
                    if (list[list.Count - 1] != null)
                    {
                        Location.Distribute(Scenario.Current.Locations[j].LocationName, list[list.Count - 1], this.Position, false);
                    }
                    list.RemoveAt(list.Count - 1);
                }
            }
        }
    }
}

