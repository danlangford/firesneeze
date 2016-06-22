using System;
using UnityEngine;

public class EventEncounteredRollBlock : Event
{
    [Tooltip("generates the bonus")]
    public NumberGenerator Bonus;
    [Tooltip("the comparison function")]
    public Comparator Comparer;
    [Tooltip("dice used to compute the damage")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("the type of dice this block should show")]
    public RollType DiceRollType;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("the penalty block to invoke")]
    public Block PenaltyBlock;

    public void CheckRoll()
    {
        Turn.Checks = null;
        Turn.ClearCheckData();
        Turn.RollReason = this.DiceRollType;
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        if (this.Bonus != null)
        {
            Turn.DiceBonus = this.Bonus.Generate();
        }
        base.RefreshDicePanel();
        Turn.SetStateData(new TurnStateData(this.Message));
        if (this.IsCardEvent())
        {
            Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventRollResolve"));
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "EventRollResolve"));
        }
        if (Turn.State != GameStateType.Roll)
        {
            Turn.State = GameStateType.Roll;
        }
        else if (!GameStateRoll.CanRollDice(Turn.Dice))
        {
            Turn.Roll(0, 0);
            Turn.Proceed();
        }
    }

    private void EventRollResolve()
    {
        Turn.ClearCheckData();
        if ((this.Comparer != null) && this.Comparer.Compare())
        {
            this.PenaltyBlock.Invoke();
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.messagePanel.Clear();
        }
        if (Event.Finished())
        {
            if (Turn.CombatCheckSequence != 1)
            {
                Turn.State = GameStateType.Combat;
            }
            else
            {
                GameStateEncounter.Continue();
            }
        }
        Event.Done();
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }

    public override void OnCardEncountered(Card card)
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

    public override bool TriggerOnEachCheck =>
        true;

    public override EventType Type =>
        EventType.OnCardEncountered;
}

