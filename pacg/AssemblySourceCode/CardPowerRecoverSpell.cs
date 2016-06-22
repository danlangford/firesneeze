using System;
using UnityEngine;

public class CardPowerRecoverSpell : CardPower
{
    [Tooltip("select arcane spells")]
    public CardSelector ArcaneSpells;
    [Tooltip("select divine spells")]
    public CardSelector DivineSpells;
    [Tooltip("the deck to pull the card from")]
    public DeckType From = DeckType.Discard;
    [Tooltip("Token of Remembrance requires the character to have an arcane/divine")]
    public bool RequiresMatchingSkill = true;
    [Tooltip("the deck to send the card to")]
    public DeckType To = DeckType.Hand;

    public override void Activate(Card card)
    {
        CardFilter cardFilter = this.GetCardFilter();
        Turn.PushReturnState();
        Turn.SetStateData(new TurnStateData(this.From.ToActionType(), cardFilter, this.To.ToActionType(), 1));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "SpellRecover_Finish"));
        Turn.EmptyLayoutDecks = false;
        Turn.State = GameStateType.Pick;
        Turn.EmptyLayoutDecks = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
        }
    }

    public override void Deactivate(Card card)
    {
        Turn.EmptyLayoutDecks = false;
        Turn.PopStateDestination();
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    private CardFilter GetCardFilter()
    {
        CardFilter filter = new CardFilter {
            ComparisonType = SelectorComparisonType.And1,
            CardTypes = new CardType[] { CardType.Spell }
        };
        if (this.RequiresMatchingSkill)
        {
            int num = Turn.Character.GetSkillRank(SkillCheckType.Arcane) + Turn.Character.GetSkillRank(SkillCheckType.Divine);
            filter.CardTraits = new TraitType[num];
            int num2 = 0;
            if (Turn.Character.GetSkillRank(SkillCheckType.Arcane) > 0)
            {
                filter.CardTraits[num2++] = TraitType.Arcane;
            }
            if (Turn.Character.GetSkillRank(SkillCheckType.Divine) > 0)
            {
                filter.CardTraits[num2++] = TraitType.Divine;
            }
        }
        return filter;
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if ((Turn.State != GameStateType.Setup) && (Turn.State != GameStateType.Finish))
        {
            return false;
        }
        if (Rules.IsCheck())
        {
            return false;
        }
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return (((!this.RequiresMatchingSkill || (Turn.Character.GetSkillRank(SkillCheckType.Arcane) > 0)) && (this.ArcaneSpells.Filter(Turn.Character.Discard) > 0)) || ((!this.RequiresMatchingSkill || (Turn.Character.GetSkillRank(SkillCheckType.Divine) > 0)) && (this.DivineSpells.Filter(Turn.Character.Discard) > 0)));
    }

    private void SpellRecover_Finish()
    {
        Turn.ReturnToReturnState();
    }
}

