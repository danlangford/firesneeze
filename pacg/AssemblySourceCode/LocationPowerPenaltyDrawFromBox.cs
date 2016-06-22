using System;
using UnityEngine;

public class LocationPowerPenaltyDrawFromBox : LocationPower
{
    [Tooltip("penalty action to be done in order to draw")]
    public ActionType Action = ActionType.Banish;
    [Tooltip("penalty amount needed to do action")]
    public int ActionAmount = 1;
    [Tooltip("these type of cards can be drawn from the box (use NONE for any card)")]
    public CardType DrawType = CardType.Blessing;
    [Tooltip("defines the type of penalty card")]
    public CardSelector PenaltyCardSelector;

    public override void Activate()
    {
        Turn.PushReturnState();
        if (this.ActionAmount > 0)
        {
            base.PowerBegin();
            Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerDraw_Cancel"));
            Turn.SetStateData(new TurnStateData(this.Action, this.PenaltyCardSelector.ToFilter(), this.ActionAmount));
            Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerDraw_Draw"));
            Turn.State = GameStateType.Penalty;
            base.ShowCancelButton(true);
        }
        else
        {
            this.LocationPowerDraw_Draw();
        }
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if ((this.PenaltyCardSelector != null) && (this.PenaltyCardSelector.Filter(Turn.Character.Hand) <= 0))
        {
            return false;
        }
        return true;
    }

    private void LocationPowerDraw_Cancel()
    {
        this.PowerEnd();
        Turn.ReturnToReturnState();
    }

    private void LocationPowerDraw_Draw()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card[] cards = new Card[] { Campaign.Box.Draw(this.DrawType) };
            window.DrawCardsFromBox(cards, Turn.Character.Hand, Turn.Number);
        }
        Turn.ReturnToReturnState();
    }
}

