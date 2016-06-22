using System;

public class EffectAttributeChange : Effect
{
    public EffectAttributeChange(string source, int duration, AttributeType attribute, DiceType diceType) : base(source, duration)
    {
        base.genericParameters = new int[2];
        this.Attribute = attribute;
        this.DiceType = diceType;
    }

    public override string GetDisplayText() => 
        (this.Attribute.ToText() + " Die Changed");

    public AttributeType Attribute
    {
        get => 
            ((AttributeType) base.genericParameters[0]);
        set
        {
            base.genericParameters[0] = (int) value;
        }
    }

    public DiceType DiceType
    {
        get => 
            ((DiceType) base.genericParameters[1]);
        set
        {
            base.genericParameters[1] = (int) value;
        }
    }

    public override EffectType Type =>
        EffectType.AttributeChange;
}

