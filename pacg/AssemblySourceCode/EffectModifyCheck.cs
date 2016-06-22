using System;

public class EffectModifyCheck : Effect
{
    public EffectModifyCheck(string source, int duration, int amount) : base(source, duration)
    {
        base.genericParameters = new int[1];
        this.Amount = amount;
    }

    public override bool Equals(object obj)
    {
        if (obj != null)
        {
            EffectModifyCheck check = obj as EffectModifyCheck;
            if (check != null)
            {
                return (check.Amount == this.Amount);
            }
        }
        return false;
    }

    public int GetCheckModifier() => 
        this.Amount;

    public override string GetDisplayText() => 
        ("Check Difficulty: " + base.ConvertBonusToText(this.Amount));

    public override int GetHashCode() => 
        base.GetHashCode();

    public override bool Stack(Effect e)
    {
        EffectModifyCheck check = e as EffectModifyCheck;
        if ((check != null) && (check.source == base.source))
        {
            this.Amount += check.Amount;
            base.sources.Add(check.source);
            return true;
        }
        return false;
    }

    private int Amount
    {
        get => 
            base.genericParameters[0];
        set
        {
            base.genericParameters[0] = value;
        }
    }

    public override bool Stacking =>
        true;

    public override EffectType Type =>
        EffectType.ModifyCheck;
}

