using System;
using UnityEngine;

public class EventOnCombatResolvedRestartCheck : Event
{
    [Tooltip("the comparator to compare the dice to")]
    public ComparatorDice Comparator;
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("why are we rolling this die")]
    public RollType RollReason;

    private void EventRestartCheck_Resolve()
    {
        Turn.RollReason = RollType.PlayerControlled;
        if (this.Comparator != null)
        {
            if (this.Comparator.Compare())
            {
                Turn.ClearCheckData();
                Turn.ClearCombatData();
                Turn.ClearEncounterData();
                while (Turn.HasDamageData())
                {
                    Turn.DequeueData();
                }
                Turn.Damage = 0;
                Turn.State = GameStateType.Combat;
            }
            else
            {
                Turn.State = GameStateType.Damage;
            }
        }
    }

    public override void OnCombatResolved()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.RollReason = this.RollReason;
        base.RefreshDicePanel();
        Turn.Checks = null;
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventRestartCheck_Resolve"));
        Turn.State = GameStateType.Roll;
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCombatResolved;
}

