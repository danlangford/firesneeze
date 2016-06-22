using System;

public class CardPowerNone : CardPower
{
    public override string GetCardDecoration(Card card)
    {
        if (((Turn.State == GameStateType.Discard) || (Turn.State == GameStateType.DiscardHand)) && (this.IsPowerAllowed(card) && (base.Action == ActionType.Recharge)))
        {
            return "Blueprints/Gui/Vfx_Card_Notice_Recharge";
        }
        return null;
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
        return true;
    }
}

