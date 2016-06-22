using System;
using UnityEngine;

public class EventLocationPlayedCard : Event
{
    [Tooltip("filter used to determine what the played card matches")]
    public CardSelector CardSelector;

    private void Clear()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            string name = "CharPlayedCard_" + Party.Characters[i].ID;
            Turn.BlackBoard.Set<string>(name, null);
        }
    }

    public override bool IsEventImplemented(EventType type) => 
        ((type == EventType.OnCardPlayed) || ((type == EventType.OnTurnEnded) || (type == EventType.OnLocationExplored)));

    public override void OnCardDeactivated(Card card)
    {
        if (this.CardSelector.Match(card))
        {
            string playedOwner = card.PlayedOwner;
            if (string.IsNullOrEmpty(playedOwner))
            {
                playedOwner = Turn.Character.ID;
            }
            string name = "CharPlayedCard_" + playedOwner;
            Turn.BlackBoard.Set<string>(name, null);
        }
        Event.Done();
    }

    public override void OnCardPlayed(Card card)
    {
        if (this.CardSelector.Match(card))
        {
            string playedOwner = card.PlayedOwner;
            if (string.IsNullOrEmpty(playedOwner))
            {
                playedOwner = Turn.Character.ID;
            }
            string name = "CharPlayedCard_" + playedOwner;
            Turn.BlackBoard.Set<string>(name, playedOwner);
        }
        Event.Done();
    }

    public override void OnLocationExplored(Card card)
    {
        this.Clear();
    }

    public override void OnTurnEnded()
    {
        this.Clear();
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardPlayed;
}

