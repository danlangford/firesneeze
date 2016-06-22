using System;
using UnityEngine;

public class EventEncounteredHorde : Event
{
    [Tooltip("the ID of the monster to summon; use $Turn.Card to repeat the current card")]
    public string Monster;
    [Tooltip("after encountering the horde banish the encounter instead of encountering it")]
    public bool PostBanish;
    [Tooltip("is the horde at this location, all open locations, or everywhere?")]
    public LocationType Range = LocationType.Current;
    [Tooltip("the horde will only be summoned when this is true")]
    public CardSelector Selector;

    public override bool IsEventValid(Card card)
    {
        if (!Rules.IsSummonPossible())
        {
            return false;
        }
        if ((this.Selector != null) && !this.Selector.Match(card))
        {
            return false;
        }
        return base.IsConditionValid(card);
    }

    public override void OnCardEncountered(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            Turn.SummonsMonster = SummonsSelector.GetSummonsMonster(this.Monster);
            Turn.SummonsType = SummonsType.Horde;
            Turn.SummonsLocation = this.Range;
            if (this.PostBanish)
            {
                Turn.Iterators.Start(TurnStateIteratorType.Horde);
            }
            else
            {
                Turn.Iterators.Start(TurnStateIteratorType.HordeThenEncounter);
            }
            if (this.HasPostHordeEvent())
            {
                Turn.NumCombatUndefeats = 0;
                Turn.NumCombatEvades = 0;
                Turn.Iterators.Current.HasPostEvent = true;
            }
            Turn.State = GameStateType.Horde;
            Event.Done();
        }
    }

    public override bool Stateless =>
        true;

    public override EventType Type =>
        EventType.OnCardEncountered;
}

