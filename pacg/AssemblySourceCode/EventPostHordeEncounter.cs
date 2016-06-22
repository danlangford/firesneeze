using System;

public class EventPostHordeEncounter : Event
{
    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }

    public override void OnPostHorde(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            card.Show(true);
            Turn.EvadeDeclined = true;
            Turn.State = GameStateType.Combat;
            Event.Done();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnPostHorde;
}

