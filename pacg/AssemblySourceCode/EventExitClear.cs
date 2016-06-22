using System;
using UnityEngine;

public class EventExitClear : Event
{
    [Tooltip("should the box be cleared?")]
    public bool Box = true;

    public override void OnScenarioExit()
    {
        if (this.Box)
        {
            Campaign.Box.Clear();
        }
    }

    public override EventType Type =>
        EventType.OnScenarioExit;
}

