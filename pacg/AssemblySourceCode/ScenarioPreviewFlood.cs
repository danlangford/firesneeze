using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioPreviewFlood : ScenarioPreviewCustom
{
    [Tooltip("for each location we preview an equivalent pile of this card")]
    public string[] Cards;
    [Tooltip("for each location we preview an equivalent pile of this type")]
    public CardType[] Types;

    public override int AddHenchmanToList(int henchmenCount, Func<string, int, Card> createCardMethod, List<Card> destination)
    {
        string str = null;
        CardSideType front = CardSideType.Front;
        CardType none = CardType.None;
        if (henchmenCount < this.Types.Length)
        {
            str = base.TypeToFakeCardID(this.Types[henchmenCount]);
            front = CardSideType.Back;
            none = this.Types[henchmenCount];
        }
        else if ((henchmenCount - this.Types.Length) < this.Cards.Length)
        {
            str = this.Cards[henchmenCount - this.Types.Length];
        }
        if (str != null)
        {
            for (int i = 0; i < Scenario.Current.LocationCards.Count; i++)
            {
                Card item = createCardMethod(str, i);
                item.Show(front);
                destination.Add(item);
                if ((none != CardType.None) && (i == (Scenario.Current.LocationCards.Count - 1)))
                {
                    base.AddTypeDecoration(item, none);
                }
            }
        }
        return 1;
    }

    public override int MaxHenchmen() => 
        (this.Cards.Length + this.Types.Length);
}

