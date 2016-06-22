using System;
using UnityEngine;

public class EventUndefeatedHorde : Event
{
    [Tooltip("the ID of the monster to summon (can be the same as this monster)")]
    public string Monster;
    [Tooltip("is the horde including the player for summons")]
    public bool Player;
    [Tooltip("is the horde at this location, all open locations, or everywhere?")]
    public LocationType Range = LocationType.Current;

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Rules.IsCardSummons(card))
        {
            return false;
        }
        if ((this.Range == LocationType.CurrentOther) && (Location.CountCharactersAtLocation(Location.Current.ID) == 1))
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCardUndefeated(Card card)
    {
        if (this.IsEventValid(card))
        {
            Turn.SummonsMonster = this.Monster;
            Turn.SummonsType = SummonsType.Horde;
            Turn.SummonsLocation = this.Range;
            Turn.Iterators.Start(TurnStateIteratorType.Horde);
            Turn.EvadeDeclined = true;
            if (this.HasPostHordeEvent())
            {
                Turn.NumCombatUndefeats = 0;
                Turn.NumCombatEvades = 0;
                Turn.Iterators.Current.HasPostEvent = true;
            }
            Turn.State = GameStateType.Horde;
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

