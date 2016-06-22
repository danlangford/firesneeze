using System;

public class GameStateTempClose : GameState
{
    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.tempClosePanel.Show(true);
        }
    }

    public override GameStateType Type =>
        GameStateType.TempClose;
}

