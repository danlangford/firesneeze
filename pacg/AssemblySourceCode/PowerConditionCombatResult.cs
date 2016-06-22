using System;
using UnityEngine;

public class PowerConditionCombatResult : PowerCondition
{
    [Tooltip("compared to the last combat result with equality")]
    public CombatResultType LastCombatResult;

    public override bool Evaluate(Card card) => 
        (Turn.LastCombatResult == this.LastCombatResult);
}

