using System;

public class CardPowerDefeat : CardPower
{
    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Validate = true;
            Turn.Character.MarkCardType(card.Type, true);
            Turn.Defeat = true;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.Validate();
            }
            base.ShowDice(false);
            base.ShowProceedButton(true);
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            base.ShowDice(true);
            Turn.Character.MarkCardType(card.Type, false);
            Turn.Defeat = false;
            base.ShowProceedButton(false);
        }
    }

    protected override bool IsEvadeOrDefeatPowerValid() => 
        true;

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
        if (Turn.State != GameStateType.Combat)
        {
            return false;
        }
        return true;
    }

    public override bool IsPowerDeactivationAllowed(Card card)
    {
        if (Turn.State != GameStateType.Combat)
        {
            return false;
        }
        return true;
    }
}

