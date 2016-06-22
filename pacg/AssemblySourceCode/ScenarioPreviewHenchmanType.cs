using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioPreviewHenchmanType : ScenarioPreviewCustom
{
    [Tooltip("in the preview the henchmen are replaced with this card type")]
    public CardType HenchmanReplacedBy = CardType.Ally;

    public override int AddHenchmanToList(int henchmenCount, Func<string, int, Card> createCardMethod, List<Card> destination)
    {
        int num = Scenario.Current.LocationCards.Count - Scenario.Current.Villains.Length;
        for (int i = 0; i < num; i++)
        {
            string str = base.TypeToFakeCardID(CardType.Henchman);
            destination.Add(createCardMethod(str, i));
            destination[i].Show(CardSideType.Back);
            if ((this.HenchmanReplacedBy != CardType.None) && (i == (num - 1)))
            {
                base.AddTypeDecoration(destination[i], this.HenchmanReplacedBy);
            }
        }
        return 1;
    }

    public override int MaxHenchmen() => 
        1;
}

