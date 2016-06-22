using System;
using UnityEngine;

public class EventDefeatedRollBlock : Event
{
    [Tooltip("the card encountered to match")]
    public CardSelector CardSelector;
    [Tooltip("dice used to compute the damage")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("string reference for helper text")]
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
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "EventDefeatedRollResolve"));
        Turn.State = GameStateType.Roll;
    }

    private void EventDefeatedRollResolve()
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
            if (this.NumberToRoll[i] == diceTotal)
            {
                return true;
            }
        }
        return false;
    }

    public override bool IsEventValid(Card card) => 
        this.CardSelector.Match(card);

    public override void OnCardDefeated(Card card)
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
        !this.IsEventValid(Turn.Card);

    public override EventType Type =>
        EventType.OnCardDefeated;
}

