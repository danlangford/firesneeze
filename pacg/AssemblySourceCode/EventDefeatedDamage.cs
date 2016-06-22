using System;
using UnityEngine;

public class EventDefeatedDamage : Event
{
    [Tooltip("dice used to compute the damage")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the damage (total not per dice)")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("true means damage can be reduced by armor and such")]
    public bool Reducible = true;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;
    [Tooltip("damage traits")]
    public TraitType[] Traits = new TraitType[] { TraitType.Melee };

    private void EventDefeatedDamage_Done()
    {
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventPlayerDamage_Damage"));
        Turn.GotoStateDestination();
        Event.Done();
    }

    public override bool IsEventValid(Card card) => 
        base.IsConditionValid(card);

    public override void OnCardDefeated(Card card)
    {
        if (!this.IsEventValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            if (Turn.State == GameStateType.Roll)
            {
                Turn.State = GameStateType.Null;
            }
            for (int i = 0; i < this.Dice.Length; i++)
            {
                Turn.Dice.Add(this.Dice[i]);
            }
            Turn.DiceBonus += this.DiceBonus;
            Turn.RollReason = RollType.EnemyDamage;
            base.RefreshDicePanel();
            Turn.EnqueueDamageData();
            Turn.AddTraits(this.Traits);
            Turn.DamageTargetType = this.Target;
            Turn.DamageReduction = this.Reducible;
            Turn.DamageFromEnemy = Turn.Card.IsEnemy();
            if (this.Target == DamageTargetType.Location)
            {
                Turn.DamageTargetAmount = this.DiceBonus;
                Turn.Iterators.Start(TurnStateIteratorType.Damage);
            }
            Turn.Checks = null;
            Turn.Damage = 0;
            Turn.SetStateData(new TurnStateData(this.Message));
            if (base.IsScenarioEvent())
            {
                Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "EventDefeatedDamage_Done"));
            }
            else
            {
                Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventDefeatedDamage_Done"));
            }
            Turn.EmptyLayoutDecks = false;
            Turn.State = GameStateType.Roll;
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardDefeated;
}

