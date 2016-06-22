using System;
using UnityEngine;

public class PowerConditionVariable : PowerCondition
{
    [Tooltip("name of the variable")]
    public string FlagName;

    public override bool Evaluate(Card card) => 
        Turn.BlackBoard.Get<bool>(this.FlagName);
}

