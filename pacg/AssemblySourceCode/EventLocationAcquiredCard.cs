using System;
using UnityEngine;

public class EventLocationAcquiredCard : Event
{
    [Tooltip("filter used to determine what the played card matches")]
    public CardSelector CardSelector;

    public override void OnCardDefeated(Card card)
    {
        if (this.CardSelector.Match(card))
        {
            Turn.BlackBoard.Set<string>("CharacterDefeatedCard", card.ID);
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

