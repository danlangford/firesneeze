using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class CharacterPowerChangeAttributeDie : CharacterPower
{
    [Tooltip("the power's attributes that it may be used on")]
    public AttributeType[] Attributes = new AttributeType[1];
    [Tooltip("the bonus to be applied to the check")]
    public int DiceBonus;
    [Tooltip("die to replace attribute with")]
    public DiceType Die = DiceType.D10;
    [Tooltip("the cost of activating this power")]
    public ActionType Penalty = ActionType.Discard;
    [Tooltip("additional traits to be added")]
    public TraitType[] Traits;

    public override void Activate()
    {
        if (this.Cancellable)
        {
            Turn.EmptyLayoutDecks = false;
        }
        base.PowerBegin();
        Turn.PushReturnState();
        Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerChangeAttributeDie_Cancel"));
        Turn.SetStateData(new TurnStateData(this.Penalty, 1));
        Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerChangeAttributeDie_Finish"));
        Turn.State = GameStateType.Power;
    }

    private void CharacterPowerChangeAttributeDie_Cancel()
    {
        Turn.EmptyLayoutDecks = false;
        Turn.ReturnToReturnState();
        this.PowerEnd();
        Turn.EmptyLayoutDecks = true;
    }

    public void CharacterPowerChangeAttributeDie_Finish()
    {
        Turn.MarkPowerActive(this, true);
        Turn.DiceBonus += this.DiceBonus;
        Turn.AddTraits(this.Traits);
        int durationTurn = Effect.DurationTurn;
        if (base.Cooldown == PowerCooldownType.Check)
        {
            durationTurn = Effect.DurationCheck;
        }
        string effectID = Effect.GetEffectID(this);
        for (int i = 0; i < this.Attributes.Length; i++)
        {
            Effect e = new EffectAttributeChange(effectID, durationTurn, this.Attributes[i], this.Die);
            Turn.Character.ApplyEffect(e);
        }
        Card cardPlayed = base.GetCardPlayed(this.Penalty);
        if (cardPlayed != null)
        {
            cardPlayed.SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
        }
        Rules.ApplyCombatAdjustments();
        Turn.ReturnToReturnState();
        this.ResetDicePanel(SkillCheckType.None);
        this.PowerEnd();
        Turn.EmptyLayoutDecks = true;
    }

    private bool CheckContainsSkill()
    {
        for (int i = 0; i < this.Attributes.Length; i++)
        {
            if (Rules.IsSkillParticipatingInCheck(this.Attributes[i].ToSkillCheckType(), true))
            {
                return true;
            }
        }
        return false;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Turn.DiceBonus -= this.DiceBonus;
        for (int i = 0; i < this.Traits.Length; i++)
        {
            Turn.DamageTraits.Remove(this.Traits[i]);
        }
        for (int j = 0; j < this.Attributes.Length; j++)
        {
            Turn.Character.RemoveEffect(Effect.GetEffectID(this));
        }
        Card cardPlayed = base.GetCardPlayed(this.Penalty);
        if (cardPlayed != null)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if (Turn.Character == base.Character)
                {
                    window.layoutHand.OnGuiDrop(cardPlayed);
                }
                else
                {
                    base.Character.Hand.Add(cardPlayed);
                }
            }
        }
        Rules.ApplyCombatAdjustments();
        this.ResetDicePanel(Turn.Check);
    }

    public override void Initialize(Character self)
    {
        base.InitializeModifier(self, ref this.Traits);
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.Character.GetNumberDiscardableCards() < 1)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!Rules.IsCheck())
        {
            return false;
        }
        return this.CheckContainsSkill();
    }

    private void ResetDicePanel(SkillCheckType newCheck = 0)
    {
        SkillCheckValueType[] checks = Turn.Checks;
        if (checks == null)
        {
            checks = Turn.Card.Checks;
        }
        if (newCheck == SkillCheckType.None)
        {
            newCheck = base.Character.GetBestSkillCheck(checks).skill;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.SetCheck(Turn.Card, checks, newCheck);
        }
    }
}

