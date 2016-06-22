using System;
using UnityEngine;

public class CardPowerModifyDifficulty : CardPower
{
    [Tooltip("Which check to defeat to modify. Negative means modify all checks to defeat.")]
    public int CheckSequence = -1;
    [Tooltip("restrict the effect to check to defeat/acquire only")]
    public bool CombatOnly;
    [Tooltip("the dice to modify with")]
    public DiceType[] Dice;
    [Tooltip("a flat value to modify the difficulty")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("multiplies the dice total/Dice Bonus before adding")]
    public int Multiplier = 1;
    [Tooltip("the skill check this power is valid for")]
    public SkillCheckType Valid;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            if (this.Dice.Length == 0)
            {
                Turn.Owner.ApplyEffect(new EffectModifyDifficulty(Effect.GetEffectID(this), Effect.DurationCheck, this.DiceBonus * this.Multiplier, this.Valid, CardFilter.Empty, this.CheckSequence, this.CombatOnly));
                this.ResetDicePanel();
            }
            else
            {
                TurnStateCallback.SetActiveCallback(this);
                Turn.EmptyLayoutDecks = false;
                Turn.Checks = null;
                Turn.Check = SkillCheckType.None;
                Turn.DiceTarget = 0;
                Turn.DiceBonus = this.DiceBonus;
                Turn.Dice.Clear();
                Turn.Dice.AddRange(this.Dice);
                Turn.PushReturnState();
                Turn.PushCancelDestination(new TurnStateCallback(base.Card, "CardPowerModifyDifficulty_Cancel"));
                Turn.PushStateDestination(new TurnStateCallback(base.Card, "CardPowerModifyDifficulty_Finish"));
                Turn.SetStateData(new TurnStateData(this.Message));
                Turn.State = GameStateType.Roll;
            }
            Turn.Character.MarkCardType(Rules.GetMarkedType(card), true);
        }
    }

    private void CardPowerModifyDifficulty_Cancel()
    {
        if (TurnStateCallback.IsCallbackRunning(this))
        {
            TurnStateCallback.SetActiveCallback(null);
            base.Card.ActionDeactivate(true);
        }
    }

    private void CardPowerModifyDifficulty_Finish()
    {
        if ((Turn.ReturnState != GameStateType.None) && TurnStateCallback.IsCallbackRunning(this))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Turn.Number = Party.IndexOf(base.Card.PlayedOwner);
                window.ProcessRechargeableCard(Turn.Character, base.Card);
                Turn.Number = Turn.Current;
                window.Refresh();
            }
            Turn.EmptyLayoutDecks = false;
            Turn.Owner.ApplyEffect(new EffectModifyDifficulty(Effect.GetEffectID(this), Effect.DurationCheck, Turn.DiceTotal * this.Multiplier, this.Valid, CardFilter.Empty, this.CheckSequence, this.CombatOnly));
            Turn.ReturnToReturnState();
            Turn.EmptyLayoutDecks = true;
            TurnStateCallback.SetActiveCallback(null);
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card) && this.IsCancelable())
        {
            Turn.EmptyLayoutDecks = false;
            Turn.Owner.RemoveEffect(Effect.GetEffectID(this));
            Turn.Character.MarkCardType(Rules.GetMarkedType(card), false);
            if (this.DiceBonus != 0)
            {
                Turn.DiceBonus = Turn.LastCheckBonus;
            }
            Turn.PopStateDestination();
            Turn.PopCancelDestination();
            if (this.Dice.Length != 0)
            {
                Turn.Dice.Clear();
                Turn.ReturnToReturnState();
            }
            Turn.Validate = true;
            this.ResetDicePanel();
            Turn.EmptyLayoutDecks = true;
        }
    }

    private bool IsCancelable() => 
        ((this.Dice.Length == 0) || (Turn.Character.GetEffect(Effect.GetEffectID(this)) == null));

    protected override bool IsEvadeOrDefeatPowerValid() => 
        true;

    protected override bool IsPowerAllowed(Card card)
    {
        if (!this.IsPowerCheck(card))
        {
            return false;
        }
        if (base.IsCardInPlay(card))
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(Rules.GetMarkedType(card)))
        {
            return false;
        }
        return true;
    }

    private bool IsPowerCheck(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (!Rules.IsCheck())
        {
            return false;
        }
        return true;
    }

    public override bool IsValidationRequired() => 
        false;

    private void ResetDicePanel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.SetCheck(Turn.Card, Turn.Card.Checks, Turn.Check);
        }
    }
}

