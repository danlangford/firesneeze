using System;
using UnityEngine;

public class EventUndefeatedExamineSeek : Event
{
    [Tooltip("execute when the window is closed")]
    public Block CloseBlock;
    [Tooltip("number of matching cards to reveal")]
    public int Number = 1;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("execute before the cards are revealed")]
    public Block PrerevealBlock;
    [Tooltip("execute when the cards are revealed")]
    public Block RevealBlock;
    [Tooltip("reveal cards until one or more of these are found")]
    public CardSelector Seek;
    [Tooltip("the deck will shuffle on close if true")]
    public bool Shuffle;

    private void EventUndefeatedExamineSeek_Close()
    {
        if (this.CloseBlock != null)
        {
            this.CloseBlock.Invoke();
        }
    }

    private void EventUndefeatedExamineSeek_Reveal()
    {
        if (this.RevealBlock != null)
        {
            this.RevealBlock.Invoke();
        }
    }

    private int GetNumberOfRevealedCards()
    {
        int num = 0;
        int num2 = 0;
        if (this.Position == DeckPositionType.Top)
        {
            for (int i = 0; i < this.Deck.Count; i++)
            {
                num++;
                if (this.Seek.Match(this.Deck[i]))
                {
                    num2++;
                }
                if (num2 >= this.Number)
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
                if (this.Seek.Match(this.Deck[j]))
                {
                    num2++;
                }
                if (num2 >= this.Number)
                {
                    return num;
                }
            }
        }
        return this.Deck.Count;
    }

    public override void OnCardUndefeated(Card card)
    {
        if (this.Seek == null)
        {
            Event.Done();
        }
        else
        {
            card.Disposition = DispositionType.None;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutExamine.Mode = ExamineModeType.Reveal;
                window.layoutExamine.Source = DeckType.Location;
                window.layoutExamine.Number = this.GetNumberOfRevealedCards();
                window.layoutExamine.RevealPosition = this.Position;
                window.layoutExamine.Shuffle = this.Shuffle;
                if (this.Seek != null)
                {
                    window.layoutExamine.Sort = this.Seek.ToFilter();
                }
                window.layoutExamine.RevealCallback = new TurnStateCallback(base.Card, "EventUndefeatedExamineSeek_Reveal");
                window.layoutExamine.CloseCallback = new TurnStateCallback(base.Card, "EventUndefeatedExamineSeek_Close");
                if (this.PrerevealBlock != null)
                {
                    this.PrerevealBlock.Invoke();
                }
            }
            Turn.PushReturnState();
            Turn.State = GameStateType.Examine;
            Event.Done();
        }
    }

    private Deck Deck =>
        Location.Current.Deck;

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

