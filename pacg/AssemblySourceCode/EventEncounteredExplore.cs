using System;
using UnityEngine;

public class EventEncounteredExplore : Event
{
    [Tooltip("any one of these types will trigger the event")]
    public CardType[] CardTypes;
    [Tooltip("if true, this event will only work during the first explore phase")]
    public bool OnlyFirstExplore;

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Rules.IsCardSummons(card))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (this.OnlyFirstExplore && (Turn.CountExplores > 1))
        {
            return false;
        }
        return true;
    }

    public override void OnCardEncountered(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            for (int i = 0; i < this.CardTypes.Length; i++)
            {
                if (card.Type == this.CardTypes[i])
                {
                    Turn.Explore = true;
                    break;
                }
            }
            Event.Done();
        }
    }

    public override EventType Type =>
        EventType.OnCardEncountered;
}

