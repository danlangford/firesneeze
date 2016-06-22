using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Rules
{
    private static int[] CrookedDiceValues = new int[13];

    public static void ApplyCombatAdjustments()
    {
        if (Turn.Card != null)
        {
            CardPropertyVulnerability component = Turn.Card.GetComponent<CardPropertyVulnerability>();
            if (component != null)
            {
                component.Apply();
            }
        }
    }

    public static void CrookedDice(DiceType dice, int roll)
    {
        if (dice == DiceType.D4)
        {
            roll = Mathf.Clamp(roll, 1, 4);
        }
        if (dice == DiceType.D6)
        {
            roll = Mathf.Clamp(roll, 1, 6);
        }
        if (dice == DiceType.D8)
        {
            roll = Mathf.Clamp(roll, 1, 8);
        }
        if (dice == DiceType.D10)
        {
            roll = Mathf.Clamp(roll, 1, 10);
        }
        if (dice == DiceType.D12)
        {
            roll = Mathf.Clamp(roll, 1, 12);
        }
        CrookedDiceValues[(int) dice] = roll;
    }

    public static void EnforceLegalDice()
    {
        for (int i = 0; i < CrookedDiceValues.Length; i++)
        {
            CrookedDiceValues[i] = 0;
        }
    }

    public static int GetAbilityCheckBonus()
    {
        int num = 0;
        for (int i = 0; i < Turn.Owner.Powers.Count; i++)
        {
            if ((Turn.Owner.Powers[i].Automatic && Turn.IsPowerActive(Turn.Owner.Powers[i].ID)) && (Turn.Owner.Powers[i] is CharacterPowerAddSkillDice))
            {
                CharacterPowerAddSkillDice dice = Turn.Owner.Powers[i] as CharacterPowerAddSkillDice;
                num += dice.DiceBonus;
            }
        }
        return num;
    }

    public static VisualEffectType GetCardLoseVfx(List<TraitType> traits)
    {
        if (traits != null)
        {
            for (int i = 0; i < traits.Count; i++)
            {
                if (((TraitType) traits[i]) == TraitType.Fire)
                {
                    return VisualEffectType.CardLoseEnemyFire;
                }
                if (((TraitType) traits[i]) == TraitType.Force)
                {
                    return VisualEffectType.CardLoseEnemyForce;
                }
                if (((TraitType) traits[i]) == TraitType.Cold)
                {
                    return VisualEffectType.CardLoseEnemyCold;
                }
                if (((TraitType) traits[i]) == TraitType.Acid)
                {
                    return VisualEffectType.CardLoseEnemyAcid;
                }
                if (((TraitType) traits[i]) == TraitType.Poison)
                {
                    return VisualEffectType.CardLoseEnemyPoison;
                }
                if (((TraitType) traits[i]) == TraitType.Electricity)
                {
                    return VisualEffectType.CardLoseEnemyElectricity;
                }
                if (((TraitType) traits[i]) == TraitType.Mental)
                {
                    return VisualEffectType.CardLoseEnemyMental;
                }
                if (traits.Contains(TraitType.Ranged))
                {
                    return VisualEffectType.CardLoseEnemyRangedPiercing;
                }
                if (((TraitType) traits[i]) == TraitType.Bludgeoning)
                {
                    if (traits.Contains(TraitType.Ranged))
                    {
                        return VisualEffectType.CardLoseEnemyRangedBlunt;
                    }
                    return VisualEffectType.CardLoseEnemyMeleeBlunt;
                }
            }
        }
        return VisualEffectType.CardLoseEnemy;
    }

    public static int GetCardsToDiscardCount()
    {
        int num = 0;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            num = Turn.Damage - window.layoutDiscard.Deck.Count;
        }
        return num;
    }

    public static VisualEffectType GetCardWinVfx(List<TraitType> traits)
    {
        if (traits != null)
        {
            for (int i = 0; i < traits.Count; i++)
            {
                if (((TraitType) traits[i]) == TraitType.Liquid)
                {
                    return VisualEffectType.CardWinEnemyLiquid;
                }
                if (((TraitType) traits[i]) == TraitType.Fire)
                {
                    return VisualEffectType.CardWinEnemyFire;
                }
                if (((TraitType) traits[i]) == TraitType.Force)
                {
                    return VisualEffectType.CardWinEnemyForce;
                }
                if (((TraitType) traits[i]) == TraitType.Cold)
                {
                    return VisualEffectType.CardWinEnemyCold;
                }
                if (((TraitType) traits[i]) == TraitType.Acid)
                {
                    return VisualEffectType.CardWinEnemyAcid;
                }
                if (((TraitType) traits[i]) == TraitType.Poison)
                {
                    return VisualEffectType.CardWinEnemyPoison;
                }
                if (((TraitType) traits[i]) == TraitType.Electricity)
                {
                    return VisualEffectType.CardWinEnemyElectricity;
                }
                if (((TraitType) traits[i]) == TraitType.Mental)
                {
                    return VisualEffectType.CardWinEnemyMental;
                }
                if (((TraitType) traits[i]) == TraitType.Bludgeoning)
                {
                    return VisualEffectType.CardWinEnemyMeleeBlunt;
                }
                if (((TraitType) traits[i]) == TraitType.Ranged)
                {
                    return VisualEffectType.CardWinEnemyRangedPiercing;
                }
            }
        }
        return VisualEffectType.CardWinEnemy;
    }

    public static int GetCharactersWithDiscardCount(string locID)
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Alive && (Party.Characters[i].Location == locID)) && (Party.Characters[i].Discard.Count > 0))
            {
                num++;
            }
        }
        return num;
    }

    public static int GetCheckBonus(SkillCheckType skill)
    {
        int skillBonus = Turn.Owner.GetSkillBonus(skill);
        if (skill == SkillCheckType.Combat)
        {
            for (int i = 0; i < Turn.CheckParticipants.Count; i++)
            {
                skillBonus = Mathf.Max(skillBonus, Turn.Owner.GetSkillBonus(Turn.CheckParticipants[i]));
            }
        }
        return ((skillBonus + Scenario.Current.GetCheckBonus(skill)) + Location.Current.GetCheckBonus(skill));
    }

    public static DiceType[] GetCheckDice(SkillCheckType skill)
    {
        Party.AutoActivateAbilities();
        List<DiceType> list = null;
        int numEffectsOfType = Turn.Owner.GetNumEffectsOfType(EffectType.BoostCheck);
        if (numEffectsOfType > 0)
        {
            list = new List<DiceType>(numEffectsOfType);
            for (int i = 0; i < Turn.Owner.GetNumEffects(); i++)
            {
                EffectBoostCheck effect = Turn.Owner.GetEffect(i) as EffectBoostCheck;
                if ((effect != null) && effect.Match(Turn.Card, skill))
                {
                    for (int j = 0; j < effect.DiceCount; j++)
                    {
                        list.Add(effect.DiceType);
                    }
                    if (effect.TraitType != TraitType.None)
                    {
                        Turn.DamageTraits.Add(effect.TraitType);
                    }
                    ApplyCombatAdjustments();
                }
            }
        }
        DiceType checkDice = Scenario.Current.GetCheckDice(skill);
        DiceType type2 = Location.Current.GetCheckDice(skill);
        DiceType skillDice = Turn.Owner.GetSkillDice(skill);
        if (skill == SkillCheckType.Combat)
        {
            for (int k = 0; k < Turn.CheckParticipants.Count; k++)
            {
                DiceType type4 = Turn.Owner.GetSkillDice(Turn.CheckParticipants[k]);
                if (type4 > skillDice)
                {
                    skillDice = type4;
                }
            }
        }
        int num5 = 0;
        if (checkDice != DiceType.D0)
        {
            num5++;
        }
        if (type2 != DiceType.D0)
        {
            num5++;
        }
        if (skillDice != DiceType.D0)
        {
            num5++;
        }
        if (list != null)
        {
            for (int m = 0; m < list.Count; m++)
            {
                if (((DiceType) list[m]) != DiceType.D0)
                {
                    num5++;
                }
            }
        }
        if (Turn.BonusCheckDice > 0)
        {
            num5 += Turn.BonusCheckDice;
        }
        int num7 = 0;
        DiceType[] typeArray = new DiceType[num5];
        if (skillDice != DiceType.D0)
        {
            typeArray[num7++] = skillDice;
            for (int n = 0; n < Turn.BonusCheckDice; n++)
            {
                typeArray[num7++] = skillDice;
            }
        }
        else if (Turn.Dice.Count > 0)
        {
            for (int num9 = 0; num9 < Turn.BonusCheckDice; num9++)
            {
                typeArray[num7++] = Turn.Dice[0];
            }
        }
        if (checkDice != DiceType.D0)
        {
            typeArray[num7++] = checkDice;
        }
        if (type2 != DiceType.D0)
        {
            typeArray[num7++] = type2;
        }
        if (list != null)
        {
            for (int num10 = 0; num10 < list.Count; num10++)
            {
                if (((DiceType) list[num10]) != DiceType.D0)
                {
                    typeArray[num7++] = list[num10];
                }
            }
        }
        return typeArray;
    }

    public static int GetCheckValue(Card card, SkillCheckType skill)
    {
        int b = 0;
        if (card != null)
        {
            SkillCheckValueType[] checks;
            if (Turn.Checks != null)
            {
                checks = Turn.Checks;
            }
            else
            {
                checks = card.Checks;
            }
            for (int i = 0; i < checks.Length; i++)
            {
                if (checks[i].skill == skill)
                {
                    b = ((((checks[i].Rank + card.GetCheckModifier()) + Scenario.Current.GetCheckModifier()) + Location.Current.GetCheckModifier()) + Turn.Owner.GetCheckModifier()) + Turn.Owner.GetDifficultyModifier();
                }
            }
        }
        return Mathf.Max(0, b);
    }

    public static int GetCheckValue(Card card, int checkValue)
    {
        if (card != null)
        {
            checkValue += (card.GetCheckModifier() + Scenario.Current.GetCheckModifier()) + Location.Current.GetCheckModifier();
        }
        return checkValue;
    }

    public static int GetDiceModifier(DiceType diceType)
    {
        int num = 0;
        if (Turn.Checks != null)
        {
            Event[] components = Scenario.Current.GetLocationPowersRoot(Location.Current.ID).GetComponents<Event>();
            for (int i = 0; i < components.Length; i++)
            {
                num += components[i].GetDiceModifier(diceType);
            }
            Event[] eventArray2 = Turn.Card.GetComponents<Event>();
            for (int j = 0; j < eventArray2.Length; j++)
            {
                num += eventArray2[j].GetDiceModifier(diceType);
            }
            for (int k = 0; k < Turn.Owner.GetNumEffects(); k++)
            {
                Effect effect = Turn.Owner.GetEffect(k);
                if (effect.Type == EffectType.ModifyDice)
                {
                    EffectModifyDice dice = effect as EffectModifyDice;
                    if (dice != null)
                    {
                        num += dice.GetDiceModifier(diceType);
                    }
                }
            }
        }
        return num;
    }

    public static float GetDiceWeight(DiceType dice)
    {
        if (dice == DiceType.D2)
        {
            return 1.5f;
        }
        if (dice == DiceType.D4)
        {
            return 2.5f;
        }
        if (dice == DiceType.D6)
        {
            return 3.5f;
        }
        if (dice == DiceType.D8)
        {
            return 4.5f;
        }
        if (dice == DiceType.D10)
        {
            return 5.5f;
        }
        if (dice == DiceType.D12)
        {
            return 6.5f;
        }
        return 0f;
    }

    public static int GetExperiencePointsForLevel(int level) => 
        Game.Rewards.GetExperiencePointsForLevel(level);

    public static int GetLevelFromExperiencePoints(int xp) => 
        Game.Rewards.GetLevelFromExperiencePoints(xp);

    public static CardType GetMarkedType(Card card)
    {
        if (((card.Type != CardType.Henchman) && (card.Type != CardType.Villain)) && (card.Type != CardType.Loot))
        {
            return card.Type;
        }
        return card.SubType;
    }

    public static DiceType GetModifiedDice(Character character, Card card, DiceType dice)
    {
        for (int i = 0; i < character.Powers.Count; i++)
        {
            CharacterPowerChangeCardDice component = character.Powers[i].GetComponent<CharacterPowerChangeCardDice>();
            if (component != null)
            {
                dice = component.GetModifiedDice(card, dice);
            }
        }
        return dice;
    }

    public static Guid GetRerollCardGuid(Card card)
    {
        if (card.GetComponent<CardPropertyReroll>() != null)
        {
            return card.GUID;
        }
        return Guid.Empty;
    }

    public static int GetTierFromExperiencePoints(int xp) => 
        Game.Rewards.GetTierFromExperiencePoints(xp);

    public static string GetTierName(int tier)
    {
        if (tier <= 0)
        {
            return "B";
        }
        return tier.ToString();
    }

    public static int GetWeaponProficiencyAdjustment(Card card)
    {
        CardPropertyProficiencyPenalty component = card.GetComponent<CardPropertyProficiencyPenalty>();
        if (component != null)
        {
            return component.GetPenality(Turn.Character);
        }
        return 0;
    }

    public static bool IsAnyActionPossible()
    {
        for (int i = 0; i < Turn.Owner.Hand.Count; i++)
        {
            if (Turn.Owner.Hand[i].IsAnyActionValid())
            {
                return true;
            }
        }
        for (int j = 0; j < Turn.Owner.Powers.Count; j++)
        {
            if (Turn.Owner.Powers[j].IsValid() && !Turn.Owner.Powers[j].Passive)
            {
                return true;
            }
        }
        for (int k = 0; k < Location.Current.Powers.Count; k++)
        {
            if (Location.Current.Powers[k].IsValid() && !Location.Current.Powers[k].Passive)
            {
                return true;
            }
        }
        for (int m = 0; m < Scenario.Current.Powers.Count; m++)
        {
            if (Scenario.Current.Powers[m].IsValid() && !Scenario.Current.Powers[m].Passive)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsCancelOutOfDiscardPossible() => 
        (Turn.DestructiveActionsCount <= 1);

    public static bool IsCardAcquiredAutomatically(Card card) => 
        ((card.GetComponent<CardPropertyAcquire>() != null) || (card.Type == CardType.Loot));

    public static bool IsCardDefeatedAutomatically(Card card) => 
        (card.Checks.Length == 0);

    public static bool IsCardRechargable(Character character, Card card)
    {
        if (!card.Played)
        {
            return false;
        }
        if (card.PlayedOwner != character.ID)
        {
            return false;
        }
        if (!IsRechargeAutomatic(character, card))
        {
            if (card.Recharge.Length <= 0)
            {
                return false;
            }
            CardPropertyRecharge component = card.GetComponent<CardPropertyRecharge>();
            if (component != null)
            {
                CardPower playedCardPower = card.GetPlayedCardPower();
                if ((playedCardPower != null) && !component.AllowedRecharge.Contains(playedCardPower.RechargeAction))
                {
                    return false;
                }
            }
            if ((!card.Displayed && (card.Deck != null)) && ((card.Deck.Layout != null) && (card.Deck.Layout.CardAction != card.Powers[card.PlayedPower].RechargeAction)))
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsCardSummons(Card card) => 
        Turn.Summons;

    public static bool IsCheck() => 
        (IsCombatCheck() || (IsNonCombatCheck() || (IsRechargeCheck() || IsCloseCheck())));

    public static bool IsCheckAutomatic(Card card) => 
        Turn.Character.IsSkillCheckAutomatic(card);

    public static bool IsCheckToDefeat()
    {
        if ((Turn.Checks == null) || (Turn.Check == SkillCheckType.None))
        {
            return true;
        }
        if ((Location.Current.Deck.Count <= 0) || (Turn.Card != Location.Current.Deck[0]))
        {
            return false;
        }
        SkillCheckValueType[] checks = Location.Current.Deck[0].Checks;
        bool flag = checks.Length == Turn.Checks.Length;
        for (int i = 0; (flag && (i < Turn.Checks.Length)) && (i < checks.Length); i++)
        {
            if ((checks[i].skill != Turn.Checks[i].skill) || (checks[i].Rank != Turn.Checks[i].Rank))
            {
                flag = false;
            }
        }
        return flag;
    }

    public static bool IsCloseCheck() => 
        (Turn.State == GameStateType.Close);

    public static bool IsCloseInProgress() => 
        (Turn.CloseType != CloseType.None);

    public static bool IsCloseInsideClosePossible()
    {
        if (Location.Current.Closed)
        {
            return false;
        }
        if (Turn.Map)
        {
            return false;
        }
        if (Turn.CloseType != CloseType.Temporary)
        {
            return false;
        }
        if (!Turn.Iterators.IsRunning(TurnStateIteratorType.Close))
        {
            return false;
        }
        return ((Turn.State == GameStateType.Done) && Turn.Close);
    }

    public static bool IsCombatCardActive() => 
        (Turn.WeaponUnarmed || ((!string.IsNullOrEmpty(Turn.Weapon1) && (Turn.Weapon1 != "Unarmed")) || (!string.IsNullOrEmpty(Turn.Spell) || (!string.IsNullOrEmpty(Turn.Item) && (Turn.Weapon1 != "Unarmed")))));

    public static bool IsCombatCheck() => 
        ((Turn.State == GameStateType.Combat) && (Turn.Check == SkillCheckType.Combat));

    public static bool IsCombatDamage(List<TraitType> damage)
    {
        if (damage != null)
        {
            for (int i = 0; i < damage.Count; i++)
            {
                if (((((TraitType) damage[i]) == TraitType.Melee) || (((TraitType) damage[i]) == TraitType.Ranged)) || (((TraitType) damage[i]) == TraitType.Combat))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsCombatPossible()
    {
        if ((Turn.State == GameStateType.Combat) && ((Turn.Card != null) && (Turn.Checks != null)))
        {
            for (int i = 0; i < Turn.Checks.Length; i++)
            {
                if (Turn.Checks[i].skill == SkillCheckType.Combat)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsCombatSkillParticipating(SkillCheckType skill, bool allChecks)
    {
        if ((skill == SkillCheckType.Combat) && (Turn.Check == SkillCheckType.Combat))
        {
            return true;
        }
        bool flag = Turn.Check == SkillCheckType.Combat;
        if ((allChecks && !flag) && (Turn.Checks != null))
        {
            for (int i = 0; i < Turn.Checks.Length; i++)
            {
                if (Turn.Checks[i].skill == SkillCheckType.Combat)
                {
                    flag = true;
                    break;
                }
            }
        }
        if (flag)
        {
            if (skill == SkillCheckType.Combat)
            {
                return true;
            }
            if ((Turn.Check == SkillCheckType.Combat) && IsSkillCompatible(Turn.Owner, skill, Turn.CombatSkill))
            {
                return true;
            }
            for (int j = 0; j < Turn.CheckParticipants.Count; j++)
            {
                if (IsSkillCompatible(Turn.Owner, skill, Turn.CheckParticipants[j]))
                {
                    return true;
                }
            }
            for (int k = 0; k < Turn.DamageTraits.Count; k++)
            {
                if ((((TraitType) Turn.DamageTraits[k]) == skill.ToTraitType()) && (skill.ToTraitType() != TraitType.None))
                {
                    return true;
                }
            }
            if (Turn.Check == SkillCheckType.Combat)
            {
                if (Turn.CombatSkill == SkillCheckType.None)
                {
                    return false;
                }
                if ((Turn.CheckParticipants.Count == 0) && ((skill == SkillCheckType.Strength) || (skill == SkillCheckType.Melee)))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsDamageCheck() => 
        ((Turn.State == GameStateType.Damage) || (Turn.State == GameStateType.Ambush));

    public static bool IsDamageReductionPossible()
    {
        if ((Turn.State != GameStateType.Damage) && (Turn.State != GameStateType.Ambush))
        {
            return false;
        }
        return Turn.DamageReduction;
    }

    public static bool IsDiceRollPossible()
    {
        if (Turn.Evade && (Turn.State != GameStateType.Recharge))
        {
            return false;
        }
        if (Turn.Defeat && (Turn.State != GameStateType.Recharge))
        {
            return false;
        }
        return true;
    }

    public static bool IsEncounterInCurrentLocation() => 
        (Turn.EncounteredLocation == Location.Current.ID);

    public static bool IsEncounterOver()
    {
        if (Turn.EncounterType == EncounterType.ReEncounter)
        {
            return false;
        }
        if (Turn.EncounterType == EncounterType.LocationDefeat)
        {
            return false;
        }
        if (Turn.EncounterType == EncounterType.LocationEncounter)
        {
            return false;
        }
        if ((!Turn.Evade && !Turn.Defeat) && (Turn.CombatCheckSequence < Turn.NumCheckSequences))
        {
            return false;
        }
        return true;
    }

    public static bool IsEvadePossible(Card card)
    {
        if (card == null)
        {
            return false;
        }
        if (Turn.EvadeDeclined)
        {
            return false;
        }
        if (Turn.Evade)
        {
            return false;
        }
        if (card.GetComponent<CardPropertyCannotEvade>() != null)
        {
            return false;
        }
        return ((Turn.State == GameStateType.Horde) || ((Turn.State == GameStateType.EvadeOption) || (Turn.State == GameStateType.Combat)));
    }

    public static bool IsExplorePossible()
    {
        if (Turn.Map)
        {
            return false;
        }
        return (((Turn.State == GameStateType.Setup) || (Turn.State == GameStateType.Finish)) && (Turn.Explore && (Location.Current.Deck.Count > 0)));
    }

    public static bool IsExplorePromptNecessary() => 
        ((Turn.Explore && ((Location.Current.Deck.Count > 0) || IsMoveLocationPossible())) && !Turn.End);

    public static bool IsGiveCardPossible()
    {
        if (Location.CountCharactersAtLocation(Turn.Owner.Location) <= 1)
        {
            return false;
        }
        if (Turn.BlackBoard.Get<int>("GiveCardCount") > 0)
        {
            return false;
        }
        if (Turn.BlackBoard.Get<int>("MoveLocationCount") > 0)
        {
            return false;
        }
        return true;
    }

    public static bool IsImmune(Card defender, Card attacker)
    {
        CardPropertyImmunity component = defender.GetComponent<CardPropertyImmunity>();
        return ((component != null) && component.IsImmune(attacker));
    }

    public static bool IsImmune(Card defender, TraitType[] traits)
    {
        CardPropertyImmunity component = defender.GetComponent<CardPropertyImmunity>();
        if (component != null)
        {
            for (int i = 0; i < traits.Length; i++)
            {
                if (component.IsImmune(traits[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsImmune(Card defender, TraitType trait)
    {
        CardPropertyImmunity component = defender.GetComponent<CardPropertyImmunity>();
        return ((component != null) && component.IsImmune(trait));
    }

    public static bool IsMapLookPossible()
    {
        if (Turn.Map)
        {
            return false;
        }
        if (((Turn.State != GameStateType.Setup) && (Turn.State != GameStateType.Finish)) && (Turn.State != GameStateType.EndTurn))
        {
            return false;
        }
        return true;
    }

    public static bool IsMoveLocationPossible()
    {
        if (Turn.BlackBoard.Get<int>("MoveLocationCount") > 0)
        {
            return false;
        }
        if (!Turn.Owner.CanMove)
        {
            return false;
        }
        if (Turn.State != GameStateType.Setup)
        {
            return false;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && window.closeLocationPanel.Visible)
        {
            return false;
        }
        return true;
    }

    public static bool IsNonCombatCheck() => 
        ((Turn.State == GameStateType.Recharge) || ((Turn.State == GameStateType.Close) || ((((Turn.State == GameStateType.Roll) && (Turn.Checks != null)) && (Turn.Checks.Length > 0)) || ((Turn.State == GameStateType.Combat) && (Turn.Check != SkillCheckType.Combat)))));

    public static bool IsNonCombatSkillParticipating(SkillCheckType skill, bool allChecks)
    {
        if (skill != SkillCheckType.Combat)
        {
            if (IsSkillCompatible(Turn.Owner, skill, Turn.Check))
            {
                return true;
            }
            if ((Turn.Checks != null) && allChecks)
            {
                for (int j = 0; j < Turn.Checks.Length; j++)
                {
                    if (IsSkillCompatible(Turn.Owner, skill, Turn.Checks[j].skill))
                    {
                        return true;
                    }
                }
            }
            for (int i = 0; (i < Turn.DamageTraits.Count) && (skill.ToTraitType() != TraitType.None); i++)
            {
                if (((TraitType) Turn.DamageTraits[i]) == skill.ToTraitType())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsPassCheckPossible()
    {
        if (!IsTurnOwner())
        {
            return false;
        }
        if (Turn.State != GameStateType.Combat)
        {
            return false;
        }
        if (Turn.Card.NumCheckSequences < 2)
        {
            return false;
        }
        if (Location.CountCharactersAtLocation(Location.Current.ID) < 2)
        {
            return false;
        }
        if (!Turn.Pass)
        {
            return false;
        }
        return true;
    }

    public static bool IsPermanentClosePossible()
    {
        if (Location.Current.Closed)
        {
            return false;
        }
        if (Turn.Map)
        {
            return false;
        }
        if (Turn.CloseType != CloseType.None)
        {
            return false;
        }
        if ((Turn.State == GameStateType.Setup) || (Turn.State == GameStateType.Finish))
        {
            return ((((Location.Current.Deck.Count <= 0) && (Turn.Owner.Location == Location.Current.ID)) && (Turn.Owner.GetEffect(EffectType.HoldCard) == null)) || Turn.Close);
        }
        return ((Turn.State == GameStateType.Done) && Turn.Close);
    }

    public static bool IsQuestRewardAllowed() => 
        (Game.GameMode == GameModeType.Quest);

    public static bool IsRangeValid(Character character, DamageRangeType range)
    {
        if ((character == null) || (Party.Characters.Count <= 0))
        {
            return false;
        }
        switch (range)
        {
            case DamageRangeType.Self:
                if (Turn.Owner.ID == character.ID)
                {
                    break;
                }
                return false;

            case DamageRangeType.Short:
                if ((Turn.Owner.ID != character.ID) && (character.Location == Turn.Owner.Location))
                {
                    break;
                }
                return false;

            case DamageRangeType.Long:
                if ((Turn.Owner.ID != character.ID) && (character.Location != Turn.Owner.Location))
                {
                    break;
                }
                return false;

            case DamageRangeType.Location:
                if (character.Location == Turn.Owner.Location)
                {
                    break;
                }
                return false;
        }
        return true;
    }

    private static bool IsRechargeAutomatic(Character character, Card card)
    {
        for (int i = 0; i < character.Powers.Count; i++)
        {
            CharacterPowerRechargeDiscard component = character.Powers[i].GetComponent<CharacterPowerRechargeDiscard>();
            if ((component != null) && component.Match(card))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsRechargeCheck() => 
        (Turn.State == GameStateType.Recharge);

    public static bool IsRechargeNecessary()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Alive && (Party.Characters[i].Recharge.Count > 0)) && IsRechargePossible(Party.Characters[i], Party.Characters[i].Recharge[0]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsRechargePossible(Character character, Card card)
    {
        if (!IsCardRechargable(character, card))
        {
            return false;
        }
        if (!IsRechargeAutomatic(character, card))
        {
            if (card.Type == CardType.Spell)
            {
                int index = Party.IndexOf(card.PlayedOwner);
                if (index >= 0)
                {
                    for (int i = 0; i < card.Recharge.Length; i++)
                    {
                        if (Party.Characters[index].GetSkillRank(card.Recharge[i].skill) > 0)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            CardPropertyRequiredSkillBanish component = card.GetComponent<CardPropertyRequiredSkillBanish>();
            if (component != null)
            {
                bool flag = false;
                int num3 = Party.IndexOf(card.PlayedOwner);
                if (num3 >= 0)
                {
                    for (int j = 0; j < component.RequiredSkills.Length; j++)
                    {
                        if (Party.Characters[num3].GetSkillRank(component.RequiredSkills[j]) > 0)
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public static bool IsRerollAutomatic(Deck deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardPowerReroll component = deck[i].GetComponent<CardPowerReroll>();
            if ((component != null) && ((Turn.DiceTarget - Turn.DiceTotal) <= component.Difference))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsRerollForced(Card card) => 
        ((card.GetComponent<EventCombatReroll>() != null) && card.GetComponent<EventCombatReroll>().IsEventValid(card));

    public static bool IsRerollPossible(Deck deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if ((!Turn.IsCardRerollEmpty() && (deck[i].GUID == Turn.Reroll)) && (Turn.DiceTotal < Turn.DiceTarget))
            {
                return true;
            }
            CardPowerReroll component = deck[i].GetComponent<CardPowerReroll>();
            if (((component != null) && ((Turn.DiceTarget - Turn.DiceTotal) <= component.Difference)) && (Turn.DiceTarget > Turn.DiceTotal))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsRerollPossible(int diceTotal)
    {
        if (Turn.IsResolveSuccess())
        {
            if (IsRerollForced(Turn.Card))
            {
                return true;
            }
        }
        else
        {
            if (IsRerollPossible(Turn.Owner.Hand))
            {
                return true;
            }
            for (int i = 0; i < Turn.Owner.Powers.Count; i++)
            {
                CharacterPowerReroll reroll = Turn.Owner.Powers[i] as CharacterPowerReroll;
                if ((reroll != null) && reroll.IsRerollPossible())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsRerollPower()
    {
        for (int i = 0; i < Turn.Owner.Powers.Count; i++)
        {
            CharacterPowerReroll reroll = Turn.Owner.Powers[i] as CharacterPowerReroll;
            if ((reroll != null) && reroll.IsValid())
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsSharingPossible()
    {
        if (Game.GameType != GameType.LocalMultiPlayer)
        {
            return false;
        }
        if (!IsTurnOwner())
        {
            return false;
        }
        return ((Turn.State == GameStateType.Setup) || (Turn.State == GameStateType.Finish));
    }

    public static bool IsSkillCompatible(Character character, SkillCheckType child, SkillCheckType parent)
    {
        if (child == parent)
        {
            return true;
        }
        SkillType type = parent.ToSkillType();
        for (int i = 0; i < character.Skills.Length; i++)
        {
            if ((character.Skills[i].skill == type) && (character.Skills[i].attribute.ToSkillCheckType() == child))
            {
                return true;
            }
        }
        for (int j = 0; j < character.GetNumEffects(); j++)
        {
            Effect effect = character.GetEffect(j);
            if (effect.Type == EffectType.SkillChange)
            {
                EffectChangeSkill skill = effect as EffectChangeSkill;
                if ((skill.Skill == type) && (skill.Attribute.ToSkillCheckType() == child))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsSkillParticipatingInCheck(SkillCheckType skill, bool allChecks = false) => 
        ((skill == Turn.Check) || (IsCombatSkillParticipating(skill, allChecks) || IsNonCombatSkillParticipating(skill, allChecks)));

    public static bool IsSkillSelectionPossible()
    {
        if (!IsTurnOwner())
        {
            return false;
        }
        if (Turn.Checks == null)
        {
            return false;
        }
        if (Turn.Card.Side == CardSideType.Back)
        {
            return false;
        }
        return ((Turn.State == GameStateType.Roll) || IsCheck());
    }

    public static bool IsSummonerBanished(Card card)
    {
        if (card == null)
        {
            return false;
        }
        return ((card.Type == CardType.Barrier) && ((Turn.Iterators.Current == null) || !Turn.Iterators.Current.HasPostEvent));
    }

    public static bool IsSummonPossible() => 
        !Turn.Summons;

    public static bool IsTargetRequired(TargetType Range)
    {
        if ((Range == TargetType.AllAtLocation) || (Range == TargetType.MultipleAtLocation))
        {
            return (Location.CountCharactersAtLocation(Turn.Character.Location) > 1);
        }
        if ((Range == TargetType.AnotherAtLocation) || (Range == TargetType.MultipleAnotherAtLocation))
        {
            return (Location.CountCharactersAtLocation(Turn.Character.Location) > 2);
        }
        if ((Range != TargetType.Another) && (Range != TargetType.All))
        {
            return false;
        }
        return (Party.CountLivingMembers() > 1);
    }

    public static bool IsTemporaryClosePossible()
    {
        if (Turn.Card.Type == CardType.Villain)
        {
            if (Turn.Summons)
            {
                return false;
            }
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if ((Party.Characters[i].Alive && (Party.Characters[i].Location != Location.Current.ID)) && Scenario.Current.IsLocationClosePossible(Party.Characters[i].Location, CloseType.Temporary))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsTurnOwner() => 
        (Turn.Number == Turn.Current);

    public static bool IsTurnPassed() => 
        ((Turn.SwitchType == SwitchType.Aid) || (Turn.SwitchType == SwitchType.AidAll));

    public static bool IsUnlimitedPlayPossible(CardType type)
    {
        for (int i = 0; i < Turn.Character.Powers.Count; i++)
        {
            CharacterPowerUnlimited component = Turn.Character.Powers[i].GetComponent<CharacterPowerUnlimited>();
            if (((component != null) && component.IsValid()) && ((component.UnlimitedPlay != null) && component.UnlimitedPlay.Match(type)))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsUnlimitedPlayPossible(Card card, ActionType originalAction)
    {
        for (int i = 0; i < Turn.Character.Powers.Count; i++)
        {
            CharacterPowerUnlimited component = Turn.Character.Powers[i].GetComponent<CharacterPowerUnlimited>();
            if ((((component != null) && component.IsValid()) && ((component.ValidAction == originalAction) && (component.UnlimitedPlay != null))) && component.UnlimitedPlay.Match(card))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsValidationRequired()
    {
        if (Turn.State == GameStateType.Move)
        {
            return false;
        }
        if (Turn.State == GameStateType.Target)
        {
            return false;
        }
        if (Turn.Operation == TurnOperationType.Validation)
        {
            return false;
        }
        return Turn.Validate;
    }

    public static bool IsWeaponOffHand(Card card) => 
        (card.GetComponent<CardPropertyWeaponOffhand>() != null);

    public static bool IsWeaponRanged(Card card) => 
        card.HasTrait(TraitType.Ranged);

    public static bool IsWeaponTwoHanded(Card card) => 
        card.HasTrait(TraitType.TwoHanded);

    public static void Reset()
    {
        EnforceLegalDice();
    }

    public static int RollDice(DiceType dice)
    {
        if (Settings.Debug.PeonMode)
        {
            return 1;
        }
        if (Settings.Debug.GodMode)
        {
            if (dice == DiceType.D2)
            {
                return 2;
            }
            if (dice == DiceType.D4)
            {
                return 4;
            }
            if (dice == DiceType.D6)
            {
                return 6;
            }
            if (dice == DiceType.D8)
            {
                return 8;
            }
            if (dice == DiceType.D10)
            {
                return 10;
            }
            if (dice == DiceType.D12)
            {
                return 12;
            }
        }
        int num = CrookedDiceValues[(int) dice];
        if (num != 0)
        {
            return num;
        }
        if (dice == DiceType.D2)
        {
            return UnityEngine.Random.Range(1, 3);
        }
        if (dice == DiceType.D4)
        {
            return UnityEngine.Random.Range(1, 5);
        }
        if (dice == DiceType.D6)
        {
            return UnityEngine.Random.Range(1, 7);
        }
        if (dice == DiceType.D8)
        {
            return UnityEngine.Random.Range(1, 9);
        }
        if (dice == DiceType.D10)
        {
            return UnityEngine.Random.Range(1, 11);
        }
        if (dice == DiceType.D12)
        {
            return UnityEngine.Random.Range(1, 13);
        }
        return 0;
    }

    public static int RollDice(DiceType[] dice)
    {
        int num = 0;
        for (int i = 0; i < dice.Length; i++)
        {
            num += RollDice(dice[i]);
        }
        return num;
    }

    public static bool WasRerollForced() => 
        Turn.BlackBoard.Get<bool>("CombatReroll");
}

