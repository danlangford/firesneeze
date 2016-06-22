using System;
using UnityEngine;

public class CardPropertyImmunity : CardProperty
{
    [Tooltip("list of immunities")]
    public TraitType[] Immunities;

    public bool IsImmune(Card attacker)
    {
        if (this.Immunities != null)
        {
            for (int i = 0; i < this.Immunities.Length; i++)
            {
                if (attacker.HasTrait(this.Immunities[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsImmune(TraitType trait)
    {
        if (this.Immunities != null)
        {
            for (int i = 0; i < this.Immunities.Length; i++)
            {
                if (trait == this.Immunities[i])
                {
                    return true;
                }
            }
        }
        return false;
    }
}

