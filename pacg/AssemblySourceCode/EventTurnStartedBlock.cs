using System;
using UnityEngine;

public class EventTurnStartedBlock : Event
{
    [Tooltip("the block to invoke")]
    public Block Block;

    public override void OnTurnStarted()
    {
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
        Event.Done();
    }

    public override bool Stateless =>
        ((this.Block == null) || this.Block.Stateless);

    public override EventType Type =>
        EventType.OnTurnStarted;
}

