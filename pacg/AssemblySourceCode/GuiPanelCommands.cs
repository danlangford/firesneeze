using System;
using UnityEngine;

public class GuiPanelCommands : GuiPanel
{
    [Tooltip("reference to the discard button on this panel")]
    public GuiButton DiscardButton;
    [Tooltip("reference to the end turn button on this panel")]
    public GuiButton EndTurnButton;
    [Tooltip("reference to the give button on this panel")]
    public GuiButton GiveButton;
    [Tooltip("reference to the move button on this panel")]
    public GuiButton MoveButton;

    public override void Initialize()
    {
        if (Turn.BlackBoard.Get<bool>("GuiPanelCommands_GiveButton_Off"))
        {
            this.GiveButton.Disable(true);
        }
        if (Turn.BlackBoard.Get<bool>("GuiPanelCommands_MoveButton_Off"))
        {
            this.MoveButton.Disable(true);
        }
        if (Turn.BlackBoard.Get<bool>("GuiPanelCommands_DiscardButton_Off"))
        {
            this.DiscardButton.Disable(true);
        }
        if (Turn.BlackBoard.Get<bool>("GuiPanelCommands_EndTurn_Off"))
        {
            this.EndTurnButton.Disable(true);
        }
        this.Refresh();
    }

    private void OnDiscardButtonPushed()
    {
        if (!UI.Window.Paused)
        {
            if (Tutorial.Running && base.Locked)
            {
                Tutorial.Notify(TutorialEventType.PanelPhaseBarButtonPushed);
            }
            else if (Rules.IsTurnOwner())
            {
                if (Turn.Map)
                {
                    (UI.Window as GuiWindowLocation).ShowMap(false);
                }
                Turn.Discard = true;
                this.OnEndTurnButtonPushed();
            }
        }
    }

    private void OnEndTurnButtonPushed()
    {
        if (!UI.Window.Paused)
        {
            if (Tutorial.Running && base.Locked)
            {
                Tutorial.Notify(TutorialEventType.PanelPhaseBarButtonPushed);
            }
            else if (Rules.IsTurnOwner())
            {
                if (Turn.Map)
                {
                    (UI.Window as GuiWindowLocation).ShowMap(false);
                }
                Turn.End = true;
                Turn.Phase = TurnPhaseType.End;
                Turn.DestructiveActionsCount = -1;
                Turn.BlackBoard.Set<int>("CancelDiscardDestination", (int) Turn.State);
                Turn.State = GameStateType.Finish;
            }
        }
    }

    private void OnGiveButtonPushed()
    {
        if (!UI.Window.Paused)
        {
            if (Tutorial.Running && base.Locked)
            {
                Tutorial.Notify(TutorialEventType.PanelPhaseBarButtonPushed);
            }
            else if (Rules.IsTurnOwner())
            {
                Turn.Phase = TurnPhaseType.Give;
                Turn.PushStateDestination(GameStateType.Setup);
                Turn.PushCancelDestination(GameStateType.Setup);
                Turn.TargetType = TargetType.AnotherAtLocation;
                Turn.State = GameStateType.Give;
            }
        }
    }

    private void OnMoveButtonPushed()
    {
        if (!UI.Window.Paused)
        {
            if (Tutorial.Running && base.Locked)
            {
                Tutorial.Notify(TutorialEventType.PanelPhaseBarButtonPushed);
            }
            else if (Rules.IsTurnOwner() && Rules.IsMoveLocationPossible())
            {
                Turn.Phase = TurnPhaseType.Move;
                Turn.PushReturnState();
                Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "MapMove_Move"));
                Turn.GotoStateDestination();
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    window.ShowMapButton(false);
                    window.ShowCancelButton(true);
                    Turn.PushCancelDestination(new TurnStateCallback(TurnStateCallbackType.Global, "MapMove_Cancel"));
                }
                this.ShowMoveButton(false);
                this.ShowGiveButton(false);
            }
        }
    }

    public void ShowDiscardButton(bool isVisible)
    {
        this.DiscardButton.Disable(!isVisible);
        Turn.BlackBoard.Set<bool>("GuiPanelCommands_DiscardButton_Off", !isVisible);
    }

    public void ShowEndTurnButton(bool isVisible)
    {
        this.EndTurnButton.Disable(!isVisible);
        Turn.BlackBoard.Set<bool>("GuiPanelCommands_EndTurn_Off", !isVisible);
    }

    public void ShowGiveButton(bool isVisible)
    {
        if (isVisible && !Rules.IsGiveCardPossible())
        {
            isVisible = false;
        }
        this.GiveButton.Disable(!isVisible);
        Turn.BlackBoard.Set<bool>("GuiPanelCommands_GiveButton_Off", !isVisible);
    }

    public void ShowMoveButton(bool isVisible)
    {
        if (isVisible && !Rules.IsMoveLocationPossible())
        {
            isVisible = false;
        }
        this.MoveButton.Disable(!isVisible);
        Turn.BlackBoard.Set<bool>("GuiPanelCommands_MoveButton_Off", !isVisible);
    }
}

