using System;
using UnityEngine;

public class ScenarioPowerDeckSize : ScenarioPower
{
    [Tooltip("the deck to be modified")]
    public DeckType DeckToModify = DeckType.Blessings;
    [Tooltip("the number of cards adjustment")]
    public int Delta = -5;

    public int GetSizeModifier(DeckType deckType)
    {
        if (deckType == this.DeckToModify)
        {
            return this.Delta;
        }
        return 0;
    }
}

