using System;
using UnityEngine;

public class PowerConditionLocation : PowerCondition
{
    [Tooltip("check if the location(s) is closed, setting to true can allow powers at closed locations")]
    public bool Closed;
    [Tooltip("if true check the current location to see if the current player is alone")]
    public bool IsCharacterAlone;
    [Tooltip("if true check if the location is the initial character's location")]
    public bool IsCharacterAtInitialLoc;
    [Tooltip("IDs of locations where the power will function. If empty just check the current location")]
    public string[] Locations;
    [Tooltip("check if the location(s) is open, setting to true can allow powers at open locations")]
    public bool Open;
    [Tooltip("check if the location(s) is permanently closed, just Closed includes temp closures")]
    public bool PermanentlyClosed;

    public override bool Evaluate(Card card)
    {
        if (this.Locations.Length > 0)
        {
            for (int i = 0; i < this.Locations.Length; i++)
            {
                if (this.IsLocationValid(this.Locations[i]))
                {
                    return true;
                }
            }
            return false;
        }
        if (!this.IsLocationValid(Location.Current.ID))
        {
            return false;
        }
        return true;
    }

    private bool IsLocationValid(string locID) => 
        ((((!this.PermanentlyClosed || (Scenario.Current.GetLocationCloseType(locID) == CloseType.Permanent)) && (!this.IsCharacterAlone || (Location.CountCharactersAtLocation(locID) == 1))) && ((Scenario.Current.IsLocationClosed(locID) == this.Closed) || (!Scenario.Current.IsLocationClosed(locID) == this.Open))) && (!this.IsCharacterAtInitialLoc || (locID == Party.Characters[Turn.InitialCharacter].Location)));
}

