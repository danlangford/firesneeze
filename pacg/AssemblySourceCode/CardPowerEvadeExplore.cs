using System;

public class CardPowerEvadeExplore : CardPower
{
    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Validate = true;
            base.ShowDice(false);
            Turn.Evade = true;
            Turn.Explore = true;
            Turn.Character.MarkCardType(card.Type, true);
            base.ShowProceedButton(true);
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            base.ShowDice(true);
            Turn.Evade = false;
            Turn.Explore = false;
            Turn.Character.MarkCardType(card.Type, false);
            base.ShowProceedButton(false);
            if (Turn.State == GameStateType.Horde)
            {
                base.ShowEncounterButton(true);
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
        return Rules.IsEvadePossible(Turn.Card);
    }

    public override bool IsPowerDeactivationAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if ((Turn.State != GameStateType.Combat) && (Turn.State != GameStateType.Horde))
        {
            return false;
        }
        return true;
    }

    public override PowerType Type =>
        PowerType.Evade;
}

