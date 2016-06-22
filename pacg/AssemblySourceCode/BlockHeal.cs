using System;
using UnityEngine;

public class BlockHeal : Block
{
    [Tooltip("Amount of random cards to move from the initial character's discard to deck")]
    public int Amount = 1;
    [Tooltip("if true it'll heal the current player instead of the initial character")]
    public bool HealCurrent;
    [Tooltip("Position the healed cards go")]
    public DeckPositionType Position = DeckPositionType.Shuffle;

    private Character GetCharacter()
    {
        if (this.HealCurrent)
        {
            return Party.Characters[Turn.Number];
        }
        return Party.Characters[Turn.InitialCharacter];
    }

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Character character = this.GetCharacter();
            character.Discard.Shuffle();
            int num = Mathf.Clamp(this.Amount, 0, character.Discard.Count);
            Card[] cards = new Card[num];
            for (int i = 0; i < num; i++)
            {
                cards[i] = character.Discard[i];
            }
            if (this.IsCurrentCharacter())
            {
                window.Heal(character, cards, this.Position);
            }
            else
            {
                for (int j = 0; j < cards.Length; j++)
                {
                    window.Recharge(cards[j], this.Position);
                }
            }
        }
    }

    private bool IsCurrentCharacter() => 
        (this.HealCurrent || (Turn.InitialCharacter == Turn.Number));
}

