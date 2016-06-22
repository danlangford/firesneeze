using System;
using UnityEngine;

public class EventDefeatedDisplay : Event
{
    [Tooltip("the card to be displayed in the summons tray")]
    public string ID;

    public override void OnCardDefeated(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutSummoner.Show(this.ID);
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

