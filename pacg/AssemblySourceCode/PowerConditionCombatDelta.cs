using System;
using UnityEngine;

public class PowerConditionCombatDelta : PowerCondition
{
    [Tooltip("amount that is being compared")]
    public int Amount;
    [Tooltip("compare operation used with the turn's combat delta")]
    public MetaCompareOperator Compare;

    public override bool Evaluate(Card card) => 
        this.Compare.Evaluate(Turn.CombatDelta, this.Amount);
}

