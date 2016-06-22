using System;

public class TutorialConditionPartyCanHelp : TutorialCondition
{
    public override bool Evaluate()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].ID != Turn.Owner.ID) && (Party.Characters[i].CanPlayCard() || Party.Characters[i].CanPlayPower()))
            {
                return true;
            }
        }
        return false;
    }
}

