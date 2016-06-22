using System;
using System.Linq;
using UnityEngine;

public class CharacterPowerAddSkillDice : CharacterPower
{
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("additional valid actions to pay the cost... I know Penalty Action should be replaced but we already have lots of prefabs with the data setup")]
    public ActionType[] PenaltyActions;
    [Tooltip("selector for which types to pass into penalty")]
    public CardSelector PenaltyFilter;
    [Tooltip("if set the message to display in the penalty state. Usually optional.")]
    public StrRefType PenaltyMessage;
    [Tooltip("short = help at this location; long = help at remote locations")]
    public DamageRangeType Range = DamageRangeType.Short;
    [Tooltip("additional traits to be added")]
    public TraitType[] Traits;

    public override void Activate()
    {
        base.PowerBegin();
        if (this.Cancellable)
        {
            base.ShowCancelButton(true);
        }
        Turn.CheckBoard.Set<bool>("CharacterPowerAddSkillDice_Used", true);
        if ((this.PenaltyAction != ActionType.Reveal) || (this.PenaltyActions.Length != 1))
        {
            if (this.PenaltyAction != ActionType.None)
            {
                TurnStateData data;
                Turn.EmptyLayoutDecks = false;
                Turn.PushReturnState();
                Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerAddCombatDice_Cancel"));
                if (this.PenaltyFilter != null)
                {
                    CardFilter filter = this.PenaltyFilter.ToFilter();
                    data = new TurnStateData(this.PenaltyAction, filter, 1);
                }
                else
                {
                    data = new TurnStateData(this.PenaltyAction, 1);
                }
                if (this.PenaltyActions.Length > 0)
                {
                    data.Actions = this.PenaltyActions;
                }
                if (!this.PenaltyMessage.IsNullOrEmpty())
                {
                    data.Message = this.PenaltyMessage.ToString();
                }
                Turn.SetStateData(data);
                Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerAddCombatDice_Finish"));
                Turn.State = GameStateType.Power;
            }
            else
            {
                Turn.MarkPowerActive(this, true);
                this.AddDice();
                UI.Window.Refresh();
                this.PowerEnd();
            }
        }
        else
        {
            if (this.PenaltyFilter != null)
            {
                for (int i = 0; i < Turn.Character.Hand.Count; i++)
                {
                    if (this.PenaltyFilter.Match(Turn.Character.Hand[i]))
                    {
                        Turn.Character.Hand[i].Revealed = true;
                        Turn.Character.Hand[i].SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
                        break;
                    }
                }
            }
            else
            {
                Turn.Character.Hand[0].Revealed = true;
                Turn.Character.Hand[0].SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
            }
            Turn.MarkPowerActive(this, true);
            this.AddDice();
            UI.Window.Refresh();
            this.PowerEnd();
        }
    }

    private void AddDice()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        Turn.AddTraits(this.Traits);
        Rules.ApplyCombatAdjustments();
    }

    private void CharacterPowerAddCombatDice_Cancel()
    {
        Turn.EmptyLayoutDecks = false;
        Turn.MarkPowerActive(this, false);
        if (Turn.ReturnState == GameStateType.Recharge)
        {
            Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", true);
        }
        Turn.ReturnToReturnState();
        this.PowerEnd();
        Turn.EmptyLayoutDecks = true;
    }

    private void CharacterPowerAddCombatDice_Finish()
    {
        Turn.MarkPowerActive(this, true);
        if (Turn.ReturnState == GameStateType.Recharge)
        {
            Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", true);
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card cardPlayed = base.GetCardPlayed(this.PenaltyAction);
            if (cardPlayed != null)
            {
                cardPlayed.SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
            }
            this.AddDice();
            window.Refresh();
            Turn.ReturnToReturnState();
        }
        this.PowerEnd();
        Turn.EmptyLayoutDecks = true;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.RemoveDice();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card cardPlayed = base.GetCardPlayed(this.PenaltyAction);
            if (cardPlayed != null)
            {
                if (Turn.Character == base.Character)
                {
                    window.layoutHand.OnGuiDrop(cardPlayed);
                }
                else
                {
                    base.Character.Hand.Add(cardPlayed);
                }
                cardPlayed.RemovePowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
            }
            window.dicePanel.Refresh();
        }
    }

    public override void Initialize(Character self)
    {
        base.InitializeModifier(self, ref this.Traits);
        this.InitializeTypeModifier(self);
        this.InitializePenaltyModifier(self);
        this.InitializeRangeModifier(self);
    }

    private void InitializePenaltyModifier(Character self)
    {
        for (int i = 0; i < self.Powers.Count; i++)
        {
            if (self.Powers[i].Modifies(base.ID))
            {
                CharacterPowerModifier modifier = self.Powers[i] as CharacterPowerModifier;
                if ((modifier != null) && (modifier.AdditionalActions.Length > 0))
                {
                    this.PenaltyActions = modifier.AdditionalActions;
                }
            }
        }
    }

    private void InitializeRangeModifier(Character self)
    {
        for (int i = 0; i < self.Powers.Count; i++)
        {
            if (self.Powers[i].Modifies(base.ID))
            {
                CharacterPowerModifier modifier = self.Powers[i] as CharacterPowerModifier;
                if ((modifier != null) && (modifier.Range != DamageRangeType.None))
                {
                    this.Range = modifier.Range;
                }
            }
        }
    }

    private void InitializeTypeModifier(Character self)
    {
        for (int i = 0; i < self.Powers.Count; i++)
        {
            if (self.Powers[i].Modifies(base.ID))
            {
                CharacterPowerModifier modifier = self.Powers[i] as CharacterPowerModifier;
                if (modifier != null)
                {
                    CardType[] cardTypes = modifier.GetCardTypes();
                    if ((cardTypes != null) && (cardTypes.Length != 0))
                    {
                        for (int j = 0; j < base.Conditions.Length; j++)
                        {
                            if (base.Conditions[j].Condition is PowerConditionCard)
                            {
                                PowerConditionCard condition = base.Conditions[j].Condition as PowerConditionCard;
                                condition.Types = condition.Types.Union<CardType>(cardTypes).ToArray<CardType>();
                                if (!condition.Strict)
                                {
                                    condition.SubTypes = condition.SubTypes.Union<CardType>(cardTypes).ToArray<CardType>();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    protected override bool IsPowerValid()
    {
        if (!Rules.IsCombatCheck() && !Rules.IsNonCombatCheck())
        {
            return false;
        }
        return base.IsPowerValid();
    }

    public override bool IsShortcutAvailable() => 
        ((this.PenaltyAction == ActionType.None) && (this.Range != DamageRangeType.Self));

    public override bool IsValid()
    {
        if (!this.IsPowerValid())
        {
            return false;
        }
        if (!base.IsSharingValid(1))
        {
            return false;
        }
        if (!Rules.IsDiceRollPossible())
        {
            return false;
        }
        if (!Rules.IsRangeValid(base.Character, this.Range))
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if ((this.PenaltyAction != ActionType.None) && (Turn.Character.Hand.Count < 1))
        {
            bool flag = false;
            for (int i = 0; i < this.PenaltyActions.Length; i++)
            {
                if ((this.PenaltyActions[i] == ActionType.FromTheTop) && (Turn.Character.Deck.Count > 0))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
        }
        if (this.PenaltyFilter != null)
        {
            CardFilter filter = this.PenaltyFilter.ToFilter();
            if (Turn.Character.Hand.Filter(filter) < 1)
            {
                return false;
            }
        }
        if (Turn.IsResolved())
        {
            return false;
        }
        return true;
    }

    private void RemoveDice()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Remove(this.Dice[i]);
        }
        Turn.DiceBonus -= this.DiceBonus;
        for (int j = 0; j < this.Traits.Length; j++)
        {
            Turn.DamageTraits.Remove(this.Traits[j]);
        }
        Rules.ApplyCombatAdjustments();
    }

    public override bool Automatic =>
        ((this.PenaltyAction == ActionType.None) && base.Passive);

    public override bool Cancellable =>
        (!base.Cancellable ? false : !Turn.CheckBoard.Get<bool>("PenaltyFromDeck"));

    private ActionType PenaltyAction
    {
        get
        {
            if (this.PenaltyActions.Length == 0)
            {
                return ActionType.None;
            }
            return this.PenaltyActions[0];
        }
    }
}

