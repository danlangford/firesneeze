using System;
using UnityEngine;

public class EventDefeatedPeek : Event
{
    [Tooltip("the number of cards that can be peeked")]
    public int Number = 1;

    public override void OnCardDefeated(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.mapPanel.Peek(this.Number);
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

