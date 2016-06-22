using System;
using UnityEngine;

public class CardPowerReduceCombatDamage : CardPower
{
    [Tooltip("all damage is reduced when set")]
    public bool All;
    [Tooltip("the amount of damage to be reduced")]
    public int Amount;
    [Tooltip("reduce all types of damage (default is just combat)")]
    public bool AnyType;
    [Tooltip("modifies this power")]
    public PowerModifier Mod;
    [Tooltip("this card stacks with items of the same type (shields, spells)")]
    public bool Stacking;
    [Tooltip("only damage of these types can be reduced")]
    public TraitType[] Traits = new TraitType[] { TraitType.Combat };

    public override void Activate(Card card)
    {
        if (Rules.IsDamageReductionPossible())
        {
            this.ActivateArmour(card);
            Turn.Damage -= this.GetAmount();
            UI.Sound.Play(SoundEffectType.ArmorDamageReduction);
            if (this.Mod != null)
            {
                for (int i = 0; i < card.Powers.Length; i++)
                {
                    if (card.Powers[i] == this)
                    {
                        this.Mod.Activate(i);
                        break;
                    }
                }
            }
            base.Activate(card);
        }
    }

    private void ActivateArmour(Card card)
    {
        if (card.HasTrait(TraitType.LightArmor) || card.HasTrait(TraitType.HeavyArmor))
        {
            Turn.Armour = card.ID;
        }
        if (card.HasTrait(TraitType.Shield))
        {
            Turn.Shield = card.ID;
        }
        if (card.Type == CardType.Spell)
        {
            Turn.Spell = card.ID;
        }
    }

    public override void Deactivate(Card card)
    {
        if (Rules.IsDamageReductionPossible())
        {
            this.DeactivateArmour(card);
            Turn.Damage += this.GetAmount();
            if (this.Mod != null)
            {
                this.Mod.Deactivate();
            }
        }
    }

    private void DeactivateArmour(Card card)
    {
        if (card.HasTrait(TraitType.LightArmor) || card.HasTrait(TraitType.HeavyArmor))
        {
            Turn.Armour = null;
        }
        if (card.HasTrait(TraitType.Shield))
        {
            Turn.Shield = null;
        }
        if (card.Type == CardType.Spell)
        {
            Turn.Spell = null;
        }
    }

    private int GetAmount()
    {
        if (this.All)
        {
            return 0x3e8;
        }
        if ((base.Action == ActionType.Discard) && (this.Amount > 0))
        {
            return (this.Amount - 1);
        }
        return this.Amount;
    }

    public override string GetCardDecoration(Card card)
    {
        if ((Turn.State == GameStateType.Damage) || (Turn.State == GameStateType.Ambush))
        {
            bool flag = this.IsPowerAllowed(card);
            if (!flag)
            {
                if (!Turn.DamageReduction)
                {
                    return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
                }
                if (card.HasTrait(TraitType.Shield) && (Turn.WeaponHands == 2))
                {
                    return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
                }
                if (!Rules.IsCombatDamage(Turn.DamageTraits) && !this.AnyType)
                {
                    return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
                }
            }
            if (flag)
            {
                if (base.Action == ActionType.Recharge)
                {
                    return "Blueprints/Gui/Vfx_Card_Notice_Recharge";
                }
                if ((((base.Action == ActionType.Discard) || (base.Action == ActionType.Bury)) || (base.Action == ActionType.Banish)) && (this.GetAmount() > 1))
                {
                    return "Blueprints/Gui/Vfx_Card_Notice_Armor";
                }
                if (base.Action == ActionType.Reveal)
                {
                    return "Blueprints/Gui/Vfx_Card_Notice_Armor";
                }
            }
        }
        if (((Turn.State == GameStateType.Discard) && this.IsPowerAllowed(card)) && (base.Action == ActionType.Recharge))
        {
            return "Blueprints/Gui/Vfx_Card_Notice_Recharge";
        }
        return null;
    }

    private bool IsDamageReductionPossible()
    {
        if (this.AnyType)
        {
            return true;
        }
        if (Turn.DamageTraits == null)
        {
            return true;
        }
        for (int i = 0; i < this.Traits.Length; i++)
        {
            for (int j = 0; j < Turn.DamageTraits.Count; j++)
            {
                if (this.Traits[i] == ((TraitType) Turn.DamageTraits[j]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool IsEqualOrBetter(CardPower x)
    {
        CardPowerReduceCombatDamage damage = x as CardPowerReduceCombatDamage;
        if (damage == null)
        {
            return false;
        }
        return (this.Amount >= damage.Amount);
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (((Turn.Spell != null) && !this.Stacking) && (card.Type == CardType.Spell))
        {
            return false;
        }
        if (((Turn.Armour != null) && !this.Stacking) && (card.HasTrait(TraitType.LightArmor) || card.HasTrait(TraitType.HeavyArmor)))
        {
            return false;
        }
        if (((Turn.Shield != null) && !this.Stacking) && card.HasTrait(TraitType.Shield))
        {
            return false;
        }
        if (card.HasTrait(TraitType.Shield) && (Turn.WeaponHands == 2))
        {
            return false;
        }
        if ((card.HasTrait(TraitType.Shield) && (Turn.Shield != null)) && !Turn.Character.ProficientWithLightArmors)
        {
            return false;
        }
        if ((card.HasTrait(TraitType.LightArmor) || card.HasTrait(TraitType.HeavyArmor)) && ((Turn.Shield != null) && !Turn.Character.ProficientWithLightArmors))
        {
            return false;
        }
        if ((card.HasTrait(TraitType.Shield) && (Turn.Armour != null)) && !Turn.Character.ProficientWithLightArmors)
        {
            return false;
        }
        if (!this.IsDamageReductionPossible())
        {
            return false;
        }
        if (!Turn.DamageReduction)
        {
            return false;
        }
        if ((base.Action != ActionType.Discard) && (Rules.GetCardsToDiscardCount() <= 0))
        {
            return false;
        }
        return Rules.IsDamageReductionPossible();
    }
}

