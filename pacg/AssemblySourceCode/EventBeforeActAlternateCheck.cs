using System;
using UnityEngine;

public class EventBeforeActAlternateCheck : Event
{
    [Tooltip("the amount of cards that must go through the penalty")]
    public int Amount = 1;
    [Tooltip("the penalty to defeat the check")]
    public ActionType Penalty = ActionType.Bury;
    [Tooltip("filter for what types of cards are legal for the Penalty")]
    public CardSelector Selector;

    private void AlternateCheck_Combat()
    {
        Turn.State = GameStateType.Combat;
        Turn.PushCancelDestination(GameStateType.Popup);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
        Event.Done();
    }

    private void AlternateCheck_Penalty()
    {
        TurnStateData data = new TurnStateData(this.Penalty, (this.Selector == null) ? CardFilter.Empty : this.Selector.ToFilter(), this.Amount);
        Turn.SetStateData(data);
        Turn.PushCancelDestination(new TurnStateCallback(base.Card, "AlternateCheck_PenaltyCancel"));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "AlternateCheck_PenaltyDone"));
        Turn.State = GameStateType.Penalty;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
        Event.Done();
    }

    private void AlternateCheck_PenaltyCancel()
    {
        this.OnBeforeAct();
    }

    private void AlternateCheck_PenaltyDone()
    {
        Turn.Defeat = true;
        Turn.State = GameStateType.Combat;
        Turn.Proceed();
    }

    public override void OnBeforeAct()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
            window.Popup.Add(this.Penalty.ToText(), new TurnStateCallback(base.Card, "AlternateCheck_Penalty"));
            window.Popup.Add(StringTableManager.GetUIText(0x111), new TurnStateCallback(base.Card, "AlternateCheck_Combat"));
            Turn.State = GameStateType.Popup;
        }
    }

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

