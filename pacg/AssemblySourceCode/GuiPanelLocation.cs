using System;
using System.Collections.Generic;
using UnityEngine;

public class GuiPanelLocation : GuiPanel
{
    [Tooltip("reference to the blocker graphic for all rows")]
    public GameObject BlockerAll;
    [Tooltip("reference to the blocker graphic for bottom 2 rows")]
    public GameObject BlockerBottom;
    [Tooltip("reference to the blocker graphic for top 2 rows")]
    public GameObject BlockerTop;
    [Tooltip("reference to the location power frame in hierarchy")]
    public GameObject ButtonFrameAtThisLocation;
    [Tooltip("reference to the location power frame in hierarchy")]
    public GameObject ButtonFrameWhenPermClosed;
    [Tooltip("reference to the glow during close prompt if it's mathmatically theoretically impossible to close current location")]
    public GameObject CloseImpossible;
    [Tooltip("reference to the glow during close prompt if it's mathmatically theoretically possible to close current location")]
    public GameObject ClosePossible;
    [Tooltip("label for card count of allies")]
    public GuiLabel CountAllyCardsLabel;
    [Tooltip("label for card count of armours")]
    public GuiLabel CountArmorCardsLabel;
    [Tooltip("label for card count of barriers")]
    public GuiLabel CountBarrierCardsLabel;
    [Tooltip("label for card count of blessings")]
    public GuiLabel CountBlessingCardsLabel;
    [Tooltip("label for card count of items")]
    public GuiLabel CountItemCardsLabel;
    [Tooltip("label for card count of monsters")]
    public GuiLabel CountMonsterCardsLabel;
    [Tooltip("label for card count of spells")]
    public GuiLabel CountSpellCardsLabel;
    [Tooltip("label for card count of unknown cards")]
    public GuiLabel CountUnknownCardsLabel;
    [Tooltip("label for card count of weapons")]
    public GuiLabel CountWeaponCardsLabel;
    [Tooltip("reference to the \"text glow\" in hierarchy")]
    public GameObject GlowTextVfx;
    [Tooltip("heading foreground color")]
    public Color HeadingColor;
    [Tooltip("label for consolidated location description")]
    public GuiLabel LocationText;
    [Tooltip("label for \"at this location\" text (can be null)")]
    public GuiLabel LocationTextAt;
    [Tooltip("label for \"when closing\" text (can be null)")]
    public GuiLabel LocationTextWhenClosing;
    [Tooltip("label for \"when perm closed\" text (can be null)")]
    public GuiLabel LocationTextWhenPermClosed;
    [Tooltip("reference to the map in this scene")]
    public GuiPanelMap mapPanel;
    [Tooltip("reference to the location cycle buttons in hierarchy")]
    public GuiButton NextButton;
    [Tooltip("reference to the location cycle buttons in hierarchy")]
    public GuiButton PreviousButton;
    private string selectedID;
    [Tooltip("label for location title text")]
    public GuiLabel TitleLabel;

    private string GetHeadingColor() => 
        $"#{((int) (this.HeadingColor.r * 255f)):X2}{((int) (this.HeadingColor.g * 255f)):X2}{((int) (this.HeadingColor.b * 255f)):X2}{((int) (this.HeadingColor.a * 255f)):X2}";

    private int GetScenarioLocationIndex(string ID)
    {
        for (int i = 0; i < Scenario.Current.Locations.Length; i++)
        {
            if (Scenario.Current.Locations[i].LocationName == ID)
            {
                return i;
            }
        }
        return -1;
    }

    public void GlowLocationClosePossible(bool possible, bool glow)
    {
        if (glow)
        {
            if (possible)
            {
                this.ClosePossible.SetActive(true);
            }
            else
            {
                this.CloseImpossible.SetActive(true);
            }
        }
        else
        {
            this.ClosePossible.SetActive(false);
            this.CloseImpossible.SetActive(false);
        }
    }

