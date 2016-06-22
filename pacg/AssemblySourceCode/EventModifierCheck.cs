using System;
using UnityEngine;

public class EventModifierCheck : Event
{
    [Tooltip("the type of card to count for the turn")]
    public CardSelector CountCard;
    [Tooltip("the amount to modify the check by")]
    public int Modifier = 2;

    public override int GetCheckModifier() => 
        (Turn.CheckBoard.Get<int>("CountModifierCheck") * this.Modifier);

    public override void OnCardActivated(Card card)
    {
        if (this.CountCard.Match(card))
        {
            int num = Turn.CheckBoard.Get<int>("CountModifierCheck") + 1;
            Turn.CheckBoard.Set<int>("CountModifierCheck", num);
            Turn.DiceTarget = Rules.GetCheckValue(Turn.Card, Turn.Check);
        }
    }

    public override void OnCardDeactivated(Card card)
    {
        if (this.CountCard.Match(card))
        {
            int num = Turn.CheckBoard.Get<int>("CountModifierCheck") - 1;
            Turn.CheckBoard.Set<int>("CountModifierCheck", num);
            Turn.DiceTarget = Rules.GetCheckValue(Turn.Card, Turn.Check);
        }
    }

    public override EventType Type =>
        EventType.OnCardActivated;
}

