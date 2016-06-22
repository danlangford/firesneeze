using System;
using UnityEngine;

public class BlockEncounterCard : Block
{
    [Tooltip("only cards that match will be encountered")]
    public CardSelector Selector;

    public override void Invoke()
    {
        if (this.IsValid())
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutLocation.Show(true);
                window.layoutLocation.Refresh();
                Turn.CombatStage = TurnCombatStageType.PreEncounter;
                Turn.State = GameStateType.Encounter;
            }
        }
    }

    protected bool IsValid()
    {
        if (Location.Current.Deck.Count <= 0)
        {
            return false;
        }
        if ((this.Selector != null) && !this.Selector.Match(Location.Current.Deck[0]))
        {
            return false;
        }
        return true;
    }

    public override bool Stateless =>
        false;
}

