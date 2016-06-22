using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationSelector : Selector
{
    [Tooltip("allow closed locations")]
    public bool Closed;
    [Tooltip("should the current location be included")]
    public bool Current;
    [Tooltip("allow only the initial character's location")]
    public bool InitialCharactersLocation;
    [Tooltip("compare the location with this operator, compares LocationActualSize (MetaOp) LocationSize")]
    public MetaCompareOperator LocationCompare;
    [Tooltip("the locations to match")]
    public string[] LocationIDs;
    [Tooltip("the size of the location")]
    public int LocationSize;
    [Tooltip("allow open locations")]
    public bool Open;

    public bool Match(string id)
    {
        for (int i = 0; i < this.LocationIDs.Length; i++)
        {
            if (Scenario.Current.IsLocationValid(this.LocationIDs[i]) && (this.LocationIDs[i] == id))
            {
                return true;
            }
        }
        if (!Scenario.Current.IsLocationValid(id))
        {
            return false;
        }
        if (!this.Current && (id == Location.Current.ID))
        {
            return false;
        }
        if (this.InitialCharactersLocation && (id != Party.Characters[Turn.InitialCharacter].Location))
        {
            return false;
        }
        if (Scenario.Current.IsLocationClosed(id))
        {
            return this.Closed;
        }
        if (this.LocationCompare != MetaCompareOperator.None)
        {
            return this.LocationCompare.Evaluate(Scenario.Current.GetCardCount(id), this.LocationSize);
        }
        return this.Open;
    }

    public string Random()
    {
        List<string> list = new List<string>(Scenario.Current.Locations.Length);
        for (int i = 0; i < Scenario.Current.Locations.Length; i++)
        {
            string locationName = Scenario.Current.Locations[i].LocationName;
            if (this.Match(locationName))
            {
                list.Add(locationName);
            }
        }
        list.Shuffle<string>();
        if (list.Count > 0)
        {
            return list[0];
        }
        return null;
    }

    public string Random(Character movingCharacter)
    {
        List<string> list = new List<string>(Scenario.Current.Locations.Length);
        for (int i = 0; i < Scenario.Current.Locations.Length; i++)
        {
            string locationName = Scenario.Current.Locations[i].LocationName;
            if (this.Match(locationName) && (!Scenario.Current.Linear || Scenario.Current.IsLocationLinked(movingCharacter.Location, locationName)))
            {
                list.Add(locationName);
            }
        }
        list.Shuffle<string>();
        if (list.Count > 0)
        {
            return list[0];
        }
        return null;
    }
}

