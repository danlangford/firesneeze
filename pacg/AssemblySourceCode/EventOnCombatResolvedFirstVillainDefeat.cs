using System;

public class EventOnCombatResolvedFirstVillainDefeat : Event
{
    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return (Scenario.Current.NumVillainDefeats <= 1);
    }

    public override void OnCombatResolved()
    {
        if (!this.IsEventValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            Turn.LastCombatResult = CombatResultType.Lose;
            Event.Done();
        }
    }

    public override EventType Type =>
        EventType.OnCombatResolved;
}

