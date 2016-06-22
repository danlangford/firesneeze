using System;
using UnityEngine;

public class TutorialConditionPartyTurn : TutorialCondition
{
    [Tooltip("the ID of the character whose turn it should be")]
    public string ID;

    public override bool Evaluate() => 
        ((Turn.Owner != null) && (Turn.Owner.ID == this.ID));
}

