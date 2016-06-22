using System;
using UnityEngine;

public class EventBeforeActDiscard : Event
{
    [Tooltip("number of cards to discard")]
    public int Amount = 1;
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

    public override void OnBeforeAct()
    {
        CardFilter filter = this.Selector.ToFilter();
        int num = Mathf.Max(0, this.Amount);
        for (int i = 0; i < num; i++)
        {
            this.Discard(filter);
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

