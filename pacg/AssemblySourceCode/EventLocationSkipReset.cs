using System;
using UnityEngine;

public class EventLocationSkipReset : Event
{
    [Tooltip("text for \"finish\"")]
    public StrRefType MessageAskNo;
    [Tooltip("text for \"reset\"")]
    public StrRefType MessageAskYes;

    private void EventLocationSkipReset_Finish()
    {
        Turn.State = GameStateType.EndTurn;
        Event.Done();
    }

    private void EventLocationSkipReset_Proceed()
    {
        Turn.State = GameStateType.Reset;
        Event.Done();
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Turn.BlackBoard.Get<bool>(this.BlackboardCooldownID))
        {
            return false;
        }
        return true;
    }

    public override void OnHandReset()
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
                window.Popup.Clear();
                window.Popup.Add(this.MessageAskNo.ToString(), new TurnStateCallback(TurnStateCallbackType.Location, "EventLocationSkipReset_Finish"));
                window.Popup.Add(this.MessageAskYes.ToString(), new TurnStateCallback(TurnStateCallbackType.Location, "EventLocationSkipReset_Proceed"));
                Turn.State = GameStateType.Popup;
                Turn.BlackBoard.Set<bool>(this.BlackboardCooldownID, true);
            }
        }
    }

    private string BlackboardCooldownID =>
        ("EventLocationSkipReset_" + base.gameObject.name);

    public override bool Stateless =>
        !this.IsEventValid(Turn.Card);

    public override EventType Type =>
        EventType.OnHandReset;
}

