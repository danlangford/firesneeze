using System;
using UnityEngine;

public class BlockExamineAcquireBoon : Block
{
    [Tooltip("examine all cards in the deck")]
    public bool All;
    [Tooltip("block executed when the panel is closed")]
    public Block BlockClose;
    [Tooltip("number of cards to examine")]
    public int Number = 1;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("true means that a shuffle animation will play when the closed")]
    public bool Shuffle;

    private void BlockExamineAcquireBoon_Close()
    {
        this.BlockClose.Invoke();
    }

    public override void Invoke()
    {
        if (this.IsBlockValid())
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutExamine.Source = DeckType.Location;
                window.layoutExamine.Finish = true;
                window.layoutExamine.Shuffle = this.Shuffle;
                window.layoutExamine.Choose = CardFilter.Boon;
                window.layoutExamine.CloseCallback = new TurnStateCallback(base.Card, "BlockExamineAcquireBoon_Close");
                Campaign.Box.Add(Turn.Card, false);
                if (this.All)
                {
                    window.layoutExamine.Scroll = true;
                    window.layoutExamine.Mode = ExamineModeType.All;
                }
                else
                {
                    window.layoutExamine.Number = Mathf.Min(this.Number, this.Deck.Count);
                    window.layoutExamine.RevealPosition = this.Position;
                    window.layoutExamine.Mode = ExamineModeType.Reveal;
                }
                Turn.PushReturnState();
                Turn.State = GameStateType.Examine;
            }
        }
    }

    private bool IsBlockValid() => 
        (this.Deck.Count >= 2);

    private Deck Deck =>
        Location.Current.Deck;

    public override bool Stateless =>
        !this.IsBlockValid();
}

