using System;

public class PowerConditionProficiency : PowerCondition
{
    public bool ProficientWithHeavyArmors;
    public bool ProficientWithLightArmors;
    public bool ProficientWithWeapons;

    public override bool Evaluate(Card card)
    {
        if (this.ProficientWithLightArmors && !Turn.Character.ProficientWithLightArmors)
        {
            return false;
        }
        if (this.ProficientWithHeavyArmors && !Turn.Character.ProficientWithHeavyArmors)
        {
            return false;
        }
        if (this.ProficientWithWeapons && !Turn.Character.ProficientWithWeapons)
        {
            return false;
        }
        return true;
    }
}

