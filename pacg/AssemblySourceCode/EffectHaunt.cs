using System;

public class EffectHaunt : EffectModifyCheck
{
    public EffectHaunt(string source, int duration, int amount) : base(source, duration, amount)
    {
    }

    public override string GetDisplayText() => 
        ("Haunted: " + base.ConvertBonusToText(base.GetCheckModifier()));

    public override bool Stacking =>
        true;

    public override EffectType Type =>
        EffectType.Haunt;
}

