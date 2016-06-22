using System;
using UnityEngine;

public class EventAcquiredTypeSetAside : Event
{
    [Tooltip("helper text that determines the mssg of the effect")]
    public StrRefType EffectMsg;
    [Tooltip("the type of cards this event is valid for")]
    public CardSelector Selector;

    public override bool IsEventValid(Card card)
    {
        if (!this.Selector.Match(card))
        {
            return false;
        }
        if (Rules.IsCardSummons(card))
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCardDefeated(Card card)
    {
        if (this.IsEventValid(card))
        {
            EffectAcquiredOutOfTotal effect = new EffectAcquiredOutOfTotal(card.ID, Effect.DurationPermament, 0, this.Selector.CardTypes[0], this.EffectMsg.id);
            Scenario.Current.ApplyEffect(effect);
            card.Disposition = DispositionType.SetAside;
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

