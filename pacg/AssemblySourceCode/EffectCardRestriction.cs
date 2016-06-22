using System;

public class EffectCardRestriction : Effect
{
    public EffectCardRestriction(string source, int duration, CardFilter filter) : base(source, duration, filter)
    {
    }

    public override string GetCardDecoration(Card card)
    {
        if ((base.filter != null) && base.filter.Match(card))
        {
            return "Blueprints/Gui/Vfx_Card_Restricted";
        }
        return null;
    }

    public override string GetDisplayText() => 
        "Card Restriction";

    public override int GetHashCode() => 
        (base.GetHashCode() ^ base.filter.GetHashCode());

    protected override bool IsEffectValid()
    {
        CardType cardType = CardTable.LookupCardType(base.source);
        return ((((cardType != CardType.Monster) && (cardType != CardType.Villain)) && (cardType != CardType.Henchman)) || (Turn.Card.ID == base.source));
    }

    public bool Match(Card card) => 
        ((this.IsEffectValid() && (base.filter != null)) && base.filter.Match(card));

    public bool Match(CardType cardType)
    {
        if ((this.IsEffectValid() && (base.filter != null)) && (base.filter.CardTypes != null))
        {
            for (int i = 0; i < base.filter.CardTypes.Length; i++)
            {
                if (base.filter.CardTypes[i] == cardType)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool Single =>
        true;

    public override EffectType Type =>
        EffectType.CardRestriction;
}

