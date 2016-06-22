using System;

public class TutorialConditionEncounterBoon : TutorialCondition
{
    public override bool Evaluate() => 
        ((Turn.Card != null) && Turn.Card.IsBoon());
}

