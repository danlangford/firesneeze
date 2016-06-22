using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelPowers : GuiPanel
{
    [Tooltip("references to character power buttons on this panel")]
    public GuiButton[] CharacterButtons;
    private Power[] characterPowers;
    private Power displayedPower;
    private List<GameObject> hilites;
    [Tooltip("reference to the info panel in this scene")]
    public GuiPanelPowersInfo infoPanel;
    [Tooltip("references to location power buttons on this panel")]
    public GuiButton[] LocationButtons;
    private Power[] locationPowers;
    private bool powerActivating;
    [Tooltip("references to scenario/location proceed animators")]
    public Animator[] ProceedButtonAnimators;
    [Tooltip("references to scenario/location proceed buttons")]
    public GuiButton[] ProceedButtons;
    [Tooltip("references to scenario power buttons on this panel")]
    public GuiButton[] ScenarioButtons;
    private ScenarioPower[] scenarioPowers;
    [Tooltip("references to the character power toggle buttons on this panel")]
    public GuiButton[] ToggleButtons;
    private bool togglesShowing;
    [Tooltip("reference to the activation vfx in this scene")]
    public VfxPowerActivation vfxActivate;
    [Tooltip("pointer to the button hilite vfx prefab")]
    public GameObject vfxHilite;
    [Tooltip("reference to the selection vfx in this panel")]
    public GameObject vfxSelect;

    [DebuggerHidden]
    private IEnumerator ActivatePower(Power power) => 
        new <ActivatePower>c__Iterator67 { 
            power = power,
            <$>power = power,
            <>f__this = this
        };

    protected override void Awake()
    {
        base.Awake();
        this.characterPowers = new Power[this.CharacterButtons.Length];
        this.locationPowers = new Power[this.LocationButtons.Length];
        this.scenarioPowers = new ScenarioPower[this.ScenarioButtons.Length];
        this.hilites = new List<GameObject>((this.CharacterButtons.Length + this.LocationButtons.Length) + this.ScenarioButtons.Length);
    }

    public void Cancel()
    {
        if (this.togglesShowing)
        {
            this.HidePowerToggles();
        }
    }

    private int GetLocationPowerIndex(LocationPower power)
    {
        if ((power == null) || ((power.Situation != LocationPowerType.WhenPermanentlyClosed) && (power.Situation != LocationPowerType.WhenClosing)))
        {
            return 1;
        }
        return 0;
    }

    private void HidePowerToggles()
    {
        for (int i = 0; i < this.ToggleButtons.Length; i++)
        {
            this.ToggleButtons[i].Glow(false);
            this.ToggleButtons[i].gameObject.GetComponent<Animator>().SetTrigger("Close");
        }
        this.togglesShowing = false;
    }

    private void Hilite(GuiButton button, GameObject prefab)
    {
        if ((prefab != null) && (button != null))
        {
            GameObject item = UnityEngine.Object.Instantiate<GameObject>(prefab);
            if (item != null)
            {
                item.transform.position = button.transform.position;
                item.transform.parent = button.transform;
                this.hilites.Add(item);
            }
        }
    }

    public override void Initialize()
    {
        this.ProceedButtons[0].Locked = true;
        this.ProceedButtons[1].Locked = true;
    }

    private bool IsPowerValid(Power power)
    {
        if (Turn.State == GameStateType.Share)
        {
            CharacterPower power2 = power as CharacterPower;
            return ((power2 != null) && power2.Shareable);
        }
        return power.IsValid();
    }

    public void OnCharacterPowerButtonPushed(int i)
    {
        if (Turn.State == GameStateType.Share)
        {
            GameStateShare.SetPowerMode(true, this.characterPowers[i].ID);
        }
        this.OnPowerButtonPushed(this.CharacterButtons[i], i, this.characterPowers);
    }

    public void OnCharacterToggleButtonPushed(int i)
    {
        this.OnToggleButtonPushed(this.ToggleButtons[i], i, this.characterPowers[0]);
    }

    public void OnDiceRolled()
    {
        if (this.togglesShowing)
        {
            this.HidePowerToggles();
        }
        if (this.displayedPower != null)
        {
            this.displayedPower.OnDiceRolled();
        }
    }

    public void OnExamineFinished()
    {
        if (this.togglesShowing)
        {
            this.HidePowerToggles();
        }
        if (this.displayedPower != null)
        {
            this.displayedPower.OnExamineComplete();
        }
    }

    public void OnLocationPowerButtonPushed(int i)
    {
        this.OnPowerButtonPushed(this.LocationButtons[i], i, this.locationPowers);
    }

    public void OnPowerActivated(Power power)
    {
        this.displayedPower = power;
    }

    private void OnPowerButtonPushed(GuiButton button, int i, Power[] powers)
    {
        if (!this.powerActivating && !UI.Window.Paused)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if ((((window != null) && !window.dicePanel.Rolling) && ((i >= 0) && (i < powers.Length))) && (powers[i] != null))
            {
                window.UnZoomCard();
                this.PopupInfo(button, powers[i].Description);
                if (powers[i].IsValid())
                {
                    base.StartCoroutine(this.ActivatePower(powers[i]));
                }
            }
        }
    }

    public void OnPowerDeactivated(Power power)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.powersPanel.infoPanel.Text = string.Empty;
            window.powersPanel.infoPanel.Show(false);
            window.powersPanel.vfxSelect.SetActive(false);
        }
        this.displayedPower = null;
    }

    public void OnScenarioPowerButtonPushed(int i)
    {
        this.OnPowerButtonPushed(this.ScenarioButtons[i], i, this.scenarioPowers);
    }

    private void OnToggleButtonPushed(GuiButton button, int i, Power power)
    {
        if ((power != null) && !UI.Window.Paused)
        {
            bool active = !power.IsModifierActive(i);
            if (power.SetModifierActive(i, active))
            {
                Rules.ApplyCombatAdjustments();
                button.Glow(active);
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    window.dicePanel.SetCheck(null, Turn.Checks, Turn.Check);
                }
            }
        }
    }

    private void PopupInfo(GuiButton button, string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.powersPanel.infoPanel.Text = text;
                window.powersPanel.infoPanel.Show(true);
            }
        }
        if (button != null)
        {
            this.vfxSelect.transform.position = button.transform.position;
            this.vfxSelect.SetActive(true);
        }
    }

    public override void Refresh()
    {
        this.RefreshPowerButtons(Turn.Character, Turn.Character.Location);
    }

    public void Refresh(Character character, string locationID)
    {
        this.RefreshPowerButtons(character, locationID);
    }

    private void RefreshPowerButtons(Character character, string locID)
    {
        for (int i = 0; i < this.characterPowers.Length; i++)
        {
            this.characterPowers[i] = null;
        }
        for (int j = 0; j < this.locationPowers.Length; j++)
        {
            this.locationPowers[j] = null;
        }
        for (int k = 0; k < this.scenarioPowers.Length; k++)
        {
            this.scenarioPowers[k] = null;
        }
        for (int m = 0; m < this.hilites.Count; m++)
        {
            UnityEngine.Object.Destroy(this.hilites[m]);
        }
        this.hilites.Clear();
        this.vfxSelect.SetActive(false);
        int index = 0;
        for (int n = 0; n < character.Powers.Count; n++)
        {
            if (!character.Powers[n].Passive)
            {
                this.characterPowers[index] = character.Powers[n];
                this.CharacterButtons[index].Image = character.Powers[n].Icon;
                this.CharacterButtons[index].ImageDisabled = character.Powers[n].IconDisabled;
                this.CharacterButtons[index].ImageHilite = character.Powers[n].IconHilite;
                if ((this.displayedPower != null) && (this.displayedPower.ID == this.characterPowers[index].ID))
                {
                    this.vfxSelect.SetActive(true);
                }
                index++;
            }
        }
        index = 0;
        List<LocationPower> locationPowers = Scenario.Current.GetLocationPowers(locID);
        for (int num7 = 0; num7 < locationPowers.Count; num7++)
        {
            if (!locationPowers[num7].Passive)
            {
                index = this.GetLocationPowerIndex(locationPowers[num7]);
                this.locationPowers[index] = locationPowers[num7];
                this.LocationButtons[index].Image = locationPowers[num7].Icon;
                this.LocationButtons[index].ImageDisabled = locationPowers[num7].IconDisabled;
                this.LocationButtons[index].ImageHilite = locationPowers[num7].IconHilite;
                if ((this.displayedPower != null) && (this.displayedPower.ID == this.locationPowers[index].ID))
                {
                    this.vfxSelect.SetActive(true);
                }
            }
        }
        index = 0;
        for (int num8 = 0; num8 < Scenario.Current.Powers.Count; num8++)
        {
            if (!Scenario.Current.Powers[num8].Passive)
            {
                this.scenarioPowers[index] = Scenario.Current.Powers[num8];
                this.ScenarioButtons[index].Image = Scenario.Current.Powers[num8].Icon;
                this.ScenarioButtons[index].ImageDisabled = Scenario.Current.Powers[num8].IconDisabled;
                this.ScenarioButtons[index].ImageHilite = Scenario.Current.Powers[num8].IconHilite;
                if ((this.displayedPower != null) && (this.displayedPower.ID == this.scenarioPowers[index].ID))
                {
                    this.vfxSelect.SetActive(true);
                }
                index++;
            }
        }
    }

    public void ShowLocationPowerProceedButton(bool isVisible)
    {
        this.ProceedButtons[1].Locked = !isVisible;
        this.ProceedButtonAnimators[1].SetBool("Show", isVisible);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.locationPanel.Refresh();
            if (isVisible)
            {
                window.proceedButton.Fade(false, 0.1f);
            }
        }
    }

    private void ShowPowerToggles(Power power, int n)
    {
        if (power is CharacterPower)
        {
            CharacterPowerModifier powerModifier = (power as CharacterPower).GetPowerModifier(n);
            if ((powerModifier != null) && powerModifier.Optional)
            {
                this.ToggleButtons[n].Show(true);
                this.ToggleButtons[n].gameObject.GetComponent<Animator>().SetTrigger("Open");
                this.ToggleButtons[n].Image = powerModifier.Icon;
                this.ToggleButtons[n].ImageActive = powerModifier.IconHilite;
                this.ToggleButtons[n].ImageHilite = powerModifier.IconHilite;
                this.ToggleButtons[n].ImageDisabled = powerModifier.IconDisabled;
                this.ToggleButtons[n].Glow(power.IsModifierActive(n));
                this.togglesShowing = true;
            }
        }
    }

    public void ShowPowerVFX(Power power)
    {
        if (!Turn.IsSwitchingCharacters())
        {
            this.vfxActivate.Icon = power.Icon;
            this.vfxActivate.Show(true);
        }
    }

    public void ShowScenarioPowerProceedButton(bool isVisible)
    {
        this.ProceedButtons[0].Locked = !isVisible;
        this.ProceedButtonAnimators[0].SetBool("Show", isVisible);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (isVisible && (window != null))
        {
            window.proceedButton.Fade(false, 0.1f);
        }
    }

    private void Update()
    {
        if ((Party.Characters.Count > 0) && !Turn.IsSwitchingCharacters())
        {
            for (int i = 0; i < this.CharacterButtons.Length; i++)
            {
                if (this.characterPowers[i] != null)
                {
                    bool disabled = this.CharacterButtons[i].Disabled;
                    bool flag2 = this.IsPowerValid(this.characterPowers[i]) && !Turn.Map;
                    if (disabled && flag2)
                    {
                        this.Hilite(this.CharacterButtons[i], this.vfxHilite);
                    }
                    this.CharacterButtons[i].Show(true);
                    if (!this.CharacterButtons[i].Selected)
                    {
                        this.CharacterButtons[i].Disable(!flag2);
                    }
                }
                else
                {
                    this.CharacterButtons[i].Show(false);
                }
            }
            for (int j = 0; j < this.LocationButtons.Length; j++)
            {
                if (this.locationPowers[j] != null)
                {
                    bool flag3 = this.LocationButtons[j].Disabled;
                    bool flag4 = this.locationPowers[j].IsValid() && !Turn.Map;
                    if (flag3 && flag4)
                    {
                        this.Hilite(this.LocationButtons[j], this.vfxHilite);
                    }
                    this.LocationButtons[j].Show(true);
                    if (!this.LocationButtons[j].Selected)
                    {
                        this.LocationButtons[j].Disable(!flag4);
                    }
                }
                else
                {
                    this.LocationButtons[j].Show(false);
                }
            }
            for (int k = 0; k < this.ScenarioButtons.Length; k++)
            {
                if ((this.scenarioPowers[k] != null) && !Turn.Map)
                {
                    if (this.scenarioPowers[k].IsLocationValid(Location.Current.ID))
                    {
                        bool flag5 = this.ScenarioButtons[k].Disabled && !Turn.Map;
                        bool flag6 = this.scenarioPowers[k].IsValid();
                        if (flag5 && flag6)
                        {
                            this.Hilite(this.ScenarioButtons[k], this.vfxHilite);
                        }
                        this.ScenarioButtons[k].Show(true);
                        if (!this.ScenarioButtons[k].Selected)
                        {
                            this.ScenarioButtons[k].Disable(!flag6);
                        }
                    }
                    else
                    {
                        this.ScenarioButtons[k].Show(false);
                    }
                }
                else
                {
                    this.ScenarioButtons[k].Show(false);
                }
            }
        }
    }

    public override uint zIndex =>
        (Constants.ZINDEX_PANEL_BASE + 1);

    [CompilerGenerated]
    private sealed class <ActivatePower>c__Iterator67 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Power <$>power;
        internal GuiPanelPowers <>f__this;
        internal Power power;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<>f__this.powerActivating = true;
                    this.<>f__this.ShowPowerVFX(this.power);
                    this.<>f__this.Pause(true);
                    UI.Window.Pause(true);
                    UI.Busy = true;
                    this.$current = new WaitForSeconds(0.7f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Pause(false);
                    UI.Window.Pause(false);
                    UI.Busy = false;
                    this.<>f__this.ShowPowerToggles(this.power, 0);
                    this.<>f__this.ShowPowerToggles(this.power, 1);
                    this.power.Activate();
                    this.<>f__this.powerActivating = false;
                    this.$PC = -1;
                    break;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

