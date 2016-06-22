using System;
using UnityEngine;

public class LocationPowerMapPeek : LocationPower
{
    [Tooltip("the number of cards that can be peeked")]
    public int Number = 1;

    public override void Activate()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.mapPanel.Peek(this.Number);
        }
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (!Location.Current.Closed)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (Turn.State != GameStateType.EndTurn)
        {
            return false;
        }
        return true;
    }
}

