using System;

public class TutorialCommandLocationSet : TutorialCommand
{
    public override void Invoke()
    {
        Tutorial.BlackBoard.Set<string>("Location", Location.Current.ID);
    }
}

