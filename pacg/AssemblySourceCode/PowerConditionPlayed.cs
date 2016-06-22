using System;
using UnityEngine;

public class PowerConditionPlayed : PowerCondition
{
    [Tooltip("exclude a single instance of ourself in-play")]
    public bool ExcludeSelf = true;
    [Tooltip("when true, the current character must have played the card")]
    public bool PlayedByMe = true;
    [Tooltip("cards of this type will be matched")]
    public CardSelector Selector;

    private int CountPlayedCards(Deck deck)
    {
        int num = 0;
        for (int i = 0; i < deck.Count; i++)
        {
            if (this.IsCardPlayed(deck[i]))
            {
                num++;
            }
        }
        return num;
    }

    public override bool Evaluate(Card card)
    {
        if (this.Selector == null)
        {
            return false;
        }
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = 0; j < Party.Characters[i].Hand.Count; j++)
            {
                if (this.IsCardPlayed(Party.Characters[i].Hand[j]))
                {
                    return true;
                }
            }
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window == null)
        {
            return false;
        }
        return ((this.CountPlayedCards(window.layoutDiscard.Deck) > 0) || ((this.CountPlayedCards(window.layoutRecharge.Deck) > 0) || ((this.CountPlayedCards(window.layoutBury.Deck) > 0) || (this.CountPlayedCards(window.layoutBanish.Deck) > 0))));
    }

    private Guid GetSelfGuid()
    {
        Card component = base.GetComponent<Card>();
        if (component != null)
        {
            return component.GUID;
        }
        return Guid.Empty;
    }

    private bool IsCardPlayed(Card card)
    {
        if (!card.Played)
        {
            return false;
        }
        if (this.ExcludeSelf && (this.GetSelfGuid() == card.GUID))
        {
            return false;
        }
        if (!this.Selector.Match(card))
        {
            return false;
        }
        if (this.PlayedByMe)
        {
            return (card.PlayedOwner == Turn.Character.ID);
        }
        return !string.IsNullOrEmpty(card.PlayedOwner);
    }
}

