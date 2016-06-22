using System;
using UnityEngine;

public class EventDefeatedTypeClose : Event
{
    [Tooltip("if this card selector is defeated set the location to close during done")]
    public CardSelector CardToDefeat;

    public override void OnCardDefeated(Card card)
    {
        if (this.CardToDefeat.Match(card))
        {
            Turn.PendingDoneEvent = new TurnStateCallback(TurnStateCallbackType.Global, "EventLocationCloseViaAcquire_Finish");
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

