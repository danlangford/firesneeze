using System;
using UnityEngine;

public class LocationPowerCheckEvent : LocationPower
{
    [Tooltip("the block to invoke if the check is passed")]
    public Block BlockEvent;
    [Tooltip("checks to roll")]
    public SkillCheckValueType[] Checks;
    [Tooltip("if true this event will allow an evasion")]
    public bool IsEvasionEvent;
    [Tooltip("the turn card must match this selector")]
    public CardSelector Selector;

    public override void Activate()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            base.PowerBegin();
            Turn.PushReturnState();
            window.CancelAllPowers(true, true);
            Turn.State = GameStateType.Null;
            Turn.ClearCheckData();
            Turn.ClearCombatData();
            this.SetupDicePanel(window.dicePanel);
            window.dicePanel.Refresh();
            Turn.State = GameStateType.Roll;
            Turn.PushCancelDestination(new TurnStateCallback(this, "EventCheckBlock_Cancel"));
            Turn.PushStateDestination(new TurnStateCallback(this, "EventCheckBlock_Resolve"));
            window.ShowCancelButton(true);
        }
    }

    private void EventCheckBlock_Cancel()
    {
        this.PowerEnd();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.CancelAllPowers(true, true);
        }
        Turn.ReturnToReturnState();
    }

    private void EventCheckBlock_Resolve()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        if (Turn.IsResolveSuccess())
        {
            this.BlockEvent.Invoke();
        }
        else
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowCancelButton(false);
            }
            Turn.ReturnToReturnState();
        }
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!this.Selector.Match(Turn.Card))
        {
            return false;
        }
        if (this.IsEvasionEvent)
        {
            if (!Rules.IsEvadePossible(Turn.Card))
            {
                return false;
            }
        }
        else if (!Rules.IsCombatCheck() && !Rules.IsNonCombatCheck())
        {
            return false;
        }
        return true;
    }

    private void SetupDicePanel(GuiPanelDice dicePanel)
    {
        SkillCheckValueType bestSkillCheck = Turn.Owner.GetBestSkillCheck(this.Checks);
        dicePanel.SetCheck(Turn.Card, this.Checks, bestSkillCheck.skill);
    }

    public override PowerType Type
    {
        get
        {
            if (this.IsEvasionEvent)
            {
                return PowerType.Evade;
            }
            return PowerType.None;
        }
    }
}

