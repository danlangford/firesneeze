using System;

public class TutorialConditionExplore : TutorialCondition
{
    public override bool Evaluate() => 
        Rules.IsExplorePossible();
}

