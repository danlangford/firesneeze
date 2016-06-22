using System;
using UnityEngine;

public class LocationPowerAcquireDrawRandom : LocationPower
{
    [Tooltip("the random card type to draw from the box")]
    public CardType CardToDraw;

    public override void Activate()
    {
        base.PowerBegin(0.5f);
        for (int i = Turn.Character.Hand.Count - 1; i >= 0; i--)
        {
            if (Turn.BlackBoard.Get<string>("CharacterDefeatedCard") == Turn.Character.Hand[i].ID)
            {
                Campaign.Box.Add(Turn.Character.Hand[i], false);
                Card card = Campaign.Box.Draw(this.CardToDraw);
                if (card != null)
                {
                    GuiWindowLocation window = UI.Window as GuiWindowLocation;
                    if (window != null)
                    {
                        Card[] cards = new Card[] { card };
                        window.DrawCardsFromBox(cards, Turn.Owner.Hand, Turn.Current);
                    }
                    break;
                }
            }
        }
        Turn.BlackBoard.Set<string>("CharacterDefeatedCard", null);
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.State == GameStateType.EndTurn)
        {
            return false;
        }
        if (Location.Current.Closed)
        {
            return false;
        }
        string id = Turn.BlackBoard.Get<string>("CharacterDefeatedCard");
        return ((id != null) && Turn.Character.Hand.Contains(id));
    }
}

