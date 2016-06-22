using System;
using UnityEngine;

public class CharacterPowerRechargeDiscard : CharacterPower
{
    [Tooltip("only cards that were played and ended in this action layout can recharge")]
    public ActionType Action = ActionType.Discard;
    [Tooltip("only cards of this type will be recharged when discarded")]
    public CardType CardType;

    public bool Match(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if ((card.Type != this.CardType) && (this.CardType != CardType.None))
        {
            return false;
        }
        if (((card.Deck != null) && (card.Deck.Layout != null)) && ((this.Action != ActionType.None) && (card.Deck.Layout.CardAction != this.Action)))
        {
            return false;
        }
        return true;
    }
}

