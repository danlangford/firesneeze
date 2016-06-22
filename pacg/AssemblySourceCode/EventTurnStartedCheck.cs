using System;
using UnityEngine;

public class EventTurnStartedCheck : Event
{
    [Tooltip("this is the penalty that should be invoked when failing a check")]
    public Block BlockPenalty;
    [Tooltip("this is the reward that should be invoked when succeeding a check")]
    public Block BlockSuccess;
    [Tooltip("checks to roll")]
    public SkillCheckValueType[] Checks;

    private void ClearDicePanel()
    {
        Turn.ClearCheckData();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Clear();
        }
    }

    private void EventTurnStartedCheck_Resolve()
    {
        if (Turn.DiceTotal < Turn.DiceTarget)
        {
            this.ClearDicePanel();
            if (this.BlockPenalty != null)
            {
                this.BlockPenalty.Invoke();
            }
            if (this.BlockPenalty.Stateless || (this.BlockPenalty == null))
            {
                Turn.State = GameStateType.StartTurn;
            }
        }
        else
        {
            this.ClearDicePanel();
            if (this.BlockSuccess != null)
            {
                this.BlockSuccess.Invoke();
            }
            if ((this.BlockSuccess == null) || this.BlockSuccess.Stateless)
            {
                Turn.State = GameStateType.StartTurn;
            }
        }
        Event.Done();
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }

    public override void OnTurnStarted()
    {
        if (!this.IsEventValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowMap(false);
                this.SetupDicePanel(window.dicePanel, this.Checks);
                window.dicePanel.Refresh();
            }
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventTurnStartedCheck_Resolve"));
            Turn.State = GameStateType.Roll;
        }
    }

    private void SetupDicePanel(GuiPanelDice dicePanel, SkillCheckValueType[] checks)
    {
        SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(checks);
        dicePanel.SetCheck(Location.Current.Card, checks, bestSkillCheck.skill);
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnTurnStarted;
}

