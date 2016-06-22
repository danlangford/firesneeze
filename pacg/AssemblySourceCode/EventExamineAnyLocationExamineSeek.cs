using System;
using UnityEngine;

public class EventExamineAnyLocationExamineSeek : Event
{
    [Tooltip("should we examine everything even if we find the target card?")]
    public bool All;
    [Tooltip("execute after the window is closed")]
    public Block CloseBlock;
    [Tooltip("true means that cards can be dragged from/to the bottom of the deck")]
    public bool ModifyBottom;
    [Tooltip("true means that cards can be dragged from/to the top of the deck")]
    public bool ModifyTop;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("execute after the cards are revealed")]
    public Block RevealBlock;
    [Tooltip("reveal cards until one of these is found")]
    public CardSelector Seek;
    [Tooltip("the deck will shuffle on close if true")]
    public bool Shuffle;
    [Tooltip("sort these cards to the top")]
    public CardSelector Sort;

    private void EventExamineAnyLocationExamineSeek_Close()
    {
        if (this.CloseBlock != null)
        {
            this.CloseBlock.Invoke();
        }
    }

    private void EventExamineAnyLocationExamineSeek_Reveal()
    {
        if (this.RevealBlock != null)
        {
            this.RevealBlock.Invoke();
        }
    }

    private int GetNumberOfRevealedCards()
    {
        int num = 0;
        if (!this.All)
        {
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
        }
        return this.Deck.Count;
    }

    private bool Match(Card card) => 
        ((this.Seek == null) || this.Seek.Match(card));

    public override void OnExamineAnyLocation()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            string str = Turn.BlackBoard.Get<string>("LastLocation");
            if (Location.Current.ID != str)
            {
                window.shadePanel.Show(window.shadePanel.TurnShade, true, 1f, 0.25f);
            }
            window.layoutExamine.Mode = ExamineModeType.Reveal;
            window.layoutExamine.Source = DeckType.Location;
            window.layoutExamine.Number = this.GetNumberOfRevealedCards();
            window.layoutExamine.RevealPosition = this.Position;
            window.layoutExamine.Shuffle = this.Shuffle;
            window.layoutExamine.ModifyTop = this.ModifyTop;
            window.layoutExamine.ModifyBottom = this.ModifyBottom;
            if (this.Sort != null)
            {
                window.layoutExamine.Sort = this.Sort.ToFilter();
            }
            window.layoutExamine.RevealCallback = new TurnStateCallback(base.Card, "EventExamineAnyLocationExamineSeek_Reveal");
            window.layoutExamine.CloseCallback = new TurnStateCallback(base.Card, "EventExamineAnyLocationExamineSeek_Close");
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
        EventType.OnExamineAnyLocation;
}

