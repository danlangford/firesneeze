using System;
using UnityEngine;

public class PowerConditionSummonedEnemy : PowerCondition
{
    [Tooltip("if true power condition returns true if any summon was created")]
    public bool AnySummon;

    public override bool Evaluate(Card card)
    {
        if (this.AnySummon)
        {
            return Rules.IsCardSummons(card);
        }
        return ((Turn.SummonsMonster == Turn.Card.ID) && Rules.IsCardSummons(card));
    }
}

