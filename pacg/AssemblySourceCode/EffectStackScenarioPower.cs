using System;

public class EffectStackScenarioPower : Effect
{
    public EffectStackScenarioPower(MetaCompareOperator comparer, int amount, int powerToActivate, string source, int duration, CardFilter filter) : base(source, duration, filter)
    {
        base.genericParameters = new int[3];
        this.Comparer = comparer;
        this.Amount = amount;
        this.PowerToActivate = powerToActivate;
    }

    private bool CanStack(EffectStackScenarioPower effectStackDamage) => 
        ((((effectStackDamage.duration == base.duration) && ((effectStackDamage.filter == base.filter) || effectStackDamage.filter.Equals(base.filter))) && ((effectStackDamage.Amount == this.Amount) && (effectStackDamage.Comparer == this.Comparer))) && (effectStackDamage.PowerToActivate == this.PowerToActivate));

    public override bool Equals(object obj)
    {
        EffectStackScenarioPower power = obj as EffectStackScenarioPower;
        if (power == null)
        {
            return false;
        }
        if (!base.Equals(power))
        {
            return false;
        }
        if (this.Amount != power.Amount)
        {
            return false;
        }
        if (this.Comparer == power.Comparer)
        {
            return false;
        }
        return true;
    }

    public override string GetDisplayText() => 
        Scenario.Current.DisplayName;

    public override int GetHashCode() => 
        (base.GetHashCode() ^ base.filter.GetHashCode());

    public override void Invoke()
    {
        if (this.StackValid())
        {
            Scenario.Current.Powers[this.PowerToActivate].Activate();
        }
    }

    public override bool Stack(Effect e)
    {
        EffectStackScenarioPower effectStackDamage = e as EffectStackScenarioPower;
        if ((effectStackDamage != null) && this.CanStack(effectStackDamage))
        {
            base.sources.AddRange(effectStackDamage.sources);
            base.sources.Sort();
            this.Invoke();
            return true;
        }
        return false;
    }

    public bool StackValid() => 
        this.Comparer.Evaluate(base.sources.Count, this.Amount);

    public int Amount
    {
        get => 
            base.genericParameters[1];
        set
        {
            base.genericParameters[1] = value;
        }
    }

    public MetaCompareOperator Comparer
    {
        get => 
            ((MetaCompareOperator) base.genericParameters[0]);
        set
        {
            base.genericParameters[0] = (int) value;
        }
    }

    public int PowerToActivate
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

    public override EffectType Type =>
        EffectType.StackPower;
}

