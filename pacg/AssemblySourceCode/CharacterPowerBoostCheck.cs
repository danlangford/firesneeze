using System;
using System.Linq;
using UnityEngine;

public class CharacterPowerBoostCheck : CharacterPower
{
    [Tooltip("cost of this power. If none and this ability is passive this ability automatically activates when conditions are valid")]
    public ActionType Action;
    [Tooltip("the dice bonus to add")]
    public int DiceBonus;
    [Tooltip("the amount of dice to add")]
    public int DiceCount = 1;
    [Tooltip("the dice to add")]
    public DiceType DiceType;
    [Tooltip("Filter of cards that this ability will match correctly against. If null all cards will match.")]
    public CardSelector Filter;
    [Tooltip("returns the range this power applies")]
    public DamageRangeType Range = DamageRangeType.Short;
    [Tooltip("the skills to apply this ability to. None means any skill is valid.")]
    public SkillCheckType SkillCheckType;
    [Tooltip("the traittype to add to the dice")]
    public TraitType TraitType;

    public override void Activate()
    {
        base.PowerBegin();
        CardFilter filter = null;
        if (this.Filter != null)
        {
            filter = this.Filter.ToFilter();
        }
        EffectBoostCheck e = new EffectBoostCheck(Effect.GetEffectID(this), Effect.DurationCheck, filter, this.DiceType, this.TraitType, this.SkillCheckType, this.DiceBonus, this.DiceCount);
        Turn.Character.ApplyEffect(e);
        Turn.MarkPowerActive(this, true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Turn.Character.RemoveEffect(Effect.GetEffectID(this));
        this.PowerEnd();
    }

    public override void Initialize(Character self)
    {
        this.InitializeTypeModifier(self);
    }

    private void InitializeTypeModifier(Character self)
    {
        for (int i = 0; i < self.Powers.Count; i++)
        {
            if (self.Powers[i].Modifies(base.ID))
            {
                CharacterPowerModifier modifier = self.Powers[i] as CharacterPowerModifier;
                if (modifier != null)
                {
                    TraitType[] cardTraits = modifier.GetCardTraits();
                    if ((cardTraits != null) && (cardTraits.Length != 0))
                    {
                        for (int j = 0; j < base.Conditions.Length; j++)
                        {
                            if (base.Conditions[j].Condition is PowerConditionCard)
                            {
                                PowerConditionCard condition = base.Conditions[j].Condition as PowerConditionCard;
                                condition.Traits = condition.Traits.Union<TraitType>(cardTraits).ToArray<TraitType>();
                            }
                        }
                    }
                }
            }
        }
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!Rules.IsRangeValid(base.Character, this.Range))
        {
            return false;
        }
        return true;
    }

    public override bool Automatic =>
        ((this.Action == ActionType.None) && base.Passive);
}

