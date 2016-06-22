using System;
using UnityEngine;

public class EventUndefeatedCardMove : Event
{
    [Tooltip("where to move the card in the deck")]
    public DeckPositionType DeckPosition = DeckPositionType.Bottom;
    [Tooltip("text for \"banish\"")]
    public StrRefType MessageAskNo;
    [Tooltip("text for \"move\"")]
    public StrRefType MessageAskYes;
    [Tooltip("is this a valid card to check?")]
    public CardSelector Selector;

    private void EventUndefeatedCardMove_Ask()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
            TurnStateCallback callback = new TurnStateCallback(TurnStateCallbackType.Location, "EventUndefeatedCardMove_No");
            window.Popup.Add(this.MessageAskNo.ToString(), callback);
            TurnStateCallback callback2 = new TurnStateCallback(TurnStateCallbackType.Location, "EventUndefeatedCardMove_Yes");
            window.Popup.Add(this.MessageAskYes.ToString(), callback2);
            window.Popup.SetDeckPosition(DeckType.Location);
            Turn.State = GameStateType.Popup;
        }
    }

    private void EventUndefeatedCardMove_No()
    {
        Turn.Card.Disposition = DispositionType.Banish;
        Turn.State = GameStateType.Damage;
        Event.Done();
    }

    private void EventUndefeatedCardMove_Yes()
    {
        Card card = Turn.Card;
        card.Disposition = DispositionType.None;
        if (this.DeckPosition == DeckPositionType.Bottom)
        {
            Location.Current.Deck.Move(0, Location.Current.Deck.Count - 1);
            card.Show(false);
        }
        Turn.State = GameStateType.Dispose;
        Event.Done();
    }

    public override void OnCardUndefeated(Card card)
    {
        if (!this.Selector.Match(card))
        {
            Event.Done();
        }
        else
        {
            this.EventUndefeatedCardMove_Ask();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

