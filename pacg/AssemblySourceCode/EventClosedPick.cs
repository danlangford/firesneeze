using System;
using UnityEngine;

public class EventClosedPick : Event
{
    [Tooltip("source deck")]
    public DeckType From;
    [Tooltip("text for \"finish\"")]
    public StrRefType MessageAskNo;
    [Tooltip("text for \"recharge\"")]
    public StrRefType MessageAskYes;
    [Tooltip("this determines what the valid cards are to be picked")]
    public CardSelector Selector;
    [Tooltip("destination deck")]
    public DeckType To = DeckType.Hand;

    private bool Contains(Deck deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (this.Selector.Match(deck[i]))
            {
                return true;
            }
        }
        return false;
    }

    private void EventClosedPick_Finish()
    {
        Turn.State = GameStateType.Done;
        Event.Done();
    }

    private void EventClosedPick_Pick()
    {
        this.RefreshLocationWindow();
        if ((this.Selector != null) && (this.Selector.Filter(this.From.GetDeck()) > 0))
        {
            Turn.SetStateData(new TurnStateData(this.From.ToActionType(), this.Selector.ToFilter(), this.To.ToActionType(), 1));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPick_Finish"));
            Turn.State = GameStateType.Pick;
        }
    }

    private void EventClosedPick_Popup()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
            window.Popup.Add(this.MessageAskNo.ToString(), new TurnStateCallback(GameStateType.Done));
            window.Popup.Add(this.MessageAskYes.ToString(), new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPick_Pick"));
            Turn.State = GameStateType.Popup;
        }
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (this.Selector.Filter(this.From.GetDeck()) < 1)
        {
            return false;
        }
        return true;
    }

    private void MoveCard(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (this.To == DeckType.Discard)
            {
                window.Discard(card);
            }
            if (this.To == DeckType.Bury)
            {
                window.Bury(card);
            }
            if (this.To == DeckType.Hand)
            {
                window.Draw(card);
            }
            if (this.To == DeckType.Character)
            {
                window.Recharge(card);
            }
            if (this.To == DeckType.Banish)
            {
                Campaign.Box.Add(card, false);
            }
            if (this.To == DeckType.Location)
            {
                Location.Current.Deck.Add(card);
            }
        }
    }

    public override void OnLocationClosed()
    {
        if (!this.IsEventValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPick_Popup"));
            Turn.State = GameStateType.Recharge;
        }
    }

    private void RefreshLocationWindow()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessLayoutDecks();
            window.Refresh();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnLocationClosed;
}

