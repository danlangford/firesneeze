using System;
using UnityEngine;

public class EventDefeatedRollDamage : Event
{
    [Tooltip("amount of damage to apply if a 1 is rolled")]
    public int DamageAmount = 1;
    [Tooltip("dice used to compute the damage")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("the number to roll in order to receive damage")]
    public int NumberToRoll = 1;
    [Tooltip("true means damage can be reduced by armor and such")]
    public bool Reducible = true;
    [Tooltip("reason for rolling this dice")]
    public RollType RollReason;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;
    [Tooltip("damage traits")]
    public TraitType[] Traits = new TraitType[] { TraitType.Melee };

    private void EventScenarioDamage()
    {
        Turn.EmptyLayoutDecks = true;
        Turn.RollReason = RollType.PlayerControlled;
        if (Turn.DiceTotal == this.NumberToRoll)
        {
            if ((this.Target == DamageTargetType.Location) && (Location.CountCharactersAtLocation(Location.Current.ID) > 1))
            {
                Turn.Iterators.Start(TurnStateIteratorType.Damage);
            }
            Turn.Damage = this.DamageAmount;
            Turn.DamageTargetAmount = Turn.Damage;
            UI.Sound.Play(SoundEffectType.MeleeDamage);
            VisualEffect.ApplyToPlayer(VisualEffectType.CardLoseEnemy, 1.3f);
            Turn.Card.Animate(AnimationType.Attack, true);
            Event.DonePost(GameStateType.Damage);
        }
        else
        {
            Turn.DamageTraits.Clear();
            Turn.DamageTargetType = DamageTargetType.None;
            Turn.DamageReduction = false;
            Turn.PriorityCardType = CardType.None;
            if (Event.Finished())
            {
                Event.DonePost(GameStateType.Post);
            }
            else
            {
                Event.Done();
            }
        }
    }

    public override void OnCardDefeated(Card card)
    {
        if (base.IsConditionValid(card))
        {
            this.RollDamage();
        }
        else
        {
            Event.Done();
        }
    }

    public void RollDamage()
    {
        Turn.State = GameStateType.Null;
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        base.RefreshDicePanel();
        Turn.EnqueueDamageData();
        Turn.AddTraits(this.Traits);
        Turn.DamageTargetType = this.Target;
        Turn.DamageReduction = this.Reducible;
        Turn.DamageFromEnemy = Turn.Card.IsEnemy();
        Turn.Checks = null;
        Turn.Damage = 0;
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "EventScenarioDamage"));
        Turn.EmptyLayoutDecks = false;
        Turn.RollReason = this.RollReason;
        Turn.State = GameStateType.Roll;
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardDefeated;
}

