using System;

public class TutorialConditionEncounterTrait : TutorialCondition
{
    public TraitType[] Traits;

    public override bool Evaluate()
    {
        if (Turn.Card != null)
        {
            for (int i = 0; i < this.Traits.Length; i++)
            {
                if ((this.Traits[i] != TraitType.None) && !Turn.Card.HasTrait(this.Traits[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

