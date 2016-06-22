using System;

public class GameStateDone : GameState
{
    public override void Enter()
    {
        if (!Turn.Pass && !Turn.IsIteratorInProgress())
        {
            Turn.SwitchCharacter(Turn.InitialCharacter);
            if (Turn.Current != Turn.Number)
            {
                Turn.Current = Turn.Number;
            }
            Turn.CombatSkill = Turn.Owner.GetCombatSkill();
            UI.Window.Refresh();
        }
        if (Rules.IsCloseInsideClosePossible())
        {
            Turn.PushReturnState();
            Turn.PushStateDestination(GameStateType.ClosePrompt);
            Turn.State = GameStateType.Recharge;
            Turn.CloseType = CloseType.CloseInsideTempClose;
        }
        else if ((!this.IsAnotherCombatRequired() && ((Turn.SummonsType == SummonsType.Close) || (Turn.EncounterType == EncounterType.Close))) && !Turn.Iterators.IsRunning(TurnStateIteratorType.Horde))
        {
            base.ProcessLayoutDecks();
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventLocationCloseSummons_Finish"));
            Turn.State = GameStateType.Recharge;
        }
        else if (Turn.Iterators.Count > 0)
        {
            base.ProcessLayoutDecks();
            Turn.Iterators.Next();
        }
        else if (Turn.End)
        {
            this.Proceed();
        }
        else if (this.IsAnotherCombatRequired())
        {
            base.ProcessLayoutDecks();
            Turn.State = GameStateType.EncounterAgain;
        }
        else if (Turn.SummonsType == SummonsType.Start)
        {
            base.ProcessLayoutDecks();
            Turn.SummonsType = SummonsType.None;
            Turn.State = GameStateType.StartTurn;
        }
        else if (Turn.EncounterType == EncounterType.ReEncounter)
        {
            base.ProcessLayoutDecks();
            Turn.EncounterType = EncounterType.None;
            Turn.LastCombatResult = CombatResultType.Win;
            Turn.ClearEncounterData();
            Turn.State = GameStateType.Encounter;
        }
        else
        {
            if (Turn.EncounterType == EncounterType.EncounterReturn)
            {
                Turn.EncounterType = EncounterType.None;
                Turn.ClearEncounterData();
                if (Turn.PendingDoneEvent == null)
                {
                    Turn.ReturnToReturnState();
                    if (!base.IsCurrentState())
                    {
                        return;
                    }
                }
            }
            if ((Turn.SummonsType == SummonsType.Target) || (Turn.SummonsType == SummonsType.Single))
            {
                base.ProcessLayoutDecks();
                Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventEncounteredSummon_Finish"));
                Turn.State = GameStateType.Recharge;
            }
            else if (Turn.PendingDoneEvent != null)
            {
                base.ProcessLayoutDecks();
                Turn.CombatStage = TurnCombatStageType.Done;
                TurnStateCallback pendingDoneEvent = Turn.PendingDoneEvent;
                Turn.PendingDoneEvent = null;
                Turn.PushStateDestination(pendingDoneEvent);
                Turn.State = GameStateType.Recharge;
            }
            else if (Rules.IsPermanentClosePossible())
            {
                Turn.PushReturnState();
                Turn.State = GameStateType.ClosePrompt;
            }
            else
            {
                Scenario.Current.OnAfterEncounter();
                Location.Current.OnAfterEncounter();
                if (base.IsCurrentState())
                {
                    if (Turn.Explore && (Location.Current.Deck.Count > 0))
                    {
                        Turn.BlackBoard.Set<bool>("CharacterPowerExamine_Encounter", true);
                        this.ShowExplorePopup();
                    }
                    else
                    {
                        Turn.Proceed();
                    }
                }
            }
        }
    }

    private bool IsAnotherCombatRequired() => 
        ((((Turn.CombatCheckSequence == 2) && (Turn.NumCheckSequences > 1)) && (Turn.Card.NumCheckSequences > 1)) && !Turn.Defeat);

    public override void Proceed()
    {
        base.ProcessLayoutDecks();
        Turn.State = GameStateType.Finish;
        Tutorial.Notify(TutorialEventType.TurnResolved);
    }

    private void ShowExplorePopup()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
            window.Popup.Add(StringTableManager.GetHelperText(0x73), new TurnStateCallback(TurnStateCallbackType.Global, "GameStateFinish_ForfeitExplore"));
            window.Popup.Add(StringTableManager.GetHelperText(0x72), new TurnStateCallback(TurnStateCallbackType.Global, "GameStateFinish_Explore"));
            window.Popup.SetDeckPosition(DeckType.Character);
            Turn.State = GameStateType.Popup;
        }
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Done;
}

