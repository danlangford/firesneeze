using System;

public class TutorialConditionHasUsefulPower : TutorialCondition
{
    public bool AnyPower = true;
    public string PowerID;

    public override bool Evaluate()
    {
        if (this.AnyPower)
        {
            return Turn.Owner.CanPlayPower();
        }
        for (int i = 0; i < Turn.Owner.Powers.Count; i++)
        {
            if (Turn.Owner.Powers[i].ID == this.PowerID)
            {
                return Turn.Owner.Powers[i].IsValid();
            }
        }
        return false;
    }
}

