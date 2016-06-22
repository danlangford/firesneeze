using System;

public class TutorialConditionCheckMany : TutorialCondition
{
    public override bool Evaluate()
    {
        if (((Turn.Checks != null) && (Turn.Checks.Length > 0)) && (Party.Characters.Count > 0))
        {
            DiceType skillDice = Turn.Owner.GetSkillDice(Turn.Checks[0].skill);
            for (int i = 1; i < Turn.Checks.Length; i++)
            {
                if (skillDice != Turn.Owner.GetSkillDice(Turn.Checks[i].skill))
                {
                    return true;
                }
            }
        }
        return false;
    }
}

