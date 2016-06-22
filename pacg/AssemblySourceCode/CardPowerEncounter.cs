using System;

public class CardPowerEncounter : CardPower
{
    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutLocation.Show(true);
                window.layoutLocation.Refresh();
                Turn.State = GameStateType.Encounter;
            }
        }
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if (Location.Current.Deck.Count < 1)
        {
            return false;
        }
        return (Turn.State == GameStateType.Finish);
    }
}

