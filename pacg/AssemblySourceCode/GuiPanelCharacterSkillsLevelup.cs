using System;
using UnityEngine;

public class GuiPanelCharacterSkillsLevelup : GuiPanelCharacterSkills
{
    [Tooltip("line top position")]
    public float PanelTop = 2f;
    private int points;

    private GuiSkillLine GetSelectedLine()
    {
        RewardFeat reward = (UI.Window as GuiWindowReward).Reward as RewardFeat;
        if ((reward != null) && reward.IsSelected(Turn.Number))
        {
            switch (reward.GetSelectedSkill())
            {
                case AttributeType.Strength:
                    return base.StrengthLine;

                case AttributeType.Dexterity:
                    return base.DexterityLine;

                case AttributeType.Constitution:
                    return base.ConstitutionLine;

                case AttributeType.Intelligence:
                    return base.IntelligenceLine;

                case AttributeType.Wisdom:
                    return base.WisdomLine;

                case AttributeType.Charisma:
                    return base.CharismaLine;
            }
        }
        return null;
    }

    public override void Initialize()
    {
        base.LineTopPosition = this.PanelTop;
    }

    private bool IsLevelupPossible(AttributeType attribute)
    {
        if (this.points <= 0)
        {
            return false;
        }
        int attributeRank = Turn.Character.GetAttributeRank(attribute);
        int attributeMaxRank = Turn.Character.GetAttributeMaxRank(attribute);
        return (attributeRank < attributeMaxRank);
    }

    private void Levelup(GuiSkillLine skillLine, AttributeType attribute)
    {
        GuiSkillLine selectedLine = this.GetSelectedLine();
        if (selectedLine != skillLine)
        {
            if (selectedLine != null)
            {
                selectedLine.Unselect(Turn.Character.GetAttributeRank(attribute));
            }
            skillLine.Select(Turn.Character.GetAttributeRank(attribute) + 1);
            GuiWindowReward window = UI.Window as GuiWindowReward;
            if (window != null)
            {
                window.OnRewardChosen(attribute);
            }
        }
    }

    private void OnLevelupCharismaButtonPushed()
    {
        this.Levelup(base.CharismaLine, AttributeType.Charisma);
    }

    private void OnLevelupConstitutionButtonPushed()
    {
        this.Levelup(base.ConstitutionLine, AttributeType.Constitution);
    }

    private void OnLevelupDexterityButtonPushed()
    {
        this.Levelup(base.DexterityLine, AttributeType.Dexterity);
    }

    private void OnLevelupIntelligenceButtonPushed()
    {
        this.Levelup(base.IntelligenceLine, AttributeType.Intelligence);
    }

    private void OnLevelupStrengthButtonPushed()
    {
        this.Levelup(base.StrengthLine, AttributeType.Strength);
    }

    private void OnLevelupWisdomButtonPushed()
    {
        this.Levelup(base.WisdomLine, AttributeType.Wisdom);
    }

    public override void Refresh()
    {
        base.Refresh();
        GuiSkillLine selectedLine = this.GetSelectedLine();
        if (selectedLine != null)
        {
            selectedLine.Select(base.Character.GetAttributeRank(selectedLine.Attribute) + 1);
        }
    }

    protected override float RefreshLine(float line, AttributeType attribute, GuiSkillLine skillLine)
    {
        skillLine.Attribute = attribute;
        skillLine.Die = base.Character.GetAttributeDice(attribute);
        skillLine.SetRank(base.Character.GetAttributeRank(attribute), base.Character.GetAttributeMaxRank(attribute));
        skillLine.ShowSkills(base.Character, attribute);
        skillLine.Show(true);
        skillLine.UpgradeButton.Glow(false);
        skillLine.UpgradeButton.Show(this.IsLevelupPossible(attribute));
        base.SetLinePosition(line, skillLine, 0f);
        line += base.GetNumTextLines(attribute);
        skillLine.UpgradeButton.Refresh();
        return line;
    }

    public void SetPoints(int n)
    {
        this.points = n;
    }
}

