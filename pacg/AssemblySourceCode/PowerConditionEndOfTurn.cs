using System;

public class PowerConditionEndOfTurn : PowerCondition
{
    public override bool Evaluate(Card card) => 
        (Turn.State == GameStateType.EndTurn);
}

