using System;

public class GameStateNull : GameState
{
    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Null;
}

