using System;

public class EventOnCombatResolvedBlock : Event
{
    public Block Block;

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCombatResolved()
    {
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
        base.OnCombatResolved();
    }

    public override EventType Type =>
        EventType.OnCombatResolved;
}

