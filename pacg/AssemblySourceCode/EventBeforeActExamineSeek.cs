using System;
using UnityEngine;

public class EventBeforeActExamineSeek : Event
{
    [Tooltip("execute after the window is closed")]
    public Block CloseBlock;
    [Tooltip("execute when the window is closing")]
    public Block DoneBlock;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("execute when the cards are revealed")]
    public Block RevealBlock;
    [Tooltip("reveal cards until one of these is found")]
    public CardSelector Seek;
    [Tooltip("the deck will shuffle on close if true")]
    public bool Shuffle;

    private void EventEncounteredExamineSeek_Close()
    {
        if (this.CloseBlock != null)
        {
            this.CloseBlock.Invoke();
        }
    }

    private void EventEncounteredExamineSeek_Done()
    {
        if (this.DoneBlock != null)
        {
            this.DoneBlock.Invoke();
        }
    }

    private void EventEncounteredExamineSeek_Reveal()
    {
        if (this.RevealBlock != null)
        {
            this.RevealBlock.Invoke();
        }
    }

    private int GetNumberOfRevealedCards()
    {
        int num = 0;
        if (this.Position == DeckPositionType.Top)
        {
            for (int i = 0; i < this.Deck.Count; i++)
            {
                num++;
                if (this.Match(this.Deck[i]))
                {
                    return num;
                }
            }
        }
        if (this.Position == DeckPositionType.Bottom)
        {
            for (int j = this.Deck.Count - 1; j >= 0; j--)
            {
                num++;
                if (this.Match(this.Deck[j]))
                {
                    return num;
                }
            }
        }
        return this.Deck.Count;
    }

    private bool Match(Card card) => 
        ((this.Seek == null) || this.Seek.Match(card));

    public override void OnBeforeAct()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Mode = ExamineModeType.Reveal;
            window.layoutExamine.Source = DeckType.Location;
            window.layoutExamine.Number = this.GetNumberOfRevealedCards();
            window.layoutExamine.RevealPosition = this.Position;
            window.layoutExamine.Shuffle = this.Shuffle;
            window.layoutExamine.Finish = true;
            window.layoutExamine.RevealCallback = new TurnStateCallback(base.Card, "EventEncounteredExamineSeek_Reveal");
            window.layoutExamine.DoneCallback = new TurnStateCallback(base.Card, "EventEncounteredExamineSeek_Done");
            window.layoutExamine.CloseCallback = new TurnStateCallback(base.Card, "EventEncounteredExamineSeek_Close");
        }
        Turn.PushReturnState();
        Turn.State = GameStateType.Examine;
        Event.Done();
    }

    private Deck Deck =>
        Location.Current.Deck;

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

