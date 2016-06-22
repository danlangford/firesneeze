using System;
using UnityEngine;

public class EventModifierDice : Event
{
    [Tooltip("if true only affects weapon rolls")]
    public bool AppliesToWeapons;
    [Tooltip("modify each die rolled by this amount")]
    public int DiceModifier = -1;

    public override int GetDiceModifier(DiceType diceType)
    {
        if (!this.IsEventValid(Turn.Card))
        {
            return 0;
        }
        return this.DiceModifier;
    }

    public override bool IsEventValid(Card card) => 
        ((!this.AppliesToWeapons || ((Turn.Weapon1 != null) && ((Turn.Weapon1 != "Unarmed") || (Turn.Weapon2 != null)))) && base.IsConditionValid(card));
}

