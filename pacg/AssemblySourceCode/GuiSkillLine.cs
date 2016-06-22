using System;
using System.Text;
using UnityEngine;

public class GuiSkillLine : GuiElement
{
    public UnityEngine.Animator Animator;
    public GuiLabel DiceLabel;
    private AttributeType myAttribute;
    public GuiLabel SkillLabel;
    public GuiLabel SubskillLabel;
    public GuiButton UpgradeButton;

    private string GetAttributeText(AttributeType attribute)
    {
        if (attribute == AttributeType.Strength)
        {
            return "STR";
        }
        if (attribute == AttributeType.Dexterity)
        {
            return "DEX";
        }
        if (attribute == AttributeType.Constitution)
        {
            return "CON";
        }
        if (attribute == AttributeType.Intelligence)
        {
            return "INT";
        }
        if (attribute == AttributeType.Wisdom)
        {
            return "WIS";
        }
        if (attribute == AttributeType.Charisma)
        {
            return "CHA";
        }
        return null;
    }

    private bool HasSubskills(Character character, AttributeType attribute)
    {
        for (int i = 0; i < character.Skills.Length; i++)
        {
            if (character.Skills[i].attribute == attribute)
            {
                return true;
            }
        }
        return false;
    }

    public void Select(int n)
    {
        this.UpgradeButton.Glow(true);
        this.Animator.SetInteger("SkillNum", n);
        this.Animator.SetTrigger("AddSkillPoint");
    }

    public void SetRank(int current, int max)
    {
        string stateName = string.Empty;
        if (max >= 4)
        {
            stateName = "skillbar_idle4";
        }
        if (max == 3)
        {
            stateName = "skillbar_idle3";
        }
        if (max == 2)
        {
            stateName = "skillbar_idle2";
        }
        if (max == 1)
        {
            stateName = "skillbar_idle1";
        }
        if (max <= 0)
        {
            stateName = "skillbar_idle0";
        }
        if (current == 1)
        {
            stateName = stateName + "_1";
        }
        if (current == 2)
        {
            stateName = stateName + "_2";
        }
        if (current == 3)
        {
            stateName = stateName + "_3";
        }
        if (current == 4)
        {
            stateName = stateName + "_4";
        }
        this.Animator.Play(stateName);
    }

    public void ShowSkills(Character character, AttributeType attribute)
    {
        if (this.HasSubskills(character, attribute))
        {
            string attributeText = this.GetAttributeText(attribute);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < character.Skills.Length; i++)
            {
                if (character.Skills[i].attribute == attribute)
                {
                    SkillType skill = character.Skills[i].skill;
                    if (builder.Length != 0)
                    {
                        builder.Append("     ");
                    }
                    builder.Append(string.Concat(new object[] { skill.ToText(), ": ", attributeText, " +", character.GetSkillRank(skill) }));
                }
            }
            this.SubskillLabel.Text = builder.ToString().ToUpper();
            this.SubskillLabel.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            this.SubskillLabel.transform.parent.gameObject.SetActive(false);
        }
    }

    public void Unselect(int n)
    {
        this.UpgradeButton.Glow(false);
        this.Animator.SetInteger("SkillNum", n);
        this.Animator.SetTrigger("Reverse");
        UI.Sound.Play(SoundEffectType.LeveldownSlider);
    }

    public AttributeType Attribute
    {
        get => 
            this.myAttribute;
        set
        {
            this.SkillLabel.Text = value.ToText().ToUpper();
            this.myAttribute = value;
        }
    }

    public DiceType Die
    {
        set
        {
            this.DiceLabel.Text = value.ToText().ToLower();
        }
    }
}

