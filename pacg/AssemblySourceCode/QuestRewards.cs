using System;
using UnityEngine;

public class QuestRewards : MonoBehaviour
{
    [Tooltip("base xp awarded for defeating cards")]
    public RewardControlCardBaseValueType[] CardExperience;
    [Tooltip(".. x card rank")]
    public RewardControlCardScaleValueType[] CardRankMultipliers;
    [Tooltip(".. x scenario difficulty")]
    public float[] DifficultyMultipliers;
    [Tooltip("amount of XP required for each level (there is no math formula for this)"), Header("Levelup")]
    public int[] ExperienceLevels;
    [Tooltip("number of levels in a tier")]
    public int LevelsPerTier = 5;
    [Tooltip("reward prefab given at each level")]
    public string[] Rewards;
    [Tooltip("base xp awarded for winning a quest"), Header("Experience")]
    public int ScenarioExperience;
    [Tooltip(".. x scenario tier (deck #)")]
    public float[] TierMultipliers;

    private int GetCardExperience(Card card)
    {
        int xp = 0;
        for (int i = 0; i < this.CardExperience.Length; i++)
        {
            if (this.CardExperience[i].type == card.Type)
            {
                xp = this.CardExperience[i].xp;
            }
        }
        if (xp <= 0)
        {
            return 0;
        }
        for (int j = 0; j < this.CardRankMultipliers.Length; j++)
        {
            if ((this.CardRankMultipliers[j].trait == TraitType.None) && (this.CardRankMultipliers[j].type == CardType.None))
            {
                xp = Mathf.CeilToInt(xp * this.CardRankMultipliers[j].multiplier);
                break;
            }
            if ((this.CardRankMultipliers[j].trait != TraitType.None) && card.HasTrait(this.CardRankMultipliers[j].trait))
            {
                xp = Mathf.CeilToInt(xp * this.CardRankMultipliers[j].multiplier);
                break;
            }
            if ((this.CardRankMultipliers[j].type != CardType.None) && (card.Type == this.CardRankMultipliers[j].type))
            {
                xp = Mathf.CeilToInt(xp * this.CardRankMultipliers[j].multiplier);
                break;
            }
        }
        int index = Mathf.Clamp(Party.Tier, 0, this.TierMultipliers.Length);
        return Mathf.CeilToInt(xp * this.TierMultipliers[index]);
    }

    public int GetExperiencePointsForLevel(int level)
    {
        if ((level > 0) && (level < this.ExperienceLevels.Length))
        {
            return this.ExperienceLevels[level - 1];
        }
        return 0;
    }

    public int GetLevelFromExperiencePoints(int xp)
    {
        for (int i = 1; i < this.ExperienceLevels.Length; i++)
        {
            if (xp < this.ExperienceLevels[i])
            {
                return i;
            }
        }
        return this.LevelCap;
    }

    private int GetScenarioExperience(Scenario scenario)
    {
        int scenarioExperience = this.ScenarioExperience;
        int index = Mathf.Clamp(Party.Tier, 0, this.TierMultipliers.Length);
        scenarioExperience = Mathf.CeilToInt(scenarioExperience * this.TierMultipliers[index]);
        int num3 = Mathf.Clamp(scenario.Difficulty, 0, this.DifficultyMultipliers.Length);
        return Mathf.CeilToInt(scenarioExperience * this.DifficultyMultipliers[num3]);
    }

    public int GetTierFromExperiencePoints(int xp) => 
        Mathf.FloorToInt(((float) this.GetLevelFromExperiencePoints(xp)) / ((float) this.LevelsPerTier));

    public void OnCardDefeated(Card card)
    {
        if (Rules.IsQuestRewardAllowed())
        {
            int cardExperience = this.GetCardExperience(card);
            if (cardExperience > 0)
            {
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    window.statusPanel.ShowExperience(cardExperience);
                }
                Party.RewardExperience(cardExperience);
            }
        }
    }

    public void OnScenarioCompleted(Scenario scenario)
    {
        if (Rules.IsQuestRewardAllowed())
        {
            int scenarioExperience = this.GetScenarioExperience(scenario);
            if (scenarioExperience > 0)
            {
                Party.RewardExperience(scenarioExperience);
            }
        }
    }

    public int LevelCap =>
        (this.ExperienceLevels.Length - 1);
}

