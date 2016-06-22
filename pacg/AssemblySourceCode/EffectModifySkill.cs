using System;

public class EffectModifySkill : Effect
{
    public EffectModifySkill(string source, int duration, SkillCheckType skill, int amount) : base(source, duration)
    {
        this.Skill = skill;
        this.Amount = amount;
    }

    public override bool Equals(object obj)
    {
        if (obj != null)
        {
            EffectModifySkill skill = obj as EffectModifySkill;
            if (skill != null)
            {
                return ((skill.Skill == this.Skill) && (skill.Amount == this.Amount));
            }
        }
        return false;
    }

    public override string GetDisplayText()
    {
        if (this.Skill == SkillCheckType.None)
        {
            return ("All Skills: " + base.ConvertBonusToText(this.Amount));
        }
        return (this.Skill.ToText() + ": " + base.ConvertBonusToText(this.Amount));
    }

    public override int GetHashCode() => 
        ((base.GetHashCode() ^ this.Skill.GetHashCode()) ^ this.Amount.GetHashCode());

    public int GetSkillModifier(SkillCheckType check)
    {
        if (this.Skill == SkillCheckType.None)
        {
            return this.Amount;
        }
        if (this.Skill == check)
        {
            return this.Amount;
        }
        return 0;
    }

    private int Amount
    {
        get => 
            base.genericParameters[1];
        set
        {
            base.genericParameters[1] = value;
        }
    }

    private SkillCheckType Skill
    {
        get => 
            ((SkillCheckType) base.genericParameters[0]);
        set
        {
            base.genericParameters[0] = (int) value;
        }
    }

    public override EffectType Type =>
        EffectType.ModifySkill;
}

