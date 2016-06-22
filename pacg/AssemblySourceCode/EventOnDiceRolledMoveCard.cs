using System;
using UnityEngine;

[RequireComponent(typeof(BlockMoveCard))]
public class EventOnDiceRolledMoveCard : Event
{
    [Tooltip("move function")]
    public BlockMoveCard MoveBlock;
    [Tooltip("shuffle remaining cards after close")]
    public bool Shuffle;

    public override bool IsEventValid(Card card)
    {
        if (Turn.Checks == null)
        {
            return false;
        }
        if (Turn.Checks.Length <= 0)
        {
            return false;
        }
        if (Turn.DiceTotal >= Turn.DiceTarget)
        {
            return false;
        }
        return base.IsConditionValid(card);
    }

    public override void OnDiceRolled()
    {
        if (this.IsEventValid(Turn.Card))
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
        }
    }

    public override EventType Type =>
        EventType.OnDiceRolled;
}

