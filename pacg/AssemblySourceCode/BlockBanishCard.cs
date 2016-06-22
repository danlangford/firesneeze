using System;
using UnityEngine;

public class BlockBanishCard : Block
{
    [Tooltip("should the game state advance to damage?")]
    public bool ChangeStates = true;

    private string GetCardID()
    {
        Card component = base.GetComponent<Card>();
        if (component != null)
        {
            return component.ID;
        }
        return null;
    }

    public override void Invoke()
    {
        string cardID = this.GetCardID();
        if (Turn.Card.ID == cardID)
        {
            Turn.Card.Disposition = DispositionType.Banish;
        }
        else if (cardID != null)
        {
            Card card = Location.Current.Deck[cardID];
            if (card != null)
            {
                Campaign.Box.Add(card, false);
            }
        }
        if (this.ChangeStates)
        {
            Turn.State = GameStateType.Damage;
        }
    }

    public override bool Stateless
    {
        get
        {
            if (this.ChangeStates)
            {
                return false;
            }
            return true;
        }
    }
}

