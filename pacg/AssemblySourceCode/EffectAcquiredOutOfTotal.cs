using System;

public class EffectAcquiredOutOfTotal : Effect
{
    public EffectAcquiredOutOfTotal(string source, int duration, int total, CardType icon, int helperTextID) : base(source, duration)
    {
        base.genericParameters = new int[3];
        this.Total = total;
        this.CardType = icon;
        this.ID = helperTextID;
    }

    public override string GetDisplayText()
    {
        if (string.IsNullOrEmpty(base.source))
        {
            return string.Format(StringTableManager.GetHelperText(this.ID), base.sources.Count - 1, this.Total);
        }
        return string.Format(StringTableManager.GetHelperText(this.ID), base.sources.Count, this.Total);
    }

    public override CardType GetEffectButtonIcon() => 
        this.CardType;

    public override bool Stack(Effect e)
    {
        base.sources.Remove(null);
        e.sources.Remove(null);
        base.sources.AddRange(e.sources);
        return true;
    }

    private CardType CardType
    {
        get => 
            ((CardType) base.genericParameters[1]);
        set
        {
            base.genericParameters[1] = (int) value;
        }
    }

    public int ID
    {
        get => 
            base.genericParameters[2];
        set
        {
            base.genericParameters[2] = value;
        }
    }

    public override bool Stacking =>
        true;

    public int Total
    {
        get => 
            base.genericParameters[0];
        set
        {
            base.genericParameters[0] = value;
        }
    }

    public override EffectType Type =>
        EffectType.AcquiredOutOfTotal;
}

