using System;
using UnityEngine;

public class EventClosedPenaltyDraw : Event
{
    [Tooltip("text for \"do not draw\"")]
    public StrRefType MessageAskNo;
    [Tooltip("text for \"draw\"")]
    public StrRefType MessageAskYes;
    [Tooltip("number of penalty cards")]
    public int PenaltyAmount = 1;
    [Tooltip("what type of penalty is this? discard, etc.")]
    public ActionType PenaltyType = ActionType.Recharge;

    private void EventClosedRechargeDraw_Ask()
    {
        if (Turn.Character.Deck.Count > 0)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.Popup.Clear();
                TurnStateCallback callback = new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedRechargeDraw_Yes");
                window.Popup.Add(this.MessageAskYes.ToString(), callback);
                TurnStateCallback callback2 = new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedRechargeDraw_No");
                window.Popup.Add(this.MessageAskNo.ToString(), callback2);
                window.Popup.SetDeckPosition(DeckType.Character);
                Turn.State = GameStateType.Popup;
            }
        }
        else
        {
            this.EventClosedRechargeDraw_Next();
        }
    }

    private void EventClosedRechargeDraw_Next()
    {
        if (this.NextCharacterAtLocation(Location.Current.ID))
        {
            this.RefreshLocationWindow();
            Turn.SetStateData(new TurnStateData(this.PenaltyType, this.PenaltyAmount));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedRechargeDraw_Ask"));
            Turn.State = GameStateType.Penalty;
        }
        else
        {
            this.RefreshLocationWindow();
            Turn.SwitchCharacter(Turn.Current);
            Turn.State = GameStateType.Done;
            Event.Done();
        }
    }

    private void EventClosedRechargeDraw_No()
    {
        this.EventClosedRechargeDraw_Next();
    }

    private void EventClosedRechargeDraw_Yes()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card card = Turn.Character.Deck.Draw();
            window.Draw(card);
        }
        LeanTween.delayedCall(0.3f, () => this.EventClosedRechargeDraw_Next());
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
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedRechargeDraw_Ask"));
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

