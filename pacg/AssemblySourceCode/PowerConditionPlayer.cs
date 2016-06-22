using System;

public class PowerConditionPlayer : PowerCondition
{
    public ClassType Class;
    public GenderType Gender;
    public RaceType Race;
    public SkillType Skill;

    public override bool Evaluate(Card card)
    {
        if ((this.Gender != GenderType.None) && (Turn.Character.Gender != this.Gender))
        {
            return false;
        }
        if ((this.Race != RaceType.None) && (Turn.Character.Race != this.Race))
        {
            return false;
        }
        if ((this.Class != ClassType.None) && (Turn.Character.Class != this.Class))
        {
            return false;
        }
        if ((this.Skill != SkillType.None) && (Turn.Character.GetSkillRank(this.Skill) <= 0))
        {
            return false;
        }
        return true;
    }
}

