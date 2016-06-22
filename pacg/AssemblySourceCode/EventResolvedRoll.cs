using System;
using UnityEngine;

public class EventResolvedRoll : Event
{
    [Tooltip("block invoked when roll was failed")]
    public Block BlockFailure;
    [Tooltip("block invoked when roll was success")]
    public Block BlockSuccess;
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("cards that match this filter will be resolved")]
    public CardSelector Selector;
    [Tooltip("the number to get on the dice")]
    public int Total = 1;

    private void EventResolvedRoll_Finish()
    {
        if (Turn.DiceTotal == this.Total)
        {
            if (this.BlockSuccess != null)
            {
                this.BlockSuccess.Invoke();
            }
        }
        else if (this.BlockFailure != null)
        {
            this.BlockFailure.Invoke();
        }
    }

    public override bool OnCombatEnd(Card card)
    {
        if ((this.Selector != null) && !this.Selector.Match(card))
        {
            return true;
        }
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        base.RefreshDicePanel();
        Turn.Checks = null;
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventResolvedRoll_Finish"));
        Turn.State = GameStateType.Roll;
        return false;
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCombatEnd;
}

