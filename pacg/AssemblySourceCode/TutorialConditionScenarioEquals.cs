using System;

public class TutorialConditionScenarioEquals : TutorialCondition
{
    public string ID;

    public override bool Evaluate() => 
        ((Scenario.Current != null) && (Scenario.Current.ID == this.ID));
}

