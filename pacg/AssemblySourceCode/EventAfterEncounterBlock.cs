using System;
using UnityEngine;

public class EventAfterEncounterBlock : Event
{
    [Tooltip("the block to run after exploration is complete")]
    public Block Block;

    public override void OnAfterExplore()
    {
        if (base.IsConditionValid(Turn.Card) && (this.Block != null))
        {
            this.Block.Invoke();
        }
        base.OnAfterExplore();
    }

    public override EventType Type =>
        EventType.OnAfterExplore;
}

