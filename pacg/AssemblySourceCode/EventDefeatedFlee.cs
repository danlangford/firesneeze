using System;
using UnityEngine;

public class EventDefeatedFlee : Event
{
    [Tooltip("which cards should flee?")]
    public CardSelector CardSelector;
    [Tooltip("which locations are valid?")]
    public LocationSelector LocationSelector;
    [Tooltip("where in the new deck should this card go?")]
    public DeckPositionType Position = DeckPositionType.Shuffle;

    private void EventDefeatedFlee_Activate()
    {
        Turn.State = GameStateType.Null;
        string str = this.LocationSelector.Random(Turn.Owner);
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
                Callback = new TurnStateCallback(TurnStateCallbackType.Scenario, "EventDefeatedFlee_Done")
            }.Play();
        }
        else
        {
            this.EventDefeatedFlee_Done();
        }
    }

    private void EventDefeatedFlee_Done()
    {
        Event.Done();
        if (!Game.Events.ContainsStatefulEvent())
        {
            Turn.PushStateDestination(GameStateType.Post);
            Turn.State = GameStateType.Recharge;
        }
    }

    public override bool IsEventValid(Card card)
    {
        if (Rules.IsCardSummons(card))
        {
            return false;
        }
        return this.CardSelector.Match(card);
    }

    public override void OnCardDefeated(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "EventDefeatedFlee_Activate"));
            GameStateProceed.Scenario = true;
            Turn.State = GameStateType.Proceed;
        }
    }

    public override bool Stateless =>
        this.IsEventValid(Turn.Card);

    public override EventType Type =>
        EventType.OnCardDefeated;
}

