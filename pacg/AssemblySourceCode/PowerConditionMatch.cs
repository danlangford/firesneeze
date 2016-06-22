using System;

public class PowerConditionMatch : PowerCondition
{
    public override bool Evaluate(Card card)
    {
        if (Scenario.Current.Discard.Count <= 0)
        {
            return false;
        }
        return (Scenario.Current.Discard[0].ID == card.ID);
    }
}

