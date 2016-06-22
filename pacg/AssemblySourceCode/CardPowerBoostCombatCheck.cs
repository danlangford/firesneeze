using System;
using UnityEngine;

public class CardPowerBoostCombatCheck : CardPower
{
    [Tooltip("if true then weapons cannot be played with this card")]
    public CombatModeType CombatType;
    [Tooltip("range of this power")]
    public DamageRangeType DamageRange;
    [Tooltip("regular dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("attribute dice added to the check (usually none, but see longbow)")]
    public AttributeType[] DiceAttributes;
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("used to attach modifiers to this power to add additional effects")]
    public PowerModifier[] PowerModifiers;
    [Tooltip("type of skill: strength, dexterity, arcane, wisdom")]
    public SkillCheckType[] Skills = new SkillCheckType[] { SkillCheckType.Strength };
    [Tooltip("should this power allow multiple weapons to be played?")]
    public bool Stacking;
    [Tooltip("type of damage done by this power")]
    public TraitType[] Traits = new TraitType[] { TraitType.Melee };

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            if (card.Type == CardType.Spell)
            {
                this.ActivateSpell(card);
            }
            else if (card.Type == CardType.Item)
            {
                this.ActivateItem(card);
            }
            else
            {
                this.ActivateWeapon(card);
            }
            if (this.PowerModifiers.Length > 0)
            {
                for (int k = 0; k < card.Powers.Length; k++)
                {
                    if (card.Powers[k] == this)
                    {
                        for (int m = 0; m < this.PowerModifiers.Length; m++)
                        {
                            this.PowerModifiers[m].Activate(k);
                        }
                        break;
                    }
                }
            }
            for (int i = 0; i < this.Dice.Length; i++)
            {
                Turn.Dice.Add(this.Dice[i]);
            }
            for (int j = 0; j < this.DiceAttributes.Length; j++)
            {
                Turn.Dice.Add(Turn.Character.GetAttributeDice(this.DiceAttributes[j]));
            }
            Turn.DiceBonus += this.DiceBonus;
            this.ResetDicePanel(SkillCheckType.Combat);
            Rules.ApplyCombatAdjustments();
            this.RefreshDicePanel();
        }
    }

    private void ActivateItem(Card card)
    {
        if (Rules.IsTurnOwner())
        {
            Turn.Character.MarkCardType(CardType.Item, true);
            if (this.CombatType == CombatModeType.Weapon)
            {
                Turn.Weapon1 = "Unarmed";
            }
            else
            {
                Turn.CombatSkill = Turn.Character.GetBestSkillCheck(this.Skills);
            }
            Turn.Item = card.ID;
            this.AddTraits(card);
        }
    }

    private void ActivateSpell(Card card)
    {
        if (Rules.IsTurnOwner())
        {
            Turn.Character.MarkCardType(CardType.Spell, true);
            Turn.Spell = card.ID;
            Turn.CheckParticipants.AddRange(this.Skills);
            this.AddTraits(card);
            Turn.CombatSkill = Turn.Character.GetBestSkillCheck(this.Skills);
        }
    }

    private void ActivateWeapon(Card card)
    {
        if (Rules.IsTurnOwner())
        {
            if (Turn.Weapon1 == null)
            {
                Turn.Weapon1 = card.ID;
                Turn.WeaponHands = this.GetWeaponHands(card);
                Turn.WeaponRanged = Rules.IsWeaponRanged(card);
                Turn.Reroll = Rules.GetRerollCardGuid(card);
                Turn.DiceTargetAdjustment += Rules.GetWeaponProficiencyAdjustment(card);
                this.AddTraits(card);
                Turn.CheckParticipants.AddRange(this.Skills);
                Turn.CombatSkill = Turn.Character.GetBestSkillCheck(this.Skills);
            }
            else
            {
                Turn.Weapon2 = card.ID;
            }
        }
    }

    private void AddTraits(Card card)
    {
        Turn.AddTraits(this.Traits);
        Turn.AddTraits(card.Traits);
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            for (int i = 0; i < this.PowerModifiers.Length; i++)
            {
                this.PowerModifiers[i].Deactivate();
            }
            if (card.Type == CardType.Spell)
            {
                this.DeactivateSpell(card);
            }
            else if (card.Type == CardType.Item)
            {
                this.DeactivateItem(card);
            }
            else
            {
                this.DeactivateWeapon(card);
            }
            for (int j = 0; j < this.Dice.Length; j++)
            {
                Turn.Dice.Remove(this.Dice[j]);
            }
            for (int k = 0; k < this.DiceAttributes.Length; k++)
            {
                Turn.Dice.Remove(Turn.Character.GetAttributeDice(this.DiceAttributes[k]));
            }
            Turn.DiceBonus -= this.DiceBonus;
            Rules.ApplyCombatAdjustments();
            this.RefreshDicePanel();
        }
    }

    private void DeactivateItem(Card card)
    {
        if (Rules.IsTurnOwner())
        {
            Turn.Character.MarkCardType(CardType.Item, false);
            if (this.CombatType == CombatModeType.Weapon)
            {
                if (!Turn.WeaponUnarmed)
                {
                    Turn.Weapon1 = null;
                }
            }
            else
            {
                Turn.CombatSkill = Turn.Character.GetCombatSkill();
            }
            Turn.Item = null;
            this.RemoveTraits(card);
            this.ResetDicePanel();
        }
    }

    private void DeactivateSpell(Card card)
    {
        if (Rules.IsTurnOwner())
        {
            Turn.Character.MarkCardType(CardType.Spell, false);
            Turn.Spell = null;
            this.RemoveTraits(card);
            for (int i = 0; i < this.Skills.Length; i++)
            {
                Turn.CheckParticipants.Remove(this.Skills[i]);
            }
            Turn.CombatSkill = Turn.Character.GetCombatSkill();
            this.ResetDicePanel();
        }
    }

    private void DeactivateWeapon(Card card)
    {
        if (Rules.IsTurnOwner())
        {
            if (Turn.Weapon1 == card.ID)
            {
                Turn.Weapon1 = null;
                Turn.WeaponRanged = false;
                Turn.WeaponHands = 1;
                Turn.Reroll = Guid.Empty;
                Turn.DiceTargetAdjustment -= Rules.GetWeaponProficiencyAdjustment(card);
                this.RemoveTraits(card);
                for (int i = 0; i < this.Skills.Length; i++)
                {
                    Turn.CheckParticipants.Remove(this.Skills[i]);
                }
                Turn.CombatSkill = Turn.Character.GetCombatSkill();
                this.ResetDicePanel();
            }
            else
            {
                Turn.Weapon2 = null;
            }
        }
    }

    public override string GetCardDecoration(Card card)
    {
        if (Turn.State == GameStateType.Combat)
        {
            for (int i = 0; i < this.Traits.Length; i++)
            {
                if (Rules.IsImmune(Turn.Card, this.Traits[i]))
                {
                    return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
                }
            }
        }
        return null;
    }

    private int GetWeaponHands(Card card)
    {
        if (Rules.IsWeaponTwoHanded(card))
        {
            return 2;
        }
        return 1;
    }

    public override bool IsEqualOrBetter(CardPower x)
    {
        CardPowerBoostCombatCheck check = x as CardPowerBoostCombatCheck;
        if (check == null)
        {
            return false;
        }
        return (this.Dice.Length >= check.Dice.Length);
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!this.IsPowerCheck(card))
        {
            return false;
        }
        if (!Rules.IsCombatPossible())
        {
            return false;
        }
        if (base.IsCardInPlay(card))
        {
            return false;
        }
        if (card.Type == CardType.Item)
        {
            if (Turn.Item != null)
            {
                return false;
            }
            if (((this.CombatType == CombatModeType.Weapon) && !string.IsNullOrEmpty(Turn.Weapon1)) && (Turn.Weapon1 != "Unarmed"))
            {
                return false;
            }
            if (this.CombatType == CombatModeType.Spell)
            {
                if (!string.IsNullOrEmpty(Turn.Spell))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(Turn.Weapon1))
                {
                    return false;
                }
            }
            if (Turn.Character.IsCardTypeMarked(CardType.Item))
            {
                return false;
            }
        }
        if (card.Type == CardType.Spell)
        {
            if (Turn.Spell != null)
            {
                return false;
            }
            if (Turn.Character.IsCardTypeMarked(CardType.Spell))
            {
                return false;
            }
        }
        if (card.Type == CardType.Spell)
        {
            if ((Turn.Weapon1 != null) || (Turn.Weapon2 != null))
            {
                return false;
            }
            if (Turn.Item != null)
            {
                return false;
            }
        }
        if ((card.Type == CardType.Spell) && Rules.IsImmune(Turn.Card, card))
        {
            return false;
        }
        if (card.Type == CardType.Weapon)
        {
            if (Turn.Spell != null)
            {
                return false;
            }
            if (Turn.Item != null)
            {
                return false;
            }
        }
        if (!Rules.IsTurnOwner())
        {
            if (Party.Characters[Turn.Current].Location != Party.Characters[Turn.Number].Location)
            {
                if (Rules.IsWeaponOffHand(card))
                {
                    return true;
                }
                if (this.DamageRange == DamageRangeType.Short)
                {
                    return false;
                }
                if (this.DamageRange == DamageRangeType.Self)
                {
                    return false;
                }
            }
            else if (this.DamageRange == DamageRangeType.Long)
            {
                return false;
            }
        }
        if ((Rules.IsTurnOwner() && (card.Type == CardType.Weapon)) && (Turn.Weapon1 != null))
        {
            if ((Turn.Weapon2 != null) && !this.Stacking)
            {
                return false;
            }
            if (!Rules.IsWeaponOffHand(card))
            {
                return false;
            }
            if (base.Action != ActionType.Discard)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPowerCheck(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (!Rules.IsDiceRollPossible())
        {
            return false;
        }
        for (int i = 0; i < this.Traits.Length; i++)
        {
            if (Rules.IsImmune(Turn.Card, this.Traits[i]))
            {
                return false;
            }
        }
        return true;
    }

    public override bool IsPowerDeactivationAllowed(Card card)
    {
        if (Turn.State == GameStateType.Power)
        {
            TurnStateCallback callback = Turn.PeekStateDestination();
            if (callback != null)
            {
                return (callback.CallbackCardID == card.ID);
            }
        }
        return (Turn.State == GameStateType.Combat);
    }

    protected override bool IsPowerValid(Card card)
    {
        if (!this.IsPowerCheck(card))
        {
            return false;
        }
        if (Turn.State != GameStateType.Combat)
        {
            return false;
        }
        return (Turn.Check == SkillCheckType.Combat);
    }

    public override bool IsValidationRequired()
    {
        if (!base.IsValid(base.Card))
        {
            return true;
        }
        for (int i = 0; i < this.PowerModifiers.Length; i++)
        {
            if (!this.PowerModifiers[i].IsValidationRequired())
            {
                return false;
            }
        }
        return base.IsValidationRequired();
    }

    private void RefreshDicePanel()
    {
        Turn.DiceTarget = Rules.GetCheckValue(Turn.Card, Turn.Check);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.SetTitle(Turn.Card, Turn.Check, Turn.DiceTarget);
            window.dicePanel.Refresh();
        }
    }

    private void RemoveTraits(Card card)
    {
        Turn.RemoveTraits(this.Traits);
        Turn.RemoveTraits(card.Traits);
    }

    private void ResetDicePanel()
    {
        this.ResetDicePanel(Turn.Check);
    }

    private void ResetDicePanel(SkillCheckType newCheck)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.SetCheck(Turn.Card, Turn.Card.Checks, newCheck);
        }
    }

    public override ActionType RechargeAction
    {
        get
        {
            if (base.Action == ActionType.Reveal)
            {
                return ActionType.Discard;
            }
            return base.RechargeAction;
        }
    }
}

