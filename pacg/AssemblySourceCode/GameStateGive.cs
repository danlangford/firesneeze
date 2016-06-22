using System;

public class GameStateGive : GameStateTarget
{
    private TurnStateData data;

    public override void Enter()
    {
        this.data = Turn.GetStateData();
        GameStateTarget.DisplayText = StringTableManager.GetUIText(0x110);
        base.Enter();
        base.Message(StringTableManager.GetUIText(0x112));
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
            window.ShowProceedButton(true);
        }
    }

    public override void Exit(GameStateType nextState)
    {
        base.Exit(nextState);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(false);
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        if (action == ActionType.Give)
        {
            if (this.data != null)
            {
                if (Turn.Character.ID == this.data.Owner)
                {
                    return true;
                }
            }
            else if (Rules.IsTurnOwner())
            {
                return true;
            }
        }
        return card.IsActionAllowed(action);
    }

    public override void Proceed()
    {
        base.Message((string) null);
        base.RefreshPanels();
        if (Turn.TargetType == TargetType.AnotherAtLocation)
        {
            Turn.BlackBoard.Set<int>("GiveCardCount", 1);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.commandsPanel.ShowGiveButton(false);
            }
            if (Turn.Phase == TurnPhaseType.Give)
            {
                Turn.Phase = TurnPhaseType.Move;
            }
        }
        Turn.TargetType = TargetType.None;
        Turn.GotoStateDestination();
    }

    public override GameStateType Type =>
        GameStateType.Give;
}

