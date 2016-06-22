using System;

public class GameStatePost : GameState
{
    private static bool isResolveInProgress;

    public override void Enter()
    {
        base.Enter();
        if (Turn.IsIteratorInProgress() && (Turn.Iterators.Current is IteratorDamage))
        {
            Turn.Iterators.Next();
        }
        else if (Game.Events.Complete() && !isResolveInProgress)
        {
            if (Turn.CombatStage == TurnCombatStageType.PostEncounter)
            {
                this.Proceed();
            }
            else
            {
                this.Resolve();
            }
        }
    }

    public override void Proceed()
    {
        if (Game.Events.ContainsStatefulEvent())
        {
            Game.Events.Next();
        }
        if (!Game.Events.ContainsStatefulEvent() && base.IsCurrentState())
        {
            Turn.PushStateDestination(GameStateType.Dispose);
            Turn.State = GameStateType.Recharge;
        }
    }

    public override void Resolve()
    {
        base.Resolve();
        if (!Turn.Disposed && Turn.Card.OnCombatEnd())
        {
            if (!Turn.Pass)
            {
                Turn.SwitchCharacter(Turn.InitialCharacter);
                Turn.Current = Turn.InitialCharacter;
            }
            Turn.CombatStage = TurnCombatStageType.PostEncounter;
            if (Turn.Evade || (Turn.LastCombatResult == CombatResultType.None))
            {
                Turn.NumCombatEvades++;
                Turn.Card.OnEvaded();
            }
            else
            {
                isResolveInProgress = true;
                Turn.Card.OnPostAct();
                if (Turn.LastCombatResult == CombatResultType.Win)
                {
                    Turn.Card.OnDefeated();
                }
                else if (Turn.LastCombatResult == CombatResultType.Lose)
                {
                    Turn.NumCombatUndefeats++;
                    Turn.Card.OnUndefeated();
                }
                isResolveInProgress = false;
            }
        }
        Turn.ClearCombatData();
        if (base.IsCurrentState())
        {
            this.Proceed();
        }
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Post;
}

