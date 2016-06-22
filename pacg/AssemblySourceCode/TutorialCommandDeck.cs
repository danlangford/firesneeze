using System;
using UnityEngine;

public class TutorialCommandDeck : TutorialCommand
{
    [Tooltip("list of card ids to add to the deck")]
    public string[] Cards;
    [Tooltip("which character (left blank if not applicable)")]
    public string Character;
    [Tooltip("will clear the deck first if set to true")]
    public bool Clear = true;
    [Tooltip("which deck?")]
    public DeckType DeckType;

    private Deck GetDeck(DeckType type)
    {
        if (type == DeckType.Location)
        {
            return Location.Current.Deck;
        }
        if (type == DeckType.Blessings)
        {
            return Scenario.Current.Blessings;
        }
        if (type == DeckType.BlessingsDiscard)
        {
            return Scenario.Current.Discard;
        }
        int index = Party.IndexOf(this.Character);
        if (index >= 0)
        {
            if (type == DeckType.Character)
            {
                return Party.Characters[index].Deck;
            }
            if (type == DeckType.Discard)
            {
                return Party.Characters[index].Discard;
            }
            if (type == DeckType.Bury)
            {
                return Party.Characters[index].Bury;
            }
            if (type == DeckType.Hand)
            {
                return Party.Characters[index].Hand;
            }
            if (type == DeckType.Revealed)
            {
                return Party.Characters[index].Hand;
            }
        }
        return null;
    }

    public override void Invoke()
    {
        Deck deck = this.GetDeck(this.DeckType);
        if (deck != null)
        {
            this.LockStatusPanel(true);
            if (this.Clear)
            {
                Campaign.Box.Combine(deck);
                deck.Clear();
            }
            for (int i = 0; i < this.Cards.Length; i++)
            {
                Card card = Campaign.Box.Draw(this.Cards[i]);
                if (card == null)
                {
                    card = CardTable.Create(this.Cards[i], "B", null);
                }
                deck.Add(card);
            }
            this.LockStatusPanel(false);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutExplore.Display();
                window.Refresh();
            }
        }
    }

    private void LockStatusPanel(bool isLocked)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.statusPanel.Locked = isLocked;
        }
    }
}

