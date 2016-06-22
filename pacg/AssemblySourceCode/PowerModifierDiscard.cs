using System;
using UnityEngine;

public class PowerModifierDiscard : PowerModifier
{
    [Tooltip("what action must be taken in penalty state")]
    public ActionType Action = ActionType.Discard;
    [Tooltip("how many cards to discard in penalty state")]
    public int Amount = 1;
    [Tooltip("Dice to grant for paying the penalty (greater than 1)")]
    public DiceType[] Dice;
    private int index = -1;
    [Tooltip("how many cards to discard minimum (for Mokmurain'sClub discard 0-1 cards)")]
    public int MinAmount = 1;
    [Tooltip("filter for the penalty state")]
    public CardSelector Selector;

    public override void Activate(int powerIndex)
    {
        this.index = powerIndex;
        TurnStateData data = new TurnStateData(this.Action, this.MinAmount) {
            MaxNumCards = this.Amount,
            Filter = (this.Selector == null) ? CardFilter.Empty : this.Selector.ToFilter()
        };
        Turn.SetStateData(data);
        Turn.PushReturnState();
        Turn.PushCancelDestination(new TurnStateCallback(this.Card, "PowerModifierDiscard_Cancel"));
        Turn.PushStateDestination(new TurnStateCallback(this.Card, "PowerModifierDiscard_Finish"));
        Turn.EmptyLayoutDecks = false;
        Turn.State = GameStateType.Power;
    }

    public override void Deactivate()
    {
        Card penaltyCard = this.GetPenaltyCard();
        if (penaltyCard != null)
        {
            penaltyCard.ActionDeactivate(true);
            for (int i = 0; i < this.Dice.Length; i++)
            {
                Turn.Dice.Remove(this.Dice[i]);
            }
        }
        if (Turn.State == GameStateType.Power)
        {
            Turn.PopStateDestination();
            Turn.PopCancelDestination();
            Turn.EmptyLayoutDecks = false;
            Turn.ReturnToReturnState();
            Turn.EmptyLayoutDecks = true;
        }
    }

    private Card GetPenaltyCard()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            GuiLayout layoutDeck = window.GetLayoutDeck(this.Action);
            if (layoutDeck != null)
            {
                int index = layoutDeck.IndexOf(this.index, this.Card);
                if ((index >= 0) && (layoutDeck.Deck[index] != this.Card))
                {
                    return layoutDeck.Deck[index];
                }
                index = layoutDeck.IndexOf(-1, null);
                if ((index >= 0) && (layoutDeck.Deck[index] != this.Card))
                {
                    return layoutDeck.Deck[index];
                }
                if (Turn.CheckBoard.Get<string>("PenaltyGuid") != null)
                {
                    Guid guid = new Guid(Turn.CheckBoard.Get<string>("PenaltyGuid"));
                    return layoutDeck.Deck[guid];
                }
            }
        }
        return null;
    }

    public override bool IsValidationRequired() => 
        false;

    private void PowerModifierDiscard_Cancel()
    {
        Turn.EmptyLayoutDecks = false;
        Turn.ReturnToReturnState();
        this.Card.ActionDeactivate(true);
        this.Card.ReturnCard(this.Card);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutHand.Refresh();
        }
        Turn.EmptyLayoutDecks = true;
    }

    private void PowerModifierDiscard_Finish()
    {
        Card penaltyCard = this.GetPenaltyCard();
        if (penaltyCard != null)
        {
            penaltyCard.SetPowerInfo(this.index, this.Card);
            for (int i = 0; i < this.Dice.Length; i++)
            {
                Turn.Dice.Add(this.Dice[i]);
            }
        }
        Turn.EmptyLayoutDecks = false;
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    private Card Card =>
        base.GetComponent<Card>();
}

