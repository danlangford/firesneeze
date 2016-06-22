using System;
using UnityEngine;

public class EventDefeatedExamineLocation : Event
{
    [Tooltip("the card to be displayed in the summons tray")]
    public string ID;
    [Tooltip("helper text shown in the message panel")]
    public StrRefType Message;

    public override void OnCardDefeated(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutSummoner.Show(this.ID);
        }
        Turn.BlackBoard.Set<string>("LastLocation", Location.Current.ID);
        Turn.PushReturnState();
        if (window != null)
        {
            window.mapPanel.Mode = MapModeType.Examine;
            window.ShowMap(true);
            window.messagePanel.Show(this.Message.ToString());
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

