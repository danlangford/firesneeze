using System;
using UnityEngine;

[RequireComponent(typeof(BlockMoveCard))]
public class EventClosedMoveCard : Event
{
    [Tooltip("move function")]
    public BlockMoveCard MoveBlock;
    [Tooltip("shuffle remaining cards after close")]
    public bool Shuffle;

    public override void OnLocationClosed()
    {
        if (this.MoveBlock != null)
        {
            this.MoveBlock.Invoke();
        }
        if (this.Shuffle)
        {
            VisualEffect.Shuffle(DeckType.Location);
            Location.Current.Deck.Shuffle();
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnLocationClosed;
}

