using System;
using UnityEngine;

public class EventUndefeatedMoveBlessing : Event
{
    [Tooltip("destination deck for cards")]
    public DeckType Destination = DeckType.Location;
    [Tooltip("number of cards to move from the top of the blessings deck")]
    public int Number = 1;
    [Tooltip("if true, the destination deck will be shuffled after the move")]
    public bool Shuffle = true;

    public override void EndGameIfNecessary(Card card)
    {
        if (this.IsEventValid(card) && (Scenario.Current.Blessings.Count < this.Number))
        {
            this.OnCardUndefeated(card);
        }
    }

    public override bool IsEventValid(Card card)
    {
        if (Turn.LastCombatResult != CombatResultType.Lose)
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCardUndefeated(Card card)
    {
        Deck deck = this.Destination.GetDeck();
        if (deck != null)
        {
            for (int i = 0; i < this.Number; i++)
            {
                if (Scenario.Current.Blessings.Count > 0)
                {
                    deck.Add(Scenario.Current.Blessings[0]);
                    GuiWindowLocation window = UI.Window as GuiWindowLocation;
                    if (window != null)
                    {
                        window.blessingsPanel.Decrement(Scenario.Current.Blessings.Count);
                    }
                }
            }
            if (this.Shuffle)
            {
                VisualEffect.Shuffle(DeckType.Location);
                if (this.Destination == DeckType.Location)
                {
                    deck.ShuffleUnderTop();
                }
                else
                {
                    deck.Shuffle();
                }
            }
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

