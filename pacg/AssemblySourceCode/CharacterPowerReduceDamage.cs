using System;
using UnityEngine;

public class CharacterPowerReduceDamage : CharacterPower
{
    [Tooltip("amount of damage to reduce")]
    public int Amount = 1;
    [Tooltip("which characters will be effected by this power")]
    public DamageRangeType Range;
    [Tooltip("damage of these types can be reduced")]
    public TraitType[] Traits;

    public override void Activate()
    {
        base.Activate();
        Turn.Damage -= this.Amount;
        Turn.CheckBoard.Set<bool>("GameStateRecharge_KeepLayout", false);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Turn.Damage += this.Amount;
    }

    public override void Initialize(Character self)
    {
        base.InitializeModifier(self, ref this.Traits);
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsDamageReductionPossible())
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (Turn.Damage <= 0)
        {
            return false;
        }
        if (!Rules.IsRangeValid(base.Character, this.Range))
        {
            return false;
        }
        if ((Turn.DamageTraits != null) && (this.Traits.Length > 0))
        {
            bool flag = false;
            for (int i = 0; i < this.Traits.Length; i++)
            {
                flag |= Turn.DamageTraits.Contains(this.Traits[i]);
                if (flag)
                {
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
        }
        return true;
    }

    public override bool Automatic =>
        base.Passive;
}

