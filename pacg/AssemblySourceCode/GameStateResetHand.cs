using System;

public class GameStateResetHand : GameStateReset
{
    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
            window.ShowCancelButton(false);
        }
        Location.Current.OnHandReset();
        if (base.IsCurrentState())
        {
            Game.Instance.StartCoroutine(this.DrawCards());
        }
    }

    public override void Proceed()
    {
        UI.Window.Refresh();
        Turn.GotoStateDestination();
    }

    protected override void Reset_Finish()
    {
        if (!Turn.Owner.Alive)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.deathPanel.Show(true);
            }
            Turn.LastCombatResult = CombatResultType.Lose;
        }
        else
        {
            this.Proceed();
        }
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.ResetHand;
}

