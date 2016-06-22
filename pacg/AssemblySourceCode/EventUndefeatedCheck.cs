using System;
using UnityEngine;

public class EventUndefeatedCheck : Event
{
    [Tooltip("check to see if this card should be banished after resolving")]
    public bool BanishAfterResolve;
    [Tooltip("this is the penalty that should be invoked when failing a check")]
    public Block BlockPenalty;
    [Tooltip("this is the block that should be invoked when succeeding a check")]
    public Block BlockSuccess;
    [Tooltip("checks to roll")]
    public SkillCheckValueType[] Checks;
    [Tooltip("true means damage can be reduced by armor and such")]
    public bool Reducible = true;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;

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
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventUndefeatedRollPenalty_Resolve"));
    }

    private void EventRoll_End()
    {
        if (this.BanishAfterResolve || Rules.IsCardSummons(Turn.Card))
        {
            Turn.Card.Disposition = DispositionType.Banish;
        }
        else
        {
            Turn.Card.Disposition = DispositionType.Shuffle;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            base.RefreshDicePanel();
            window.dicePanel.Clear();
        }
        Turn.State = GameStateType.Dispose;
    }

    private void EventUndefeatedRollPenalty_Resolve()
    {
        if (!Turn.IsResolveSuccess())
        {
            if (this.BlockPenalty != null)
            {
                this.BlockPenalty.Invoke();
            }
            if (((this.BlockPenalty == null) || this.BlockPenalty.Stateless) && (Turn.Card.Type != CardType.Villain))
            {
                Turn.PushStateDestination(GameStateType.Dispose);
                Turn.State = GameStateType.Recharge;
            }
        }
        else
        {
            if (this.BlockSuccess != null)
            {
                this.BlockSuccess.Invoke();
            }
            if (((this.BlockSuccess == null) || this.BlockSuccess.Stateless) && (Turn.Card.Type != CardType.Villain))
            {
                Turn.PushStateDestination(GameStateType.Dispose);
                Turn.State = GameStateType.Recharge;
            }
        }
        Event.Done();
    }

    public override void OnCardUndefeated(Card card)
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
        EventType.OnCardUndefeated;
}

