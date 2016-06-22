using System;

public class TutorialConditionLocationEquals : TutorialCondition
{
    public string ID;

    public override bool Evaluate() => 
        ((Location.Current != null) && (Location.Current.ID == this.ID));
}

