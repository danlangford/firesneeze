using System;

public class GameStateAcquire : GameState
{
    public override void Enter()
    {
        this.Proceed();
    }

    public override void Proceed()
    {
        Turn.Card.Disposition = DispositionType.Acquire;
        Turn.State = GameStateType.Dispose;
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Acquire;
}

