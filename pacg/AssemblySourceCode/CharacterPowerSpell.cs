using System;
using UnityEngine;

public class CharacterPowerSpell : CharacterPower
{
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("the spell's skills: arcane, wisdom")]
    public SkillCheckType[] Skills = new SkillCheckType[] { SkillCheckType.Arcane };
    [Tooltip("the spell's traits")]
    public TraitType[] Traits;

    public override void Activate()
    {
        EffectCardRestrictionPending effect = Turn.Character.GetEffect(EffectType.CardRestrictionPending) as EffectCardRestrictionPending;
        if ((effect != null) && effect.Match(CardType.Spell))
        {
            effect.Invoke(this);
        }
        else
        {
            base.PowerBegin();
            Turn.EmptyLayoutDecks = false;
            Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerSpell_Cancel"));
            Turn.SetStateData(new TurnStateData(ActionType.Discard, 1));
            Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerSpell_Finish"));
            Turn.State = GameStateType.Power;
        }
    }

    private void AddDice()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
    }

    private void CharacterPowerSpell_Cancel()
    {
        this.PowerEnd();
        Turn.EmptyLayoutDecks = false;
        Turn.State = GameStateType.Combat;
        Turn.EmptyLayoutDecks = true;
    }

    public void CharacterPowerSpell_Finish()
    {
        Turn.Spell = "CharacterPowerSpell";
        base.Character.MarkCardType(CardType.Spell, true);
        Turn.MarkPowerActive(this, true);
        this.AddDice();
        Turn.DiceBonus += this.DiceBonus;
        Turn.AddTraits(this.Traits);
        Turn.CheckParticipants.AddRange(this.Skills);
        Card cardPlayed = this.GetCardPlayed();
        if (cardPlayed != null)
        {
            cardPlayed.SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
        }
        Turn.CombatSkill = this.GetBestSkill(this.Skills);
        Rules.ApplyCombatAdjustments();
        this.ResetDicePanel(SkillCheckType.Combat);
        Turn.State = GameStateType.Combat;
        Turn.EmptyLayoutDecks = true;
        this.PowerEnd();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Turn.Spell = null;
        base.Character.MarkCardType(CardType.Spell, false);
        this.RemoveDice();
        Turn.DiceBonus -= this.DiceBonus;
        Turn.RemoveTraits(this.Traits);
        for (int i = 0; i < this.Skills.Length; i++)
        {
            Turn.CheckParticipants.Remove(this.Skills[i]);
        }
        Card cardPlayed = this.GetCardPlayed();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((cardPlayed != null) && (window != null))
        {
            if (Turn.Character == base.Character)
            {
                window.layoutHand.OnGuiDrop(cardPlayed);
            }
            else
            {
                base.Character.Hand.Add(cardPlayed);
            }
            window.dicePanel.Refresh();
        }
        Turn.CombatSkill = Turn.Character.GetCombatSkill();
        this.ResetDicePanel(Turn.Check);
        Rules.ApplyCombatAdjustments();
    }

    private SkillCheckType GetBestSkill(SkillCheckType[] skills)
    {
        if ((skills != null) && (skills.Length > 0))
        {
            return this.Skills[0];
        }
        return Turn.Character.GetCombatSkill();
    }

    private Card GetCardPlayed()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            int index = window.layoutDiscard.IndexOf(base.Character.Powers.IndexOf(this), null);
            if (index >= 0)
            {
                return window.layoutDiscard.Deck[index];
            }
            index = window.layoutDiscard.IndexOf(-1, null);
            if (index >= 0)
            {
                return window.layoutDiscard.Deck[index];
            }
            if (window.layoutDiscard.Deck.Count > 0)
            {
                return window.layoutDiscard.Deck[window.layoutDiscard.Deck.Count - 1];
            }
        }
        return null;
    }

    public override void Initialize(Character self)
    {
        base.InitializeModifier(self, ref this.Traits);
    }

    public override bool IsLegalActivation()
    {
        if (Turn.Check != SkillCheckType.Combat)
        {
            return false;
        }
        return base.IsLegalActivation();
    }

    public override bool IsModifierActive(int n)
    {
        CharacterPowerModifier powerModifier = base.GetPowerModifier(n);
        if (powerModifier != null)
        {
            return base.IsModifierInTraits(powerModifier, this.Traits);
        }
        return base.IsModifierActive(n);
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.Character.GetNumberDiscardableCards() <= 0)
        {
            return false;
        }
        if (!base.IsCardPlayable(CardType.Spell))
        {
            return false;
        }
        if (!Rules.IsDiceRollPossible())
        {
            return false;
        }
        if ((Turn.Weapon1 != null) || (Turn.Weapon2 != null))
        {
            return false;
        }
        if (Turn.Item != null)
        {
            return false;
        }
        if (Turn.Spell != null)
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(CardType.Spell))
        {
            return false;
        }
        if (Rules.IsImmune(Turn.Card, this.Traits) && !base.ActivateBestModifier(ref this.Traits))
        {
            return false;
        }
        return Rules.IsCombatPossible();
    }

    private void RemoveDice()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Remove(this.Dice[i]);
        }
    }

    private void ResetDicePanel(SkillCheckType newCheck)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.SetCheck(Turn.Card, Turn.Card.Checks, newCheck);
        }
    }

    public override bool SetModifierActive(int n, bool active)
    {
        CharacterPowerModifier powerModifier = base.GetPowerModifier(n);
        if (powerModifier == null)
        {
            return false;
        }
        TraitType[] cardTraits = powerModifier.GetCardTraits();
        if (Rules.IsImmune(Turn.Card, cardTraits))
        {
            return false;
        }
        base.SetModifierTraits(active, powerModifier, ref this.Traits);
        return true;
    }
}

