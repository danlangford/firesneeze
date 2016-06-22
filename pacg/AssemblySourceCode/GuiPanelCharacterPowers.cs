using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelCharacterPowers : GuiPanel
{
    [Tooltip("pointer to check box (checked) image")]
    public Sprite CheckBoxNoIcon;
    [Tooltip("pointer to check box (unchecked) image")]
    public Sprite CheckBoxYesIcon;
    protected Character currentCharacter;
    [Tooltip("array of 3 reusable icons")]
    public CharacterPowerIcon[] HandIcons;
    [Tooltip("checkboxes in hand size row")]
    public GuiImage[] HandImages;
    [Tooltip("pointer to proficiency icons (4-7)")]
    public Sprite[] HandSizeArt;
    [Tooltip("it says \"HAND SIZE:\"")]
    public GuiLabel HandSizeLabel;
    [Tooltip("world space height of an image icon")]
    public float IconHeight = 2f;
    [Tooltip("amount of world space between checkbox and icon (center to center)")]
    public float IconPadding = 0.75f;
    [Tooltip("world space between between image icons (center to center)")]
    public float IconWidth = 2f;
    [Tooltip("it says \"INFORMATION\"")]
    public GuiLabel InfoLabel;
    protected float LeftMargin;
    [Tooltip("max number of icons per row in the table")]
    public int MaxIconsPerRow = 5;
    protected const int NumColumns = 4;
    [Tooltip("displays the active power description")]
    public GuiLabel PowerDescriptionLabel;
    [Tooltip("array of 6 reusable labels for \"active\" or \"innate\"")]
    public GuiLabel[] PowerFamilyInfo;
    [Tooltip("array of 6 reusable labels for power family titles")]
    public GuiLabel[] PowerFamilyTitles;
    [Tooltip("background hilite for active power")]
    public GuiImage PowerHilite;
    [Tooltip("array of 16 reusable icons")]
    public CharacterPowerIcon[] PowerIcons;
    [Tooltip("array of 16 reusable checkboxes")]
    public GuiImage[] PowerImages;
    [Tooltip("displays the active power name")]
    public GuiLabel PowerNameLabel;
    [Tooltip("reference to the scrolling region on this panel")]
    public GuiScrollRegion powerScroller;
    [Tooltip("checkboxes in proficiency row")]
    public GuiImage[] ProficencyImages;
    [Tooltip("it says \"PROFICIENT WITH:\"")]
    public GuiLabel ProficencyLabel;
    [Tooltip("pointers to proficiency icons (LA, HA, WP)")]
    public Sprite[] ProficiencyArt;
    [Tooltip("icons for LA, HA, WP")]
    public CharacterPowerIcon[] ProficiencyIcons;
    [Tooltip("reference to our role subpanel/menu (can be null on the rewards screen)")]
    public GuiPanelRoles RolePanel;
    [Tooltip("reference to the tab button that brought us here")]
    public GuiButton TabButton;
    protected TKTapRecognizer tapRecognizer;
    [Tooltip("amount of world space for title bar")]
    public float TitleHeight = 0.5f;
    [Tooltip("amount of world space between icon row and title (on bottom)")]
    public float TitlePaddingBottom;
    [Tooltip("amount of world space between icon row and title (on top)")]
    public float TitlePaddingTop;
    public float TitleTop = 1.75f;

    public void Focus(string id)
    {
        for (int i = 0; i < this.PowerIcons.Length; i++)
        {
            if (this.PowerIcons[i].ID == id)
            {
                this.SelectIcon(this.PowerIcons[i]);
                break;
            }
        }
    }

    protected string GetPowerFamilyInfo(string id)
    {
        PowerTableEntry entry = PowerTable.Get(id);
        if (entry == null)
        {
            return string.Empty;
        }
        if (entry.Active)
        {
            return "Active";
        }
        return "Innate";
    }

    protected string GetPowerFamilyTitle(string id)
    {
        PowerTableEntry entry = PowerTable.Get(id);
        if (entry != null)
        {
            return entry.Name;
        }
        return null;
    }

    protected Sprite GetPowerImage(string id)
    {
        SpriteRenderer renderer = Game.Cache.Get<SpriteRenderer>(id);
        if (renderer != null)
        {
            return renderer.sprite;
        }
        return null;
    }

    protected int GetProficencyIndex(ProficencyType type)
    {
        if (type == ProficencyType.LightArmor)
        {
            return 0;
        }
        if (type == ProficencyType.HeavyArmor)
        {
            return 1;
        }
        return 2;
    }

    public override void Initialize()
    {
        this.powerScroller.Owner = this;
        this.powerScroller.Initialize();
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = this.zIndex - 1;
        this.tapRecognizer.boundaryFrame = new TKRect?(this.powerScroller.GetScrollBounds());
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!base.Paused)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        if ((this.PowerIcons.Length > 0) && (this.PowerIcons[0] != null))
        {
            this.LeftMargin = this.PowerIcons[0].transform.localPosition.x;
        }
    }

    protected virtual void OnGuiTap(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_ICON);
        if (hitd != 0)
        {
            CharacterPowerIcon component = hitd.collider.transform.GetComponent<CharacterPowerIcon>();
            this.SelectIcon(component);
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        if (this.tapRecognizer != null)
        {
            this.tapRecognizer.enabled = !isPaused;
        }
        this.powerScroller.Pause(isPaused);
        this.RolePanel.Pause(isPaused);
    }

    public override void Refresh()
    {
        this.Role = RoleTable.Get(this.Character.Roles[this.Character.Role]);
        GuiWindowReward window = UI.Window as GuiWindowReward;
        if (window != null)
        {
            RewardFeat reward = window.Reward as RewardFeat;
            if ((reward != null) && (reward.GetSelectedRole(Turn.Number) != null))
            {
                this.Role = reward.GetSelectedRole(Turn.Number);
            }
        }
        this.RefreshContents();
    }

    public virtual void Refresh(int n)
    {
        if ((n >= 0) && (n < this.Character.Roles.Length))
        {
            this.Role = RoleTable.Get(this.Character.Roles[n]);
            this.RefreshContents();
        }
    }

    protected void RefreshContents()
    {
        this.RefreshHandSize();
        this.RefreshProficiencies();
        this.RefreshPowers();
        this.powerScroller.Top();
        if (this.tapRecognizer != null)
        {
            this.tapRecognizer.boundaryFrame = new TKRect?(this.powerScroller.GetScrollBounds());
        }
    }

    protected virtual void RefreshHandSize()
    {
        for (int i = 0; i < this.HandIcons.Length; i++)
        {
            this.HandIcons[i].Show(false);
        }
        for (int j = 0; j < this.HandImages.Length; j++)
        {
            this.HandImages[j].Show(false);
        }
        float y = this.TitleTop - (this.TitleHeight / 2f);
        this.HandSizeLabel.Show(true);
        this.HandSizeLabel.transform.localPosition = new Vector3(this.HandSizeLabel.transform.localPosition.x, y, 0f);
        y = (y - this.TitleHeight) - (this.IconHeight / 2f);
        for (int k = 0; k < this.HandImages.Length; k++)
        {
            Vector3 vector = new Vector3(this.LeftMargin + (this.IconWidth * k), y, 0f);
            int num5 = this.Character.HandSize + k;
            bool isVisible = num5 <= this.Role.HandSize;
            this.HandImages[k].Show(isVisible);
            this.HandImages[k].Image = (this.Character.HandSize != num5) ? this.CheckBoxNoIcon : this.CheckBoxYesIcon;
            this.HandImages[k].transform.localPosition = vector - new Vector3(this.IconPadding, 0f, 0f);
            if ((num5 - 4) < this.HandSizeArt.Length)
            {
                this.HandIcons[k].Image = this.HandSizeArt[num5 - 4];
            }
            this.HandIcons[k].Show(isVisible);
            this.HandIcons[k].transform.localPosition = vector;
        }
    }

    protected virtual void RefreshPowers()
    {
        for (int i = 0; i < this.PowerIcons.Length; i++)
        {
            this.PowerIcons[i].Show(false);
        }
        for (int j = 0; j < this.PowerImages.Length; j++)
        {
            this.PowerImages[j].Show(false);
        }
        for (int k = 0; k < this.PowerFamilyTitles.Length; k++)
        {
            this.PowerFamilyTitles[k].Show(false);
        }
        for (int m = 0; m < this.PowerFamilyInfo.Length; m++)
        {
            this.PowerFamilyInfo[m].Show(false);
        }
        this.PowerNameLabel.Clear();
        this.PowerDescriptionLabel.Clear();
        this.PowerHilite.Show(false);
        List<string> list = new List<string>();
        for (int n = 0; n < this.Role.Powers.Length; n++)
        {
            PowerTableEntry entry = PowerTable.Get(this.Role.Powers[n]);
            if ((entry != null) && !list.Contains(entry.Family))
            {
                list.Add(entry.Family);
            }
        }
        float y = (this.TitleTop - (2f * this.IconHeight)) - (1.25f * this.TitleHeight);
        int index = 0;
        for (int num8 = 0; num8 < list.Count; num8++)
        {
            string id = list[num8];
            y -= this.TitleHeight;
            if (num8 < this.PowerFamilyTitles.Length)
            {
                this.PowerFamilyTitles[num8].Text = this.GetPowerFamilyTitle(id);
                this.PowerFamilyTitles[num8].Show(true);
                this.PowerFamilyTitles[num8].transform.localPosition = new Vector3(this.PowerFamilyTitles[num8].transform.localPosition.x, y, 0f);
            }
            if (num8 < this.PowerFamilyInfo.Length)
            {
                this.PowerFamilyInfo[num8].Text = this.GetPowerFamilyInfo(id);
                this.PowerFamilyInfo[num8].Show(true);
                this.PowerFamilyInfo[num8].transform.localPosition = new Vector3(this.PowerFamilyInfo[num8].transform.localPosition.x, y, 0f);
            }
            y -= this.TitlePaddingTop;
            y -= this.IconHeight;
            int num9 = 0;
            int num10 = 0;
            for (int num11 = 0; num11 < this.Role.Powers.Length; num11++)
            {
                string iD = this.Role.Powers[num11];
                PowerTableEntry entry2 = PowerTable.Get(iD);
                if ((entry2 != null) && (entry2.Family == id))
                {
                    if (++num9 > this.MaxIconsPerRow)
                    {
                        num9 = 0;
                        num10++;
                        y -= this.IconHeight;
                    }
                    RoleTableEntry entry3 = RoleTable.Get(this.Character.Roles[this.Character.Role]);
                    bool flag = false;
                    if (entry3 != this.Role)
                    {
                        entry3 = RoleTable.Get(this.Character.Roles[0]);
                        for (int num12 = 0; num12 < entry3.Powers.Length; num12++)
                        {
                            if (entry3.Powers[num12] == iD)
                            {
                                flag = true;
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    float x = (this.LeftMargin + (this.IconWidth * entry2.Index)) - ((num10 * this.IconWidth) * this.MaxIconsPerRow);
                    Vector3 vector = new Vector3(x, y, 0f);
                    if (index < this.PowerImages.Length)
                    {
                        this.PowerImages[index].Image = (!this.Character.HasPower(iD) || !flag) ? this.CheckBoxNoIcon : this.CheckBoxYesIcon;
                        this.PowerImages[index].transform.localPosition = vector - new Vector3(this.IconPadding, 0f, 0f);
                        this.PowerImages[index].Show(true);
                    }
                    if (index < this.PowerIcons.Length)
                    {
                        this.PowerIcons[index].ID = iD;
                        this.PowerIcons[index].Image = this.GetPowerImage(entry2.GetIconPath(entry2.Icon));
                        this.PowerIcons[index].transform.localPosition = vector;
                        this.PowerIcons[index].Show(true);
                    }
                    index++;
                }
            }
            y -= this.IconHeight / 2f;
            y -= this.TitlePaddingBottom;
        }
        float num14 = this.powerScroller.Min.y + (list.Count * ((((1.5f * this.IconHeight) + this.TitleHeight) + this.TitlePaddingTop) + this.TitlePaddingBottom));
        this.powerScroller.Max = new Vector2(0f, num14);
    }

    protected virtual void RefreshProficiencies()
    {
        for (int i = 0; i < 3; i++)
        {
            this.ProficiencyIcons[i].Show(false);
        }
        for (int j = 0; j < 3; j++)
        {
            this.ProficencyImages[j].Show(false);
        }
        float y = (this.TitleTop - this.TitleHeight) - this.IconHeight;
        this.ProficencyLabel.Show(true);
        this.ProficencyLabel.transform.localPosition = new Vector3(this.ProficencyLabel.transform.localPosition.x, y, 0f);
        y = (y - this.TitleHeight) - (this.IconHeight / 2f);
        for (int k = 0; k < this.Role.Proficiencies.Length; k++)
        {
            Vector3 vector = new Vector3(this.LeftMargin + (this.IconWidth * k), y, 0f);
            bool flag = false;
            ProficencyType type = this.Role.Proficiencies[k];
            if ((type == ProficencyType.Weapons) && this.Character.ProficientWithWeapons)
            {
                flag = true;
            }
            if ((type == ProficencyType.LightArmor) && this.Character.ProficientWithLightArmors)
            {
                flag = true;
            }
            if ((type == ProficencyType.HeavyArmor) && this.Character.ProficientWithHeavyArmors)
            {
                flag = true;
            }
            int proficencyIndex = this.GetProficencyIndex(type);
            this.ProficencyImages[proficencyIndex].Image = !flag ? this.CheckBoxNoIcon : this.CheckBoxYesIcon;
            this.ProficencyImages[proficencyIndex].Show(true);
            this.ProficencyImages[proficencyIndex].transform.localPosition = vector - new Vector3(this.IconPadding, 0f, 0f);
            this.ProficiencyIcons[proficencyIndex].Image = this.ProficiencyArt[proficencyIndex];
            this.ProficiencyIcons[proficencyIndex].Show(true);
            this.ProficiencyIcons[proficencyIndex].transform.localPosition = vector;
        }
    }

    private void SelectIcon(CharacterPowerIcon icon)
    {
        if (icon != null)
        {
            icon.Tap();
            this.PowerHilite.transform.position = icon.transform.position;
            this.PowerHilite.Show(true);
            this.PowerNameLabel.Text = icon.Name;
            this.PowerDescriptionLabel.Text = icon.Description;
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.TabButton.Glow(isVisible);
        if (this.tapRecognizer != null)
        {
            this.tapRecognizer.enabled = isVisible;
        }
        this.powerScroller.Pause(!isVisible);
        if (this.RolePanel != null)
        {
            this.RolePanel.ShowCurrentRole();
            this.RolePanel.Refresh();
        }
        if (!isVisible)
        {
            this.PowerNameLabel.Clear();
            this.PowerDescriptionLabel.Clear();
            this.PowerHilite.Show(false);
            this.powerScroller.Top();
            if (this.RolePanel != null)
            {
                this.RolePanel.Show(false);
            }
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
            if (this.RolePanel != null)
            {
                this.RolePanel.Show(false);
            }
        }
    }

    protected RoleTableEntry Role { get; set; }

    public override uint zIndex =>
        (Constants.ZINDEX_PANEL_FULL + 10);
}

