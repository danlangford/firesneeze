using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationPowerAddDice : LocationPower
{
    [Tooltip("number of cards in hand that must fulfill the penalty selector")]
    public int Amount = 1;
    [Tooltip("bonus added (total not per dice)")]
    public int DiceBonus;
    [Tooltip("dice added to check if you paid the penalty")]
    public DiceType[] DiscardDie = new DiceType[1];
    [Tooltip("penalty action the player must make")]
    public ActionType Penalty = ActionType.Discard;
    [Tooltip("what types of cards can be used for the penalty")]
    public CardSelector Selector;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushReturnState();
        if (this.Penalty != ActionType.None)
        {
            Turn.EmptyLayoutDecks = false;
            Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerAddDice_Cancel"));
            if (this.Selector != null)
            {
                Turn.SetStateData(new TurnStateData(this.Penalty, this.Selector.ToFilter(), this.Amount));
            }
            else
            {
                Turn.SetStateData(new TurnStateData(this.Penalty, this.Amount));
            }
            Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerAddDice_Finish"));
            Turn.State = GameStateType.Power;
        }
        else
        {
            this.LocationPowerAddDice_Finish();
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.RemoveDice();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            foreach (Card card in this.GetPenaltyCards())
            {
                Character character = Party.Characters[card.PlayedOwner];
                if (Turn.Character.ID == card.PlayedOwner)
                {
                    window.layoutHand.OnGuiDrop(card);
                }
                else
                {
                    character.Hand.Add(card);
                }
                card.RemovePowerInfo(Location.Current.Powers.IndexOf(this), Location.Current.ID);
            }
            window.dicePanel.Refresh();
        }
    }

    private Card[] GetPenaltyCards()
    {
        List<Card> list = new List<Card>(this.Amount);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card item = window.GetLayoutDeck(this.Penalty).Deck[new Guid(Turn.CheckBoard.Get<string>("PenaltyGuid"))];
            if (item != null)
            {
                list.Add(item);
            }
            Deck deck = window.GetLayoutDeck(this.Penalty).Deck;
            for (int i = 0; i < deck.Count; i++)
            {
                if ((deck[i].PlayedPower == -1) && !list.Contains(deck[i]))
                {
                    list.Add(deck[i]);
                }
            }
        }
        return list.ToArray();
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (this.Selector != null)
        {
            if ((this.Selector.Filter(Turn.Character.Hand) < this.Amount) && (this.Penalty != ActionType.None))
            {
                return false;
            }
        }
        else if ((Turn.Character.Hand.Count < this.Amount) && (this.Penalty != ActionType.None))
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        return true;
    }

    private void LocationPowerAddDice_Cancel()
    {
        this.PowerEnd();
        Turn.ReturnToReturnState();
    }

    private void LocationPowerAddDice_Finish()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            for (int j = 0; j < this.DiscardDie.Length; j++)
            {
                Turn.Dice.Add(this.DiscardDie[j]);
            }
            Turn.DiceBonus += this.DiceBonus;
            window.dicePanel.Refresh();
        }
        Card[] penaltyCards = this.GetPenaltyCards();
        for (int i = 0; i < penaltyCards.Length; i++)
        {
            penaltyCards[i].SetPowerInfo(Location.Current.Powers.IndexOf(this), Location.Current.ID);
        }
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    private void RemoveDice()
    {
        for (int i = 0; i < this.DiscardDie.Length; i++)
        {
            Turn.Dice.Remove(this.DiscardDie[i]);
        }
        Turn.DiceBonus -= this.DiceBonus;
    }
}

