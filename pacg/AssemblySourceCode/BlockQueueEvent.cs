using System;
using UnityEngine;

public class BlockQueueEvent : Block
{
    [Tooltip("name of the event to queue")]
    public string EventName;
    [Tooltip("type of event")]
    public bool LocationEvent;
    [Tooltip("type of event")]
    public bool ScenarioEvent = true;

    public override void Invoke()
    {
        if (this.ScenarioEvent)
        {
            Turn.PendingDoneEvent = new TurnStateCallback(TurnStateCallbackType.Scenario, this.EventName);
        }
        if (this.LocationEvent)
        {
            Turn.PendingDoneEvent = new TurnStateCallback(TurnStateCallbackType.Location, this.EventName);
        }
    }
}

