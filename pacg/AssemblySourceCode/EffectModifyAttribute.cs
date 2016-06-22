using System;

public class EffectModifyAttribute : Effect
{
    public EffectModifyAttribute(string source, int duration, AttributeType attribute, int amount) : base(source, duration)
    {
        base.genericParameters = new int[2];
        this.Attribute = attribute;
        this.Amount = amount;
    }

    public override bool Equals(object obj)
    {
        if (obj != null)
        {
            EffectModifyAttribute attribute = obj as EffectModifyAttribute;
            if (attribute != null)
            {
                return ((attribute.Attribute == this.Attribute) && (attribute.Amount == this.Amount));
            }
        }
        return false;
    }

    public int GetAttributeModifier(AttributeType attribute)
    {
        if (this.Attribute == attribute)
        {
            return this.Amount;
        }
        return 0;
    }

    public override string GetDisplayText() => 
        (this.Attribute.ToText() + ": " + base.ConvertBonusToText(this.Amount));

    public override int GetHashCode() => 
        ((base.GetHashCode() ^ this.Attribute.GetHashCode()) ^ this.Amount.GetHashCode());

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
            ((AttributeType) base.genericParameters[0]);
        set
        {
            base.genericParameters[0] = (int) value;
        }
    }

    public override EffectType Type =>
        EffectType.ModifyAttribute;
}

