using System;
using UnityEngine;

public class BlockSetDisposition : Block
{
    [Tooltip("should the game state advance to damage?")]
    public bool ChangeStates = true;
    [Tooltip("the disposition the card is set to")]
    public DispositionType Disposition = DispositionType.Shuffle;

    private string GetCardID()
    {
        Card component = base.GetComponent<Card>();
        if (component != null)
        {
            return component.ID;
        }
        return Turn.Card.ID;
    }

    public override void Invoke()
    {
        string cardID = this.GetCardID();
        if (Turn.Card.ID == cardID)
        {
            Turn.Card.Disposition = this.Disposition;
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

