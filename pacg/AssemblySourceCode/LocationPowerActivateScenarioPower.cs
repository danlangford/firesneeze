using System;
using UnityEngine;

public class LocationPowerActivateScenarioPower : LocationPower
{
    [Tooltip("scenario power to activate")]
    public string ScenarioPower;

    public override void Activate()
    {
        base.Activate();
        for (int i = 0; i < Scenario.Current.Powers.Count; i++)
        {
            if (Scenario.Current.Powers[i].ID == this.ScenarioPower)
            {
                Scenario.Current.Powers[i].Activate();
                break;
            }
        }
    }

    public override bool IsValid()
    {
        if (Turn.IsPowerActive(this.ScenarioPower))
        {
            return false;
        }
        return base.IsConditionValid(Turn.Card);
    }
}

