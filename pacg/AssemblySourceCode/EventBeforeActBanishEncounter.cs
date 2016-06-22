using System;
using UnityEngine;

public class EventBeforeActBanishEncounter : Event
{
    [Tooltip("the type of card used to banish the encounter")]
    public CardSelector CardSelector;
    [Tooltip("text for \"encounter\"")]
    public StrRefType MessageAskNo;
    [Tooltip("text for \"banish\"")]
    public StrRefType MessageAskYes;
    [Tooltip("the penalty required to banish the encounter")]
    public ActionType PenaltyAction = ActionType.Banish;

    private void EventEncounteredBanish_Cancel()
    {
        this.OnCardEncountered(Turn.Card);
    }

    private void EventEncounteredBanish_Encounter()
    {
        Turn.PushCancelDestination(new TurnStateCallback(Turn.Card, "EventEncounteredBanish_Cancel"));
        GameStateEncounter.Continue();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
        Event.Done();
    }

    private void EventEncounteredBanish_Finish()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
        }
        Turn.Card.Disposition = DispositionType.Banish;
        Turn.PushStateDestination(GameStateType.Dispose);
        Turn.State = GameStateType.Recharge;
        Event.Done();
    }

    private void EventEncounteredBanish_Penalty()
    {
        Turn.SetStateData(new TurnStateData(ActionType.Banish, this.CardSelector.ToFilter(), 1));
        Turn.PushCancelDestination(new TurnStateCallback(Turn.Card, "EventEncounteredBanish_Cancel"));
        Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventEncounteredBanish_Finish"));
        Turn.State = GameStateType.Penalty;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (this.CardSelector.Filter(Turn.Character.Hand) < 1)
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
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.Popup.Clear();
                window.Popup.Add(this.MessageAskYes.ToString(), new TurnStateCallback(Turn.Card, "EventEncounteredBanish_Penalty"));
                window.Popup.Add(this.MessageAskNo.ToString(), new TurnStateCallback(Turn.Card, "EventEncounteredBanish_Encounter"));
                Turn.State = GameStateType.Popup;
            }
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardEncountered;
}

