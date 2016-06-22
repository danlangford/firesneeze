using System;
using UnityEngine;

public class EventUndefeatedFlee : Event
{
    [Tooltip("which locations are valid?")]
    public LocationSelector LocationSelector;
    [Tooltip("where in the new deck should this card go?")]
    public DeckPositionType Position = DeckPositionType.Shuffle;

    private void EventUndefeatedBoostLocation_Done()
    {
        Event.Done();
        if (!Game.Events.ContainsStatefulEvent())
        {
            Turn.PushStateDestination(GameStateType.Dispose);
            Turn.State = GameStateType.Recharge;
        }
    }

    private void EventUndefeatedFlee_Activate()
    {
        Turn.State = GameStateType.Null;
        string str = this.LocationSelector.Random();
        if (str != null)
        {
            if (str == Location.Current.ID)
            {
                Turn.Card.Disposition = DispositionType.Shuffle;
            }
            else
            {
                Turn.Card.Disposition = DispositionType.Destroy;
            }
        }
        else
        {
            Turn.Card.Disposition = DispositionType.Shuffle;
        }
        if (str != null)
        {
            new GuiScriptBoostLocationOnMap { 
                LocationID = str,
                Card = Turn.Card,
                Position = this.Position,
                Dispose = false,
                Callback = new TurnStateCallback(Turn.Card, "EventUndefeatedBoostLocation_Done")
            }.Play();
        }
        else
        {
            this.EventUndefeatedBoostLocation_Done();
        }
    }

    public override bool IsEventValid(Card card)
    {
        if (card.Disposition == DispositionType.Banish)
        {
            return false;
        }
        return true;
    }

    public override void OnCardUndefeated(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            this.EventUndefeatedFlee_Activate();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

