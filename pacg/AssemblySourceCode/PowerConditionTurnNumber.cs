using System;
using UnityEngine;

public class PowerConditionTurnNumber : PowerCondition
{
    [Tooltip("return true if the current character is the original turn owner (for iterators)")]
    public bool IsOriginalOwner;
    [Tooltip("return true if the current character is the current turn owner")]
    public bool IsTurnOwner;

    public override bool Evaluate(Card card) => 
        ((this.IsOriginalOwner && (Turn.InitialCharacter == Turn.Number)) || (this.IsTurnOwner && Rules.IsTurnOwner()));
}

