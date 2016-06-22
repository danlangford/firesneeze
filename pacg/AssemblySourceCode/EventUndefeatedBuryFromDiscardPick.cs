using System;
using UnityEngine;

public class EventUndefeatedBuryFromDiscardPick : Event
{
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("number of cards to bury")]
    public int Number = 1;
    [Tooltip("cards matching this selector will be buried")]
    public CardSelector Selector;

    private void EventUndefeatedBuryFromDiscardPick_Finish()
    {
        Turn.State = GameStateType.Dispose;
        Event.Done();
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (this.Selector.Filter(Turn.Character.Discard) < this.Number)
        {
            return false;
        }
        return true;
    }

    public override void OnCardUndefeated(Card card)
    {
        TurnStateData data = new TurnStateData(ActionType.Discard, this.Selector.ToFilter(), ActionType.Bury, this.Number) {
            Message = StringTableManager.Get(this.Message)
        };
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventUndefeatedBuryFromDiscardPick_Finish"));
        Turn.State = GameStateType.Pick;
    }

    public override bool Stateless =>
        this.IsEventValid(Turn.Card);

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

