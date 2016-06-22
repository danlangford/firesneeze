using System;
using UnityEngine;

public class BlockSortCard : Block
{
    [Tooltip("the cards to be sorted to the top")]
    public CardSelector CardsToSort;
    [Tooltip("do nothing if any of these cards are in the deck")]
    public CardSelector Exceptions;
    [Tooltip("the deck to search")]
    public DeckType FromDeck = DeckType.Location;

    public override void Invoke()
    {
        Deck deck = this.FromDeck.GetDeck();
        if (((deck != null) && ((this.Exceptions == null) || (this.Exceptions.Filter(deck) <= 0))) && (this.CardsToSort != null))
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                if (this.CardsToSort.Match(deck[i]))
                {
                    deck.Move(i, 0);
                }
            }
        }
    }
}

