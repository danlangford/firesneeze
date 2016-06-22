using System;
using UnityEngine;

public class CardPropertyDamage : CardProperty
{
    [Tooltip("can the damage be reduced by the player's armor or items?")]
    public bool Reducible = true;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;
    [Tooltip("the types of damage done by this card")]
    public TraitType[] Traits = new TraitType[] { TraitType.Melee };

    public void Activate()
    {
        Turn.AddTraits(this.Traits);
        Turn.DamageReduction = this.Reducible;
        Turn.DamageTargetType = this.Target;
    }
}

