using System;
using UnityEngine;

public class EventUndefeatedRollBlock : Event
{
    [Tooltip("the card encountered to match")]
    public CardSelector CardSelector;
    [Tooltip("dice used to compute the damage")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("Message to display during the roll")]
    public StrRefType Message;
    [Tooltip("the number(s) to roll to trigger block")]
    public int[] NumberToRoll;
    [Tooltip("the penalty block to invoke")]
    public Block PenaltyBlock;
    [Tooltip("reason we are rolling")]
    public RollType RollReason = RollType.StandardEnemyDice;

    public void CheckRoll()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.RollReason = this.RollReason;
        Turn.Checks = null;
        base.RefreshDicePanel();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
        }
        if (!this.Message.IsNullOrEmpty())
        {
            Turn.SetStateData(new TurnStateData(this.Message.ToString()));
        }
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "EventUndefeatedRollResolve"));
        Turn.State = GameStateType.Roll;
    }

    private void EventUndefeatedRollResolve()
    {
        Turn.RollReason = RollType.PlayerControlled;
        if (this.IsDiceTotalValid(Turn.DiceTotal))
        {
            this.PenaltyBlock.Invoke();
        }
        if (this.PenaltyBlock.Stateless)
        {
            Turn.State = GameStateType.Damage;
        }
        Event.Done();
    }

    private bool IsDiceTotalValid(int diceTotal)
    {
        for (int i = 0; i < this.NumberToRoll.Length; i++)
        {
            if (diceTotal == this.NumberToRoll[i])
            {
                return true;
            }
        }
        return false;
    }

    public override bool IsEventValid(Card card) => 
        this.CardSelector.Match(card);

    public override void OnCardUndefeated(Card card)
    {
        if (this.IsEventValid(card))
        {
            this.CheckRoll();
        }
        else
        {
            Event.Done();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

