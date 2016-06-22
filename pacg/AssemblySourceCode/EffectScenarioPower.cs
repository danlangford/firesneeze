using System;

public class EffectScenarioPower : Effect
{
    public EffectScenarioPower(string source, int duration) : base(source, duration)
    {
    }

    public override string GetDisplayText()
    {
        for (int i = 0; i < Scenario.Current.Powers.Count; i++)
        {
            if (Scenario.Current.Powers[i].ID == base.source)
            {
                return Scenario.Current.Powers[i].Name;
            }
        }
        return Scenario.Current.DisplayName;
    }

    public override EffectType Type =>
        EffectType.ScenarioPower;
}

