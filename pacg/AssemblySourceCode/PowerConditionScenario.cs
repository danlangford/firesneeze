using System;
using UnityEngine;

public class PowerConditionScenario : PowerCondition
{
    [Tooltip("IDs of scenarios where the power will function")]
    public string[] Scenarios;

    public override bool Evaluate(Card card)
    {
        if (this.Scenarios.Length > 0)
        {
            bool flag = false;
            for (int i = 0; i < this.Scenarios.Length; i++)
            {
                if (this.Scenarios[i] == Scenario.Current.ID)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                return false;
            }
        }
        return true;
    }
}

