using System;
using UnityEngine;

public class EventDefeatedCheck : Event
{
    [Tooltip("this is the penalty that should be invoked when failing a check")]
    public Block BlockPenalty;
    [Tooltip("this is the action that should be invoked when passing a check")]
    public Block BlockSuccess;
    [Tooltip("checks to roll")]
    public SkillCheckValueType[] Checks;
    [Tooltip("true means damage can be reduced by armor and such")]
    public bool Reducible = true;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;

    private void EventDefeatedRollPenalty_Resolve()
    {
        Turn.PushStateDestination(GameStateType.Dispose);
        if (Turn.IsResolveSuccess())
        {
            if (this.BlockSuccess != null)
            {
                this.BlockSuccess.Invoke();
            }
        }
        else if (this.BlockPenalty != null)
        {
            this.BlockPenalty.Invoke();
        }
        Event.Done();
        if (Turn.State == GameStateType.Roll)
        {
            Turn.State = GameStateType.Recharge;
        }
    }

    private void EventRoll_Check()
    {
        Turn.State = GameStateType.Null;
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
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventDefeatedRollPenalty_Resolve"));
        Turn.State = GameStateType.Roll;
    }

    private void EventRoll_End()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            base.RefreshDicePanel();
            window.dicePanel.Clear();
        }
        Turn.State = GameStateType.Damage;
        Event.Done();
    }

    public override void OnCardDefeated(Card card)
    {
        if (this.Target == DamageTargetType.Location)
        {
            Turn.Iterators.Start(TurnStateIteratorType.Check);
        }
        this.EventRoll_Check();
    }

    private void SetupDicePanel(GuiPanelDice dicePanel)
    {
        SkillCheckValueType bestSkillCheck = Turn.Owner.GetBestSkillCheck(this.Checks);
        dicePanel.SetCheck(Turn.Card, this.Checks, bestSkillCheck.skill);
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardDefeated;
}

