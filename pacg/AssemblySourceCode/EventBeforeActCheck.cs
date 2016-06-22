using System;
using UnityEngine;

public class EventBeforeActCheck : Event
{
    [Tooltip("this is the penalty that should be invoked when failing a check")]
    public Block BlockPenalty;
    [Tooltip("this is the reward that should be invoked when passing a check")]
    public Block BlockSuccess;
    [Tooltip("checks to roll")]
    public SkillCheckValueType[] Checks;
    [Tooltip("who is affected by the damage")]
    public DamageTargetType Target = DamageTargetType.Player;

    private void EventEncounteredRollPenalty_Resolve()
    {
        Party.OnCheckCompleted();
        bool stateless = true;
        if (!Turn.IsResolveSuccess())
        {
            if (this.BlockPenalty != null)
            {
                this.BlockPenalty.Invoke();
                stateless = this.BlockPenalty.Stateless;
            }
        }
        else if (this.BlockSuccess != null)
        {
            this.BlockSuccess.Invoke();
            stateless = this.BlockSuccess.Stateless;
        }
        if (stateless)
        {
            if (this.Target == DamageTargetType.Player)
            {
                Turn.PushStateDestination(GameStateType.Combat);
                Turn.State = GameStateType.Recharge;
            }
            else if ((this.Target == DamageTargetType.Location) || (this.Target == DamageTargetType.Party))
            {
                if (Turn.Iterators.IsRunning(TurnStateIteratorType.Check))
                {
                    Turn.Iterators.Next();
                }
                else
                {
                    Turn.PushStateDestination(GameStateType.Combat);
                    Turn.State = GameStateType.Recharge;
                }
            }
        }
        Event.Done();
    }

    private void EventRoll_Check()
    {
        Turn.Defeat = false;
        Turn.Evade = false;
        Turn.ClearCheckData();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
            window.ShowCancelButton(false);
            this.SetupDicePanel(window.dicePanel, this.Checks);
            window.dicePanel.Refresh();
        }
        if (this.IsCardEvent())
        {
            Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventEncounteredRollPenalty_Resolve"));
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(this.CallbackType, "EventEncounteredRollPenalty_Resolve"));
        }
        if (Turn.State != GameStateType.Roll)
        {
            Turn.State = GameStateType.Roll;
        }
    }

    private void EventRoll_End()
    {
        if (Turn.State == GameStateType.Combat)
        {
            Turn.State = GameStateType.Damage;
        }
        Turn.Current = Turn.InitialCharacter;
        Turn.Number = Turn.Current;
        if ((this.Target == DamageTargetType.Party) && this.IsCardEvent())
        {
            Turn.FocusedCard = null;
            if (Location.Current.Deck[0].Disposition == DispositionType.Destroy)
            {
                Card card = Location.Current.Deck[0];
                Location.Current.Deck.Remove(card);
                card.Destroy();
            }
            Location.Current.Deck.Add(base.Card, DeckPositionType.Top);
        }
        this.FinishEventEncounteredCheck(true);
        UI.Window.Refresh();
    }

    private void FinishEventEncounteredCheck(bool stateless)
    {
        if (Event.Finished() && stateless)
        {
            Turn.DamageTargetType = DamageTargetType.Player;
            Turn.PushStateDestination(GameStateType.Combat);
            Turn.State = GameStateType.Recharge;
        }
        Event.Done();
    }

    public override bool IsEventValid(Card card) => 
        base.IsConditionValid(card);

    public override void OnBeforeAct()
    {
        if (this.IsEventValid(Turn.Card))
        {
            if (this.Target != DamageTargetType.Player)
            {
                Turn.DamageTargetType = this.Target;
                Turn.Iterators.Start(TurnStateIteratorType.Check, this.CallbackType);
                if ((this.Target == DamageTargetType.Party) && this.IsCardEvent())
                {
                    Deck deck = base.Card.Deck;
                    deck.Remove(base.Card);
                    deck.Add(CardTable.Create(base.Card.ID), DeckPositionType.Top);
                    deck[0].Disposition = DispositionType.Destroy;
                    Turn.FocusedCard = base.Card;
                    Turn.FocusedCard.Show(CardSideType.Front);
                }
            }
            this.EventRoll_Check();
        }
        else
        {
            Event.Done();
        }
    }

    private void SetupDicePanel(GuiPanelDice dicePanel, SkillCheckValueType[] checks)
    {
        SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(checks);
        dicePanel.SetCheck(Turn.Card, checks, bestSkillCheck.skill);
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

