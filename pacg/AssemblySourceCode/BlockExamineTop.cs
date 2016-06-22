using System;
using UnityEngine;

public class BlockExamineTop : Block
{
    [Tooltip("after close button activated run this")]
    public Block CloseBlock;
    [Tooltip("which locations we examine the top card and do stuff etc.")]
    public LocationSelector LocSelector;
    [Tooltip("number of cards to reveal from this location")]
    public int RevealAmount;
    [Tooltip("after revealing cards run this")]
    public Block RevealBlock;
    [Tooltip("if you match this selector banish the top card(s)")]
    public CardSelector Selector;

    private void BlockDetectEvil_CloseBlock()
    {
        if (this.CloseBlock != null)
        {
            this.CloseBlock.Invoke();
        }
        Location.Load(Turn.Owner.Location);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Clear();
            window.Refresh();
        }
    }

    private void BlockDetectEvil_RevealBlock()
    {
        if (this.RevealBlock != null)
        {
            this.RevealBlock.Invoke();
        }
    }

    public override void Invoke()
    {
        string str = this.LocSelector.Random();
        if (!string.IsNullOrEmpty(str))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Location.Load(str);
                window.Refresh();
                window.locationPanel.Show(str);
                int amountFromTop = Mathf.Min(this.RevealAmount, Location.Current.Deck.Count);
                window.layoutExamine.Mode = ExamineModeType.Reveal;
                window.layoutExamine.Source = DeckType.Location;
                window.layoutExamine.RevealPosition = DeckPositionType.Top;
                window.layoutExamine.Number = amountFromTop;
                window.layoutExamine.ModifyTop = false;
                window.layoutExamine.ModifyBottom = false;
                window.layoutExamine.Shuffle = this.ShouldShuffle(amountFromTop);
                window.layoutExamine.RevealCallback = new TurnStateCallback(TurnStateCallbackType.Scenario, "BlockDetectEvil_RevealBlock");
                window.layoutExamine.CloseCallback = new TurnStateCallback(TurnStateCallbackType.Scenario, "BlockDetectEvil_CloseBlock");
                if (this.Selector != null)
                {
                    window.layoutExamine.Sort = this.Selector.ToFilter();
                }
                Turn.PushReturnState(GameStateType.StartTurn);
                Turn.State = GameStateType.Examine;
            }
            Turn.EmptyLayoutDecks = true;
        }
    }

    private bool ShouldShuffle(int amountFromTop)
    {
        for (int i = 0; i < amountFromTop; i++)
        {
            if (!this.Selector.Match(Location.Current.Deck[i]))
            {
                return true;
            }
        }
        return false;
    }

    public override bool Stateless =>
        false;
}

