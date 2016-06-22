using System;

public class TutorialConditionQuesting : TutorialCondition
{
    public override bool Evaluate()
    {
        if (UI.Window.Type == WindowType.MainMenu)
        {
            return false;
        }
        if (UI.Window.Type == WindowType.Loading)
        {
            return false;
        }
        return (Game.GameMode == GameModeType.Quest);
    }
}

