using System;
using UnityEngine;

public class EventSecondCombatDiscardReset : Event
{
    [Tooltip("number of cards to discard")]
    public int DiscardAmount = 1;
    [Tooltip("specifies the card types to be discarded (empty selector matches any card)")]
    public CardSelector Selector;

    private void Discard(CardFilter filter)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Deck hand = Turn.Character.Hand;
            int max = hand.Filter(filter);
            if (max > 0)
            {
                int num2 = UnityEngine.Random.Range(0, max);
                int num3 = 0;
                for (int i = 0; i < hand.Count; i++)
                {
                    if (filter.Match(hand[i]) && (num2 == num3++))
                    {
                        window.Discard(hand[i]);
                        break;
                    }
                }
            }
        }
    }

    private void EventDiscardReset_Finish()
    {
        if (Turn.Owner.Alive)
        {
            Event.DonePost(GameStateType.Combat);
        }
        else
        {
            UI.Window.Refresh();
            Event.DonePost(GameStateType.Combat);
        }
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }

    public override void OnSecondCombat(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            this.Discard(this.Selector.ToFilter());
            Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventDiscardReset_Finish"));
            Turn.State = GameStateType.DiscardHand;
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnSecondCombat;
}

