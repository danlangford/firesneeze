using System;
using UnityEngine;

public class EventDiscardBury : Event
{
    [Tooltip("this function will only apply to this type of card")]
    public CardType CardFilter;

    public override void OnCardDiscarded(Card card)
    {
        if (card.Type == this.CardFilter)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.Bury(card);
            }
        }
    }

    public override EventType Type =>
        EventType.OnCardDiscarded;
}

