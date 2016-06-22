using System;
using UnityEngine;

public class LocationPowerPenaltyMove : LocationPower
{
    [Tooltip("what encounter cards are applicable")]
    public CardSelector CardSelector;
    [Tooltip("number of penalty cards")]
    public int PenaltyAmount = 1;
    [Tooltip("what type of penalty is this? discard, etc.")]
    public ActionType PenaltyType = ActionType.Recharge;
    [Tooltip("destination deck for cards")]
    public DeckType To = DeckType.Hand;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerPenaltyAcquire_Cancel"));
        Turn.SetStateData(new TurnStateData(this.PenaltyType, this.PenaltyAmount));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerPenaltyAcquire_Aquire"));
        Turn.State = GameStateType.Penalty;
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.Card == null)
        {
            return false;
        }
        return (((this.CardSelector != null) && this.CardSelector.Match(Turn.Card)) && (Turn.State == GameStateType.Combat));
    }

    private void LocationPowerPenaltyAcquire_Aquire()
    {
        this.PowerEnd();
        Turn.State = GameStateType.Damage;
        this.MoveCard(Turn.Card);
    }

    private void LocationPowerPenaltyAcquire_Cancel()
    {
        this.PowerEnd();
        Turn.State = GameStateType.Combat;
    }

    private void MoveCard(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (this.To == DeckType.Banish)
            {
                Campaign.Box.Add(card, false);
            }
            if (this.To == DeckType.Discard)
            {
                window.Discard(card);
            }
            if (this.To == DeckType.Hand)
            {
                window.Draw(card);
            }
            if (this.To == DeckType.Character)
            {
                window.Recharge(card);
            }
            if (this.To == DeckType.Bury)
            {
                window.Bury(card);
            }
            if (this.To == DeckType.Location)
            {
                Location.Current.Deck.Add(card);
            }
            window.locationPanel.RefreshCardList();
        }
    }
}

