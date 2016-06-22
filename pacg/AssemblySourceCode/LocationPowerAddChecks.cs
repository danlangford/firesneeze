using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationPowerAddChecks : LocationPower
{
    [Tooltip("if true the check will be added together instead of replacing")]
    public bool Add = true;
    [Tooltip("list of checks that will be added to the turn check")]
    public SkillCheckValueType[] Checks;
    [Tooltip("specifies which cards this power can be used against")]
    public CardSelector Selector;

    public override void Activate()
    {
        base.Activate();
        base.PowerBegin();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            SkillCheckValueType[] checks = this.Checks;
            if (this.Add)
            {
                checks = this.Merge(Turn.Checks, this.Checks);
            }
            SkillCheckValueType bestSkillCheck = Turn.Owner.GetBestSkillCheck(checks);
            window.dicePanel.SetCheck(Turn.Card, checks, bestSkillCheck.skill);
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            SkillCheckValueType bestSkillCheck = Turn.Owner.GetBestSkillCheck(Turn.Card.Checks);
            window.dicePanel.SetCheck(Turn.Card, Turn.Card.Checks, bestSkillCheck.skill);
        }
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (Turn.State != GameStateType.Combat)
        {
            return false;
        }
        if ((this.Selector != null) && !this.Selector.Match(Turn.Card))
        {
            return false;
        }
        return true;
    }

    private SkillCheckValueType[] Merge(SkillCheckValueType[] a1, SkillCheckValueType[] a2)
    {
        List<SkillCheckValueType> list = new List<SkillCheckValueType>(a1);
        for (int i = 0; i < a2.Length; i++)
        {
            bool flag = false;
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].skill == a2[i].skill)
                {
                    flag = true;
                    list[j].rank = Mathf.Min(list[j].Rank, a2[i].Rank);
                    break;
                }
            }
            if (!flag)
            {
                list.Add(a2[i]);
            }
        }
        return list.ToArray();
    }

    public override void OnDiceRolled()
    {
        this.PowerEnd();
    }
}

