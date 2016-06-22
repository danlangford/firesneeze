using System;
using UnityEngine;

public class BlockStateChange : Block
{
    [Tooltip("the next state will be the turn's return state")]
    public bool EnterReturnState;
    [Tooltip("the state to enter")]
    public GameStateType NextState;
    [Tooltip("the return state for flex states")]
    public GameStateType ReturnState;
    [Tooltip("will return to the current state when true")]
    public bool ReturnToCurrentState;

    public override void Invoke()
    {
        if (this.EnterReturnState)
        {
            this.NextState = Turn.PopReturnState();
        }
        if (this.ReturnToCurrentState)
        {
            this.ReturnState = Turn.State;
        }
        if (this.ReturnState != GameStateType.None)
        {
            Turn.PushStateDestination(this.ReturnState);
        }
        if (this.NextState == GameStateType.Recharge)
        {
            Turn.EmptyLayoutDecks = false;
        }
        Turn.State = this.NextState;
    }

    public override bool Stateless =>
        false;
}

