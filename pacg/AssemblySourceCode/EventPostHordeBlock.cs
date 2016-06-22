using System;
using UnityEngine;

public class EventPostHordeBlock : Event
{
    [Tooltip("the block to invoke after the horde")]
    public Block Block;

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }

    public override void OnPostHorde(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            if (this.Block != null)
            {
                this.Block.Invoke();
            }
            Event.Done();
        }
    }

    public override bool Stateless
    {
        get
        {
            if ((this.Block != null) && !this.Block.Stateless)
            {
                return false;
            }
            return true;
        }
    }

    public override EventType Type =>
        EventType.OnPostHorde;
}

