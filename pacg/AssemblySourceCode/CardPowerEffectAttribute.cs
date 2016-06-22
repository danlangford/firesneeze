using System;
using UnityEngine;

public class CardPowerEffectAttribute : CardPower
{
    [Tooltip("the amount of bonus or penalty")]
    public int Amount = 1;
    [Tooltip("the attribute that this effect applies to")]
    public AttributeType Attribute;
    [Tooltip("the number of turns that this effect lasts")]
    public int Duration = 2;
    [Tooltip("what happens to the card after everything is all said and done")]
    public ActionType FinalDestination = ActionType.Discard;
    [Tooltip("the type of targeting mode to use")]
    public TargetType Range = TargetType.AllAtLocation;

    public override void Activate(Card card)
    {
        Turn.EmptyLayoutDecks = false;
        Turn.PushReturnState();
        if (Rules.IsTargetRequired(this.Range))
        {
            if (Rules.IsCheck())
            {
                Turn.Target = Turn.Current;
                Turn.PopReturnState();
                this.EffectAttribute_Confirm();
            }
            else
            {
                this.EffectAttribute_Target(card);
            }
        }
        else
        {
            Turn.Target = Turn.Number;
            this.EffectAttribute_Confirm();
        }
    }

    private void ApplyEffectAttribute()
    {
        Character character = Party.Characters[Turn.Target];
        string source = null;
        if (base.Card != null)
        {
            source = base.Card.ID;
        }
        Effect e = new EffectModifyAttribute(source, this.Duration, this.Attribute, this.Amount);
        character.ApplyEffect(e);
        this.RefreshDicePanel();
        Turn.Character.MarkCardType(base.Card.Type, true);
        base.LockInDisplayed(true);
        Turn.EmptyLayoutDecks = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(false);
        }
    }

    public override void Deactivate(Card card)
    {
        if ((Turn.State == GameStateType.ConfirmProceed) || (Turn.State == GameStateType.Target))
        {
            Turn.GotoCancelDestination();
        }
        else
        {
            Turn.Character.MarkCardType(card.Type, false);
            Turn.Owner.RemoveEffect(base.Card.ID);
            Turn.PopStateDestination();
            this.RefreshDicePanel();
        }
    }

    private void EffectAttribute_Activate()
    {
        if (Turn.ReturnState != GameStateType.None)
        {
            Turn.PushStateDestination(Turn.PopReturnState());
            Turn.Proceed();
        }
        this.ApplyEffectAttribute();
    }

    private void EffectAttribute_Confirm()
    {
        if (base.IsLockInDisplayNecessary())
        {
            Turn.PushCancelDestination(new TurnStateCallback(base.Card, "EffectAttribute_Deactivate"));
            Turn.PushStateDestination(new TurnStateCallback(base.Card, "EffectAttribute_Activate"));
            Turn.State = GameStateType.ConfirmProceed;
        }
        else
        {
            Turn.PopReturnState();
            this.ApplyEffectAttribute();
        }
    }

    private void EffectAttribute_Deactivate()
    {
        if (Turn.ReturnState != GameStateType.None)
        {
            Turn.PushStateDestination(Turn.PopReturnState());
            Turn.Proceed();
        }
        Turn.EmptyLayoutDecks = true;
        base.Card.OnCardDeactivated();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutHand.ReturnCard(base.Card.GUID);
            window.layoutHand.Refresh();
        }
    }

    private void EffectAttribute_Target(Card card)
    {
        Turn.TargetType = this.Range;
        Turn.PushCancelDestination(new TurnStateCallback(card, "EffectAttribute_Deactivate"));
        Turn.PushStateDestination(new TurnStateCallback(card, "EffectAttribute_Activate"));
        GameStateTarget.DisplayText = card.DisplayName;
        Turn.State = GameStateType.Target;
    }

    public override bool IsEqualOrBetter(CardPower x)
    {
        CardPowerEffectAttribute attribute = x as CardPowerEffectAttribute;
        if (attribute == null)
        {
            return false;
        }
        if (this.Attribute != attribute.Attribute)
        {
            return false;
        }
        return (this.Amount >= attribute.Amount);
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
        if (((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup)) && !Rules.IsCheck())
        {
            return false;
        }
        if (!this.MatchesSkill())
        {
            return false;
        }
        if (card.Locked)
        {
            return false;
        }
        return true;
    }

    private bool MatchesSkill()
    {
        if ((Turn.Checks == null) || (Turn.Checks.Length == 0))
        {
            return true;
        }
        for (int i = 0; i < Turn.Checks.Length; i++)
        {
            SkillCheckType skill = Turn.Checks[i].skill;
            bool flag = (skill == SkillCheckType.Combat) && (Turn.CombatSkill == SkillCheckType.Melee);
            if (skill == SkillCheckType.Combat)
            {
                skill = Turn.CombatSkill;
            }
            if (this.Attribute.ToSkillCheckType() == skill)
            {
                return true;
            }
            for (int j = 0; j < Turn.Owner.Skills.Length; j++)
            {
                if ((Turn.Owner.Skills[j].attribute == this.Attribute) && (skill.ToSkillType() == Turn.Owner.Skills[j].skill))
                {
                    return true;
                }
            }
            if (flag && (this.Attribute == AttributeType.Strength))
            {
                return true;
            }
        }
        return false;
    }

    protected void RefreshDicePanel()
    {
        int num = Turn.Owner.GetSkillBonus(Turn.Check) - Turn.LastCheckBonus;
        Turn.DiceBonus += num;
        Turn.LastCheckBonus += num;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    public override ActionType RechargeAction =>
        this.FinalDestination;
}

