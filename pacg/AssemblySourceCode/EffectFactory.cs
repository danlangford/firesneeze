using System;

public class EffectFactory
{
    public static Effect Create(EffectType type, string source, int duration, SkillCheckValueType[] checks, CardFilter filter, int[] genericParameters)
    {
        switch (type)
        {
            case EffectType.AttributeChange:
                if (genericParameters.Length < 2)
                {
                    break;
                }
                return new EffectAttributeChange(source, duration, (AttributeType) genericParameters[0], (DiceType) genericParameters[1]);

            case EffectType.BoostCheck:
                if (genericParameters.Length < 5)
                {
                    break;
                }
                return new EffectBoostCheck(source, duration, filter, (DiceType) genericParameters[0], (TraitType) genericParameters[1], (SkillCheckType) genericParameters[2], genericParameters[3], genericParameters[4]);

            case EffectType.CardRestriction:
                return new EffectCardRestriction(source, duration, filter);

            case EffectType.CardRestrictionPending:
                return new EffectCardRestrictionPending(source, duration, checks, genericParameters[0], filter);

            case EffectType.ExploreRestriction:
                if (genericParameters.Length < 1)
                {
                    break;
                }
                return new EffectExploreRestriction(source, duration, (DispositionType) genericParameters[0], filter);

            case EffectType.Haunt:
                if (genericParameters.Length < 1)
                {
                    break;
                }
                return new EffectHaunt(source, duration, genericParameters[0]);

            case EffectType.ModifyAttribute:
                if (genericParameters.Length < 2)
                {
                    break;
                }
                return new EffectModifyAttribute(source, duration, (AttributeType) genericParameters[0], genericParameters[1]);

            case EffectType.ModifyCheck:
                if (genericParameters.Length < 1)
                {
                    break;
                }
                return new EffectModifyCheck(source, duration, genericParameters[0]);

            case EffectType.ModifyDice:
                if (genericParameters.Length < 1)
                {
                    break;
                }
                return new EffectModifyDice(source, duration, genericParameters[0]);

            case EffectType.ModifyDifficulty:
                if (genericParameters.Length < 4)
                {
                    break;
                }
                return new EffectModifyDifficulty(source, duration, genericParameters[0], (SkillCheckType) genericParameters[1], filter, genericParameters[2], genericParameters[3] != 0);

            case EffectType.ModifySkill:
                if (genericParameters.Length < 2)
                {
                    break;
                }
                return new EffectModifySkill(source, duration, (SkillCheckType) genericParameters[0], genericParameters[1]);

            case EffectType.MirrorImage:
                return new EffectMirrorImage(source, duration);

            case EffectType.ScenarioPower:
                return new EffectScenarioPower(source, duration);

            case EffectType.StackPower:
                if (genericParameters.Length < 3)
                {
                    break;
                }
                return new EffectStackScenarioPower((MetaCompareOperator) genericParameters[0], genericParameters[1], genericParameters[2], source, duration, filter);

            case EffectType.Nameable:
                if (genericParameters.Length < 1)
                {
                    break;
                }
                return new EffectNameable(source, duration, genericParameters[0]);

            case EffectType.AcquiredOutOfTotal:
                if (genericParameters.Length < 3)
                {
                    break;
                }
                return new EffectAcquiredOutOfTotal(source, duration, genericParameters[0], (CardType) genericParameters[1], genericParameters[2]);

            case EffectType.SkillChange:
                if (genericParameters.Length < 3)
                {
                    break;
                }
                return new EffectChangeSkill(source, duration, (SkillType) genericParameters[0], genericParameters[1], (AttributeType) genericParameters[2]);

            case EffectType.OnDefeatHeal:
                if (genericParameters.Length < 4)
                {
                    break;
                }
                return new EffectOnDefeatHeal(source, duration, filter, genericParameters[0], genericParameters[1], (DeckType) genericParameters[2], (DeckPositionType) genericParameters[3]);
        }
        return null;
    }
}

