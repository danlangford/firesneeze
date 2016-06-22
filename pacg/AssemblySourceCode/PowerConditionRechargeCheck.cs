using System;

public class PowerConditionRechargeCheck : PowerCondition
{
    public override bool Evaluate(Card card)
    {
        if (Turn.State != GameStateType.Recharge)
        {
            return false;
        }
        CardPropertyRecharge component = Turn.Card.GetComponent<CardPropertyRecharge>();
        if ((component != null) && (component.SuccessDestination != ActionType.Recharge))
        {
            return false;
        }
        return true;
    }
}

