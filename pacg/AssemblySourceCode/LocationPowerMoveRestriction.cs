using System;
using UnityEngine;

public class LocationPowerMoveRestriction : LocationPowerBaseMoveRestriction
{
    [Tooltip("the player must make a check to move")]
    public SkillCheckValueType[] Checks;

    public override void Activate()
    {
        base.PowerBegin();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (window.mapPanel.IsChooseAllowed() && (Turn.Target != Turn.Current))
            {
                Turn.EmptyLayoutDecks = true;
                Turn.SwitchCharacter(Turn.Target);
                Turn.Current = Turn.Target;
            }
            Turn.Target = Turn.Current;
            SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(this.Checks);
            window.dicePanel.SetCheck(Location.Current.Card, this.Checks, bestSkillCheck.skill);
            window.Refresh();
            window.ShowMap(false);
        }
        Turn.RollReason = RollType.PlayerControlled;
        if (Turn.Phase == TurnPhaseType.Move)
        {
            Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerMoveRestriction_Cancel"));
        }
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerMoveRestriction_Resolve"));
        Turn.State = GameStateType.Roll;
    }

    private void DisableMoveButton()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.BlackBoard.Set<int>("GiveCardCount", 1);
            window.commandsPanel.ShowGiveButton(false);
            Turn.BlackBoard.Set<int>("MoveLocationCount", 1);
            window.commandsPanel.ShowMoveButton(false);
        }
    }

    public override bool IsValid()
    {
        if (Turn.Defeat)
        {
            return false;
        }
        return base.IsValid();
    }

    private void LocationPowerMoveRestriction_Cancel()
    {
        if (Turn.ReturnState != GameStateType.None)
        {
            this.PowerEnd();
            Turn.PopStateDestination();
            Turn.PopCancelDestination();
            Turn.EmptyLayoutDecks = true;
            Turn.ReturnToReturnState();
        }
    }

    private void LocationPowerMoveRestriction_Failure()
    {
        this.RestoreTarget();
        this.PowerEnd();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Location.Load(Turn.Owner.Location);
            window.Refresh();
        }
        if (Turn.Phase == TurnPhaseType.Move)
        {
            this.DisableMoveButton();
        }
        Turn.PopStateDestination();
        Turn.PopCancelDestination();
        if (((Turn.PeekStateDestination() != null) && !string.IsNullOrEmpty(Turn.PeekStateDestination().CallbackCardID)) && Turn.PeekStateDestination().CallbackCardID.StartsWith("PW"))
        {
            Turn.GotoStateDestination();
        }
        else
        {
            Turn.PopStateDestination();
            Turn.ReturnToReturnState();
        }
    }

    private void LocationPowerMoveRestriction_Resolve()
    {
        if (Turn.IsResolveSuccess())
        {
            this.LocationPowerMoveRestriction_Success();
        }
        else
        {
            Turn.EmptyLayoutDecks = true;
            Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerMoveRestriction_Failure"));
            Turn.State = GameStateType.Recharge;
        }
    }

    private void LocationPowerMoveRestriction_Success()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            int target = Turn.Target;
            Turn.State = GameStateType.Null;
            Turn.Target = target;
            Turn.GotoStateDestination();
        }
        this.RestoreTarget();
        this.PowerEnd();
    }

    private void RestoreTarget()
    {
        Turn.Target = Turn.Current;
        if (!Turn.IsIteratorInProgress())
        {
            Turn.SwitchCharacter(Turn.InitialCharacter);
            Turn.Current = Turn.InitialCharacter;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.partyPanel.Refresh();
        }
    }
}

