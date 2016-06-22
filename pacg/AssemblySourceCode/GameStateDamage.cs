using System;

public class GameStateDamage : GameState
{
    private bool CanProceed(GuiWindowLocation window) => 
        ((window != null) && ((window.layoutDiscard.Deck.Count >= Turn.Damage) || (Turn.Character.GetNumberDiscardableCards() <= 0)));

    private void ClearDamageData()
    {
        if (!Turn.Iterators.IsRunning(TurnStateIteratorType.Damage) && !Turn.Iterators.IsRunning(TurnStateIteratorType.DamageRoll))
        {
            Turn.Damage = 0;
            Turn.DamageTraits.Clear();
            Turn.DamageReduction = false;
            Turn.PriorityCardType = CardType.None;
        }
    }

    public override void Enter()
    {
        base.Enter();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Show(false);
            window.layoutLocation.Show(true);
            window.layoutLocation.Refresh();
        }
        while ((Turn.Damage <= 0) && Turn.HasDamageData())
        {
            Turn.DequeueData();
            base.ApplyDecorations(true);
        }
        if ((Turn.Damage > 0) && (Turn.Character.GetNumberDiscardableCards() > 0))
        {
            Turn.CombatDelta = -Turn.Damage;
            this.Refresh();
            EffectMirrorImage effect = Turn.Character.GetEffect(EffectType.MirrorImage) as EffectMirrorImage;
            if ((effect != null) && effect.IsDamageAvoidPossible(Turn.Card))
            {
                effect.Invoke();
            }
            if (base.IsCurrentState())
            {
                if (Turn.PriorityCardType == CardType.None)
                {
                    UI.Sound.Play(SoundEffectType.DamagedChooseCards);
                }
                else
                {
                    UI.Sound.Play(SoundEffectType.DamagePriorityType);
                }
            }
            Tutorial.Notify(TutorialEventType.StateDamageStart);
        }
        else
        {
            this.Proceed();
        }
    }

    public override void Exit(GameStateType nextState)
    {
        base.Exit(nextState);
        if (Turn.Damage > 0)
        {
            base.Message((string) null);
        }
        if (nextState != GameStateType.Examine)
        {
            this.ClearDamageData();
        }
        if ((nextState != GameStateType.Roll) && (nextState != GameStateType.Examine))
        {
            EffectMirrorImage effect = Turn.Character.GetEffect(EffectType.MirrorImage) as EffectMirrorImage;
            if (effect != null)
            {
                effect.Clear();
            }
        }
    }

    protected override string GetHelpText() => 
        string.Format(StringTableManager.GetHelperText(0x44), Rules.GetCardsToDiscardCount());

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        if (action != ActionType.Discard)
        {
            return card.IsActionAllowed(action);
        }
        if (((Turn.PriorityCardType != CardType.None) && (card.Type != Turn.PriorityCardType)) && (Turn.Character.Hand.Filter(Turn.PriorityCardType) > 0))
        {
            return card.IsActionAllowed(action);
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Rules.GetCardsToDiscardCount() <= 0)
        {
            return false;
        }
        return true;
    }

    public override void Proceed()
    {
        Turn.DamageDiscarded = (UI.Window as GuiWindowLocation).layoutDiscard.Deck.Count;
        Turn.ClearCombatData();
        if (Turn.Damage > 0)
        {
            Scenario.Current.OnDamageTaken(Turn.Card);
            Turn.Card.OnDamageTaken(Turn.Card);
            if (!base.IsCurrentState())
            {
                return;
            }
        }
        bool flag = this.CanProceed(UI.Window as GuiWindowLocation);
        base.SaveRechargableCards();
        if (Turn.HasDamageData())
        {
            Turn.State = GameStateType.Null;
            Turn.PushStateDestination(GameStateType.Damage);
            Turn.State = GameStateType.Recharge;
        }
        else
        {
            this.ClearDamageData();
            if (Turn.PeekStateDestination() != null)
            {
                Turn.EmptyLayoutDecks = false;
                Turn.GotoStateDestination();
                Turn.EmptyLayoutDecks = true;
                flag = this.CanProceed(UI.Window as GuiWindowLocation);
            }
            if (base.IsCurrentState() && flag)
            {
                Turn.Number = Turn.Current;
                Turn.PushStateDestination(GameStateType.Post);
                Turn.State = GameStateType.Recharge;
            }
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if ((Turn.SwitchType == SwitchType.AidAll) || (Turn.SwitchType == SwitchType.Aid))
            {
                base.Message(this.GetHelpText());
                window.ShowProceedButton(true);
                window.GlowLayoutDeck(ActionType.Discard, false);
            }
            else if (this.CanProceed(window))
            {
                base.Message((string) null);
                window.ShowProceedButton(true);
                window.GlowLayoutDeck(ActionType.Discard, false);
            }
            else if (Rules.IsTurnOwner())
            {
                base.Message(this.GetHelpText());
                window.ShowProceedButton(false);
                window.GlowLayoutDeck(ActionType.Discard, true);
            }
            else
            {
                base.Message(this.GetHelpText());
                window.ShowProceedButton(false);
                window.GlowLayoutDeck(ActionType.Discard, false);
            }
        }
    }

    public override GameStateType Type =>
        GameStateType.Damage;
}

