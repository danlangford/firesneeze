using System;

public class GameStateEncounter : GameState
{
    public static void Continue()
    {
        if (Scenario.Current.IsCurrentVillain(Turn.Card.ID))
        {
            Turn.State = GameStateType.VillainIntro;
        }
        else
        {
            Turn.State = GameStateType.EvadeOption;
        }
    }

    public override void Enter()
    {
        Turn.CombatStage = TurnCombatStageType.PreEncounter;
        EffectExploreRestriction effect = Turn.Owner.GetEffect(EffectType.ExploreRestriction) as EffectExploreRestriction;
        if (effect != null)
        {
            if (effect.Match(Turn.Card))
            {
                Turn.Card.Disposition = effect.Disposition;
                Turn.State = GameStateType.Dispose;
                Turn.Owner.RemoveEffect(effect);
                return;
            }
            Turn.Owner.RemoveEffect(effect);
        }
        if (!Rules.IsCardSummons(Turn.Card) && (Scenario.Current.GetCardCount(Location.Current.ID, Turn.Card.Type) <= 0))
        {
            Scenario.Current.AddCardCount(Location.Current.ID, Turn.Card.Type, 1);
            Scenario.Current.AddCardCount(Location.Current.ID, CardType.None, -1);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.locationPanel.RefreshCardList();
            }
        }
        Turn.SetEncounteredInformation();
        Turn.Card.OnEncountered();
        if (base.IsCurrentState())
        {
            this.Proceed();
        }
    }

    public override void Proceed()
    {
        Continue();
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Encounter;
}

