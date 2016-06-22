using System;
using UnityEngine;

public class PowerConditionIsPowerActive : PowerCondition
{
    [Tooltip("If any of these powers are active return false")]
    public string[] PowersToCheck;

    public override bool Evaluate(Card card)
    {
        for (int i = 0; i < this.PowersToCheck.Length; i++)
        {
            if (Turn.IsPowerActive(this.PowersToCheck[i]))
            {
                return true;
            }
        }
        return false;
    }
}

