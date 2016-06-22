using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Extensions
{
    public static bool Evaluate(this MetaCompareOperator meta, int a, int b)
    {
        if (meta == MetaCompareOperator.Equals)
        {
            return (a == b);
        }
        if (meta == MetaCompareOperator.Less)
        {
            return (a < b);
        }
        if (meta == MetaCompareOperator.LessOrEqual)
        {
            return (a <= b);
        }
        if (meta == MetaCompareOperator.More)
        {
            return (a > b);
        }
        return ((meta == MetaCompareOperator.MoreOrEqual) && (a >= b));
    }

    public static T Extract<T>(this IList<T> list, int index)
    {
        if ((index >= 0) && (index < list.Count))
        {
            T local = list[index];
            list.RemoveAt(index);
            return local;
        }
        return default(T);
    }

    public static Deck GetDeck(this DeckType type)
    {
        if (type == DeckType.Character)
        {
            return Turn.Character.Deck;
        }
        if (type == DeckType.Discard)
        {
            return Turn.Character.Discard;
        }
        if (type == DeckType.Bury)
        {
            return Turn.Character.Bury;
        }
        if (type == DeckType.Hand)
        {
            return Turn.Character.Hand;
        }
        if (type == DeckType.Revealed)
        {
            return Turn.Character.Hand;
        }
        if (type == DeckType.Location)
        {
            return Location.Current.Deck;
        }
        return null;
    }

    public static GlossaryEntry[] GetEntryArray(this GlossaryCategory category)
    {
        switch (category)
        {
            case GlossaryCategory.CoreConcepts:
                return GuiPanelRules.MainGlossaryEntryList.CoreConcepts;

            case GlossaryCategory.Phases:
                return GuiPanelRules.MainGlossaryEntryList.Phases;

            case GlossaryCategory.Terms:
                return GuiPanelRules.MainGlossaryEntryList.Terms;

            case GlossaryCategory.Tips:
                return GuiPanelRules.MainGlossaryEntryList.Tips;

            case GlossaryCategory.DoNotForget:
                return GuiPanelRules.MainGlossaryEntryList.DoNotForget;

            case GlossaryCategory.AttemptingACheck:
                return GuiPanelRules.MainGlossaryEntryList.AttemptingACheck;
        }
        return null;
    }

    public static GuiLayout GetLayout(this DeckType type)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            return window.GetLayoutDeck(type);
        }
        return null;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int count = list.Count;
        while (count > 1)
        {
            count--;
            int num2 = UnityEngine.Random.Range(0, count + 1);
            T local = list[num2];
            list[num2] = list[count];
            list[count] = local;
        }
    }

    public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
    {
        if (firstIndex != secondIndex)
        {
            T local = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = local;
        }
    }

    public static ActionType ToActionType(this DeckType type)
    {
        if (type == DeckType.Discard)
        {
            return ActionType.Discard;
        }
        if (type == DeckType.Bury)
        {
            return ActionType.Bury;
        }
        if (type == DeckType.Character)
        {
            return ActionType.Recharge;
        }
        if (type == DeckType.Banish)
        {
            return ActionType.Banish;
        }
        if (type == DeckType.Revealed)
        {
            return ActionType.Reveal;
        }
        if (type == DeckType.Hand)
        {
            return ActionType.Draw;
        }
        return ActionType.None;
    }

    public static AttributeType ToAttributeType(this SkillCheckType skill)
    {
        if (skill == SkillCheckType.Charisma)
        {
            return AttributeType.Charisma;
        }
        if (skill == SkillCheckType.Constitution)
        {
            return AttributeType.Constitution;
        }
        if (skill == SkillCheckType.Dexterity)
        {
            return AttributeType.Dexterity;
        }
        if (skill == SkillCheckType.Intelligence)
        {
            return AttributeType.Intelligence;
        }
        if (skill == SkillCheckType.Strength)
        {
            return AttributeType.Strength;
        }
        if (skill == SkillCheckType.Wisdom)
        {
            return AttributeType.Wisdom;
        }
        return AttributeType.None;
    }

    public static DeckType ToDeckType(this ActionType type)
    {
        if (type == ActionType.Discard)
        {
            return DeckType.Discard;
        }
        if (type == ActionType.Bury)
        {
            return DeckType.Bury;
        }
        if (type == ActionType.Recharge)
        {
            return DeckType.Character;
        }
        if (type == ActionType.Top)
        {
            return DeckType.Character;
        }
        if (type == ActionType.Banish)
        {
            return DeckType.Banish;
        }
        if (type == ActionType.Reveal)
        {
            return DeckType.Revealed;
        }
        if (type == ActionType.Draw)
        {
            return DeckType.Hand;
        }
        return DeckType.None;
    }

    public static SkillCheckType ToSkillCheckType(this AttributeType attribute)
    {
        if (attribute == AttributeType.Charisma)
        {
            return SkillCheckType.Charisma;
        }
        if (attribute == AttributeType.Constitution)
        {
            return SkillCheckType.Constitution;
        }
        if (attribute == AttributeType.Dexterity)
        {
            return SkillCheckType.Dexterity;
        }
        if (attribute == AttributeType.Intelligence)
        {
            return SkillCheckType.Intelligence;
        }
        if (attribute == AttributeType.Strength)
        {
            return SkillCheckType.Strength;
        }
        if (attribute == AttributeType.Wisdom)
        {
            return SkillCheckType.Wisdom;
        }
        return SkillCheckType.None;
    }

    public static SkillType ToSkillType(this SkillCheckType skill)
    {
        if (skill == SkillCheckType.Melee)
        {
            return SkillType.Melee;
        }
        if (skill == SkillCheckType.Ranged)
        {
            return SkillType.Ranged;
        }
        if (skill == SkillCheckType.Diplomacy)
        {
            return SkillType.Diplomacy;
        }
        if (skill == SkillCheckType.Perception)
        {
            return SkillType.Perception;
        }
        if (skill == SkillCheckType.Survival)
        {
            return SkillType.Survival;
        }
        if (skill == SkillCheckType.Arcane)
        {
            return SkillType.Arcane;
        }
        if (skill == SkillCheckType.Divine)
        {
            return SkillType.Divine;
        }
        if (skill == SkillCheckType.Knowledge)
        {
            return SkillType.Knowledge;
        }
        if (skill == SkillCheckType.Acrobatics)
        {
            return SkillType.Acrobatics;
        }
        if (skill == SkillCheckType.Fortitude)
        {
            return SkillType.Fortitude;
        }
        if (skill == SkillCheckType.Disable)
        {
            return SkillType.Disable;
        }
        if (skill == SkillCheckType.Stealth)
        {
            return SkillType.Stealth;
        }
        return SkillType.None;
    }

    public static SoundEffectType ToSoundtype(this VisualEffectType type)
    {
        switch (type)
        {
            case VisualEffectType.CardWinEnemy:
            case VisualEffectType.CardLoseEnemy:
                return SoundEffectType.MeleeDamage;

            case VisualEffectType.CardWinEnemyFire:
            case VisualEffectType.CardLoseEnemyFire:
                return SoundEffectType.FireDamage;

            case VisualEffectType.CardWinEnemyForce:
            case VisualEffectType.CardLoseEnemyForce:
                return SoundEffectType.ForceDamage;

            case VisualEffectType.CardWinEnemyAcid:
            case VisualEffectType.CardLoseEnemyAcid:
                return SoundEffectType.AcidDamage;

            case VisualEffectType.CardWinEnemyCold:
            case VisualEffectType.CardLoseEnemyCold:
                return SoundEffectType.ColdDamage;

            case VisualEffectType.CardWinEnemyPoison:
            case VisualEffectType.CardLoseEnemyPoison:
                return SoundEffectType.PoisonDamage;

            case VisualEffectType.CardWinEnemyElectricity:
            case VisualEffectType.CardLoseEnemyElectricity:
                return SoundEffectType.ElectricityDamage;

            case VisualEffectType.CardWinEnemyMental:
            case VisualEffectType.CardLoseEnemyMental:
                return SoundEffectType.MentalDamage;

            case VisualEffectType.CardWinEnemyMeleeBlunt:
            case VisualEffectType.CardLoseEnemyMeleeBlunt:
            case VisualEffectType.CardLoseEnemyRangedBlunt:
                return SoundEffectType.BludgMeleeDamage;

            case VisualEffectType.CardWinEnemyRangedPiercing:
            case VisualEffectType.CardLoseEnemyRangedPiercing:
                return SoundEffectType.PierceRangedDamage;

            case VisualEffectType.CardWinEnemyLiquid:
                return SoundEffectType.LiquidDamage;

            case VisualEffectType.CardLoseEnemyMeleePiercing:
                return SoundEffectType.PierceMeleeDamage;
        }
        return SoundEffectType.None;
    }

    public static string ToText(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        if (field != null)
        {
            StrRefAttr[] customAttributes = (StrRefAttr[]) field.GetCustomAttributes(typeof(StrRefAttr), false);
            if (customAttributes.Length > 0)
            {
                return customAttributes[0].Text;
            }
        }
        return value.ToString();
    }

    public static TraitType ToTraitType(this SkillCheckType skillCheck)
    {
        switch (skillCheck)
        {
            case SkillCheckType.Arcane:
                return TraitType.Arcane;

            case SkillCheckType.Combat:
                return TraitType.Combat;

            case SkillCheckType.Melee:
                return TraitType.Melee;

            case SkillCheckType.Ranged:
                return TraitType.Ranged;

            case SkillCheckType.Divine:
                return TraitType.Divine;
        }
        return TraitType.None;
    }
}

