using System;
using UnityEngine;

public class PowerConditionDefeated : PowerCondition
{
    [Tooltip("difference between roll and target")]
    public int Amount = 1;
    [Tooltip("operator used to evaluate this condition")]
    public MetaCompareOperator Operator = MetaCompareOperator.MoreOrEqual;

    public override bool Evaluate(Card card)
    {
        int a = Turn.DiceTotal - Turn.DiceTarget;
        return this.Operator.Evaluate(a, this.Amount);
    }
}

