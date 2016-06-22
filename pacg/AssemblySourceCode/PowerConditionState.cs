using System;
using UnityEngine;

public class PowerConditionState : PowerCondition
{
    [Tooltip("which state are we looking for?")]
    public GameStateType State = GameStateType.Recharge;

    public override bool Evaluate(Card card) => 
        Turn.IsState(this.State);
}

