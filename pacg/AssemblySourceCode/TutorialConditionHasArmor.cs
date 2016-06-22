using System;

public class TutorialConditionHasArmor : TutorialCondition
{
    public override bool Evaluate()
    {
        if (Party.Characters.Count > 0)
        {
            for (int i = 0; i < Turn.Owner.Hand.Count; i++)
            {
                if (Turn.Owner.Hand[i].Type == CardType.Armor)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

