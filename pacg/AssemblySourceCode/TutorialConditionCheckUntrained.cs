using System;

public class TutorialConditionCheckUntrained : TutorialCondition
{
    public override bool Evaluate()
    {
        if ((Party.Characters.Count > 0) && (Turn.Check != SkillCheckType.None))
        {
            if (Turn.Check == SkillCheckType.Strength)
            {
                return false;
            }
            if (Turn.Check == SkillCheckType.Dexterity)
            {
                return false;
            }
            if (Turn.Check == SkillCheckType.Constitution)
            {
                return false;
            }
            if (Turn.Check == SkillCheckType.Charisma)
            {
                return false;
            }
            if (Turn.Check == SkillCheckType.Intelligence)
            {
                return false;
            }
            if (Turn.Check == SkillCheckType.Wisdom)
            {
                return false;
            }
            if (Turn.Check == SkillCheckType.Combat)
            {
                return false;
            }
            for (int i = 0; i < Turn.Owner.Skills.Length; i++)
            {
                if (Turn.Owner.Skills[i].skill == Turn.Check.ToSkillType())
                {
                    return false;
                }
            }
            if (Turn.Owner.GetSkillDice(Turn.Check) == DiceType.D4)
            {
                return true;
            }
        }
        return false;
    }
}

