using System;

public class TutorialConditionEncounterBane : TutorialCondition
{
    public override bool Evaluate() => 
        ((Turn.Card != null) && Turn.Card.IsBane());
}

