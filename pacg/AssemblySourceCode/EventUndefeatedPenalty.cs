using System;
using UnityEngine;

public class EventUndefeatedPenalty : Event
{
    [Tooltip("number of penalty cards")]
    public int Amount = 1;
    private CardFilter filter;
    [Tooltip("should this event count as a \"forced action\" for triggers?")]
    public bool Forced = true;
    [Tooltip("used for card filters during penalty state")]
    public CardSelector PenaltyFilter;
    [Tooltip("the type of penalty action to perform")]
    public ActionType PenaltyType;
    [Tooltip("position to add the cards in the deck")]
    public DeckPositionType Position;

    public override void OnCardUndefeated(Card card)
    {
        TurnStateData data;
        if (this.PenaltyType == ActionType.Recharge)
        {
            Turn.RechargePositionType = this.Position;
            if (this.Forced)
            {
                Turn.RechargeReason = GameReasonType.MonsterForced;
            }
        }
        if (this.PenaltyFilter != null)
        {
            this.filter = this.PenaltyFilter.ToFilter();
            data = new TurnStateData(this.PenaltyType, this.filter, this.Amount);
        }
        else
        {
            data = new TurnStateData(this.PenaltyType, this.Amount);
        }
        Turn.SetStateData(data);
        if (Turn.Owner.Hand.Filter(this.filter) <= 0)
        {
            Turn.State = GameStateType.Dispose;
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "State_FinishTurn"));
            Turn.State = GameStateType.Penalty;
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

