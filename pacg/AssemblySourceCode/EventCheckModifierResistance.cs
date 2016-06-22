using System;
using UnityEngine;

public class EventCheckModifierResistance : Event
{
    [Tooltip("amount added to check (negative numbers would be weaknesses)")]
    public int Amount = 3;
    [Tooltip("player races that resistance applies to")]
    public RaceType[] Races;
    [Tooltip("traits that resistance applies to")]
    public TraitType[] Traits;

    public override int GetCheckModifier()
    {
        for (int i = 0; i < Turn.DamageTraits.Count; i++)
        {
            for (int k = 0; k < this.Traits.Length; k++)
            {
                if (this.Traits[k] == ((TraitType) Turn.DamageTraits[i]))
                {
                    return this.Amount;
                }
            }
        }
        for (int j = 0; j < this.Races.Length; j++)
        {
            if (this.Races[j] == Turn.Owner.Race)
            {
                return this.Amount;
            }
        }
        return 0;
    }
}

