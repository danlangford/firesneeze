using System;

public class TutorialConditionLocationNotAlone : TutorialCondition
{
    public override bool Evaluate()
    {
        int num = 0;
        if (Location.Current != null)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (Party.Characters[i].Location == Location.Current.ID)
                {
                    num++;
                }
            }
        }
        return (num >= 2);
    }
}

