using System;
using UnityEngine;

public class PowerConditionSpecialCombatEnd : PowerCondition
{
    [Tooltip("if true power condition returns true if defeat is currently true")]
    public bool Defeat;
    [Tooltip("if true power condition returns true if evade is currently true")]
    public bool Evade = true;

    public override bool Evaluate(Card card) => 
        ((this.Evade && Turn.Evade) || (this.Defeat && Turn.Defeat));
}

