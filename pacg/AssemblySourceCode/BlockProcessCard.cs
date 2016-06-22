using System;
using UnityEngine;

public class BlockProcessCard : Block
{
    [Tooltip("the card to be processed")]
    public string ID;

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            this.MoveCard(this.ID, window.layoutDiscard.Deck, Turn.Character.Discard);
            this.MoveCard(this.ID, window.layoutBury.Deck, Turn.Character.Bury);
            this.MoveCard(this.ID, window.layoutRecharge.Deck, Turn.Character.Deck);
            for (int i = 0; i < window.layoutBanish.Deck.Count; i++)
            {
                Card card = window.layoutBanish.Deck[i];
                if (card.ID == this.ID)
                {
                    Campaign.Box.Add(card, false);
                    break;
                }
            }
        }
    }

    private void MoveCard(string ID, Deck from, Deck to)
    {
        for (int i = 0; i < from.Count; i++)
        {
            if (from[i].ID == ID)
            {
                from[i].Clear();
                to.Add(from[i]);
                break;
            }
        }
    }
}

