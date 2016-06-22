using System;

public class GameStateCombat : GameState
{
    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        if (Rules.IsCardDefeatedAutomatically(Turn.Card) || Rules.IsCardAcquiredAutomatically(Turn.Card))
        {
            Turn.Defeat = true;
            this.Resolve();
        }
        else
        {
            base.Enter();
            Turn.RollReason = RollType.PlayerControlled;
            Turn.NumCheckSequences = Turn.Card.NumCheckSequences;
            Turn.EmptyLayoutDecks = true;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutLocation.ShowPreludeFX(false);
                window.dicePanel.Show(true);
                window.layoutLocation.Show(true);
                window.layoutLocation.Refresh();
                if (!Turn.Defeat)
                {
                    this.SetupDicePanel(window.dicePanel);
                    window.dicePanel.Refresh();
                }
                if (Turn.Defeat || Turn.Evade)
                {
                    window.dicePanel.Show(false);
                    window.ShowProceedButton(true);
                }
                else
                {
                    window.ShowProceedButton(false);
                }
            }
            base.ShowAidButton();
            if (Turn.CombatStage != TurnCombatStageType.Encounter)
            {
                Turn.CombatStage = TurnCombatStageType.Encounter;
                if (Rules.IsCombatPossible() && (window != null))
                {
                    window.ShowCombatCheckAnimation();
                }
            }
            Tutorial.Notify(TutorialEventType.StateCombatStart);
        }
    }

    public override void Exit(GameStateType nextState)
    {
        base.Exit(nextState);
        if ((((((nextState != GameStateType.Reroll) && (nextState != GameStateType.Pick)) && (nextState != GameStateType.Roll)) && (nextState != GameStateType.Power)) && (nextState != GameStateType.Flee)) && (Turn.ReturnState != GameStateType.Combat))
        {
            Turn.ClearCheckData();
        }
    }

    private CombatResultType GetCombatResult()
    {
        if (Turn.Evade)
        {
            return CombatResultType.None;
        }
        if (this.IsResolveSuccess())
        {
            return CombatResultType.Win;
        }
        return CombatResultType.Lose;
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        EffectCardRestriction effect = Turn.Character.GetEffect(EffectType.CardRestriction) as EffectCardRestriction;
        if ((effect != null) && effect.Match(card))
        {
            return false;
        }
        return card.IsActionAllowed(action);
    }

    public override bool IsResolveSuccess()
    {
        if (Turn.Defeat)
        {
            return true;
        }
        if (!Turn.Card.IsDefeatable())
        {
            return false;
        }
        return (Turn.DiceTotal >= Turn.DiceTarget);
    }

    public override void Proceed()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (Turn.Defeat && (window != null))
        {
            window.dicePanel.ShowResolveResult(true);
        }
        if (window != null)
        {
            Turn.ClearCheckData();
            window.dicePanel.Clear();
            window.layoutLocation.Show(false);
        }
        CombatResultType lastCombatResult = Turn.LastCombatResult;
        CombatResultType combatResult = this.GetCombatResult();
        Turn.LastCombatResult = combatResult;
        if (((Turn.EncounterType == EncounterType.LocationDefeat) && (combatResult == CombatResultType.Win)) && (lastCombatResult == CombatResultType.Lose))
        {
            Turn.LastCombatResult = CombatResultType.Lose;
        }
        if (Turn.Evade || Turn.Defeat)
        {
            Party.OnCheckCompleted();
        }
        if (Turn.Defeat)
        {
            Scenario.Current.OnCombatResolved(Turn.Card);
            Location.Current.OnCombatResolved(Turn.Card);
            Turn.Card.OnCombatResolved();
        }
        if (base.IsCurrentState())
        {
            Turn.State = GameStateType.Damage;
        }
    }

    public override void Resolve()
    {
        CombatResultType lastCombatResult = Turn.LastCombatResult;
        CombatResultType combatResult = this.GetCombatResult();
        Turn.LastCombatResult = combatResult;
        if ((!Turn.Defeat && (((Turn.Card.NumCheckSequences > 1) && (Turn.CombatCheckSequence > 1)) || (Turn.EncounterType == EncounterType.LocationDefeat))) && ((combatResult == CombatResultType.Win) && (lastCombatResult == CombatResultType.Lose)))
        {
            Turn.LastCombatResult = CombatResultType.Lose;
        }
        if ((combatResult == CombatResultType.Lose) && Turn.Card.IsEnemy())
        {
            if (Turn.Damage > 0)
            {
                Turn.EnqueueDamageData();
            }
            Turn.DamageTraits.Clear();
            Turn.AddTraits(Turn.Card.GetDamageTraits());
            Turn.DamageReduction = Turn.Card.GetDamageReduction();
            Turn.DamageFromEnemy = true;
            Turn.Damage = Turn.Card.GetDamageAmount(Turn.DiceTarget - Turn.DiceTotal);
            Turn.LastCombatDamage = Turn.Damage;
            Scenario.Current.OnPlayerDamaged(Turn.Card);
            Turn.Character.OnPlayerDamaged(Turn.Card);
            Turn.Card.OnPlayerDamaged(Turn.Card);
        }
        if (combatResult != CombatResultType.None)
        {
            Turn.CombatDelta = Turn.DiceTotal - Turn.DiceTarget;
        }
        base.Resolve();
        Turn.ClearCheckData();
        if ((combatResult == CombatResultType.Win) && (Turn.Card.Type == CardType.Villain))
        {
            Scenario current = Scenario.Current;
            current.NumVillainDefeats++;
        }
        Scenario.Current.OnCombatResolved(Turn.Card);
        Location.Current.OnCombatResolved(Turn.Card);
        Turn.Card.OnCombatResolved();
        if ((Turn.State == GameStateType.Combat) || (Turn.State == GameStateType.Reroll))
        {
            Turn.State = GameStateType.Damage;
        }
    }

    private void SetupDicePanel(GuiPanelDice dicePanel)
    {
        SkillCheckValueType[] checks = Turn.Checks;
        if ((checks == null) || (checks.Length == 0))
        {
            checks = Turn.Card.Checks;
        }
        SkillCheckType check = Turn.Check;
        if (Turn.Check == SkillCheckType.None)
        {
            check = Turn.Owner.GetBestSkillCheck(checks).skill;
        }
        dicePanel.SetCheck(Turn.Card, checks, check);
    }

    public override GameStateType Type =>
        GameStateType.Combat;
}

