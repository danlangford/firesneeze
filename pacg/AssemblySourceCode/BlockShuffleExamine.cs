using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockShuffleExamine : Block
{
    private Stack<Card> botCards = new Stack<Card>(3);
    private Stack<Card> rejCards = new Stack<Card>(3);
    [Tooltip("optional: only cards matching this selector are preserved")]
    public CardSelector Selector;
    private Stack<Card> topCards = new Stack<Card>(3);

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && (this.Deck.Count > 0))
        {
            int top = window.layoutExamine.Top;
            for (int i = 0; i < top; i++)
            {
                Card card = this.Deck.Draw();
                if (this.Match(card))
                {
                    this.topCards.Push(card);
                }
                else
                {
                    this.rejCards.Push(card);
                }
            }
            int bottom = window.layoutExamine.Bottom;
            for (int j = 0; j < bottom; j++)
            {
                this.Deck.Move(this.Deck.Count - 1, 0);
                if (this.Match(this.Deck[0]))
                {
                    Card t = this.Deck.Draw();
                    this.botCards.Push(t);
                }
            }
            while (this.rejCards.Count > 0)
            {
                Card card3 = this.rejCards.Pop();
                this.Deck.Add(card3);
            }
            this.Deck.Shuffle();
            while (this.topCards.Count > 0)
            {
                Card card4 = this.topCards.Pop();
                this.Deck.Add(card4, DeckPositionType.Top);
            }
            while (this.botCards.Count > 0)
            {
                Card card5 = this.botCards.Pop();
                this.Deck.Add(card5, DeckPositionType.Bottom);
            }
            this.topCards.Clear();
            this.botCards.Clear();
            this.rejCards.Clear();
        }
    }

    private bool Match(Card card) => 
        ((this.Selector == null) || this.Selector.Match(card));

    private Deck Deck =>
        Location.Current.Deck;
}

