using System;
using UnityEngine;

public class BlockFlipCards : Block
{
    [Tooltip("the deck to flip")]
    public DeckType DeckType = DeckType.Location;
    [Tooltip("optionally set the disposition of matching cards")]
    public CardSelector Dispose;
    [Tooltip(".. to this value")]
    public DispositionType Disposition;
    [Tooltip("flip cards matching this selector")]
    public CardSelector Selector;
    [Tooltip(".. to this side")]
    public CardSideType Side = CardSideType.Back;

    public override void Invoke()
    {
        Deck deck = Location.Current.Deck;
        for (int i = 0; i < deck.Count; i++)
        {
            Card card = deck[i];
            if ((this.Selector != null) && this.Selector.Match(card))
            {
                card.Side = this.Side;
            }
        }
        if ((this.Dispose != null) && (this.Dispose.Filter(deck) <= 0))
        {
            Turn.Card.Disposition = DispositionType.Banish;
        }
    }
}

