using System;
using UnityEngine;

public class EventLocationCloseAttemptPeek : Event
{
    [Tooltip("number of cards to peek at")]
    public int Number = 1;

    public override void OnLocationCloseAttempted()
    {
        Turn.PushStateDestination(Turn.State);
        Turn.State = GameStateType.Null;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutSummoner.Clear();
            window.mapPanel.Peek(this.Number);
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnLocationCloseAttempted;
}

