using System;
using UnityEngine;

public class BlockPenalty : Block
{
    [Tooltip("the state to enter after the penalty")]
    public GameStateType NextState = GameStateType.Setup;
    [Tooltip("the type of penalty")]
    public ActionType PenaltyAction = ActionType.Bury;
    [Tooltip("the number of penalty cards")]
    public int PenaltyAmount = 1;
    [Tooltip("what cards are allowed to pay the penalty")]
    public CardSelector Selector;

    public override void Invoke()
    {
        if (this.Selector != null)
        {
            Turn.SetStateData(new TurnStateData(this.PenaltyAction, this.Selector.ToFilter(), this.PenaltyAmount));
        }
        else
        {
            Turn.SetStateData(new TurnStateData(this.PenaltyAction, this.PenaltyAmount));
        }
        TurnStateCallback callback = Turn.PeekStateDestination();
        if (callback != null)
        {
            if (callback.CallbackState != this.NextState)
            {
                Turn.PushStateDestination(this.NextState);
            }
        }
        else
        {
            Turn.PushStateDestination(this.NextState);
        }
        Turn.State = GameStateType.Penalty;
    }

    public override bool Stateless =>
        false;
}

