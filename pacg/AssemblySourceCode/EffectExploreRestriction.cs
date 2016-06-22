using System;

public class EffectExploreRestriction : Effect
{
    public EffectExploreRestriction(string source, int duration, DispositionType disposition, CardFilter filter) : base(source, duration, filter)
    {
        base.genericParameters = new int[1];
        this.Disposition = disposition;
    }

    public override bool Equals(object obj)
    {
        EffectExploreRestriction restriction = obj as EffectExploreRestriction;
        if (restriction == null)
        {
            return false;
        }
        if (!base.Equals(restriction))
        {
            return false;
        }
        if (this.Disposition != restriction.Disposition)
        {
            return false;
        }
        return true;
    }

    public override string GetDisplayText() => 
        ("Explore: " + this.Disposition.ToText() + " " + base.filter.ToText());

    public override int GetHashCode() => 
        (base.GetHashCode() ^ base.filter.GetHashCode());

    public bool Match(Card card) => 
        ((base.filter != null) && base.filter.Match(card));

    public DispositionType Disposition
    {
        get => 
            ((DispositionType) base.genericParameters[0]);
        set
        {
            base.genericParameters[0] = (int) value;
        }
    }

    public override bool Single =>
        true;

    public override EffectType Type =>
        EffectType.ExploreRestriction;
}

