using System;
using UnityEngine;

public class EventClosedPenalty : Event
{
    [Tooltip("number of penalty cards")]
    public int PenaltyAmount = 1;
    [Tooltip("what type of penalty is this? discard, etc.")]
    public ActionType PenaltyType = ActionType.Discard;

    private void EventClosedPenalty_Finish()
    {
        if (this.NextCharacterAtLocation(Location.Current.ID))
        {
            this.RefreshLocationWindow();
            Turn.SetStateData(new TurnStateData(this.PenaltyType, this.PenaltyAmount));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPenalty_Finish"));
            Turn.State = GameStateType.Penalty;
        }
        else
        {
            this.RefreshLocationWindow();
            Turn.SwitchCharacter(Turn.Current);
            Turn.State = GameStateType.Done;
        }
        Event.Done();
    }

    private bool NextCharacterAtLocation(string locID)
    {
        int number = Turn.Number;
        int num2 = 0;
        while (num2++ < Party.Characters.Count)
        {
            number++;
            if (number >= Party.Characters.Count)
            {
                number = 0;
            }
            if (number == Turn.Current)
            {
                return false;
            }
            if (Party.Characters[number].Alive && (Party.Characters[number].Location == locID))
            {
                Turn.SwitchCharacter(number);
                return true;
            }
        }
        return false;
    }

    public override void OnLocationClosed()
    {
        this.RefreshLocationWindow();
        Turn.SetStateData(new TurnStateData(this.PenaltyType, this.PenaltyAmount));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPenalty_Finish"));
        Turn.State = GameStateType.Penalty;
    }

    private void RefreshLocationWindow()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessLayoutDecks();
            window.Refresh();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnLocationClosed;
}

