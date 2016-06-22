using System;
using UnityEngine;

public class EventUndefeatedDamage : Event
{
    [Tooltip("should the regular combat damage be applied to all players at location?")]
    public bool DamageAll;
    [Tooltip("should location damage also affect the current character?")]
    public bool DamagePlayer = true;
    [Tooltip("dice used to compute the damage")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the damage (total not per dice)")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("true means damage can be reduced by armor and such")]
    public bool Reducible = true;
    [Tooltip("should the players roll for damage or be dealt direct damage")]
    public bool RollforDamage;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;
    [Tooltip("damage traits")]
    public TraitType[] Traits = new TraitType[] { TraitType.Combat };

    private void EventUndefeatedDamage_Done()
    {
        bool flag = Turn.Iterators.IsRunning(TurnStateIteratorType.Damage) || Turn.Iterators.IsRunning(TurnStateIteratorType.DamageRoll);
        if ((this.Target == DamageTargetType.Player) || (!flag && this.DamagePlayer))
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventDone"));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventPlayerDamage_Damage"));
            Turn.GotoStateDestination();
        }
        else if ((this.Target == DamageTargetType.Location) && this.DamagePlayer)
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventDone"));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventPlayerDamage_Damage"));
            Turn.GotoStateDestination();
        }
        else if (((this.Target == DamageTargetType.Location) && !this.DamagePlayer) && flag)
        {
            Turn.Iterators.Next(TurnStateIteratorType.Damage);
        }
        else
        {
            Turn.PushStateDestination(GameStateType.Post);
            Turn.State = GameStateType.Recharge;
            Turn.RollReason = RollType.PlayerControlled;
            Event.Done();
        }
    }

    public override void OnCardUndefeated(Card card)
    {
        Turn.DamageTargetType = this.Target;
        if (this.Target == DamageTargetType.Location)
        {
            if (this.RollforDamage)
            {
                Turn.Iterators.Start(TurnStateIteratorType.DamageRoll);
            }
            else
            {
                Turn.Iterators.Start(TurnStateIteratorType.Damage);
            }
        }
        this.RollDamage();
    }

    public void RollDamage()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            for (int i = 0; i < this.Dice.Length; i++)
            {
                Turn.Dice.Add(this.Dice[i]);
            }
            Turn.DiceBonus += this.DiceBonus;
            Turn.RollReason = RollType.EnemyDamage;
            base.RefreshDicePanel();
            Turn.AddTraits(this.Traits);
            Turn.DamageTargetType = this.Target;
            Turn.DamageReduction = this.Reducible;
            Turn.DamageFromEnemy = Turn.Card.IsEnemy();
            if (this.Target == DamageTargetType.Location)
            {
                if (this.DamageAll)
                {
                    Turn.DamageTargetAmount = Turn.LastCombatDamage;
                }
                else
                {
                    Turn.DamageTargetAmount = this.DiceBonus;
                }
            }
            Turn.Checks = null;
            Turn.Damage = 0;
            Turn.SetStateData(new TurnStateData(this.Message));
            Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventUndefeatedDamage_Done"));
            Turn.State = GameStateType.Roll;
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

