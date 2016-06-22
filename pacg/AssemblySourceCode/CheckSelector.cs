using System;
using UnityEngine;

public class CheckSelector : Selector
{
    [Tooltip("the current skill that the player is rolling")]
    public SkillCheckType Check;
    [Tooltip("the current check must include at least one of these")]
    public TraitType[] DamageTraits;

    protected override bool IsEmpty() => 
        ((this.Check == SkillCheckType.None) && (this.DamageTraits.Length == 0));

    public override bool Match()
    {
        if (this.IsEmpty())
        {
            return true;
        }
        if ((this.Check != SkillCheckType.None) && Rules.IsSkillParticipatingInCheck(this.Check, false))
        {
            return true;
        }
        for (int i = 0; i < this.DamageTraits.Length; i++)
        {
            if (Turn.DamageTraits.Contains(this.DamageTraits[i]))
            {
                return true;
            }
        }
        return false;
    }
}

