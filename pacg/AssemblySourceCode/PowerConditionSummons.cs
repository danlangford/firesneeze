using System;
using UnityEngine;

public class PowerConditionSummons : PowerCondition
{
    [Tooltip("the ID of the card that performed the summons")]
    public string Summoner;

    public override bool Evaluate(Card card) => 
        (!string.IsNullOrEmpty(this.Summoner) && (Turn.SummonsSource == this.Summoner));
}

