using System;

public class CharacterPowerUnlimited : CharacterPower
{
    public CardSelector UnlimitedPlay;
    public ActionType ValidAction = ActionType.Discard;

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
        return true;
    }
}

