using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiWindowAdventure : GuiWindow
{
    private AdventureMapIcon[] adventureMapIcons;
    [Tooltip("text field containing the adventure name")]
    public GuiLabel AdventureName;
    [Tooltip("reference to the adventure panel in this hierarchy")]
    public GameObject AdventurePanel;
    [Tooltip("text field containing the adventure reward")]
    public GuiLabel AdventureReward;
    [Tooltip("reference to the GO button in the scenario panel")]
    public AdventureMapButton ButtonGo;
    [Tooltip("reference to the store button in this scene")]
    public GuiButton ButtonStore;
    [Tooltip("reference to the cancel button in this scene")]
    public GuiButton CancelButton;
    [Tooltip("reference to the character sheet panel in this scene")]
    public GuiPanelCharacter characterPanel;
    [Tooltip("reference to the difficulty button in the scenario panel")]
    public GuiButtonRegion DifficultyButton;
    [Tooltip("reference to the difficulty button background in the scenario panel")]
    public GameObject DifficultyButtonBackground;
    [Tooltip("reference to the difficulty updated animator in the scenario panel")]
    public Animator DifficultyChangeAnimator;
    [Tooltip("reference to the scenario difficulty panel in this scene")]
    public GuiPanelScenarioDifficulty difficultyPanel;
    [Tooltip("reference to the \"unlock\" animator in our hierarchy")]
    public Animator IconUnlockedAmiator;
    private bool isLoadingInProgress;
    [Tooltip("reference to the position marker for adventure panel hidden")]
    public GameObject MarkerAdventurePanelHide;
    [Tooltip("reference to the position marker for adventure panel visible")]
    public GameObject MarkerAdventurePanelShow;
    [Tooltip("reference to the position marker for the menu button when at left")]
    public GameObject MarkerMenuButtonLeft;
    [Tooltip("reference to the position marker for the menu button when at right")]
    public GameObject MarkerMenuButtonRight;
    [Tooltip("reference to the position marker for scenario panel hidden")]
    public GameObject MarkerScenarioPanelHide;
    [Tooltip("reference to the position marker for scenario panel visible")]
    public GameObject MarkerScenarioPanelShow;
    [Tooltip("reference to the position marker for store panel hidden")]
    public GameObject MarkerStorePanelHide;
    [Tooltip("reference to the position marker for store panel visible")]
    public GameObject MarkerStorePanelShow;
    [Tooltip("reference to the menu button in this scene")]
    public GuiButton MenuButton;
    [Tooltip("reference to the reward card button in this scene")]
    public GuiButton RewardButton;
    [Tooltip("reference to the reward card panel in this scene")]
    public GuiPanelAdventureReward rewardPanel;
    [Tooltip("reference to the holder for the dynamic loaded scenario art")]
    public GameObject ScenarioArtHolder;
    [Tooltip("reference to the \"bottom\" object in the scroll region")]
    public GameObject ScenarioBottom;
    [Tooltip("each button has a row of pips beside it to indicate completion")]
    public ScenarioPipLayout[] ScenarioButtonPips;
    [Tooltip("one button for each scenario in the adventure")]
    public AdventureMapButton[] ScenarioButtons;
    [Tooltip("text field containing the scenario henchmen")]
    public GuiLabel ScenarioHenchmen;
    [Tooltip("text field containing the scenario information")]
    public GuiLabel ScenarioInfo;
    [Tooltip("reference to the scenario hilight line in this scene")]
    public GameObject scenarioLineHilite;
    [Tooltip("reference to the scenario panel in this hierarchy")]
    public GameObject ScenarioPanel;
    [Tooltip("pointer to the blueprint sprite that marks available scenario")]
    public Sprite ScenarioPipAvailableSprite;
    [Tooltip("pointer to the blueprint sprite that marks completed scenario")]
    public Sprite ScenarioPipCompletedSprite;
    [Tooltip("pointer to the blueprint sprite that marks locked scenario")]
    public Sprite ScenarioPipLockedSprite;
    [Tooltip("pointer to the blueprint sprite that marks unavailable scenario")]
    public Sprite ScenarioPipUnavailableSprite;
    [Tooltip("text field containing the scenario reward")]
    public GuiLabel ScenarioReward;
    private List<ScenarioTableEntry> scenarios = new List<ScenarioTableEntry>(6);
    [Tooltip("reference to the scrolling text region of the scenario window")]
    public GuiScrollRegion scenarioScroller;
    [Tooltip("text field containing the scenario title")]
    public GuiLabel ScenarioTitle;
    [Tooltip("text field containing the scenario villain")]
    public GuiLabel ScenarioVillain;
    private Adventure selectedAdventure;
    [Tooltip("reference to the selected adventure image in this hierarchy")]
    public GuiImage SelectedAdventureImage;
    private int selectedDifficulty;
    private string selectedScenario;
    private GameObject selectedScenarioArt;
    [Tooltip("reference to the \"buy adventure now\" panel in this hierarchy")]
    public GameObject StorePanel;
    private TKTapRecognizer tapRecognizer;

    private void AdjustBottom(string henchmen)
    {
        int num = 0;
        for (int i = 0; i < henchmen.Length; i++)
        {
            if (henchmen[i] == '\n')
            {
                num++;
            }
        }
        this.ScenarioBottom.transform.localPosition = new Vector3(0f, -1.3f - (num * 0.33f), 0f);
    }

    [DebuggerHidden]
    private IEnumerator AnimateIconUnlocks(string[] adventures) => 
        new <AnimateIconUnlocks>c__Iterator85 { 
            adventures = adventures,
            <$>adventures = adventures,
            <>f__this = this
        };

    private void BuildDecks()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Deck.Count <= 0)
            {
                Party.Characters[i].BuildDeck();
            }
        }
        Party.Validate();
    }

    private void CancelButtonPushed()
    {
        Game.UI.ShowCreatePartyScene();
    }

    private void ClearScenario()
    {
        this.scenarioLineHilite.SetActive(false);
        this.selectedScenario = null;
        if (this.selectedScenarioArt != null)
        {
            UnityEngine.Object.Destroy(this.selectedScenarioArt);
        }
        this.selectedScenarioArt = null;
        this.ScenarioTitle.Clear();
        this.ScenarioInfo.Clear();
        this.ScenarioVillain.Clear();
        this.ScenarioHenchmen.Clear();
        this.ScenarioReward.Clear();
        this.ButtonGo.Show(false);
        this.DifficultyButton.Show(false);
        this.DifficultyButtonBackground.SetActive(false);
        this.RewardButton.Show(false);
        UI.Busy = false;
    }

    public override void Close()
    {
        base.Close();
        if (this.isLoadingInProgress)
        {
            this.SetupAdventure(this.selectedAdventure.ID);
            this.SetupScenario(this.selectedScenario);
            if (Scenario.Current != null)
            {
                Scenario.Current.Difficulty = this.selectedDifficulty;
                Scenario.Current.Initialize();
                Scenario.Current.Play();
            }
        }
    }

    private void CloseSubWindows()
    {
        if (this.difficultyPanel.Visible)
        {
            this.difficultyPanel.Show(false);
        }
        if (this.rewardPanel.Visible)
        {
            this.rewardPanel.Show(false);
        }
    }

    private int GetAdventureIndex(Adventure adventure)
    {
        for (int i = 0; i < this.adventureMapIcons.Length; i++)
        {
            if (this.adventureMapIcons[i].Adventure.ID == adventure.ID)
            {
                return i;
            }
        }
        return 0;
    }

    private AdventureMapIcon GetCurrentAdventure()
    {
        AdventureMapIcon icon = null;
        for (int i = 0; i < this.adventureMapIcons.Length; i++)
        {
            if (!this.adventureMapIcons[i].Adventure.Completed && ((icon == null) || (this.adventureMapIcons[i].Adventure.Number < icon.Adventure.Number)))
            {
                icon = this.adventureMapIcons[i];
            }
        }
        return icon;
    }

    private int GetCurrentScenario()
    {
        for (int i = 0; i < this.scenarios.Count; i++)
        {
            if (this.IsScenarioAvailable(this.scenarios[i].id) && !Campaign.IsScenarioComplete(this.scenarios[i].id))
            {
                return i;
            }
        }
        return 0;
    }

    private Vector2 GetScenarioScrollerSize()
    {
        float num = 0f;
        num += this.ScenarioVillain.Size.y;
        num += this.ScenarioHenchmen.Size.y;
        return new Vector2(0f, num + this.ScenarioInfo.Size.y);
    }

    private void GoButtonPushed()
    {
        UI.Busy = false;
        this.isLoadingInProgress = true;
        if (Party.Audit())
        {
            Game.UI.ShowCutsceneScene();
        }
        else
        {
            Game.UI.ShowSelectCardScene();
        }
    }

    private void HideScenario()
    {
        this.difficultyPanel.Show(false);
        this.rewardPanel.Show(false);
        LeanTween.move(this.ScenarioPanel, this.MarkerScenarioPanelHide.transform.position, 0.4f).setEase(LeanTweenType.easeInQuad).setOnComplete(new Action(this.ClearScenario));
        if (this.ScenarioPanel.transform.position != this.MarkerScenarioPanelHide.transform.position)
        {
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
            LeanTween.move(this.MenuButton.gameObject, this.MarkerMenuButtonLeft.transform.position, 0.4f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(new Action(this.RefreshWindowButtons));
        }
    }

    private void HideStore()
    {
        this.ClearScenario();
        LeanTween.move(this.StorePanel, this.MarkerStorePanelHide.transform.position, 0.33f).setEase(LeanTweenType.easeInQuad);
        if (this.StorePanel.transform.position != this.MarkerStorePanelHide.transform.position)
        {
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
        }
        this.ButtonStore.Show(false);
    }

    private bool IsScenarioAvailable(string ID)
    {
        if (!Settings.Debug.StoryMode)
        {
            return true;
        }
        if (this.selectedAdventure.Available)
        {
            ScenarioTableEntry entry = ScenarioTable.Get(ID);
            if (entry != null)
            {
                if (entry.number == 1)
                {
                    return true;
                }
                for (int i = 0; i < this.scenarios.Count; i++)
                {
                    if (this.scenarios[i].number == (entry.number - 1))
                    {
                        return Campaign.IsScenarioComplete(this.scenarios[i].id);
                    }
                }
            }
        }
        return false;
    }

    private void LoadScenarioArt(string ID)
    {
        if (this.selectedScenarioArt != null)
        {
            UnityEngine.Object.Destroy(this.selectedScenarioArt);
        }
        GameObject prefab = Resources.Load<GameObject>("Art/Scenarios/" + ID);
        if (prefab != null)
        {
            this.selectedScenarioArt = Game.Instance.Create(prefab);
            if (this.selectedScenarioArt != null)
            {
                this.selectedScenarioArt.transform.parent = this.ScenarioArtHolder.transform;
                this.selectedScenarioArt.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void OnAdventureNextButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            int index = this.GetAdventureIndex(this.selectedAdventure) + 1;
            if (index > (this.adventureMapIcons.Length - 1))
            {
                index = 0;
            }
            this.HideScenario();
            this.HideStore();
            this.ShowAdventure(this.adventureMapIcons[index]);
        }
    }

    private void OnAdventurePreviousButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            int index = this.GetAdventureIndex(this.selectedAdventure) - 1;
            if (index < 0)
            {
                index = this.adventureMapIcons.Length - 1;
            }
            this.HideScenario();
            this.HideStore();
            this.ShowAdventure(this.adventureMapIcons[index]);
        }
    }

    private void OnCancelButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            this.CloseSubWindows();
            LeanTween.scale(this.CancelButton.gameObject, new Vector3(0.75f, 0.75f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.CancelButtonPushed));
        }
    }

    private void OnCardsButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            Game.UI.ShowSelectCardScene();
        }
    }

    private void OnCharacterSheetButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.Pause(true);
            Game.UI.ShowCreatePartyScene();
        }
    }

    private void OnDifficultyButtonPushed()
    {
        if (!UI.Busy)
        {
            if (!this.difficultyPanel.Visible)
            {
                this.CloseSubWindows();
                if (!string.IsNullOrEmpty(this.selectedScenario))
                {
                    this.difficultyPanel.Show(this.selectedScenario, this.selectedDifficulty);
                }
            }
            else
            {
                this.difficultyPanel.Show(false);
            }
        }
    }

    private void OnGoButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.ButtonGo.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.GoButtonPushed));
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_MAP);
        if (hitd != 0)
        {
            AdventureMapIcon component = hitd.collider.transform.GetComponent<AdventureMapIcon>();
            if (component != null)
            {
                component.Tap();
                if (this.selectedAdventure != component.Adventure)
                {
                    this.ShowAdventure(component);
                }
            }
            else
            {
                AdventureMapButton button = hitd.collider.transform.GetComponent<AdventureMapButton>();
                if (button != null)
                {
                    button.Tap();
                }
            }
        }
    }

    private void OnMenuButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.Pause(true);
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnRewardCardButtonPushed()
    {
        if (!UI.Busy)
        {
            if (!this.rewardPanel.Visible)
            {
                this.CloseSubWindows();
                this.rewardPanel.Show(this.selectedAdventure);
            }
            else
            {
                this.rewardPanel.Show(false);
            }
        }
    }

    private void OnScenarioButton01Pushed()
    {
        this.OnScenarioButtonPushed(0);
    }

    private void OnScenarioButton02Pushed()
    {
        this.OnScenarioButtonPushed(1);
    }

    private void OnScenarioButton03Pushed()
    {
        this.OnScenarioButtonPushed(2);
    }

    private void OnScenarioButton04Pushed()
    {
        this.OnScenarioButtonPushed(3);
    }

    private void OnScenarioButton05Pushed()
    {
        this.OnScenarioButtonPushed(4);
    }

    private void OnScenarioButtonPushed(int n)
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            if (this.selectedAdventure.Purchased)
            {
                this.ShowScenario(this.selectedAdventure.Scenarios[n], this.scenarios[n]);
            }
            else
            {
                this.ShowStore();
            }
            this.scenarioLineHilite.transform.position = this.ScenarioButtons[n].transform.position;
            this.scenarioLineHilite.SetActive(true);
            LeanTween.scale(this.ScenarioButtonPips[n].Pips[0].gameObject, new Vector3(0.75f, 0.75f, 1f), 0.1f).setLoopPingPong().setLoopCount(2);
        }
    }

    private void OnStoreButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !Settings.Debug.DemoMode)
        {
            Game.UI.ShowStoreWindow(this.selectedAdventure.License, LicenseType.Adventure);
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.tapRecognizer.enabled = !isPaused;
        this.scenarioScroller.Pause(isPaused);
    }

    private void RefreshWindowButtons()
    {
        this.MenuButton.Refresh();
        this.DifficultyButton.Refresh();
        this.RewardButton.Refresh();
    }

    public void SetScenarioDifficulty(int level, bool vfx)
    {
        if (vfx && (this.selectedDifficulty != level))
        {
            this.DifficultyChangeAnimator.SetTrigger("Start");
        }
        this.selectedDifficulty = level;
        this.ScenarioInfo.Text = this.difficultyPanel.GetDifficultyText(this.selectedScenario, this.selectedDifficulty);
        this.DifficultyButton.Image = this.difficultyPanel.GetDifficultyImage(this.selectedDifficulty);
    }

    private void SetupAdventure(string ID)
    {
        if (Adventure.Current != null)
        {
            UnityEngine.Object.Destroy(Adventure.Current.gameObject);
        }
        Adventure.Current = AdventureTable.Create(ID);
    }

    private void SetupAdventurePath(string ID)
    {
        if (AdventurePath.Current != null)
        {
            UnityEngine.Object.Destroy(AdventurePath.Current.gameObject);
        }
        AdventurePath.Current = AdventurePathTable.Create(ID);
    }

    private void SetupScenario(string ID)
    {
        if (Scenario.Current != null)
        {
            UnityEngine.Object.Destroy(Scenario.Current.gameObject);
        }
        Scenario.Current = ScenarioTable.Create(ID);
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
    }

    private void ShowAdventure(AdventureMapIcon icon)
    {
        if (icon != null)
        {
            this.selectedAdventure = icon.Adventure;
            this.SelectedAdventureImage.transform.position = icon.transform.position;
            this.SelectedAdventureImage.Show(true);
            this.scenarios.Clear();
            for (int i = 0; i < this.selectedAdventure.Scenarios.Length; i++)
            {
                ScenarioTableEntry item = ScenarioTable.Get(this.selectedAdventure.Scenarios[i]);
                if (item != null)
                {
                    this.scenarios.Add(item);
                }
            }
            this.AdventureName.Text = this.selectedAdventure.DisplayName;
            for (int j = 0; j < this.ScenarioButtons.Length; j++)
            {
                this.ScenarioButtons[j].Show(j < this.scenarios.Count);
                this.ScenarioButtonPips[j].Show(j < this.scenarios.Count);
                if (j < this.scenarios.Count)
                {
                    this.ScenarioButtons[j].Text = this.scenarios[j].Name;
                    if (this.IsScenarioAvailable(this.scenarios[j].id))
                    {
                        this.ScenarioButtonPips[j].ShowAvailable(this.scenarios[j].id, this.ScenarioPipAvailableSprite, this.ScenarioPipCompletedSprite, this.ScenarioPipUnavailableSprite);
                    }
                    else
                    {
                        this.ScenarioButtonPips[j].ShowUnavailable(this.scenarios[j].id, this.ScenarioPipLockedSprite);
                    }
                }
            }
            this.AdventureReward.Text = this.selectedAdventure.RewardText;
            this.HideScenario();
            this.HideStore();
            LeanTween.move(this.AdventurePanel, this.MarkerAdventurePanelShow.transform.position, 0.3f).setEase(LeanTweenType.easeOutQuad);
            if (this.AdventurePanel.transform.position != this.MarkerAdventurePanelShow.transform.position)
            {
                UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
            }
            LeanTween.delayedCall(0.5f, new Action(this.ShowFirstScenarioInAdventure));
        }
    }

    [DebuggerHidden]
    private IEnumerator ShowCurrentAdventureAndScenario() => 
        new <ShowCurrentAdventureAndScenario>c__Iterator84 { <>f__this = this };

    private void ShowFirstScenarioInAdventure()
    {
        int currentScenario = this.GetCurrentScenario();
        this.OnScenarioButtonPushed(currentScenario);
    }

    private void ShowScenario(string ID, ScenarioTableEntry entry)
    {
        Tutorial.Hide();
        this.difficultyPanel.Show(false);
        this.rewardPanel.Show(false);
        if (this.selectedScenario != null)
        {
            this.scenarioScroller.Top();
        }
        this.selectedScenario = ID;
        this.ScenarioTitle.Text = entry.Name;
        this.ScenarioVillain.Text = entry.Villain;
        this.ScenarioHenchmen.Text = entry.Henchmen;
        this.ScenarioReward.Text = entry.Reward;
        this.SetScenarioDifficulty(this.difficultyPanel.GetDifficultyLevel(this.selectedScenario), false);
        this.LoadScenarioArt(ID);
        this.AdjustBottom(entry.Henchmen);
        this.ButtonGo.Show(this.IsScenarioAvailable(this.selectedScenario));
        this.DifficultyButton.Show(this.IsScenarioAvailable(this.selectedScenario));
        this.DifficultyButtonBackground.SetActive(this.IsScenarioAvailable(this.selectedScenario));
        this.RewardButton.Show(this.IsScenarioAvailable(this.selectedScenario));
        UI.Busy = true;
        LeanTween.move(this.ScenarioPanel, this.MarkerScenarioPanelShow.transform.position, 0.4f).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.ShowScenarioDone));
        if (this.ScenarioPanel.transform.position != this.MarkerScenarioPanelShow.transform.position)
        {
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
            LeanTween.move(this.MenuButton.gameObject, this.MarkerMenuButtonRight.transform.position, 0.4f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(new Action(this.RefreshWindowButtons));
        }
    }

    private void ShowScenarioDone()
    {
        this.scenarioScroller.Reset();
        this.scenarioScroller.Max = this.GetScenarioScrollerSize();
        UI.Busy = false;
    }

    private void ShowStore()
    {
        this.ButtonGo.Show(false);
        UI.Busy = true;
        LeanTween.move(this.StorePanel, this.MarkerStorePanelShow.transform.position, 0.33f).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.ShowStoreDone));
        if (this.StorePanel.transform.position != this.MarkerStorePanelShow.transform.position)
        {
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
        }
        this.ButtonStore.Show(true);
    }

    private void ShowStoreDone()
    {
        this.ButtonStore.Refresh();
        UI.Busy = false;
    }

    protected override void Start()
    {
        base.Start();
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        Turn.Number = 0;
        Turn.Switch = 0;
        Turn.Current = 0;
        this.BuildDecks();
        this.scenarioScroller.Initialize();
        this.characterPanel.Initialize();
        this.difficultyPanel.Initialize();
        this.rewardPanel.Initialize();
        this.AdventurePanel.transform.position = this.MarkerAdventurePanelHide.transform.position;
        this.ScenarioPanel.transform.position = this.MarkerScenarioPanelHide.transform.position;
        this.StorePanel.transform.position = this.MarkerStorePanelHide.transform.position;
        this.SelectedAdventureImage.Show(false);
        this.adventureMapIcons = base.GetComponentsInChildren<AdventureMapIcon>(true);
        this.SetupAdventurePath("AP1B_RiseOfTheRuneLords");
        if (Adventure.Current != null)
        {
            Game.Save();
            Game.Synchronize();
        }
        if (Adventure.Current != null)
        {
            this.CancelButton.Show(false);
        }
        base.StartCoroutine(this.ShowCurrentAdventureAndScenario());
        base.StartCoroutine(this.AnimateIconUnlocks(Campaign.GetUnlockedAdventures()));
        Tutorial.Notify(TutorialEventType.ScreenAdventureShown);
    }

    public override WindowType Type =>
        WindowType.Adventure;

    [CompilerGenerated]
    private sealed class <AnimateIconUnlocks>c__Iterator85 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string[] <$>adventures;
        internal string[] <$s_155>__0;
        internal int <$s_156>__1;
        internal string[] <$s_157>__6;
        internal int <$s_158>__7;
        internal GuiWindowAdventure <>f__this;
        internal AdventureMapIcon <currentAdventure>__5;
        internal float <delay>__4;
        internal int <i>__3;
        internal string <id>__2;
        internal string <id>__8;
        internal string <msg>__10;
        internal string <pid>__9;
        internal string[] adventures;

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
                    if (this.adventures != null)
                    {
                        this.<$s_155>__0 = this.adventures;
                        this.<$s_156>__1 = 0;
                        while (this.<$s_156>__1 < this.<$s_155>__0.Length)
                        {
                            this.<id>__2 = this.<$s_155>__0[this.<$s_156>__1];
                            this.<i>__3 = 0;
                            while (this.<i>__3 < this.<>f__this.adventureMapIcons.Length)
                            {
                                if (this.<>f__this.adventureMapIcons[this.<i>__3].Adventure.ID == this.<id>__2)
                                {
                                    this.<delay>__4 = this.<>f__this.adventureMapIcons[this.<i>__3].AnimateUnlock(this.<>f__this.IconUnlockedAmiator);
                                    this.$current = new WaitForSeconds(this.<delay>__4);
                                    this.$PC = 1;
                                    return true;
                                }
                            Label_00D4:
                                this.<i>__3++;
                            }
                            this.<$s_156>__1++;
                        }
                        this.<currentAdventure>__5 = this.<>f__this.GetCurrentAdventure();
                        if ((this.<currentAdventure>__5 != null) && (this.<currentAdventure>__5.Adventure.Number > 0))
                        {
                            this.<$s_157>__6 = this.adventures;
                            this.<$s_158>__7 = 0;
                            while (this.<$s_158>__7 < this.<$s_157>__6.Length)
                            {
                                this.<id>__8 = this.<$s_157>__6[this.<$s_158>__7];
                                if (this.<currentAdventure>__5.Adventure.ID == this.<id>__8)
                                {
                                    this.<pid>__9 = LicenseManager.GetLicenseIdentifierForDeck(this.<currentAdventure>__5.Adventure.Set);
                                    if (!LicenseManager.GetIsSupported(this.<pid>__9))
                                    {
                                        this.<msg>__10 = string.Format(StringTableManager.Get("ui", 0x26b), this.<currentAdventure>__5.Adventure.DisplayName);
                                        Tutorial.Message(this.<msg>__10, 0.2f, 0.35f);
                                    }
                                    break;
                                }
                                this.<$s_158>__7++;
                            }
                        }
                    }
                    break;

                case 1:
                    goto Label_00D4;

                default:
                    goto Label_0235;
            }
            this.$PC = -1;
        Label_0235:
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

    [CompilerGenerated]
    private sealed class <ShowCurrentAdventureAndScenario>c__Iterator84 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowAdventure <>f__this;

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
                    this.$current = new WaitForSeconds(0.8f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.ShowAdventure(this.<>f__this.GetCurrentAdventure());
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