    public void GlowText(TextHilightType hilightType)
    {
        if (hilightType == TextHilightType.None)
        {
            this.ShowGlow(this.GlowTextVfx, false);
        }
        if (hilightType == TextHilightType.DuringThisScenario)
        {
            this.ShowGlow(this.GlowTextVfx, false);
        }
        if (hilightType == TextHilightType.AtThisLocation)
        {
            this.SetGlowPosition(this.GlowTextVfx, 1.3f, 2f);
            this.ShowGlow(this.GlowTextVfx, true);
        }
        if (hilightType == TextHilightType.WhenClosing)
        {
            this.SetGlowPosition(this.GlowTextVfx, 0.25f, 2f);
            this.ShowGlow(this.GlowTextVfx, true);
        }
        if (hilightType == TextHilightType.WhenPermanentlyClosed)
        {
            this.SetGlowPosition(this.GlowTextVfx, -0.7f, 2f);
            this.ShowGlow(this.GlowTextVfx, true);
        }
        this.RefreshBlockers(Location.Current.ID);
    }

    private void HidePanel()
    {
        base.Show(false);
    }

    private bool IsBigProceedButtonShowing()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        return ((window != null) && !window.powersPanel.ProceedButtons[1].Locked);
    }

    private bool IsLocationPowerShowing(string locID, LocationPowerType situation)
    {
        List<LocationPower> locationPowers = Scenario.Current.GetLocationPowers(locID);
        if (locationPowers != null)
        {
            for (int i = 0; i < locationPowers.Count; i++)
            {
                if ((locationPowers[i].Situation == situation) && !locationPowers[i].Passive)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnNextLocationButtonPushed()
    {
        if (!UI.Window.Paused || ((base.Owner != null) && !base.Owner.Paused))
        {
            int scenarioLocationIndex = this.GetScenarioLocationIndex(this.selectedID);
            if (scenarioLocationIndex >= 0)
            {
                string id = null;
                int num2 = 0;
                while (num2++ < Scenario.Current.Locations.Length)
                {
                    if (++scenarioLocationIndex >= Scenario.Current.Locations.Length)
                    {
                        scenarioLocationIndex = 0;
                    }
                    if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[scenarioLocationIndex].LocationName))
                    {
                        id = Scenario.Current.Locations[scenarioLocationIndex].LocationName;
                        break;
                    }
                }
                if (this.mapPanel != null)
                {
                    this.mapPanel.Seek(id);
                }
                else
                {
                    this.Show(id);
                }
            }
        }
    }

    private void OnPreviousLocationButtonPushed()
    {
        if (!UI.Window.Paused || ((base.Owner != null) && !base.Owner.Paused))
        {
            int scenarioLocationIndex = this.GetScenarioLocationIndex(this.selectedID);
            if (scenarioLocationIndex >= 0)
            {
                string id = null;
                int num2 = 0;
                while (num2++ < Scenario.Current.Locations.Length)
                {
                    if (--scenarioLocationIndex < 0)
                    {
                        scenarioLocationIndex = Scenario.Current.Locations.Length - 1;
                    }
                    if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[scenarioLocationIndex].LocationName))
                    {
                        id = Scenario.Current.Locations[scenarioLocationIndex].LocationName;
                        break;
                    }
                }
                if (this.mapPanel != null)
                {
                    this.mapPanel.Seek(id);
                }
                else
                {
                    this.Show(id);
                }
            }
        }
    }

    public override void Refresh()
    {
        if (Turn.Character != null)
        {
            this.Show(Turn.Character.Location);
        }
    }

    private void RefreshBlockers(string ID)
    {
        if (((this.BlockerAll != null) && (this.BlockerTop != null)) && (this.BlockerBottom != null))
        {
            bool flag = Scenario.Current.IsLocationClosed(ID);
            bool activeInHierarchy = this.GlowTextVfx.activeInHierarchy;
            bool flag3 = false;
            bool flag4 = activeInHierarchy;
            List<LocationPower> locationPowers = Scenario.Current.GetLocationPowers(ID);
            for (int i = 0; i < locationPowers.Count; i++)
            {
                if (locationPowers[i].UsefulWhenClosed)
                {
                    if (locationPowers[i].Situation == LocationPowerType.AtThisLocation)
                    {
                        flag3 = true;
                        flag4 = false;
                    }
                    if (locationPowers[i].Situation == LocationPowerType.WhenPermanentlyClosed)
                    {
                        flag4 = true;
                        flag3 = false;
                    }
                    if (((locationPowers[i].Situation == LocationPowerType.WhenClosing) && (ID == Location.Current.ID)) && Location.Current.ClosedThisTurn)
                    {
                        flag4 = true;
                        flag3 = false;
                    }
                }
            }
            this.BlockerAll.SetActive((flag && !flag3) && !flag4);
            this.BlockerTop.SetActive((flag && !flag3) && flag4);
            this.BlockerBottom.SetActive((flag && flag3) && !flag4);
        }
    }

    private void RefreshCardCount(CardType type, GuiLabel label)
    {
        if (label != null)
        {
            if (type == CardType.None)
            {
                label.Text = ((Scenario.Current.GetCardCount(Turn.Character.Location, type) + Scenario.Current.GetCardCount(Turn.Character.Location, CardType.Henchman)) + Scenario.Current.GetCardCount(Turn.Character.Location, CardType.Villain)).ToString();
            }
            else
            {
                label.Text = Scenario.Current.GetCardCount(Turn.Character.Location, type).ToString();
            }
        }
    }

    public void RefreshCardList()
    {
        this.RefreshCardCount(CardType.Monster, this.CountMonsterCardsLabel);
        this.RefreshCardCount(CardType.Barrier, this.CountBarrierCardsLabel);
        this.RefreshCardCount(CardType.Weapon, this.CountWeaponCardsLabel);
        this.RefreshCardCount(CardType.Spell, this.CountSpellCardsLabel);
        this.RefreshCardCount(CardType.Armor, this.CountArmorCardsLabel);
        this.RefreshCardCount(CardType.Item, this.CountItemCardsLabel);
        this.RefreshCardCount(CardType.Ally, this.CountAllyCardsLabel);
        this.RefreshCardCount(CardType.Blessing, this.CountBlessingCardsLabel);
        this.RefreshCardCount(CardType.None, this.CountUnknownCardsLabel);
    }

    private void SetGlowPosition(GameObject glowObject, float yOffset, float yScale)
    {
        glowObject.transform.localScale = new Vector3(1f, yScale, 1f);
        glowObject.transform.localPosition = new Vector3(glowObject.transform.localPosition.x, yOffset, glowObject.transform.localPosition.z);
    }

    private void SetLocationText(string atThisLocation, string whenClosing, string whenClosed)
    {
        if (((this.LocationTextAt != null) && (this.LocationTextWhenClosing != null)) && (this.LocationTextWhenPermClosed != null))
        {
            string headingColor = this.GetHeadingColor();
            string[] textArray1 = new string[] { "<b><color=", headingColor, ">", UI.Text(0x134), ": </color></b> ", atThisLocation };
            this.LocationTextAt.Text = string.Concat(textArray1);
            string[] textArray2 = new string[] { "<b><color=", headingColor, ">", UI.Text(0x1a1), ": </color></b> ", whenClosing };
            this.LocationTextWhenClosing.Text = string.Concat(textArray2);
            string[] textArray3 = new string[] { "<b><color=", headingColor, ">", UI.Text(0x135), ": </color></b> ", whenClosed };
            this.LocationTextWhenPermClosed.Text = string.Concat(textArray3);
        }
    }

    private void SetTextMargins(string locID)
    {
        if (this.ButtonFrameAtThisLocation != null)
        {
            if (this.IsLocationPowerShowing(locID, LocationPowerType.AtThisLocation) || this.IsBigProceedButtonShowing())
            {
                this.ButtonFrameAtThisLocation.SetActive(true);
                this.LocationTextAt.LineWidth = 290f;
            }
            else
            {
                this.ButtonFrameAtThisLocation.SetActive(false);
                this.LocationTextAt.LineWidth = 395f;
            }
        }
        if (this.ButtonFrameWhenPermClosed != null)
        {
            if (this.IsLocationPowerShowing(locID, LocationPowerType.WhenPermanentlyClosed) || this.IsLocationPowerShowing(locID, LocationPowerType.WhenClosing))
            {
                this.ButtonFrameWhenPermClosed.SetActive(true);
                this.LocationTextWhenPermClosed.LineWidth = 290f;
            }
            else
            {
                this.ButtonFrameWhenPermClosed.SetActive(false);
                this.LocationTextWhenPermClosed.LineWidth = 395f;
            }
        }
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            base.Show(true);
        }
        else
        {
            Vector3 to = base.transform.position - new Vector3(4f, 0f, 0f);
            LeanTween.move(base.gameObject, to, 0.5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.HidePanel));
        }
    }

    public void Show(string ID)
    {
        this.Show(true);
        if (this.mapPanel != null)
        {
            this.NextButton.Show(Turn.Map);
            this.PreviousButton.Show(Turn.Map);
        }
        if (ID != null)
        {
            LocationTableEntry entry = LocationTable.Get(ID);
            if (entry != null)
            {
                this.selectedID = ID;
                this.TitleLabel.Text = entry.Name;
                this.SetTextMargins(ID);
                this.SetLocationText(entry.Location, entry.Closing, entry.Closed);
                this.RefreshBlockers(this.selectedID);
                if (this.CountUnknownCardsLabel != null)
                {
                    this.CountUnknownCardsLabel.Text = ((Scenario.Current.GetCardCount(ID, CardType.None) + Scenario.Current.GetCardCount(ID, CardType.Henchman)) + Scenario.Current.GetCardCount(ID, CardType.Villain)).ToString();
                }
                this.CountMonsterCardsLabel.Text = Scenario.Current.GetCardCount(ID, CardType.Monster).ToString();
                this.CountBarrierCardsLabel.Text = Scenario.Current.GetCardCount(ID, CardType.Barrier).ToString();
                this.CountWeaponCardsLabel.Text = Scenario.Current.GetCardCount(ID, CardType.Weapon).ToString();
                this.CountSpellCardsLabel.Text = Scenario.Current.GetCardCount(ID, CardType.Spell).ToString();
                this.CountArmorCardsLabel.Text = Scenario.Current.GetCardCount(ID, CardType.Armor).ToString();
                this.CountItemCardsLabel.Text = Scenario.Current.GetCardCount(ID, CardType.Item).ToString();
                this.CountAllyCardsLabel.Text = Scenario.Current.GetCardCount(ID, CardType.Ally).ToString();
                this.CountBlessingCardsLabel.Text = Scenario.Current.GetCardCount(ID, CardType.Blessing).ToString();
                if (base.Owner == null)
                {
                    UI.Window.SendMessage("OnLocationChanged", ID, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    base.Owner.SendMessage("OnLocationChanged", ID, SendMessageOptions.DontRequireReceiver);
                }
            }
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.powersPanel.Refresh(Turn.Character, ID);
                window.layoutExplore.Counter.Display(Scenario.Current.GetCardCount(ID));
            }
        }
    }

    private void ShowGlow(GameObject glowObject, bool isVisible)
    {
        glowObject.SetActive(isVisible);
        if (isVisible)
        {
            Animator component = glowObject.GetComponent<Animator>();
            if (component != null)
            {
                component.SetTrigger("Glow");
            }
        }
    }
}

