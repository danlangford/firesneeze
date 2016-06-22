using System;
using UnityEngine;

public class PowerConditionCountOpenLocations : PowerCondition
{
    [Tooltip("the right number in the comparison")]
    public int Amount;
    [Tooltip("the comparison operator to use")]
    public MetaCompareOperator Comparison;

    public override bool Evaluate(Card card) => 
        this.Comparison.Evaluate(Scenario.Current.GetNumOpenLocations(), this.Amount);
}

