using System;
using System.Linq;
using UnityEngine;

public class CharacterPowerModifier : BaseCharacterPowerMod
{
    [Tooltip("add these action types to the power's penalty action")]
    public ActionType[] AdditionalActions;
    [Tooltip("add these traits to the power")]
    public TraitType[] CardTraits;
    [Tooltip("add these card types to the power")]
    public CardType[] CardTypes;
    [Tooltip("the relative order of modifiers within the family")]
    public int Number;
    [Tooltip("can this modifier be toggled on/off by the player?")]
    public bool Optional;
    [Tooltip("change the range of the power to this")]
    public DamageRangeType Range = DamageRangeType.None;
    [Tooltip("remove these traits from the power")]
    public TraitType[] RemoveTraits;

    public TraitType[] CombineModTraits(TraitType[] powerTraits)
    {
        powerTraits = powerTraits.Except<TraitType>(this.RemoveTraits).ToArray<TraitType>();
        powerTraits = powerTraits.Union<TraitType>(this.CardTraits).ToArray<TraitType>();
        return powerTraits;
    }

    public virtual TraitType[] GetCardTraits()
    {
        if ((this.CardTraits != null) && (this.CardTraits.Length != 0))
        {
            return this.CardTraits;
        }
        return null;
    }

    public virtual CardType[] GetCardTypes()
    {
        if ((this.CardTypes != null) && (this.CardTypes.Length != 0))
        {
            return this.CardTypes;
        }
        return null;
    }

    public TraitType[] RevertModTraits(TraitType[] powerTraits)
    {
        powerTraits = powerTraits.Except<TraitType>(this.CardTraits).ToArray<TraitType>();
        powerTraits = powerTraits.Concat<TraitType>(this.RemoveTraits).ToArray<TraitType>();
        return powerTraits;
    }
}

