using System;
using UnityEngine;

public class EventTurnStartedEncounter : Event
{
    [Tooltip("defines the creature to summon")]
    public SummonsSelector Summons;

    public override bool IsEventValid(Card card)
    {
        if (Location.Current.Closed)
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnTurnStarted()
    {
        if (!this.IsEventValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Card card = this.Summons.Summon();
                if (card != null)
                {
                    Turn.SummonsType = SummonsType.Start;
                    Location.Current.Deck.Add(card, DeckPositionType.Top);
                    window.layoutLocation.Explore();
                }
            }
            Event.Done();
        }
    }

    public override EventType Type =>
        EventType.OnTurnStarted;
}

