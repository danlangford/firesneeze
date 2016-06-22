using System;
using UnityEngine;

public class ScenarioPowerMoveRestrictionPopulation : ScenarioPowerMoveRestriction
{
    [Tooltip("compare to the locID. LocID [Comparer] People number of Occupants")]
    public MetaCompareOperator Comparer;
    [Tooltip("required number of people")]
    public int People = 1;

    public override bool IsLocationValid(string LocID)
    {
        if (Scenario.Current.IsLocationClosed(LocID))
        {
            return false;
        }
        return (base.IsLocationValid(LocID) && this.Comparer.Evaluate(Location.CountCharactersAtLocation(LocID), this.People));
    }
}

