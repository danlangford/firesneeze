using System;
using UnityEngine;

public class PowerConditionPhase : PowerCondition
{
    [Tooltip("the phases we are checking for")]
    public TurnPhaseType[] Phase;

    public override bool Evaluate(Card card)
    {
        for (int i = 0; i < this.Phase.Length; i++)
        {
            if (this.Phase[i] == Turn.Phase)
            {
                return true;
            }
        }
        return false;
    }
}

