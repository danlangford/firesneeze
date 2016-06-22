using System;

public class PowerConditionClosedThisTurn : PowerCondition
{
    public override bool Evaluate(Card card) => 
        Location.Current.ClosedThisTurn;
}

