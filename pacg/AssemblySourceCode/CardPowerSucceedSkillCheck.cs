using System;
using UnityEngine;

public class CardPowerSucceedSkillCheck : CardPower
{
    [Tooltip("the types of skill checks where this card works")]
    public SkillCheckType[] Checks;
    [Tooltip("combat or non-combat")]
    public SkillCheckSituationType Situation;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            base.SetupTurnSkillCheck(this.Situation, this.Checks);
            if (((Turn.State == GameStateType.Combat) || (Turn.State == GameStateType.Close)) || ((Turn.State == GameStateType.Roll) || ((Turn.State == GameStateType.Recharge) && (Turn.Dice.Count > 0))))
            {
                base.ShowDice(false);
                Turn.Defeat = true;
                Turn.Character.MarkCardType(card.Type, true);
                base.ShowProceedButton(true);
            }
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            base.ShowDice(true);
            Turn.Defeat = false;
            Turn.Character.MarkCardType(card.Type, false);
            base.ShowProceedButton(false);
        }
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!this.IsPowerCheck(card))
        {
            return false;
        }
        if ((Turn.Card != null) && (Turn.Checks != null))
        {
            for (int i = 0; i < this.Checks.Length; i++)
            {
                for (int j = 0; j < Turn.Checks.Length; j++)
                {
                    if (this.Checks[i] == Turn.Checks[j].skill)
                    {
                        return true;
                    }
                }
            }
        }
        return (this.Checks.Length == 0);
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
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if (!Rules.IsDiceRollPossible())
        {
            return false;
        }
        if ((this.Situation == SkillCheckSituationType.NonCombat) && Rules.IsCombatCheck())
        {
            return false;
        }
        if ((this.Situation == SkillCheckSituationType.Combat) && Rules.IsNonCombatCheck())
        {
            return false;
        }
        if (((this.Situation == SkillCheckSituationType.Any) && !Rules.IsCombatCheck()) && !Rules.IsNonCombatCheck())
        {
            return false;
        }
        return true;
    }

    protected override bool IsPowerValid(Card card)
    {
        if (this.IsPowerCheck(card))
        {
            for (int i = 0; i < this.Checks.Length; i++)
            {
                if (this.Checks[i] == Turn.Check)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

