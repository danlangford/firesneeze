using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationPowerPenaltyExplore : LocationPower
{
    [Tooltip("number of penalty cards")]
    public int PenaltyAmount = 1;
    [Tooltip("what type of penalty is this? discard, etc.")]
    public ActionType PenaltyType = ActionType.Discard;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerPenaltyExplore_Cancel"));
        Turn.SetStateData(new TurnStateData(this.PenaltyType, this.PenaltyAmount));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerPenaltyExplore_Finish"));
        Turn.EmptyLayoutDecks = false;
        Turn.State = GameStateType.Power;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
    }

    public override void Deactivate()
    {
        this.PowerEnd();
        Turn.Explore = false;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Deck deck = window.GetLayoutDeck(this.PenaltyType).Deck;
            for (int i = 0; i < deck.Count; i++)
            {
                int num;
                string str;
                LocationPower playedPower = deck[i].GetPlayedPower(out num, out str) as LocationPowerPenaltyExplore;
                if ((playedPower != null) && (str == Location.Current.ID))
                {
                    Card card = deck[i];
                    card.ReturnCard(card);
                    card.RemovePowerInfo(num, str);
                }
            }
            window.ShowExploreButton(Turn.Explore);
        }
        base.Deactivate();
    }

    private Card[] GetPenaltyCards()
    {
        List<Card> list = new List<Card>(this.PenaltyAmount);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card item = window.GetLayoutDeck(this.PenaltyType).Deck[new Guid(Turn.CheckBoard.Get<string>("PenaltyGuid"))];
            if (item != null)
            {
                list.Add(item);
            }
            Deck deck = window.GetLayoutDeck(this.PenaltyType).Deck;
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
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.Explore)
        {
            return false;
        }
        if (Location.Current.Deck.Count <= 0)
        {
            return false;
        }
        if (Turn.Character.GetNumberDiscardableCards() < this.PenaltyAmount)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        return (Turn.State == GameStateType.Finish);
    }

    private void LocationPowerPenaltyExplore_Cancel()
    {
        this.PowerEnd();
        Turn.State = GameStateType.Finish;
    }

    private void LocationPowerPenaltyExplore_Finish()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        Turn.Explore = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card[] penaltyCards = this.GetPenaltyCards();
            for (int i = 0; i < penaltyCards.Length; i++)
            {
                penaltyCards[i].SetPowerInfo(Location.Current.Powers.IndexOf(this), Location.Current.ID);
            }
            window.ShowExploreButton(true);
        }
        Turn.State = GameStateType.Finish;
        Turn.EmptyLayoutDecks = true;
    }
}

