using System;
using UnityEngine;

public class PowerConditionDamage : PowerCondition
{
    [Tooltip("the traits to evaluate")]
    public TraitType[] Traits;

    public override bool Evaluate(Card card)
    {
        for (int i = 0; i < this.Traits.Length; i++)
        {
            if (Turn.DamageTraits.Contains(this.Traits[i]))
            {
                return true;
            }
        }
        return false;
    }
}

