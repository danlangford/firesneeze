using System;

public class EffectNameable : Effect
{
    public EffectNameable(string source, int duration, int ID) : base(source, duration)
    {
        base.genericParameters = new int[1];
        this.ID = ID;
    }

    public override string GetDisplayText() => 
        string.Format(StringTableManager.GetHelperText(this.ID), base.sources.Count - 1);

    public override bool Stack(Effect e)
    {
        e.sources.Remove(e.source);
        base.sources.AddRange(e.sources);
        return true;
    }

    public int ID
    {
        get => 
            base.genericParameters[0];
        set
        {
            base.genericParameters[0] = value;
        }
    }

    public override bool ShowSources =>
        false;

    public override bool Stacking =>
        true;

    public override EffectType Type =>
        EffectType.Nameable;
}

