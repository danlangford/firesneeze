using System;
using UnityEngine;

public class CharacterPowerReplaceChecks : CharacterPower
{
    [Tooltip("list of checks that will be added to the turn check")]
    public SkillCheckValueType[] Checks;
    [Tooltip("if true replace a none check with yourself")]
    public bool ReplaceEmptyCheck;
    [Tooltip("use the existing check to determine the difficulty")]
    public bool UseExistingCheck = true;

    public override void Activate()
    {
        base.Activate();
        this.SetupCheckArray();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            SkillCheckValueType bestSkillCheck = base.Character.GetBestSkillCheck(this.Checks);
            window.dicePanel.SetCheck(Turn.Card, this.Checks, bestSkillCheck.skill);
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.ResetCheck();
    }

    public override bool IsLegalActivation() => 
        this.IsPowerValid();

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        return true;
    }

    public override void OnCheckCompleted()
    {
        this.PowerEnd();
    }

    private void ResetCheck()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Turn.State == GameStateType.Recharge)
            {
                window.dicePanel.SetCheck(Turn.Card, Turn.Card.Recharge, Turn.Character.GetBestSkillCheck(Turn.Card.Recharge).skill);
            }
            else
            {
                window.dicePanel.SetCheck(Turn.Card, Turn.Card.Checks, Turn.Character.GetBestSkillCheck(Turn.Card.Checks).skill);
            }
        }
    }

    private void SetupCheckArray()
    {
        if (this.UseExistingCheck)
        {
            SkillCheckValueType[] recharge;
            if (Turn.State == GameStateType.Recharge)
            {
                recharge = Turn.Card.Recharge;
            }
            else
            {
                recharge = Turn.Card.Checks;
            }
            int b = (recharge.Length <= 0) ? 0 : recharge[0].Rank;
            for (int i = 0; i < recharge.Length; i++)
            {
                b = Mathf.Min(Rules.GetCheckValue(Turn.Card, recharge[i].skill), b);
            }
            for (int j = 0; j < this.Checks.Length; j++)
            {
                this.Checks[j].rank = b;
            }
        }
    }
}

