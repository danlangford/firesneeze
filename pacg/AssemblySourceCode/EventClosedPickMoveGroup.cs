using System;
using UnityEngine;

public class EventClosedPickMoveGroup : Event
{
    [Tooltip("source deck")]
    public DeckType From = DeckType.Discard;
    [Tooltip("text for \"pass\"")]
    public StrRefType MessageAskNo;
    [Tooltip("text for \"move\"")]
    public StrRefType MessageAskYes;
    [Tooltip("the deny message on the dialog box")]
    public string NoMessage = string.Empty;
    [Tooltip("selects the cards that can be picked")]
    public CardSelector Selector;
    [Tooltip("if true, the destination deck will shuffle after the move finishes")]
    public bool Shuffle = true;
    [Tooltip("destination deck")]
    public DeckType To = DeckType.Character;
    [Tooltip("the accept message on the dialog box")]
    public string YesMessage = string.Empty;

    private void EventClosedPickMoveGroup_Ask()
    {
        if (this.Selector.Filter(Turn.Character.GetDeck(this.From)) > 0)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.Popup.Clear();
                TurnStateCallback callback = new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPickMoveGroup_No");
                window.Popup.Add(this.MessageAskNo.ToString(), callback);
                TurnStateCallback callback2 = new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPickMoveGroup_Yes");
                window.Popup.Add(this.MessageAskYes.ToString(), callback2);
                window.Popup.SetDeckPosition(DeckType.Character);
                Turn.State = GameStateType.Popup;
            }
        }
        else
        {
            this.EventClosedPickMoveGroup_Next();
        }
    }

    private void EventClosedPickMoveGroup_Next()
    {
        if (this.NextCharacterAtLocation(Location.Current.ID))
        {
            this.RefreshLocationWindow();
            this.EventClosedPickMoveGroup_Ask();
        }
        else
        {
            this.RefreshLocationWindow();
            Turn.SwitchCharacter(Turn.Current);
            Turn.PushStateDestination(GameStateType.Done);
            Turn.State = GameStateType.Recharge;
            Event.Done();
        }
    }

    private void EventClosedPickMoveGroup_No()
    {
        this.EventClosedPickMoveGroup_Next();
    }

    private void EventClosedPickMoveGroup_Yes()
    {
        Turn.SetStateData(new TurnStateData(this.From.ToActionType(), this.Selector.ToFilter(), this.To.ToActionType(), 1));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPickMoveGroup_YesNext"));
        Turn.State = GameStateType.Pick;
    }

    private void EventClosedPickMoveGroup_YesNext()
    {
        Deck deck = Turn.Character.GetDeck(this.From);
        if (this.Shuffle)
        {
            VisualEffect.Shuffle(this.To);
            deck.Shuffle();
        }
        LeanTween.delayedCall(0.3f, () => this.EventClosedPickMoveGroup_Next());
    }

    private bool NextCharacterAtLocation(string locID)
    {
        int number = Turn.Number;
        int num2 = 0;
        while (num2++ < Party.Characters.Count)
        {
            number++;
            if (number >= Party.Characters.Count)
            {
                number = 0;
            }
            if (number == Turn.Current)
            {
                return false;
            }
            if ((Party.Characters[number].Alive && (Party.Characters[number].Location == locID)) && (this.Selector.Filter(Party.Characters[number].GetDeck(this.From)) > 0))
            {
                Turn.SwitchCharacter(number);
                return true;
            }
        }
        return false;
    }

    public override void OnLocationClosed()
    {
        this.RefreshLocationWindow();
        this.EventClosedPickMoveGroup_Ask();
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

