using System;
using UnityEngine;

public class CardPowerEffectBoostCheck : CardPower
{
    [Tooltip("when true applies this effect to the current party member")]
    public bool ApplyToCurrentCharacter;
    [Tooltip("when true applies this effect to the whole party")]
    public bool ApplyToParty = true;
    [Tooltip("the card this effect will trigger against")]
    public CardSelector CardSelector;
    [Tooltip("the number of dice to add")]
    public int DiceCount = 1;
    [Tooltip("the dice type to add to the check")]
    public DiceType DiceType = DiceType.D6;
    [Tooltip("the number of turns that this effect lasts")]
    public int Duration = 1;
    [Tooltip("final destination this card will go to at the end of the turn")]
    public ActionType FinalDestination = ActionType.Discard;
    [Tooltip("the check type this effect will trigger against (none means all)")]
    public SkillCheckType SkillCheck;
    [Tooltip("should this power allow multiple cards of the same type to be played?")]
    public bool Stacking;
    [Tooltip("the trait type to add to the check")]
    public TraitType TraitType = TraitType.Poison;

    public override void Activate(Card card)
    {
        Effect e = new EffectBoostCheck(card.ID, this.Duration, this.CardSelector.ToFilter(), this.DiceType, this.TraitType, this.SkillCheck, 0, this.DiceCount);
        if (this.ApplyToParty)
        {
            Party.ApplyEffect(e);
        }
        else if (this.ApplyToCurrentCharacter)
        {
            Turn.Owner.ApplyEffect(e);
        }
        else
        {
            Turn.Character.ApplyEffect(e);
        }
        if (!this.Stacking)
        {
            Turn.Character.MarkCardType(base.Card.Type, true);
        }
        if (Turn.State == GameStateType.Combat)
        {
            this.RefreshDicePanel(this.SkillCheck);
        }
    }

    public override void Deactivate(Card card)
    {
        Effect e = new EffectBoostCheck(card.ID, this.Duration, this.CardSelector.ToFilter(), this.DiceType, this.TraitType, this.SkillCheck, 0, this.DiceCount);
        if (this.ApplyToParty)
        {
            Party.RemoveEffect(e);
        }
        else if (this.ApplyToCurrentCharacter)
        {
            Turn.Owner.RemoveEffect(e);
        }
        else
        {
            Turn.Character.RemoveEffect(e);
        }
        if (!this.Stacking)
        {
            Turn.Character.MarkCardType(base.Card.Type, false);
        }
        if (Turn.State == GameStateType.Combat)
        {
            this.RefreshDicePanel(SkillCheckType.None);
        }
    }

    public override string GetCardDecoration(Card card)
    {
        if (Turn.State == GameStateType.Combat)
        {
            if (Rules.IsImmune(Turn.Card, this.TraitType))
            {
                return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
            }
            for (int i = 0; i < card.Traits.Length; i++)
            {
                if (Rules.IsImmune(Turn.Card, card.Traits[i]))
                {
                    return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
                }
            }
            EffectCardRestriction effect = Turn.Character.GetEffect(EffectType.CardRestriction) as EffectCardRestriction;
            if ((effect != null) && effect.Match(card))
            {
                return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
            }
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
        if (((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup)) && ((Turn.Checks == null) || (Turn.Checks.Length == 0)))
        {
            return false;
        }
        if ((Turn.Card != null) && (Turn.EncounteredGuid == Turn.Card.GUID))
        {
            if (Rules.IsImmune(Turn.Card, this.TraitType))
            {
                return false;
            }
            if (!this.CardSelector.Match(Turn.Card))
            {
                return false;
            }
            for (int i = 0; i < card.Traits.Length; i++)
            {
                if (Rules.IsImmune(Turn.Card, card.Traits[i]))
                {
                    return false;
                }
            }
        }
        if (((Turn.Checks != null) && (Turn.Checks.Length > 0)) && !Rules.IsSkillParticipatingInCheck(this.SkillCheck, true))
        {
            return false;
        }
        return true;
    }

    protected override bool IsPowerValid(Card card)
    {
        if (!this.Stacking && Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if (((Turn.Checks != null) && (Turn.Checks.Length > 0)) && (!card.Locked && !Rules.IsSkillParticipatingInCheck(this.SkillCheck, false)))
        {
            return false;
        }
        return base.IsPowerValid(card);
    }

    protected void RefreshDicePanel(SkillCheckType overrideSkill)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            SkillCheckType skill = Turn.Owner.GetBestSkillCheck(Turn.Checks).skill;
            if (overrideSkill != SkillCheckType.None)
            {
                skill = overrideSkill;
            }
            window.dicePanel.SetCheck(Turn.Card, Turn.Checks, skill);
            window.dicePanel.Refresh();
        }
    }

    public override ActionType RechargeAction =>
        this.FinalDestination;
}

