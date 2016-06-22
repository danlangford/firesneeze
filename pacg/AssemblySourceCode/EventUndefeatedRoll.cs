using System;
using UnityEngine;

public class EventUndefeatedRoll : Event
{
    [Tooltip("invoked after the roll")]
    public Block[] Blocks;
    [Tooltip("dice to roll")]
    public DiceType[] Dice;
    [Tooltip("bonus to be added to the dice")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("style of dice to roll")]
    public RollType Style = RollType.EnemyDamage;

    private void EventUndefeatedRoll_Finish()
    {
        for (int i = 0; i < this.Blocks.Length; i++)
        {
            this.Blocks[i].Invoke();
        }
        if (Turn.State == GameStateType.Roll)
        {
            Turn.State = GameStateType.Dispose;
        }
        Event.Done();
    }

    public override void OnCardUndefeated(Card card)
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        Turn.RollReason = this.Style;
        base.RefreshDicePanel();
        Turn.Checks = null;
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventUndefeatedRoll_Finish"));
        Turn.State = GameStateType.Roll;
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

