using System;
using UnityEngine;

public class CharacterPowerReroll : CharacterPower
{
    [Tooltip("the difference this power can overcome")]
    public int Difference = 1;
    [Tooltip("message to display when you can activate this power")]
    public StrRefType HelperText;
    [Tooltip("the penalty paid for activating this power")]
    public ActionType Penalty;
    [Tooltip("the amount of Penalty cards used")]
    public int PenaltyAmount;
    [Tooltip("optional only allow specific cards used to pay the penalty")]
    public CardSelector Selector;

    public override void Activate()
    {
        base.Activate();
        base.PowerBegin();
        if (this.IsFree())
        {
            this.CharacterPowerReroll_Activate();
        }
        else
        {
            Turn.EmptyLayoutDecks = false;
            Turn.PushReturnState();
            Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerReroll_Activate"));
            Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerReroll_Cancel"));
            if (this.Selector != null)
            {
                Turn.SetStateData(new TurnStateData(this.Penalty, this.Selector.ToFilter(), this.PenaltyAmount));
            }
            else
            {
                Turn.SetStateData(new TurnStateData(this.Penalty, CardFilter.Empty, this.PenaltyAmount));
            }
            Turn.State = GameStateType.Power;
        }
    }

    private void CharacterPowerReroll_Activate()
    {
        Turn.Roll(0, (Turn.DiceTotal - Turn.DiceBonus) + this.Difference);
        if (!this.IsFree())
        {
            Card cardPlayed = base.GetCardPlayed(this.Penalty);
            if (cardPlayed != null)
            {
                cardPlayed.SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
            }
            Turn.ReturnToReturnState();
        }
        base.ShowDice(false);
        Turn.EmptyLayoutDecks = false;
    }

    private void CharacterPowerReroll_Cancel()
    {
        this.Deactivate();
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.PowerEnd();
        Turn.Roll(0, (Turn.DiceTarget - Turn.DiceBonus) - this.Difference);
        base.ShowDice(true);
    }

    private bool IsFree() => 
        ((this.Penalty == ActionType.None) || (this.PenaltyAmount == 0));

    public bool IsRerollPossible()
    {
        if ((this.Penalty != ActionType.None) && (Turn.Character.GetNumberDiscardableCards() <= 0))
        {
            return false;
        }
        return ((Turn.DiceTarget - Turn.DiceTotal) <= this.Difference);
    }

    public override bool IsValid()
    {
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!this.IsRerollPossible())
        {
            return false;
        }
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        return ((Turn.State == GameStateType.Reroll) || (Turn.State == GameStateType.RollAgain));
    }

    public override void OnCheckCompleted()
    {
        this.PowerEnd();
    }
}

