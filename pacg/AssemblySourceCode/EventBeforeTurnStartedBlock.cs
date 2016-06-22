using System;
using UnityEngine;

public class EventBeforeTurnStartedBlock : Event
{
    [Tooltip("the block to invoke")]
    public Block Block;

    public override void OnBeforeTurnStart()
    {
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
        base.OnBeforeTurnStart();
    }

    public override bool Stateless =>
        ((this.Block == null) || this.Block.Stateless);

    public override EventType Type =>
        EventType.OnBeforeTurnStart;
}

