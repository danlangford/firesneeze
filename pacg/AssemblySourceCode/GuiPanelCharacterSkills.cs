using System;
using UnityEngine;

public class GuiPanelCharacterSkills : GuiPanel
{
    [Tooltip("character description")]
    public GuiLabel CharacterDescription;
    public GuiSkillLine CharismaLine;
    public GuiSkillLine ConstitutionLine;
    protected Character currentCharacter;
    public GuiSkillLine DexterityLine;
    public GuiSkillLine IntelligenceLine;
    protected float LineTopPosition;
    protected const int NumColumns = 4;
    [Tooltip("height of each skill line in world space")]
    public float PrimarySkillLineHeight = 0.9f;
    public GuiSkillLine StrengthLine;
    [Tooltip("height of each subskill line in world space")]
    public float SubSkillLineHeight = 0.45f;
    [Tooltip("reference to the tab button that brought us here")]
    public GuiButton TabButton;
    public GuiSkillLine WisdomLine;

    protected float GetNumTextLines(AttributeType attribute)
    {
        int num = 1;
        for (int i = 0; i < this.Character.Skills.Length; i++)
        {
            if (this.Character.Skills[i].attribute == attribute)
            {
                num++;
            }
        }
        if (num == 1)
        {
            return this.PrimarySkillLineHeight;
        }
        return (this.PrimarySkillLineHeight + this.SubSkillLineHeight);
    }

    public override void Initialize()
    {
        this.LineTopPosition = this.StrengthLine.transform.position.y;
    }

    public override void Refresh()
    {
        if (this.Character != null)
        {
            this.CharacterDescription.Text = this.Character.DisplayText;
            this.RefreshAllLines();
        }
    }

    protected void RefreshAllLines()
    {
        float line = 0f;
        line = this.RefreshLine(line, AttributeType.Strength, this.StrengthLine);
        line = this.RefreshLine(line, AttributeType.Dexterity, this.DexterityLine);
        line = this.RefreshLine(line, AttributeType.Constitution, this.ConstitutionLine);
        line = this.RefreshLine(line, AttributeType.Intelligence, this.IntelligenceLine);
        line = this.RefreshLine(line, AttributeType.Wisdom, this.WisdomLine);
        line = this.RefreshLine(line, AttributeType.Charisma, this.CharismaLine);
    }

    protected virtual float RefreshLine(float line, AttributeType attribute, GuiSkillLine skillLine)
    {
        skillLine.Show(true);
        skillLine.Attribute = attribute;
        skillLine.Die = this.Character.GetAttributeDice(attribute);
        skillLine.SetRank(this.Character.GetAttributeRank(attribute), this.Character.GetAttributeMaxRank(attribute));
        skillLine.ShowSkills(this.Character, attribute);
        skillLine.UpgradeButton.Show(false);
        this.SetLinePosition(line, skillLine, 0f);
        line += this.GetNumTextLines(attribute);
        return line;
    }

    protected void SetLinePosition(float line, GuiSkillLine widget, float offset)
    {
        widget.transform.position = new Vector3(widget.transform.position.x, (this.LineTopPosition - line) + offset, widget.transform.position.z);
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.TabButton.Glow(isVisible);
        if (isVisible)
        {
            this.Refresh();
        }
    }

    public Character Character
    {
        get
        {
            if (this.currentCharacter != null)
            {
                return this.currentCharacter;
            }
            if (Party.Characters.Count > Turn.Number)
            {
                return Party.Characters[Turn.Number];
            }
            return null;
        }
        set
        {
            this.currentCharacter = value;
        }
    }

    public override uint zIndex =>
        (Constants.ZINDEX_PANEL_FULL + 20);
}

