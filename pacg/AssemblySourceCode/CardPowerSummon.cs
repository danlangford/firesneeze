using System;
using UnityEngine;

public class CardPowerSummon : CardPower
{
    [Tooltip("mandatory type of card to draw")]
    public CardType CardType = CardType.Ally;
    [Tooltip("number of cards to draw from the box")]
    public int Number = 1;
    [Tooltip("optional required trait on cards to draw")]
    public TraitType[] Traits;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if (this.Number > 0)
                {
                    Card[] cards = new Card[this.Number];
                    for (int i = 0; i < this.Number; i++)
                    {
                        Card card2 = null;
                        if (this.Traits.Length > 0)
                        {
                            card2 = Campaign.Box.Draw(this.CardType, this.Traits);
                        }
                        else
                        {
                            card2 = Campaign.Box.Draw(this.CardType);
                        }
                        cards[i] = card2;
                    }
                    window.DrawCardsFromBox(cards, Turn.Character.Hand, Turn.Number);
                }
                if (this.GetIndex(card) >= 0)
                {
                    card.SetPowerInfo(this.GetIndex(card), card);
                }
                window.ProcessRechargableCards();
                window.ProcessLayoutDecks();
                if (Turn.Character.GetSkillRank(SkillCheckType.Arcane) <= 0)
                {
                    VisualEffect.ApplyToCard(VisualEffectType.CardBanishFromDisplay, card, 2.1f);
                    Campaign.Box.Add(base.Card, false);
                }
            }
        }
    }

    private int GetIndex(Card card)
    {
        for (int i = 0; i < card.Powers.Length; i++)
        {
            if (card.Powers[i].Equals(this))
            {
                return i;
            }
        }
        return -1;
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if ((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup))
        {
            return false;
        }
        return true;
    }
}

