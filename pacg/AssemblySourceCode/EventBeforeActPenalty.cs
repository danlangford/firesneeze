using System;
using UnityEngine;

public class EventBeforeActPenalty : Event
{
    [Tooltip("number of penalty cards")]
    public int Amount = 1;
    [Tooltip("should this event count as a \"forced action\" for triggers?")]
    public bool Forced = true;
    [Tooltip("helper text displayed in the penalty state")]
    public StrRefType Message;
    [Tooltip("the type of penalty action to perform")]
    public ActionType PenaltyType;
    [Tooltip("position to add the cards in the deck")]
    public DeckPositionType Position;

    private void EventEncounteredPenalty_Done()
    {
        Turn.State = GameStateType.Combat;
        Event.Done();
    }

    public override void OnBeforeAct()
    {
        if (this.Amount > Turn.Character.HandSize)
        {
            this.Amount = Turn.Character.HandSize;
        }
        if ((this.PenaltyType == ActionType.Recharge) || (this.PenaltyType == ActionType.Top))
        {
            Turn.RechargePositionType = this.Position;
            if (this.Forced)
            {
                Turn.RechargeReason = GameReasonType.MonsterForced;
            }
        }
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventEncounteredPenalty_Done"));
        TurnStateData data = new TurnStateData(this.PenaltyType, this.Amount) {
            Message = this.Message.ToString()
        };
        Turn.SetStateData(data);
        Turn.State = GameStateType.Penalty;
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

