using System;
using System.Runtime.CompilerServices;

[Serializable]
public class SkillCheckValueType
{
    public int rank;
    public SkillCheckType skill;

    public SkillCheckValueType(SkillCheckType skill, int rank)
    {
        this.skill = skill;
        this.rank = rank;
    }

    public int Bonus { get; set; }

    public int Rank =>
        (this.rank + this.Bonus);
}

