using System;

public class EventCheckModifierAdventureDeck : Event
{
    public override int GetBaseCheckModifier()
    {
        if (Adventure.Current != null)
        {
            return Adventure.Current.Rank;
        }
        return base.GetBaseCheckModifier();
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }
}

