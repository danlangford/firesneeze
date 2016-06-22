using System;
using UnityEngine;

public class CardPowerReduceDamage : CardPower
{
    [Tooltip("all damage is reduced when set")]
    public bool All;
    [Tooltip("the amount of damage to be reduced")]
    public int Amount;
    [Tooltip("reduce all types of damage")]
    public bool AnyType;
    [Tooltip("this card stacks with items of the same type (shields, spells)")]
    public bool Stacking;
    [Tooltip("only damage of these types can be reduced")]
    public TraitType[] Traits;

    public override void Activate(Card card)
    {
        if (Rules.IsDamageReductionPossible())
        {
            Turn.Damage -= this.GetAmount();
            Turn.Character.MarkCardType(card.Type, true);
            UI.Sound.Play(SoundEffectType.ArmorDamageReduction);
            base.Activate(card);
        }
    }

    public override void Deactivate(Card card)
    {
        if (Rules.IsDamageReductionPossible())
        {
            Turn.Damage += this.GetAmount();
            Turn.Character.MarkCardType(card.Type, false);
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
        if (((Turn.State == GameStateType.Damage) || (Turn.State == GameStateType.Ambush)) && !this.IsPowerAllowed(Turn.Card))
        {
            return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
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
        CardPowerReduceDamage damage = x as CardPowerReduceDamage;
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
        if (Turn.Character.IsCardTypeMarked(card.Type) && !this.Stacking)
        {
            return false;
        }
        if (!Turn.DamageReduction)
        {
            return false;
        }
        if (!this.IsDamageReductionPossible())
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

