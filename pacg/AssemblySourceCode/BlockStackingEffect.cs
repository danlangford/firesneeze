using System;
using UnityEngine;

public class BlockStackingEffect : BlockEffect
{
    [Tooltip("compared to the amount of cards stacked under the effect")]
    public int Amount = 1;
    [Tooltip("used to compare to the amount. The formula is : The Amount Stacked Under Here [Comparer] Amount")]
    public MetaCompareOperator Comparer = MetaCompareOperator.Equals;
    [Tooltip("the scenario power to activate")]
    public ScenarioPower Power;

    protected override Effect CreateEffect(string source, int duration, CardFilter filter) => 
        new EffectStackScenarioPower(this.Comparer, this.Amount, Scenario.Current.Powers.IndexOf(this.Power), source, duration, filter);

    private bool StackValid(Character character)
    {
        EffectStackScenarioPower effect = character.GetEffect(EffectType.StackPower) as EffectStackScenarioPower;
        return ((effect != null) && effect.StackValid());
    }

    private bool StackValid(Scenario scenario)
    {
        EffectStackScenarioPower effect = scenario.GetEffect(EffectType.StackPower) as EffectStackScenarioPower;
        return ((effect != null) && effect.StackValid());
    }

    public override bool Stateless
    {
        get
        {
            switch (base.Target)
            {
                case DamageTargetType.Player:
                    return !this.StackValid(Turn.Owner);

                case DamageTargetType.Location:
                    for (int i = 0; i < Party.Characters.Count; i++)
                    {
                        if (Party.Characters[i].Location == Location.Current.ID)
                        {
                            return !this.StackValid(Party.Characters[i]);
                        }
                    }
                    return false;

                case DamageTargetType.Party:
                    return !this.StackValid(Scenario.Current);
            }
            return true;
        }
    }
}

