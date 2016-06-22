using System;
using UnityEngine;

public class EventLocationChangedPenalty : Event
{
    [Tooltip("amount of cards to perform in the penalty")]
    public int PenaltyAmount = 1;
    [Tooltip("Location which invokes this penalty event")]
    public LocationSelector PenaltyLocation;
    [Tooltip("the type of penalty to invoke when the location matches")]
    public ActionType PenaltyType = ActionType.Discard;

    public override bool IsEventValid(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window == null)
        {
            return false;
        }
        if (window.mapPanel.IsChooseAllowed())
        {
            return this.PenaltyLocation.Match(Party.Characters[Turn.Target].Location);
        }
        return this.PenaltyLocation.Match(Turn.Owner.Location);
    }

    private bool IsStateless()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        return ((window == null) || (!window.mapPanel.IsChooseAllowed() && !this.PenaltyLocation.Match(Location.Current.ID)));
    }

    public override void OnLocationChange()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (window.mapPanel.IsChooseAllowed())
            {
                if (this.PenaltyLocation.Match(Party.Characters[Turn.Target].Location))
                {
                    Turn.PushReturnState();
                    Turn.Evade = false;
                    Turn.BlackBoard.Set<int>("LastIteratedCharacter", Turn.Number);
                    Turn.SwitchCharacter(Turn.Target);
                    Turn.SetStateData(new TurnStateData(this.PenaltyType, this.PenaltyAmount));
                    Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "PenaltyReturn_Proceed"));
                    Turn.State = GameStateType.Penalty;
                }
                else
                {
                    Event.Done();
                }
            }
            else if (this.PenaltyLocation.Match(Turn.Owner.Location))
            {
                Turn.PushReturnState();
                Turn.Evade = false;
                Turn.SetStateData(new TurnStateData(this.PenaltyType, this.PenaltyAmount));
                Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "PenaltyReturn_Done"));
                Turn.State = GameStateType.Penalty;
            }
            else
            {
                Event.Done();
            }
        }
        else
        {
            Event.Done();
        }
    }

    private void PenaltyReturn_Done()
    {
        if (Turn.IsIteratorInProgress())
        {
            Turn.PopReturnState();
            Turn.State = GameStateType.Done;
        }
        else
        {
            Turn.ReturnToReturnState();
        }
        if (Turn.ReturnState != GameStateType.Done)
        {
            if ((Turn.State == GameStateType.Move) || (Turn.State == GameStateType.Null))
            {
                if (Turn.PeekStateDestination() != null)
                {
                    Turn.GotoStateDestination();
                }
                else
                {
                    Turn.ReturnToReturnState();
                }
                Event.Done();
            }
            else
            {
                Event.Done();
            }
        }
    }

    private void PenaltyReturn_Proceed()
    {
        Turn.SwitchCharacter(Turn.BlackBoard.Get<int>("LastIteratedCharacter"));
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
        }
        if (Turn.ReturnState == GameStateType.Setup)
        {
            Turn.ReturnToReturnState();
        }
        else if (Turn.ReturnState == GameStateType.EndTurn)
        {
            Turn.GotoStateDestination();
        }
        else
        {
            Turn.State = GameStateType.Damage;
        }
        Event.Done();
    }

    public override bool Stateless =>
        true;

    public override EventType Type =>
        EventType.OnLocationChange;
}

