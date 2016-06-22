using System;
using UnityEngine;

public class EventDamagedBlock : Event
{
    [Tooltip("the block to invoke")]
    public Block Block;

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Turn.CombatDelta >= 0)
        {
            return false;
        }
        if (!Turn.DamageFromEnemy)
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnDamageTaken(Card card)
    {
        if (this.IsEventValid(card) && (this.Block != null))
        {
            this.Block.Invoke();
        }
        base.OnDamageTaken(card);
    }

    public override bool Stateless =>
        ((this.Block == null) || this.Block.Stateless);

    public override EventType Type =>
        EventType.OnDamageTaken;
}

