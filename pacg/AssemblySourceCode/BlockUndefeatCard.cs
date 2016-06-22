using System;

public class BlockUndefeatCard : Block
{
    public override void Invoke()
    {
        Turn.Card.OnUndefeated();
        Turn.State = GameStateType.Damage;
    }

    public override bool Stateless =>
        false;
}

