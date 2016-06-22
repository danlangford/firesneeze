using System;
using UnityEngine;

public class CharacterPowerChangeCombatSkill : CharacterPower
{
    [Tooltip("the type of skill to adjust the combat to")]
    public SkillCheckType Skill = SkillCheckType.Dexterity;
    [Tooltip("additional traits to add to the check")]
    public TraitType[] Traits;

    public override void Activate()
    {
        base.Activate();
        base.PowerBegin();
        Turn.Weapon1 = "Unarmed";
        Turn.WeaponUnarmed = true;
        Turn.AddTraits(this.Traits);
        Turn.CheckParticipants.Add(this.Skill);
        Turn.CombatSkill = this.Skill;
        Rules.ApplyCombatAdjustments();
        this.ResetDicePanel(SkillCheckType.Combat);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.PowerEnd();
        Turn.Weapon1 = null;
        Turn.WeaponUnarmed = false;
        Turn.DamageTraits.Clear();
        Turn.CheckParticipants.Remove(this.Skill);
        Turn.CombatSkill = Turn.Character.GetCombatSkill();
        Rules.ApplyCombatAdjustments();
        this.ResetDicePanel(SkillCheckType.Combat);
    }

    public override void Initialize(Character self)
    {
        base.InitializeModifier(self, ref this.Traits);
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
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
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
        if (Turn.CombatSkill == SkillCheckType.None)
        {
            return false;
        }
        if (!string.IsNullOrEmpty(Turn.Weapon1) && (Turn.Weapon1 != "Unarmed"))
        {
            return false;
        }
        if (Rules.IsImmune(Turn.Card, this.Traits) && !base.ActivateBestModifier(ref this.Traits))
        {
            return false;
        }
        return Rules.IsCombatPossible();
    }

    public override void OnCheckCompleted()
    {
        if (base.Cooldown == PowerCooldownType.Check)
        {
            this.PowerEnd();
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

