using System;
using UnityEngine;

public class CharacterPowerPenaltyActivateBlock : CharacterPower
{
    [Tooltip("the amount of price to pay")]
    public int Amount = 1;
    [Tooltip("the block to activate")]
    public Block Block;
    [Tooltip("if false this ability will automatically activate whether players want it or not")]
    public bool Optional = true;
    [Tooltip("the price to pay")]
    public ActionType Penalty = ActionType.Discard;
    public DamageRangeType Range;
    [Tooltip("the allowed cards for the price")]
    public CardSelector Selector;

    public override void Activate()
    {
        base.PowerBegin();
        if (!this.IsFree())
        {
            Turn.PushReturnState();
            Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerPenaltyActivateBlock_Activate"));
            Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerPenaltyActivateBlock_Deactivate"));
            Turn.SetStateData(new TurnStateData(this.Penalty, (this.Selector == null) ? CardFilter.Empty : this.Selector.ToFilter(), this.Amount));
            Turn.State = GameStateType.Penalty;
            if (this.Cancellable)
            {
                base.ShowCancelButton(true);
            }
        }
        else
        {
            this.CharacterPowerPenaltyActivateBlock_Activate();
        }
    }

    private void CharacterPowerPenaltyActivateBlock_Activate()
    {
        base.Activate();
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
        if (!this.IsFree())
        {
            Turn.ReturnToReturnState();
        }
        this.PowerEnd();
    }

    private void CharacterPowerPenaltyActivateBlock_Deactivate()
    {
        base.Deactivate();
        Turn.ReturnToReturnState();
        this.PowerEnd();
    }

    private bool IsFree() => 
        ((this.Amount == 0) && (this.Penalty == ActionType.None));

    public override bool IsValid()
    {
        if ((this.Selector != null) && (this.Selector.Filter(base.Character.Hand) <= 0))
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!Rules.IsRangeValid(base.Character, this.Range))
        {
            return false;
        }
        return base.IsConditionValid(Turn.Card);
    }

    public override bool Automatic =>
        (base.Passive && !this.Optional);

    public override PowerType Type
    {
        get
        {
            if ((this.Block != null) && (this.Block is BlockEvade))
            {
                return PowerType.Evade;
            }
            return base.Type;
        }
    }
}

