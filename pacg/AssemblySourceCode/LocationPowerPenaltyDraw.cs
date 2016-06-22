using System;
using UnityEngine;

public class LocationPowerPenaltyDraw : LocationPower
{
    [Tooltip("action to be done in order to draw")]
    public ActionType Action = ActionType.Recharge;
    [Tooltip("amount needed to do action")]
    public int ActionAmount = 1;
    [Tooltip("creates a card selector in order to pick valid cards")]
    public CardSelector CardSelector;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerDraw_Cancel"));
        Turn.SetStateData(new TurnStateData(this.Action, this.CardSelector.ToFilter(), this.ActionAmount));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerDraw_Draw"));
        Turn.State = GameStateType.Penalty;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
    }

    private bool CheckValid() => 
        (this.CardSelector?.Filter(Turn.Character.Hand) >= this.ActionAmount);

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.Character.Deck.Count == 0)
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
        return (Turn.State == GameStateType.StartTurn);
    }

    private void LocationPowerDraw_Cancel()
    {
        this.PowerEnd();
        Turn.State = GameStateType.StartTurn;
    }

    private void LocationPowerDraw_Draw()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Draw(Turn.Character.Deck.Draw());
        }
        Turn.State = GameStateType.StartTurn;
    }
}

