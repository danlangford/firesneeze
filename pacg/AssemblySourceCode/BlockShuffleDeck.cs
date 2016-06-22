using System;
using UnityEngine;

public class BlockShuffleDeck : Block
{
    [Tooltip("which deck to shuffle")]
    public DeckType DeckType = DeckType.Location;
    [Tooltip("make sure cards are face down before shuffling")]
    public bool FaceDown;
    [Tooltip("shuffle except the top card")]
    public bool ShuffleUnderTop;
    [Tooltip("should we visually shuffle the deck?")]
    public bool VisualShuffle = true;

    public override void Invoke()
    {
        Deck deck = this.DeckType.GetDeck();
        if ((this.DeckType == DeckType.Location) && !this.ShuffleUnderTop)
        {
            Turn.Card.Show(false);
        }
        if (this.FaceDown)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                deck[i].Show(CardSideType.Back);
            }
        }
        if (deck != null)
        {
            if (this.VisualShuffle)
            {
                VisualEffect.Shuffle(this.DeckType);
            }
            if (this.ShuffleUnderTop)
            {
                deck.ShuffleUnderTop();
                Turn.Card.Show(true);
            }
            else
            {
                deck.Shuffle();
            }
        }
    }
}

