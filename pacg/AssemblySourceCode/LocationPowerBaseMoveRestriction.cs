using System;

public class LocationPowerBaseMoveRestriction : LocationPower
{
    public override void Activate()
    {
        base.PowerBegin(1f);
        Turn.PopReturnState();
        Turn.GotoCancelDestination();
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Location.Current.Card))
        {
            return false;
        }
        if (Location.Current.Closed)
        {
            return false;
        }
        return true;
    }
}

