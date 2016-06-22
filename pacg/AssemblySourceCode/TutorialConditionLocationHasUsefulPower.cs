using System;

public class TutorialConditionLocationHasUsefulPower : TutorialCondition
{
    public override bool Evaluate() => 
        ((Location.Current != null) && Location.Current.CanPlayPower());
}

