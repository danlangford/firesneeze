using System;

public class TutorialConditionConquestQuest : TutorialCondition
{
    public override bool Evaluate() => 
        Conquests.IsComplete(Constants.QUEST_MODE_UNLOCKED);
}

