using System;
using UnityEngine;

public class PowerConditionExplorationsUsed : PowerCondition
{
    [Tooltip("the number of explores is compared to this")]
    public int Amount = 1;
    [Tooltip("we compare by using this operator. #OfExplores (Comparer) Amount")]
    public MetaCompareOperator Comparer = MetaCompareOperator.Equals;

    public override bool Evaluate(Card card) => 
        this.Comparer.Evaluate(Turn.CountExplores, this.Amount);
}

