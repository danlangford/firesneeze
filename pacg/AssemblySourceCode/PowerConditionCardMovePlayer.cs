using System;

public class PowerConditionCardMovePlayer : PowerCondition
{
    public override bool Evaluate(Card card) => 
        (card.GetComponent<EventUndefeatedMovePlayer>() != null);
}

