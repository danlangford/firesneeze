using System;
using UnityEngine;

public class ComparatorDice : Comparator
{
    [Tooltip("the comparison operation")]
    public MetaCompareOperator Operator = MetaCompareOperator.Equals;
    [Tooltip("the value to compare against (rhs)")]
    public int Total;

    public override bool Compare() => 
        this.Operator.Evaluate(Turn.DiceTotal, this.Total);
}

