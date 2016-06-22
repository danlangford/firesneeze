using System;
using UnityEngine;

[RequireComponent(typeof(BlockMoveCard))]
public class EventUndefeatedMoveCard : Event
{
    public BlockMoveCard MoveBlock;

    public override void OnCardUndefeated(Card card)
    {
        this.MoveBlock.Invoke();
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

