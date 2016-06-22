using System;
using UnityEngine;

public class EventEncounteredHaunt : Event
{
    [Tooltip("the stacking penalty applied to all checks")]
    public int CheckPenalty = 1;

    public override void OnCardEncountered(Card card)
    {
        EffectHaunt e = new EffectHaunt(card.ID, Effect.DurationPermament, this.CheckPenalty);
        Turn.Owner.ApplyEffect(e);
        Turn.LastCombatResult = CombatResultType.Win;
        Turn.State = GameStateType.Damage;
        Event.Done();
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardEncountered;
}

