using System;

public class GameStateExamine : GameState
{
    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            base.Enter();
            UI.Window.Pause(true);
            if ((Turn.GetStateData() != null) && (Turn.GetStateData().Message != null))
            {
                window.messagePanel.Show(Turn.GetStateData().Message);
            }
            window.ShowProceedButton(false);
            window.ShowCancelButton(false);
            window.dicePanel.Show(false);
            window.layoutExamine.Show(true);
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Show(false);
            window.messagePanel.Clear();
            UI.Window.Pause(false);
            window.dicePanel.Show(true);
        }
    }

    public override void Proceed()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (window.layoutExamine.Finish)
            {
                Turn.State = GameStateType.Damage;
            }
            else
            {
                if (Turn.EncounterType == EncounterType.EncounterReturn)
                {
                    Turn.EncounterType = EncounterType.None;
                }
                Turn.PushStateDestination(Turn.PopReturnState());
                Turn.State = GameStateType.Recharge;
            }
        }
    }

    public override GameStateType Type =>
        GameStateType.Examine;
}

