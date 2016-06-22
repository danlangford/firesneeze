using System;
using UnityEngine;

public class EventDefeatedSummon : Event
{
    [Tooltip("the ID of the monster to summon; use $Turn.Card to repeat the current card")]
    public string Monster;
    [Tooltip("none means only the current player; location means all at this location\t\t\t\t\t")]
    public LocationType SummonRange;

    public override void OnCardDefeated(Card card)
    {
        Turn.SummonsType = SummonsType.Single;
        Turn.SummonsMonster = SummonsSelector.GetSummonsMonster(this.Monster);
        Turn.SummonsLocation = this.SummonRange;
        Turn.Iterators.Start(TurnStateIteratorType.Horde);
        if (this.HasPostHordeEvent())
        {
            Turn.NumCombatUndefeats = 0;
            Turn.NumCombatEvades = 0;
            Turn.Iterators.Current.HasPostEvent = true;
        }
        Turn.State = GameStateType.Horde;
        Turn.EvadeDeclined = true;
        Event.Done();
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardDefeated;
}

