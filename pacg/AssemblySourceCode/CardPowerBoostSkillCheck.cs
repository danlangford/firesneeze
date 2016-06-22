using System;
using UnityEngine;

public class CardPowerBoostSkillCheck : CardPower
{
    [Tooltip("the types of skill checks where this card works")]
    public SkillCheckType[] Checks;
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("combat or non-combat")]
    public SkillCheckSituationType Situation;
    [Tooltip("should this power trigger/care about how many of this type are played before/after?")]
    public bool Stacking;
    [Tooltip("optional traits to add when this power is used")]
    public TraitType[] Traits;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            base.SetupTurnSkillCheck(this.Situation, this.Checks);
            for (int i = 0; i < this.Dice.Length; i++)
            {
                DiceType item = Rules.GetModifiedDice(Party.Find(card.PlayedOwner), card, this.Dice[i]);
                if (item == DiceType.D0)
                {
                    Turn.BonusCheckDice++;
                }
                else
                {
                    Turn.Dice.Add(item);
                }
            }
            Turn.DiceBonus += this.DiceBonus;
            this.AddTraits();
            if (!this.Stacking)
            {
                Turn.Character.MarkCardType(card.Type, true);
            }
            this.ResetDicePanel();
            this.RefreshDicePanel();
            base.Activate(card);
        }
    }

    private void AddTraits()
    {
        Turn.AddTraits(this.Traits);
        Rules.ApplyCombatAdjustments();
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            for (int i = 0; i < this.Dice.Length; i++)
            {
                DiceType item = Rules.GetModifiedDice(Party.Find(card.PlayedOwner), card, this.Dice[i]);
                if (item == DiceType.D0)
                {
                    Turn.BonusCheckDice--;
                }
                else
                {
                    Turn.Dice.Remove(item);
                }
            }
            Turn.DiceBonus -= this.DiceBonus;
            this.RemoveTraits();
            if (!this.Stacking)
            {
                Turn.Character.MarkCardType(card.Type, false);
            }
            if (!Rules.IsCombatCardActive())
            {
                Turn.CombatSkill = Turn.Owner.GetCombatSkill();
            }
            this.ResetDicePanel();
            this.RefreshDicePanel();
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

    public override bool IsEqualOrBetter(CardPower x)
    {
        CardPowerBoostSkillCheck check = x as CardPowerBoostSkillCheck;
        if (check == null)
        {
            return false;
        }
        return (this.Dice.Length >= check.Dice.Length);
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (this.IsPowerCheck(card))
        {
            if (Turn.Character.IsCardTypeMarked(card.Type) && !this.Stacking)
            {
                return false;
            }
            if (card.IsPowerAlreadyActivated(this))
            {
                return false;
            }
            bool allChecks = !Rules.IsUnlimitedPlayPossible(card.Type);
            if (Turn.Checks != null)
            {
                for (int i = 0; i < this.Checks.Length; i++)
                {
                    switch (this.Situation)
                    {
                        case SkillCheckSituationType.Any:
                            if (!Rules.IsSkillParticipatingInCheck(this.Checks[i], allChecks))
                            {
                                break;
                            }
                            return true;

                        case SkillCheckSituationType.Combat:
                            if (!Rules.IsCombatSkillParticipating(this.Checks[i], allChecks))
                            {
                                break;
                            }
                            return true;

                        case SkillCheckSituationType.NonCombat:
                            if (!Rules.IsNonCombatSkillParticipating(this.Checks[i], allChecks))
                            {
                                break;
                            }
                            return true;
                    }
                }
            }
            if ((Turn.Card != null) && (Turn.Checks != null))
            {
                for (int j = 0; j < this.Checks.Length; j++)
                {
                    for (int k = 0; k < Turn.Checks.Length; k++)
                    {
                        if ((this.Checks[j] == SkillCheckType.Ranged) && Turn.WeaponRanged)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
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
        if (!Rules.IsCombatCheck() && !Rules.IsNonCombatCheck())
        {
            return false;
        }
        if (((this.Situation == SkillCheckSituationType.Any) && !Rules.IsCombatCheck()) && !Rules.IsNonCombatCheck())
        {
            return false;
        }
        if (card.HasTrait(TraitType.Shield) && (Turn.WeaponHands == 2))
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
        if (Turn.Checks != null)
        {
            for (int j = 0; j < this.Checks.Length; j++)
            {
                if (Rules.IsSkillParticipatingInCheck(this.Checks[j], false))
                {
                    return true;
                }
            }
        }
        return true;
    }

    public override bool IsPowerDeactivationAllowed(Card card)
    {
        if (!Rules.IsCombatCheck() && !Rules.IsNonCombatCheck())
        {
            return false;
        }
        return base.IsPowerDeactivationAllowed(card);
    }

    protected override bool IsPowerValid(Card card)
    {
        if (this.IsPowerCheck(card))
        {
            if (((this.Situation == SkillCheckSituationType.NonCombat) && Rules.IsCombatCheck()) || ((this.Situation == SkillCheckSituationType.NonCombat) && !Rules.IsNonCombatCheck()))
            {
                return false;
            }
            if (((this.Situation == SkillCheckSituationType.Combat) && Rules.IsNonCombatCheck()) || ((this.Situation == SkillCheckSituationType.Combat) && !Rules.IsCombatCheck()))
            {
                return false;
            }
            if (Turn.Checks != null)
            {
                for (int j = 0; j < this.Checks.Length; j++)
                {
                    if (Rules.IsSkillParticipatingInCheck(this.Checks[j], false))
                    {
                        return true;
                    }
                }
            }
            for (int i = 0; i < this.Checks.Length; i++)
            {
                if (this.Checks[i] == Turn.Check)
                {
                    return true;
                }
                if ((this.Checks[i] == SkillCheckType.Ranged) && Turn.WeaponRanged)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void RefreshDicePanel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    private void RemoveTraits()
    {
        for (int i = 0; i < this.Traits.Length; i++)
        {
            Turn.DamageTraits.Remove(this.Traits[i]);
        }
        Rules.ApplyCombatAdjustments();
    }

    private void ResetDicePanel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (((window != null) && (Turn.Check != SkillCheckType.None)) && (Turn.Checks != null))
        {
            window.dicePanel.SetCheck(null, Turn.Checks, Turn.Check);
        }
    }
}

