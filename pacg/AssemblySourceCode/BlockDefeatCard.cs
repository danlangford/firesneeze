using System;
using UnityEngine;

public class BlockDefeatCard : Block
{
    [Tooltip("only cards that match will be defeated")]
    public CardSelector Selector;

    public override void Invoke()
    {
        if ((Location.Current.Deck.Count > 0) && ((this.Selector == null) || this.Selector.Match(Location.Current.Deck[0])))
        {
            Turn.Card.OnDefeated();
            Turn.Character.Hand.Layout.Refresh();
            Turn.State = GameStateType.Damage;
        }
    }

    public override bool Stateless =>
        false;
}

