using System;

public class TutorialConditionLocationDifferent : TutorialCondition
{
    public override bool Evaluate()
    {
        string str = Tutorial.BlackBoard.Get<string>("Location");
        return (Location.Current.ID != str);
    }
}

