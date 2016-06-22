using System;
using UnityEngine;

public class CardPowerReroll : CardPower
{
    [Tooltip("max difference between player's roll and target number")]
    public int Difference = 1;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Defeat = true;
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            Turn.Defeat = false;
        }
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if ((Turn.State != GameStateType.Reroll) && (Turn.State != GameStateType.RollAgain))
        {
            return false;
        }
        if (Turn.Defeat)
        {
            return false;
        }
        return ((Turn.DiceTarget - Turn.DiceTotal) == this.Difference);
    }

    public override PowerType Type =>
        PowerType.Reroll;
}

