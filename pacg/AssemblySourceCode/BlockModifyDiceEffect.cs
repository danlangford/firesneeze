using System;
using UnityEngine;

public class BlockModifyDiceEffect : Block
{
    [Tooltip("the amount to change the difficulty")]
    public int Amount = -1;
    [Tooltip("only activate this block if this conditional filter matches the current card")]
    public CardSelector ConditionalSelector;
    [Tooltip("number of turns")]
    public int Duration = 1;
    [Tooltip("replace Filter's inclusions with the current card")]
    public bool IncludeTurnCard;
    [Tooltip("filter for which cards should this block modify")]
    public CardSelector Selector;

    public override void Invoke()
    {
        if (this.IsValid())
        {
            if (this.IncludeTurnCard)
            {
                this.Selector.CardIDs = new string[] { Turn.Card.ID };
            }
            EffectModifyDice e = new EffectModifyDice(base.Card.ID, this.Duration, this.Amount, this.Selector.ToFilter());
            Turn.Owner.ApplyEffect(e);
        }
    }

    private bool IsValid()
    {
        if (Turn.Card == null)
        {
            return false;
        }
        if ((this.ConditionalSelector != null) && !this.ConditionalSelector.Match(Turn.Card))
        {
            return false;
        }
        return true;
    }
}

