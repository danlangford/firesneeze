using System;
using UnityEngine;

public class CardPropertyModifyDice : CardProperty
{
    [Tooltip("the number to modify the dice result to")]
    public int AdjustedValue;
    [Tooltip("the specific dice type that needs to be modified")]
    public DiceType DiceType;
    [Tooltip("determines if the card should be discarded after modifying the result")]
    public bool DiscardCard;
    [Tooltip("the specific roll result required to modify")]
    public int ExpectedValue;

    public bool Affects(int roll, DiceType diceType) => 
        ((this.DiceType == diceType) && (roll == this.ExpectedValue));

    public void ProcessDiscard()
    {
        if (this.DiscardCard)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (((window != null) && window.layoutReveal.Deck.Contains(this.Card)) && !window.layoutDiscard.Deck.Contains(this.Card))
            {
                window.DiscardToLayout(this.Card);
            }
        }
    }

    public Card Card =>
        base.GetComponent<Card>();
}

