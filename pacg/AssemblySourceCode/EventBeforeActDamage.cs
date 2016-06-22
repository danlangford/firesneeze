using System;
using UnityEngine;

public class EventBeforeActDamage : Event
{
    [Tooltip("if set the player chooses from these targets to take damage")]
    public TargetType Choice;
    [Tooltip("should this damage the player as well")]
    public bool DamagePlayer = true;
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
    [Tooltip("Bottom text when you are in the target state explaining why you are choosing a character.")]
    public StrRefType TargetMessage;
    [Tooltip("damage traits")]
    public TraitType[] Traits = new TraitType[] { TraitType.Melee };

    private void DamageTarget()
    {
        Turn.TargetType = this.Choice;
        Turn.EmptyLayoutDecks = false;
        GameStateTarget.DisplayText = StringTableManager.Get(this.Message);
        Turn.SetStateData(new TurnStateData(this.TargetMessage));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "RollDamage"));
        Turn.State = GameStateType.Target;
        Turn.EmptyLayoutDecks = true;
    }

    private void EventEncounteredDamage_ChoiceRestore()
    {
        if (this.Choice != TargetType.None)
        {
            Turn.Current = Turn.InitialCharacter;
            Turn.Number = Turn.Current;
            Turn.CombatSkill = Turn.Character.GetCombatSkill();
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.Refresh();
            }
        }
        Event.Done();
    }

    private void EventEncounteredDamage_Damage()
    {
        if (this.Choice != TargetType.None)
        {
            this.SetFinalDestination();
        }
        Turn.CombatStage = TurnCombatStageType.PreEncounter;
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventPlayerDamage_Damage"));
        Turn.GotoStateDestination();
        if (this.Choice == TargetType.None)
        {
            Event.Done();
        }
    }

    private void EventEncounteredDamage_Start()
    {
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventEncounteredDamage_Damage"));
    }

    public override bool IsEventValid(Card card) => 
        base.IsConditionValid(card);

    public override void OnBeforeAct()
    {
        if (this.IsEventValid(Turn.Card))
        {
            Turn.DamageTargetType = this.Target;
            if (this.Target == DamageTargetType.Location)
            {
                if (this.Dice.Length == 0)
                {
                    Turn.Iterators.Start(TurnStateIteratorType.Damage);
                }
                else
                {
                    Turn.Iterators.Start(TurnStateIteratorType.DamageRoll);
                }
            }
            if (!this.DamagePlayer && (this.Target == DamageTargetType.Location))
            {
                this.SetDamageData();
                Turn.Iterators.Next(TurnStateIteratorType.Damage);
            }
            else if (this.Choice != TargetType.None)
            {
                if (Rules.IsTargetRequired(this.Choice))
                {
                    this.DamageTarget();
                }
                else
                {
                    Turn.Target = Turn.Number;
                    this.RollDamage();
                }
            }
            else
            {
                this.RollDamage();
            }
        }
        else
        {
            Event.Done();
        }
    }

    protected void RollDamage()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            if (this.Choice != TargetType.None)
            {
                Turn.SwitchCharacter(Turn.Target);
                Turn.Current = Turn.Target;
            }
            this.SetDamageData();
            Turn.SetStateData(new TurnStateData(this.Message));
            this.EventEncounteredDamage_Start();
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
    }

    private void SetDamageData()
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
        Turn.Checks = null;
        Turn.Damage = 0;
        if (this.Target == DamageTargetType.Location)
        {
            Turn.DamageTargetAmount = this.DiceBonus;
        }
    }

    private void SetFinalDestination()
    {
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventEncounteredDamage_ChoiceRestore"));
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

