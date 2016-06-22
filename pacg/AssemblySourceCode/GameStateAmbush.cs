using System;

public class GameStateAmbush : GameStateDamage
{
    public override void Proceed()
    {
        Turn.DamageDiscarded = (UI.Window as GuiWindowLocation).layoutDiscard.Deck.Count;
        base.SaveRechargableCards();
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
        if (Turn.Evade)
        {
            Turn.State = GameStateType.Post;
        }
        else if ((Turn.IsIteratorInProgress() && !(Turn.Iterators.Current is IteratorClose)) && !(Turn.Iterators.Current is IteratorHorde))
        {
            Turn.Iterators.Next();
        }
        else
        {
            Turn.GotoStateDestination();
            if (!base.IsCurrentState())
            {
                Turn.PushStateDestination(Turn.State);
            }
            else
            {
                Turn.PushStateDestination(GameStateType.Combat);
            }
            Turn.DiceBonus = 0;
            Turn.State = GameStateType.Recharge;
        }
    }

    public override GameStateType Type =>
        GameStateType.Ambush;
}

