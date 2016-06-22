using System;
using UnityEngine;

public class EventBeforeActHorde : Event
{
    [Tooltip("does the horde attack immediately (barriers) or after the first combat? (monsters)")]
    public bool Immediate = true;
    [Tooltip("the ID of the monster to summon; use $Turn.Card to repeat the current card")]
    public string Monster;
    [Tooltip("should this horde have the option to not summon/have next button/not have optional button?")]
    public TargetPanelType OptionalButton = TargetPanelType.Next;
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
        if (this.Selector != null)
        {
            return this.Selector.Match(card);
        }
        return base.IsConditionValid(card);
    }

    public override void OnBeforeAct()
    {
        if (!this.IsEventValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            Turn.SummonsMonster = SummonsSelector.GetSummonsMonster(this.Monster);
            Turn.SummonsType = SummonsType.Horde;
            Turn.SummonsLocation = this.Range;
            Turn.OptionalTarget = this.OptionalButton;
            Turn.Iterators.Start(TurnStateIteratorType.Horde);
            if (this.HasPostHordeEvent())
            {
                Turn.NumCombatUndefeats = 0;
                Turn.NumCombatEvades = 0;
                Turn.Iterators.Current.HasPostEvent = true;
            }
            if (this.Immediate)
            {
                Turn.State = GameStateType.Horde;
            }
            Event.Done();
        }
    }

    public override bool Stateless
    {
        get
        {
            if (this.IsEventValid(Turn.Card))
            {
                return this.Immediate;
            }
            return true;
        }
    }

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

