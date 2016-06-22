using System;
using UnityEngine;

public class EventClosedBlock : Event
{
    [Tooltip("the block to invoke")]
    public Block Block;

    public override void OnLocationClosed()
    {
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnLocationClosed;
}

