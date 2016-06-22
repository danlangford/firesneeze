using System;

public class TutorialConditionEncounterVillain : TutorialCondition
{
    public override bool Evaluate() => 
        (((Turn.Card != null) && (Scenario.Current != null)) && Scenario.Current.IsCurrentVillain(Turn.Card.ID));
}

