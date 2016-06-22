using System;
using UnityEngine;

public class CardPropertyDisplay : CardProperty
{
    [Tooltip("after displaying a card, bury it instead of discarding")]
    public bool BuryOnDisplayDone = true;

    public DeckType GetDeck()
    {
        if (this.BuryOnDisplayDone)
        {
            return DeckType.Bury;
        }
        return DeckType.Discard;
    }
}

