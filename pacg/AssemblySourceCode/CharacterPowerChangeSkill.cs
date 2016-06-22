using System;
using UnityEngine;

public class CharacterPowerChangeSkill : CharacterPower
{
    [Tooltip("the amount of penalty to pay")]
    public int Amount;
    [Tooltip("the duration of this new skill. If negative the duration is permanent and it doesn't become a visible effect.")]
    public int Duration = -1;
    [Tooltip("the new skills to gain")]
    public SkillValueType[] NewSkills;
    [Tooltip("Only activate this ability if it's this owner's original turn")]
    public bool OwnerTurnOnly;
    [Tooltip("penalty to pay to activate this power")]
    public ActionType Penalty;
    [Tooltip("add these traits to the check assuming it's not a permanent change of skill.")]
    public TraitType[] Traits;

    public override void Activate()
    {
        base.Activate();
        base.PowerBegin();
        Turn.MarkPowerActive(this, true);
        if (!this.IsFree())
        {
            Turn.PushReturnState();
            Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerChangeSkill_Cancel"));
            Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerChangeSkill_Activate"));
            Turn.SetStateData(new TurnStateData(this.Penalty, this.Amount));
            Turn.State = GameStateType.Power;
        }
        else
        {
            this.CharacterPowerChangeSkill_Activate();
        }
    }

    private void CharacterPowerChangeSkill_Activate()
    {
        if (this.Duration < 0)
        {
            this.SetNewSkills();
        }
        else
        {
            for (int i = 0; i < this.NewSkills.Length; i++)
            {
                Effect e = new EffectChangeSkill(Effect.GetEffectID(this), this.Duration, this.NewSkills[i].skill, this.NewSkills[i].rank, this.NewSkills[i].attribute);
                base.Character.ApplyEffect(e);
            }
            Turn.DamageTraits.AddRange(this.Traits);
        }
        if (!this.IsFree())
        {
            Turn.ReturnToReturnState();
        }
        this.ResetCheck();
    }

    private void CharacterPowerChangeSkill_Cancel()
    {
        this.Deactivate();
        Turn.ReturnToReturnState();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.PowerEnd();
        Turn.MarkPowerActive(this, false);
        if (this.Duration >= 0)
        {
            for (int i = 0; i < this.NewSkills.Length; i++)
            {
                base.Character.RemoveEffect(Effect.GetEffectID(this));
            }
            for (int j = 0; j < this.Traits.Length; j++)
            {
                Turn.DamageTraits.Remove(this.Traits[j]);
            }
        }
    }

    private bool IsFree() => 
        ((this.Amount == 0) && (this.Penalty == ActionType.None));

    public override bool IsValid()
    {
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if ((this.Penalty != ActionType.None) && (base.Character.GetNumberDiscardableCards() <= 0))
        {
            return false;
        }
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        int num = 0;
        for (int i = 0; i < this.NewSkills.Length; i++)
        {
            if (base.Character.GetSkillRank(this.NewSkills[i].skill) == this.NewSkills[i].rank)
            {
                num++;
            }
        }
        if (num == this.NewSkills.Length)
        {
            return false;
        }
        if ((Turn.Owner != base.Character) && this.OwnerTurnOnly)
        {
            return false;
        }
        return true;
    }

    private void ResetCheck()
    {
        if (Rules.IsCheck())
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Turn.CombatSkill = Turn.Character.GetBestSkillCheck(Turn.CheckParticipants.ToArray());
                if (Turn.CombatSkill == SkillCheckType.None)
                {
                    Turn.CombatSkill = Turn.Character.GetCombatSkill();
                }
                window.dicePanel.SetCheck(Turn.Card, Turn.Checks, Turn.Check);
            }
        }
    }

    private void SetNewSkills()
    {
        for (int i = 0; i < this.NewSkills.Length; i++)
        {
            base.Character.SetSkillRank(this.NewSkills[i].skill, this.NewSkills[i].rank);
        }
        for (int j = 0; j < this.NewSkills.Length; j++)
        {
            bool flag = false;
            for (int k = 0; k < base.Character.Skills.Length; k++)
            {
                if (base.Character.Skills[k].skill == this.NewSkills[j].skill)
                {
                    flag = true;
                    if (base.Character.Skills[k].rank < this.NewSkills[k].rank)
                    {
                        base.Character.Skills[k].rank = this.NewSkills[j].rank;
                    }
                }
            }
            if (!flag)
            {
                SkillValueType[] skills = base.Character.Skills;
                base.Character.Skills = new SkillValueType[base.Character.Skills.Length + 1];
                skills.CopyTo(base.Character.Skills, 0);
                base.Character.Skills[base.Character.Skills.Length - 1] = this.NewSkills[j];
            }
        }
    }

    public override bool Automatic =>
        (base.Passive && this.IsFree());
}

