using System;

public class GameStateSwitch : GameState
{
    public override void Enter()
    {
        this.Proceed();
    }

    public override void Proceed()
    {
        if (Turn.Iterators.Count > 0)
        {
            Turn.Iterators.Invoke();
        }
        else
        {
            if (Game.GameType == GameType.LocalMultiPlayer)
            {
                Game.UI.SwitchPanel.Show(true);
            }
            Turn.State = GameStateType.Blessing;
        }
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Switch;
}

