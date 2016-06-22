using System;
using UnityEngine;

public class EventUndefeatedExamine : Event
{
    [Tooltip("execute when the window is closed")]
    public Block CloseBlock;
    [Tooltip("number of cards to examine")]
    public int Number = 1;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("execute when the cards are revealed")]
    public Block RevealBlock;
    [Tooltip("the deck will shuffle on close if true")]
    public bool Shuffle;
    [Tooltip("message to display while you are examining the top cards")]
    public StrRefType UndefeatedMessage;

    private void EventUndefeatedExamine_Close()
    {
        if (this.CloseBlock != null)
        {
            this.CloseBlock.Invoke();
        }
    }

    private void EventUndefeatedExamine_Reveal()
    {
        if (this.RevealBlock != null)
        {
            this.RevealBlock.Invoke();
        }
    }

    public override void OnCardUndefeated(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Mode = ExamineModeType.Reveal;
            window.layoutExamine.Source = DeckType.Character;
            window.layoutExamine.Number = Mathf.Min(this.Number, Turn.Character.Deck.Count);
            window.layoutExamine.RevealPosition = this.Position;
            window.layoutExamine.Shuffle = this.Shuffle;
            window.layoutExamine.RevealCallback = new TurnStateCallback(base.Card, "EventUndefeatedExamine_Reveal");
            window.layoutExamine.CloseCallback = new TurnStateCallback(base.Card, "EventUndefeatedExamine_Close");
        }
        Turn.PushReturnState();
        Turn.SetStateData(new TurnStateData(this.UndefeatedMessage.ToString()));
        Turn.State = GameStateType.Examine;
        Event.Done();
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

