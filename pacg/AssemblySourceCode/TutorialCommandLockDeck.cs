using System;
using UnityEngine;

public class TutorialCommandLockDeck : TutorialCommand
{
    [Tooltip("the deck containing the cards")]
    public DeckType FromDeck = DeckType.Hand;
    [Tooltip("if true, ignore the list of cards and lock everything")]
    public bool LockAll;
    [Tooltip("list of cards to lock")]
    public string[] LockedCards;

    private Deck GetDeck(DeckType deckType)
    {
        if (deckType == DeckType.Location)
        {
            return Location.Current.Deck;
        }
        return Turn.Character.GetDeck(this.FromDeck);
    }

    public override void Invoke()
    {
        Deck deck = this.GetDeck(this.FromDeck);
        if (deck != null)
        {
            if (this.LockAll)
            {
                for (int i = 0; i < deck.Count; i++)
                {
                    deck[i].Locked = true;
                }
            }
            else
            {
                for (int j = 0; j < deck.Count; j++)
                {
                    deck[j].Locked = false;
                }
                for (int k = 0; k < this.LockedCards.Length; k++)
                {
                    string str = this.LockedCards[k];
                    Card card = deck[str];
                    if (card != null)
                    {
                        card.Locked = true;
                    }
                }
            }
        }
    }
}

