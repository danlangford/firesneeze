using System;

public class CardPowerEvade : CardPower
{
    public bool Shuffle = true;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Validate = true;
            if (this.Shuffle)
            {
                Turn.Card.Disposition = DispositionType.Shuffle;
            }
            else
            {
                Turn.Card.Disposition = DispositionType.Top;
                Turn.Card.Known = true;
            }
            base.ShowDice(false);
            Turn.Evade = true;
            Turn.Character.MarkCardType(card.Type, true);
            base.ShowProceedButton(true);
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            Turn.Card.Disposition = DispositionType.None;
            Turn.Card.Known = false;
            base.ShowDice(true);
            Turn.Evade = false;
            Turn.Character.MarkCardType(card.Type, false);
            base.ShowProceedButton(false);
            if ((Turn.State == GameStateType.Horde) || (Turn.State == GameStateType.EvadeOption))
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
        if ((card.Type == CardType.Spell) && Rules.IsImmune(Turn.Card, card))
        {
            return false;
        }
        return Rules.IsEvadePossible(Turn.Card);
    }

    public override bool IsValidationRequired() => 
        !base.IsConditionValid(Turn.Card);

    public override PowerType Type =>
        PowerType.Evade;
}

