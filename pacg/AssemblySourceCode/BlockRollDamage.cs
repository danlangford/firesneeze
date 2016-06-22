using System;
using UnityEngine;

public class BlockRollDamage : Block
{
    [Tooltip("should the regular combat damage be applied to all players?")]
    public bool DamageAll;
    [Tooltip("dice used to compute the damage")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the damage (total not per dice)")]
    public int DiceBonus;
    [Tooltip("If true make sure not to clobber prev damage information.")]
    public bool EnqueueDamage;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("should this damage override the encounter data? (Prevents ambush state)")]
    public bool PostEncounterDamage;
    [Tooltip("the card type that must be discarded before any other type")]
    public CardType PriorityType;
    [Tooltip("true means damage can be reduced by armor and such")]
    public bool Reducible = true;
    [Tooltip("should we start an iterator roll?")]
    public bool RollForDamage = true;
    [Tooltip("What type of dice should be rolled when determining damage.")]
    public RollType RollReason = RollType.EnemyDamage;
    [Tooltip("the attached card is the source of this damage")]
    public bool SourceIsThis;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;
    [Tooltip("damage traits")]
    public TraitType[] Traits = new TraitType[] { TraitType.Melee };

    public override void Invoke()
    {
        Turn.State = GameStateType.Null;
        Turn.DamageTargetType = this.Target;
        if ((this.Target == DamageTargetType.Location) || (this.Target == DamageTargetType.Party))
        {
            if (this.RollForDamage)
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
        if (this.PostEncounterDamage)
        {
            Turn.CombatStage = TurnCombatStageType.PostEncounter;
        }
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        Turn.RollReason = this.RollReason;
        base.RefreshDicePanel();
        if (Turn.DamageTargetType != DamageTargetType.None)
        {
            if (this.EnqueueDamage)
            {
                Turn.EnqueueDamageData();
            }
            else
            {
                Turn.ClearDamageData();
            }
        }
        Turn.PriorityCardType = this.PriorityType;
        Turn.AddTraits(this.Traits);
        if (((Turn.DamageTargetType != DamageTargetType.Location) && (Location.CountCharactersAtLocation(Location.Current.ID) > 1)) || (this.Target == DamageTargetType.Party))
        {
            Turn.DamageTargetType = this.Target;
        }
        Turn.DamageReduction = this.Reducible;
        Turn.DamageFromEnemy = Turn.Card.IsEnemy();
        if (this.SourceIsThis)
        {
            if (base.Card != null)
            {
                Turn.DamageFromEnemy = base.Card.IsEnemy();
            }
            else
            {
                Turn.DamageFromEnemy = false;
            }
        }
        Turn.Checks = null;
        Turn.Check = SkillCheckType.None;
        Turn.Damage = 0;
        if ((Turn.DamageTargetType == DamageTargetType.Location) && (Location.CountCharactersAtLocation(Location.Current.ID) <= 1))
        {
            Turn.DamageTargetType = DamageTargetType.None;
        }
        if (this.DamageAll)
        {
            Turn.DamageTargetAmount = Turn.LastCombatDamage;
            Turn.DiceBonus += Turn.DamageTargetAmount;
        }
        else if ((Turn.DamageTargetType == DamageTargetType.Location) || (Turn.DamageTargetType == DamageTargetType.Party))
        {
            Turn.DamageTargetAmount = this.DiceBonus;
        }
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventPlayerDamage_Damage"));
        Turn.State = GameStateType.Roll;
    }

    public override bool Stateless =>
        false;
}

