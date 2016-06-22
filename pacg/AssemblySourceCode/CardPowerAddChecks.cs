using System;
using UnityEngine;

public class CardPowerAddChecks : CardPower
{
    [Tooltip("list of checks that will be added to the turn check")]
    public SkillCheckValueType[] Checks;
    [Tooltip("use the existing check to determine the difficutly")]
    public bool UseExistingCheck = true;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Character.MarkCardType(card.Type, true);
            if (this.UseExistingCheck)
            {
                for (int i = 0; i < this.Checks.Length; i++)
                {
                    this.Checks[i].rank = Turn.Card.GetCurrentLowestCheck();
                }
            }
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                SkillCheckValueType[] checks = this.Checks;
                SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(this.Checks);
                window.dicePanel.SetCheck(Turn.Card, checks, bestSkillCheck.skill);
            }
            base.Activate(card);
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            Turn.Character.MarkCardType(card.Type, false);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(Turn.Card.Checks);
                window.dicePanel.SetCheck(Turn.Card, Turn.Card.Checks, bestSkillCheck.skill);
            }
        }
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
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if (!Rules.IsDiceRollPossible())
        {
            return false;
        }
        if (Turn.State != GameStateType.Combat)
        {
            return false;
        }
        return true;
    }

    public override bool IsPowerDeactivationAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Turn.State != GameStateType.Combat)
        {
            return false;
        }
        return true;
    }

    protected override bool IsPowerValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (Turn.State != GameStateType.Combat)
        {
            return false;
        }
        return true;
    }
}

