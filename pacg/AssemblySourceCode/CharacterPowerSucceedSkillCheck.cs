using System;
using UnityEngine;

public class CharacterPowerSucceedSkillCheck : CharacterPower
{
    [Tooltip("exclusively applies to close checks")]
    public bool AppliesToCloseChecks;
    [Tooltip("exclusively applies to combat checks")]
    public bool AppliesToCombatChecks;
    [Tooltip("exclusively applies to recharge checks")]
    public bool AppliesToRechargeChecks;
    [Tooltip("the card must have this trait")]
    public TraitType RequiredTrait;
    [Tooltip("the card must have this type")]
    public CardType RequiredType;

    public bool Resolve(Card card)
    {
        if (this.AppliesToRechargeChecks && !Rules.IsRechargeCheck())
        {
            return false;
        }
        if (this.AppliesToCombatChecks && !Rules.IsCombatCheck())
        {
            return false;
        }
        if (this.AppliesToCloseChecks && !Rules.IsCloseCheck())
        {
            return false;
        }
        if ((this.RequiredType != CardType.None) && (card.Type != this.RequiredType))
        {
            return false;
        }
        if ((this.RequiredTrait != TraitType.None) && !card.HasTrait(this.RequiredTrait))
        {
            return false;
        }
        return true;
    }
}

