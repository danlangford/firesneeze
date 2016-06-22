using System;

public class PowerConditionEvadePossible : PowerCondition
{
    public override bool Evaluate(Card card) => 
        Rules.IsEvadePossible(card);
}

