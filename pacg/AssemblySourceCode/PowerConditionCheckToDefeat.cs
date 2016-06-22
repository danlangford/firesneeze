using System;

public class PowerConditionCheckToDefeat : PowerCondition
{
    public override bool Evaluate(Card card) => 
        Rules.IsCheckToDefeat();
}

