using System;

public class GameStateSetup : GameState
{
    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        if (Rules.IsGiveCardPossible())
        {
            Turn.Phase = TurnPhaseType.Give;
        }
        else if (Rules.IsMoveLocationPossible())
        {
            Turn.Phase = TurnPhaseType.Move;
        }
        else
        {
            Turn.Phase = TurnPhaseType.Explore;
        }
        Turn.CombatStage = TurnCombatStageType.PreEncounter;
        Turn.Explore = true;
        Turn.Pass = true;
        if (Scenario.Current.IsScenarioOver())
        {
            Turn.State = GameStateType.End;
        }
        else
        {
            base.Enter();
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowExploreButton(true);
                window.ShowProceedButton(false);
                base.ShowAidButton();
                window.commandsPanel.ShowMoveButton(Rules.IsMoveLocationPossible());
                window.ShowMapButton(true);
                window.commandsPanel.ShowGiveButton(true);
                window.commandsPanel.ShowDiscardButton(true);
                window.commandsPanel.ShowEndTurnButton(true);
                if (Rules.IsPermanentClosePossible())
                {
                    window.closeLocationPanel.Show(CloseType.Permanent);
                }
            }
            Tutorial.Notify(TutorialEventType.StateSetupStart);
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowMapButton(false);
            window.commandsPanel.ShowMoveButton(false);
            window.commandsPanel.ShowGiveButton(false);
            window.commandsPanel.ShowDiscardButton(false);
            window.commandsPanel.ShowEndTurnButton(false);
            if (window.closeLocationPanel.Visible)
            {
                window.closeLocationPanel.Show(CloseType.None);
            }
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    public override void Proceed()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && Rules.IsExplorePromptNecessary())
        {
            window.Popup.Clear();
            window.Popup.SetDeckPosition(DeckType.Character);
            window.Popup.Add(StringTableManager.GetUIText(0x107), new TurnStateCallback(GameStateType.Setup));
            window.Popup.Add(StringTableManager.GetUIText(0x108), new TurnStateCallback(GameStateType.Finish));
            Turn.PushStateDestination(GameStateType.Popup);
            Turn.State = GameStateType.Recharge;
        }
        else
        {
            Turn.State = GameStateType.Finish;
        }
    }

    public override GameStateType Type =>
        GameStateType.Setup;
}

