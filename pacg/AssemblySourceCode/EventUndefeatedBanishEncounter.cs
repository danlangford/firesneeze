using System;
using UnityEngine;

public class EventUndefeatedBanishEncounter : Event
{
    [Tooltip("the type of card used to banish the encounter")]
    public CardSelector CardSelector;
    [Tooltip("text for \"shuffle\"")]
    public StrRefType MessageAskNo;
    [Tooltip("text for \"banish\"")]
    public StrRefType MessageAskYes;
    [Tooltip("the penalty required to banish the encounter")]
    public ActionType PenaltyAction = ActionType.Banish;

    private void EventUndefeatedBanish_Finish()
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

    private void EventUndefeatedBanish_Penalty()
    {
        Turn.SetStateData(new TurnStateData(ActionType.Banish, this.CardSelector.ToFilter(), 1));
        Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventUndefeatedBanish_Finish"));
        Turn.State = GameStateType.Penalty;
    }

    private void EventUndefeatedBanish_Proceed()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
        }
        Turn.Card.Disposition = DispositionType.Shuffle;
        Turn.PushStateDestination(Turn.PopReturnState());
        Turn.State = GameStateType.Recharge;
        Event.Done();
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

    public override void OnCardUndefeated(Card card)
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
                Turn.PushReturnState();
                window.Popup.Clear();
                window.Popup.Add(this.MessageAskNo.ToString(), new TurnStateCallback(Turn.Card, "EventUndefeatedBanish_Proceed"));
                window.Popup.Add(this.MessageAskYes.ToString(), new TurnStateCallback(Turn.Card, "EventUndefeatedBanish_Penalty"));
                Turn.State = GameStateType.Popup;
            }
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

