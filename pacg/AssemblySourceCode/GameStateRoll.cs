using System;
using System.Collections.Generic;

public class GameStateRoll : GameState
{
    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public static bool CanRollDice(List<DiceType> dice)
    {
        if (dice.Count <= 0)
        {
            return false;
        }
        for (int i = 0; i < dice.Count; i++)
        {
            if (((DiceType) dice[i]) == DiceType.D0)
            {
                return false;
            }
        }
        return true;
    }

    public override void Enter()
    {
        base.Enter();
        if (CanRollDice(Turn.Dice))
        {
            base.Message(80);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if ((Turn.GetStateData() != null) && (Turn.GetStateData().Message != null))
                {
                    window.messagePanel.Show(Turn.GetStateData().Message);
                }
                window.dicePanel.Show(true);
                window.dicePanel.Refresh();
                if (Turn.Check != SkillCheckType.None)
                {
                    window.dicePanel.SetCheck(null, Turn.Checks, Turn.Check);
                }
                window.ShowProceedButton(false);
                window.Refresh();
                base.ShowAidButton();
                if (((Turn.RollReason == RollType.EnemyDamage) || (Turn.RollReason == RollType.EnemyIncreaseDifficulty)) || (Turn.RollReason == RollType.EnemyRandomPower))
                {
                    window.dicePanel.Roll();
                }
            }
        }
        else
        {
            Turn.Roll(0, 0);
            this.Proceed();
        }
    }

    public override void Exit(GameStateType nextState)
    {
        if ((nextState != GameStateType.RollAgain) && (nextState != GameStateType.Sacrifice))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if (nextState == GameStateType.Power)
                {
                    window.dicePanel.Show(false);
                }
                else
                {
                    if (Turn.Checks != null)
                    {
                        Turn.ClearCheckData();
                    }
                    window.dicePanel.Clear();
                }
            }
            base.Message((string) null);
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    public override bool IsResolveSuccess() => 
        ((Turn.Checks == null) || (Turn.Defeat || (Turn.DiceTotal >= Turn.DiceTarget)));

    public override void Proceed()
    {
        if (Turn.Checks != null)
        {
            Party.OnCheckCompleted();
        }
        if (Turn.Evade)
        {
            Turn.LastCombatResult = CombatResultType.None;
            if (Turn.PeekStateDestination() != null)
            {
                Turn.GotoStateDestination();
            }
            else
            {
                Turn.State = GameStateType.Post;
            }
        }
        else if (Turn.PeekStateDestination() != null)
        {
            Turn.GotoStateDestination();
        }
        else
        {
            Turn.State = GameStateType.Post;
        }
    }

    public override void Refresh()
    {
    }

    public override void Resolve()
    {
        base.Resolve();
        this.Proceed();
    }

    public override GameStateType Type =>
        GameStateType.Roll;
}

