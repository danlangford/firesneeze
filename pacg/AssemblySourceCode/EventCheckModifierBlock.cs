using System;
using UnityEngine;

public class EventCheckModifierBlock : Event
{
    [Tooltip("the amount to modify the check by")]
    public int Modifier = 2;

    public override int GetCheckModifier()
    {
        if (this.IsEventValid(Turn.Card))
        {
            if (Turn.CheckBoard.Get<bool>("BlockModifyCheck1") && (Turn.CombatCheckSequence == 1))
            {
                return this.Modifier;
            }
            if (Turn.CheckBoard.Get<bool>("BlockModifyCheck2") && (Turn.CombatCheckSequence == 2))
            {
                return this.Modifier;
            }
        }
        return 0;
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }
}

