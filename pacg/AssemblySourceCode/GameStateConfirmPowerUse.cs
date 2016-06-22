using System;

public class GameStateConfirmPowerUse : GameStateConfirmProceed
{
    public override void Enter()
    {
        if (Rules.IsAnyActionPossible())
        {
            base.Enter();
        }
        else
        {
            this.Proceed();
        }
    }

    public override void Proceed()
    {
        Turn.BlackBoard.Set<bool>("ConfirmContinue", true);
        base.Proceed();
    }

    public override GameStateType Type =>
        GameStateType.ConfirmPowerUse;
}

