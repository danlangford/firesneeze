using System;
using UnityEngine;

public class EventDefeatedBlockPending : Event
{
    [Tooltip("block to invoke when matched")]
    public Block Block;
    [Tooltip("determines which cards trigger the block")]
    public CardSelector Selector;

    private void EventBlockDelayed_Invoke()
    {
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
        if ((this.Block != null) && this.Block.Stateless)
        {
            Turn.State = GameStateType.Done;
        }
    }

    public override void OnCardDefeated(Card card)
    {
        if (this.Selector.Match(card))
        {
            Turn.PendingDoneEvent = new TurnStateCallback(TurnStateCallbackType.Scenario, "EventBlockDelayed_Invoke");
            Turn.CheckBoard.Set<string>("BlockPendingCard", card.ID);
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

