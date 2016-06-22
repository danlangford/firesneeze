using System;

public class EventDefeatedRespawn : Event
{
    public int MinimumDamage = 4;

    public override void OnCardDefeated(Card card)
    {
        if (Turn.CombatDelta < this.MinimumDamage)
        {
            card.Disposition = DispositionType.Shuffle;
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

