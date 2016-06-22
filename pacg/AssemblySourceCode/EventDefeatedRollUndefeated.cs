using System;
using UnityEngine;

public class EventDefeatedRollUndefeated : Event
{
    [Tooltip("dice used for undefeated roll")]
    public DiceType Dice = DiceType.D10;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("the dice roll values in which the monster is undefeated")]
    public int[] RollOn = new int[] { 1 };
    [Tooltip("reason we are rolling")]
    public RollType RollReason;

    private void EventDefeatedRollUndefeated_Roll()
    {
        Turn.RollReason = RollType.PlayerControlled;
        this.LocationDefeatedBane_Roll(Turn.DiceTotal, Turn.Card);
        Turn.State = GameStateType.Damage;
        Event.Done();
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Turn.LastCombatResult != CombatResultType.Win)
        {
            return false;
        }
        if (!card.OnCombatEnd())
        {
            return false;
        }
        return true;
    }

    private void LocationDefeatedBane_Roll(int amount, Card card)
    {
        for (int i = 0; i < this.RollOn.Length; i++)
        {
            if (amount == this.RollOn[i])
            {
                Turn.LastCombatResult = CombatResultType.Lose;
                return;
            }
        }
    }

    public override void OnCombatResolved()
    {
        if (!this.IsEventValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            Turn.Checks = null;
            Turn.Dice.Clear();
            Turn.Dice.Add(this.Dice);
            base.RefreshDicePanel();
            Turn.SetStateData(new TurnStateData(this.Message));
            Turn.PushStateDestination(new TurnStateCallback(this.CallbackType, "EventDefeatedRollUndefeated_Roll"));
            Turn.RollReason = this.RollReason;
            Turn.State = GameStateType.Roll;
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCombatResolved;
}

