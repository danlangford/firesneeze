using System;

public class NumberGeneratorHaunts : NumberGenerator
{
    public override int Generate()
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Effect effect = Party.Characters[i].GetEffect(EffectType.Haunt);
            if (effect != null)
            {
                EffectHaunt haunt = effect as EffectHaunt;
                if (haunt != null)
                {
                    num += haunt.GetCheckModifier();
                }
            }
        }
        return (num + base.Bonus);
    }
}

