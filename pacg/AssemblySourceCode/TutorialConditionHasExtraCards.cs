using System;

public class TutorialConditionHasExtraCards : TutorialCondition
{
    public override bool Evaluate() => 
        ((Party.Characters.Count > 0) && (Turn.Owner.Hand.Count > Turn.Owner.HandSize));
}

