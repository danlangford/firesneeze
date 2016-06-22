using System;

public class TutorialConditionLocationVillain : TutorialCondition
{
    public override bool Evaluate()
    {
        if ((Location.Current != null) && (Location.Current.Deck != null))
        {
            for (int i = 0; i < Location.Current.Deck.Count; i++)
            {
                if (Location.Current.Deck[i].Type == CardType.Villain)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

