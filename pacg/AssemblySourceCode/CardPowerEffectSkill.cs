using System;
using UnityEngine;

public class CardPowerEffectSkill : CardPower
{
    [Tooltip("the amount of bonus or penalty")]
    public int Amount = 1;
    [Tooltip("the number of turns that this effect lasts")]
    public int Duration = 1;
    [Tooltip("the skill that this effect applies to; NONE means any skill")]
    public SkillCheckType Skill;

    public override void Activate(Card card)
    {
        Effect e = new EffectModifySkill(card.ID, 1, this.Skill, this.Amount);
        Turn.Character.ApplyEffect(e);
        this.RefreshDicePanel();
    }

    public override void Deactivate(Card card)
    {
        Effect e = new EffectModifySkill(card.ID, 1, this.Skill, this.Amount);
        Turn.Character.RemoveEffect(e);
        this.RefreshDicePanel();
    }

    public override bool IsEqualOrBetter(CardPower x)
    {
        CardPowerEffectSkill skill = x as CardPowerEffectSkill;
        if (skill == null)
        {
            return false;
        }
        if (this.Skill != skill.Skill)
        {
            return false;
        }
        return (this.Amount >= skill.Amount);
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
        if (((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup)) && ((Turn.State != GameStateType.Combat) && (Turn.State != GameStateType.Recharge)))
        {
            return false;
        }
        return true;
    }

    protected void RefreshDicePanel()
    {
        int num = Turn.Owner.GetSkillBonus(Turn.Check) - Turn.LastCheckBonus;
        Turn.DiceBonus += num;
        Turn.LastCheckBonus += num;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }
}

