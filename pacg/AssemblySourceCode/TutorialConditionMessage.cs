using System;

public class TutorialConditionMessage : TutorialCondition
{
    public int ID;

    public override bool Evaluate() => 
        Tutorial.IsMessageDisplayed(this.ID);
}

