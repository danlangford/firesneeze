using System;
using UnityEngine;

public class TutorialCommandEncounter : TutorialCommand
{
    [Tooltip("the ID of the card to insert at the top of the location deck")]
    public string ID;

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            CardType cardType = CardTable.LookupCardType(this.ID);
            bool flag = true;
            if (Location.Current.Deck.Count > 0)
            {
                Location.Current.Deck[0].Show(false);
            }
            for (int i = 0; i < Location.Current.Deck.Count; i++)
            {
                Card card = Location.Current.Deck[i];
                if (card.ID == this.ID)
                {
                    Location.Current.Deck.Move(i, 0);
                    flag = false;
                    break;
                }
            }
            if (flag && (cardType == CardType.Villain))
            {
                for (int j = 0; j < Location.Current.Deck.Count; j++)
                {
                    Card card2 = Location.Current.Deck[j];
                    if (card2.Type == CardType.Henchman)
                    {
                        Location.Current.Deck.Remove(card2);
                        Campaign.Box.Add(card2, false);
                        break;
                    }
                }
            }
            if (flag && (cardType != CardType.Villain))
            {
                for (int k = 0; k < Location.Current.Deck.Count; k++)
                {
                    Card card3 = Location.Current.Deck[k];
                    if (card3.Type == cardType)
                    {
                        Location.Current.Deck.Remove(card3);
                        Campaign.Box.Add(card3, false);
                        break;
                    }
                }
            }
            if (flag)
            {
                Card card4 = CardTable.Create(this.ID);
                Location.Current.Deck.Add(card4, DeckPositionType.Top);
            }
            if (Location.Current.Deck.Count > 0)
            {
                Location.Current.Deck[0].transform.position = window.layoutLocation.transform.position;
                Location.Current.Deck[0].transform.localScale = window.layoutLocation.Scale;
            }
        }
    }
}

