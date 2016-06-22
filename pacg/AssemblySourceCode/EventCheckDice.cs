using System;
using UnityEngine;

public class EventCheckDice : Event
{
    [Tooltip("the dice type to add")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("the dice bonus to add")]
    public int DiceBonus;
    [Tooltip("determines when this event occurs")]
    public Selector Selector;

    public override int GetCheckBonus()
    {
        if (this.IsEventValid(Turn.Card))
        {
            return this.DiceBonus;
        }
        return 0;
    }

    public override DiceType GetCheckDice()
    {
        if (this.IsEventValid(Turn.Card) && (this.Dice.Length > 0))
        {
            return this.Dice[0];
        }
        return DiceType.D0;
    }

    public override bool IsEventValid(Card card)
    {
        if ((this.Selector != null) && !this.Selector.Match())
        {
            return false;
        }
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return base.IsEventValid(card);
    }
}

