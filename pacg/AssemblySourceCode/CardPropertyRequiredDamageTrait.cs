using System;
using UnityEngine;

public class CardPropertyRequiredDamageTrait : CardProperty
{
    [Tooltip("trait required to defeat this card")]
    public TraitType[] Trait = new TraitType[] { TraitType.Magic };

    public bool IsDefeatable()
    {
        for (int i = 0; i < Turn.DamageTraits.Count; i++)
        {
            for (int j = 0; j < this.Trait.Length; j++)
            {
                if (((TraitType) Turn.DamageTraits[i]) == this.Trait[j])
                {
                    return true;
                }
            }
        }
        return false;
    }
}

