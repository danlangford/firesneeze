using System;

public class TutorialConditionHasSmallHand : TutorialCondition
{
    public override bool Evaluate() => 
        (Turn.Owner.Hand.Count < Turn.Owner.HandSize);
}

