using System;
using UnityEngine;

public class EventOnCombatResolvedCheck : Event
{
    [Tooltip("checks to roll")]
    public SkillCheckValueType[] Checks;
    [Tooltip("If you fail the check run this block")]
    public Block FailBlock;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;

    private void EventOnCombatResolved_Resolve()
    {
        if (!Turn.IsResolveSuccess())
        {
            if (this.FailBlock != null)
            {
                this.FailBlock.Invoke();
                if (this.FailBlock.Stateless)
                {
                    Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "State_FinishDamage"));
                    Turn.State = GameStateType.Recharge;
                }
            }
        }
        else
        {
            Turn.State = GameStateType.Damage;
        }
        Event.Done();
    }

    private void EventRoll_Check()
    {
        Turn.Card.Show(true);
        Turn.ClearCheckData();
        Turn.ClearCombatData();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
            this.SetupDicePanel(window.dicePanel);
            window.dicePanel.Refresh();
        }
        Turn.State = GameStateType.Roll;
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventOnCombatResolved_Resolve"));
    }

    private void EventRoll_End()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            base.RefreshDicePanel();
            window.dicePanel.Clear();
            window.Refresh();
        }
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "State_FinishDamage"));
        Turn.State = GameStateType.Recharge;
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCombatResolved()
    {
        if (this.IsEventValid(Turn.Card))
        {
            if (this.Target == DamageTargetType.Location)
            {
                Turn.Iterators.Start(TurnStateIteratorType.Check);
            }
            this.EventRoll_Check();
        }
    }

    private void SetupDicePanel(GuiPanelDice dicePanel)
    {
        SkillCheckValueType bestSkillCheck = Turn.Owner.GetBestSkillCheck(this.Checks);
        dicePanel.SetCheck(Turn.Card, this.Checks, bestSkillCheck.skill);
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCombatResolved;
}

