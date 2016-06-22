using System;

public class ScenarioPowerMoveRestrictionVillain : ScenarioPowerMoveRestriction
{
    public override bool IsLocationValid(string LocID)
    {
        if (Scenario.Current.NumVillainEncounters >= 1)
        {
            return false;
        }
        return true;
    }
}

