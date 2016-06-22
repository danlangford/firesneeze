using System;

public class GameStateConfirm : GameState
{
    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowExploreButton(false);
            window.ShowConfirmButton(true);
            window.ShowProceedButton(false);
            window.ShowCancelButton(true);
        }
        this.Refresh();
    }

    public override void Exit(GameStateType nextState)
    {
        base.Message((string) null);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowExploreButton(false);
            window.ShowConfirmButton(false);
            window.ShowProceedButton(false);
            window.ShowCancelButton(false);
        }
    }

    protected override string GetHelpText()
    {
        TurnStateData stateData = Turn.GetStateData();
        if ((stateData != null) && (stateData.Message != null))
        {
            return stateData.Message;
        }
        return StringTableManager.GetHelperText(0x43);
    }

    public override void Proceed()
    {
        Turn.GotoStateDestination();
    }

    public override void Refresh()
    {
        base.Message(this.GetHelpText());
    }

    public override GameStateType Type =>
        GameStateType.Confirm;
}

