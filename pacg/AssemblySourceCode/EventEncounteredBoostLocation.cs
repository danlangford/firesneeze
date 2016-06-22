using System;
using UnityEngine;

public class EventEncounteredBoostLocation : Event
{
    [Tooltip("this event only fires for cards that match this filter")]
    public CardSelector CardSelector;
    [Tooltip("the type of card to draw from the box")]
    public CardType DrawType = CardType.Monster;
    [Tooltip("specifies the type of locations eligible")]
    public LocationSelector LocationSelector;
    [Tooltip("how should the new card be inserted into the location deck?")]
    public DeckPositionType Position = DeckPositionType.Top;

    private void EventEncounteredBoostLocation_Activate()
    {
        Turn.State = GameStateType.Null;
        Card card = Campaign.Box.Draw(this.DrawType);
        if (card == null)
        {
            this.EventEncounteredBoostLocation_Done();
        }
        else
        {
            card.Show(false);
            string str = this.LocationSelector.Random();
            if (str != null)
            {
                new GuiScriptBoostLocationOnMap { 
                    LocationID = str,
                    Card = card,
                    Position = this.Position,
                    Side = CardSideType.Back,
                    Dispose = true,
                    Callback = new TurnStateCallback(TurnStateCallbackType.Location, "EventEncounteredBoostLocation_Done")
                }.Play();
            }
            else
            {
                this.EventEncounteredBoostLocation_Done();
            }
        }
    }

    private void EventEncounteredBoostLocation_Done()
    {
        if (!Game.Events.ContainsStatefulEvent())
        {
            GameStateEncounter.Continue();
        }
        Event.Done();
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return this.CardSelector.Match(card);
    }

    public override void OnCardEncountered(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventEncounteredBoostLocation_Activate"));
            GameStateProceed.Scenario = false;
            Turn.State = GameStateType.Proceed;
        }
    }

    public override bool Stateless =>
        this.IsEventValid(Turn.Card);

    public override EventType Type =>
        EventType.OnCardEncountered;
}

