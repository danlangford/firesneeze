﻿using System;
using UnityEngine;

public class EventTurnEndedBlock : Event
{
    [Tooltip("the location to check if open and empty")]
    public string LocationID;
    [Tooltip("the block to invoke if location is open and empty")]
    public Block PenaltyBlock;

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (string.IsNullOrEmpty(this.LocationID))
        {
            return false;
        }
        if (Scenario.Current.IsLocationClosed(this.LocationID))
        {
            return false;
        }
        if (Location.CountCharactersAtLocation(this.LocationID) > 0)
        {
            return false;
        }
        return true;
    }

    public override void OnTurnEnded()
    {
        if (this.IsEventValid(Turn.Card))
        {
            this.PenaltyBlock.Invoke();
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnTurnEnded;
}
