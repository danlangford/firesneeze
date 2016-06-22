using System;

public class EffectMirrorImage : Effect
{
    public EffectMirrorImage(string source, int duration) : base(source, duration)
    {
    }

    public override void Clear()
    {
        Turn.BlackBoard.Set<int>("EffectMirrorImage_Rolled", 0);
    }

    public override bool Equals(object obj) => 
        (obj is EffectMirrorImage);

    public override string GetDisplayText() => 
        "Mirror Image";

    public override int GetHashCode() => 
        base.GetHashCode();

    public override void Invoke()
    {
        new TurnStateCallback("SP1B_MirrorImage", "CardPowerMirrorImage_Roll").Invoke();
    }

    public bool IsDamageAvoidPossible(Card enemy)
    {
        if (Turn.BlackBoard.Get<int>("CardPowerMirrorImage_Played") == Turn.BlackBoard.Get<int>("EffectMirrorImage_Rolled"))
        {
            return false;
        }
        if (!Rules.IsDamageReductionPossible())
        {
            return false;
        }
        return Turn.DamageFromEnemy;
    }

    public override bool IsInvokePossible() => 
        true;

    public override bool Single =>
        true;

    public override EffectType Type =>
        EffectType.MirrorImage;
}

