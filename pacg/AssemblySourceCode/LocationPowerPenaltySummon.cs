using System;
using UnityEngine;

public class LocationPowerPenaltySummon : LocationPower
{
    [Tooltip("penalty action to be done by the player")]
    public ActionType Action = ActionType.Banish;
    [Tooltip("amount of cards to draw from the box")]
    public int BoxAmount = 1;
    [Tooltip("defines the type of card drawn from the box")]
    public CardSelector BoxSelector;
    [Tooltip("amount needed to do action")]
    public int PenaltyAmount = 1;
    [Tooltip("defines the penalty card")]
    public CardSelector PenaltySelector;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushReturnState();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerPenaltySummon_Cancel"));
        Turn.SetStateData(new TurnStateData(this.Action, this.PenaltySelector.ToFilter(), this.PenaltyAmount));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerPenaltySummon_Summon"));
        Turn.State = GameStateType.Penalty;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
    }

    private bool CheckValid() => 
        (this.PenaltySelector?.Filter(Turn.Character.Hand) >= this.PenaltyAmount);

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (!this.CheckValid())
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        return Location.Current.ClosedThisTurn;
    }

    private void LocationPowerPenaltySummon_Cancel()
    {
        this.PowerEnd();
        Turn.ReturnToReturnState();
    }

    private void LocationPowerPenaltySummon_Finish()
    {
        this.PowerEnd();
        Turn.ReturnToReturnState();
    }

    private void LocationPowerPenaltySummon_Summon()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card[] cards = new Card[this.BoxAmount];
            for (int i = 0; i < this.BoxAmount; i++)
            {
                cards[i] = Campaign.Box.Draw(this.BoxSelector);
            }
            window.DrawCardsFromBox(cards, Turn.Owner.Hand, Turn.Current);
            LeanTween.delayedCall(3.25f, new System.Action(this.LocationPowerPenaltySummon_Finish));
        }
        else
        {
            this.LocationPowerPenaltySummon_Finish();
        }
    }
}

