using System;
using UnityEngine;

public class CardDistribution
{
    [Tooltip("card id")]
    public string ID;
    [Tooltip("which position should this card be inserted?")]
    public DeckPositionType Position;
    [Tooltip("card type (optional)")]
    public CardType Type;

    public CardDistribution(Card card, DeckPositionType position)
    {
        this.ID = card.ID;
        this.Position = position;
        this.Type = card.Type;
    }

    public CardDistribution(string id, DeckPositionType position)
    {
        this.ID = id;
        this.Position = position;
        this.Type = CardType.None;
    }

    public void Distribute(Deck deck)
    {
        if (deck != null)
        {
            Card card = CardTable.Create(this.ID);
            if (card != null)
            {
                deck.Add(card, this.Position);
            }
        }
    }
}

