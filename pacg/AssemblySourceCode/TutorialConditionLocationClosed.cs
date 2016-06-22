using System;

public class TutorialConditionLocationClosed : TutorialCondition
{
    public override bool Evaluate() => 
        ((Location.Current != null) && Location.Current.Closed);
}

