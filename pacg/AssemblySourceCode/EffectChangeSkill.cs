using System;

public class EffectChangeSkill : Effect
{
    public EffectChangeSkill(string source, int duration, SkillType skill, int amount, AttributeType attribute) : base(source, duration)
    {
        base.genericParameters = new int[3];
        this.Skill = skill;
        this.Amount = amount;
        this.Attribute = attribute;
    }

    public override string GetDisplayText() => 
        (this.Skill.ToText() + ": " + this.Attribute.ToText() + base.ConvertBonusToText(this.Amount));

    public int Amount
    {
        get => 
            base.genericParameters[1];
        set
        {
            base.genericParameters[1] = value;
        }
    }

    public AttributeType Attribute
    {
        get => 
            ((AttributeType) base.genericParameters[2]);
        set
        {
            base.genericParameters[2] = (int) value;
        }
    }

    public SkillType Skill
    {
        get => 
            ((SkillType) base.genericParameters[0]);
        set
        {
            base.genericParameters[0] = (int) value;
        }
    }

    public override EffectType Type =>
        EffectType.SkillChange;
}

