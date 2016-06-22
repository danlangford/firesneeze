using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiWindowQuest : GuiWindow
{
    [Tooltip("reference to the GO button in the quest panel")]
    public AdventureMapButton ButtonGo;
    [Tooltip("reference to the cancel button in this scene")]
    public GuiButton CancelButton;
    [Tooltip("reference to the character sheet panel in this scene")]
    public GuiPanelCharacter characterPanel;
    [Tooltip("reference to the difficulty button in the quest panel")]
    public GuiButtonRegion DifficultyButton;
    [Tooltip("reference to the difficulty updated animator in the quest panel")]
    public Animator DifficultyChangeAnimator;
    [Tooltip("reference to the scenario difficulty panel in this scene")]
    public GuiPanelScenarioDifficulty difficultyPanel;
    [Tooltip("text field containing a summary of events")]
    public GuiLabel EventSummary;
    private bool isLoadingInProgress;
    [Tooltip("reference to the position marker for the menu button when at left")]
    public GameObject MarkerMenuButtonLeft;
    [Tooltip("reference to the position marker for the menu button when at right")]
    public GameObject MarkerMenuButtonRight;
    [Tooltip("reference to the position marker for quest panel hidden")]
    public GameObject MarkerQuestPanelHide;
    [Tooltip("reference to the position marker for quest panel visible")]
    public GameObject MarkerQuestPanelShow;
    [Tooltip("reference to the menu button in this scene")]
    public GuiButton MenuButton;
    [Tooltip("reference to the sound played when the quest panel drops")]
    public AudioClip PanelDropSound;
    private QuestMapIcon[] questMapIcons;
    private QuestMapPoint[] questMapPoints;
    [Tooltip("reference to the quest panel in this hierarchy")]
    public GameObject QuestPanel;
    [Tooltip("reference to the holder for the dynamic loaded scenario art")]
    public GameObject ScenarioArtHolder;
    [Tooltip("reference to the \"bottom\" object in the scroll region")]
    public GameObject ScenarioBottom;
    [Tooltip("text field containing the scenario henchmen")]
    public GuiLabel ScenarioDifficulty;
    [Tooltip("text field containing the scenario information (description, level, difficulty)")]
    public GuiLabel ScenarioInfo;
    [Tooltip("text field containing the scenario villain")]
    public GuiLabel ScenarioLevel;
    [Tooltip("text field containing the scenario reward")]
    public GuiLabel ScenarioReward;
    [Tooltip("reference to the scrolling text region of the quest panel")]
    public GuiScrollRegion scenarioScroller;
    [Tooltip("text field containing the scenario title")]
    public GuiLabel ScenarioTitle;
    private GameObject selectedQuestArt;
    [Tooltip("reference to the selected quest image in this hierarchy")]
    public GuiImage SelectedQuestImage;
    private TKTapRecognizer tapRecognizer;

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

    public override void Close()
    {
        base.Close();
        if (this.isLoadingInProgress)
        {
            Scenario.Current.Initialize();
            Scenario.Current.Play();
        }
    }

    private void CloseSubWindows()
    {
        if (this.difficultyPanel.Visible)
        {
            this.difficultyPanel.Show(false);
        }
    }

    private void DeleteQuestSaveFile()
    {
        if (!GameDirectory.Empty(Constants.SAVE_SLOT_QUEST))
        {
            new GameSaveFile(Constants.SAVE_SLOT_QUEST).Delete();
        }
    }

    private void GenerateQuest()
    {
        if (AdventurePath.Current != null)
        {
            UnityEngine.Object.Destroy(AdventurePath.Current.gameObject);
        }
        AdventurePath.Current = AdventurePathTable.Create("AP1Q_Quest");
        if (Adventure.Current != null)
        {
            UnityEngine.Object.Destroy(Adventure.Current.gameObject);
        }
        Adventure.Current = AdventureTable.Create("AD1Q_Quest");
        if (Scenario.Current != null)
        {
            UnityEngine.Object.Destroy(Scenario.Current.gameObject);
        }
        Scenario.Current = ScenarioTable.Create("SC1Q_Quest");
        (Scenario.Current as QuestScenario).Generate();
    }

    private string GetCardTitle(string ID)
    {
        CardTableEntry entry = CardTable.Get(ID);
        if (entry != null)
        {
            return entry.Name;
        }
        return null;
    }

    private Vector2 GetScenarioScrollerSize()
    {
        float num = 0f;
        num += this.ScenarioLevel.Size.y;
        num += this.ScenarioDifficulty.Size.y;
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

    private void LoadScenarioArt(string ID)
    {
        if (this.selectedQuestArt != null)
        {
            UnityEngine.Object.Destroy(this.selectedQuestArt);
        }
        GameObject prefab = Resources.Load<GameObject>("Art/Scenarios/" + ID);
        if (prefab != null)
        {
            this.selectedQuestArt = Game.Instance.Create(prefab);
            if (this.selectedQuestArt != null)
            {
                this.selectedQuestArt.transform.parent = this.ScenarioArtHolder.transform;
                this.selectedQuestArt.transform.localPosition = Vector3.zero;
            }
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
        if (!UI.Busy && !UI.Window.Paused)
        {
            if (!this.difficultyPanel.Visible)
            {
                this.difficultyPanel.Show(Scenario.Current.ID, Scenario.Current.Difficulty);
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
            QuestMapIcon component = hitd.collider.transform.GetComponent<QuestMapIcon>();
            if (component != null)
            {
                component.Tap();
                this.ShowQuestDetails(component);
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

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.tapRecognizer.enabled = !isPaused;
        this.scenarioScroller.Pause(isPaused);
    }

    private void RandomizeMapIcons(QuestMapIcon[] icons, QuestMapPoint[] points)
    {
        List<QuestMapPoint> list = new List<QuestMapPoint>(points);
        list.Shuffle<QuestMapPoint>();
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].transform.parent.position = list[i].transform.position;
        }
    }

    private void RefreshWindowButtons()
    {
        this.MenuButton.Refresh();
        this.DifficultyButton.Refresh();
    }

    public void SetScenarioDifficulty(int level, bool vfx)
    {
        if (vfx && (Scenario.Current.Difficulty != level))
        {
            this.DifficultyChangeAnimator.SetTrigger("Start");
        }
        Scenario.Current.Difficulty = level;
        this.ScenarioDifficulty.Text = this.difficultyPanel.GetDifficultyName(level);
        this.ScenarioInfo.Text = UI.Text(0x20c) + Environment.NewLine + this.difficultyPanel.GetDifficultyText(null, level);
        this.DifficultyButton.Image = this.difficultyPanel.GetDifficultyImage(level);
        for (int i = 0; i < this.questMapIcons.Length; i++)
        {
            if (this.questMapIcons[i].Difficulty == level)
            {
                this.SelectedQuestImage.transform.position = this.questMapIcons[i].transform.position;
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator ShowCurrentQuest(QuestMapIcon icon) => 
        new <ShowCurrentQuest>c__Iterator8C { 
            icon = icon,
            <$>icon = icon,
            <>f__this = this
        };

    private void ShowQuestDetails(QuestMapIcon icon)
    {
        this.SelectedQuestImage.transform.position = icon.transform.position;
        this.SelectedQuestImage.Show(true);
        this.SetScenarioDifficulty(icon.Difficulty, true);
        this.scenarioScroller.Top();
        this.ScenarioTitle.Text = Scenario.Current.DisplayName;
        this.ScenarioLevel.Text = UI.Text(0x1d0) + " " + Rules.GetTierName(Party.Tier);
        this.ScenarioReward.Text = string.Empty;
        LeanTween.move(this.QuestPanel, this.MarkerQuestPanelShow.transform.position, 0.4f).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.ShowQuestDone));
        if (this.QuestPanel.transform.position != this.MarkerQuestPanelShow.transform.position)
        {
            UI.Sound.Play(this.PanelDropSound);
            LeanTween.move(this.MenuButton.gameObject, this.MarkerMenuButtonRight.transform.position, 0.4f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(new Action(this.RefreshWindowButtons));
        }
        Game.Network.GetScenarioGold(Scenario.Current.ID, Scenario.Current.Difficulty, delegate (int gold) {
            if (this.ScenarioReward != null)
            {
                string str = UI.Text(0x23b);
                if (gold >= 0)
                {
                    str = gold + " " + UI.Text(0x1e7);
                }
                this.ScenarioReward.Text = str;
            }
        });
    }

    private void ShowQuestDone()
    {
        this.scenarioScroller.Reset();
        this.scenarioScroller.Max = new Vector2(0f, 0.8f);
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
        this.LoadScenarioArt("SC1B_Quest");
        this.SelectedQuestImage.Show(false);
        this.DeleteQuestSaveFile();
        this.GenerateQuest();
        this.questMapPoints = base.GetComponentsInChildren<QuestMapPoint>(true);
        this.questMapIcons = base.GetComponentsInChildren<QuestMapIcon>(true);
        this.RandomizeMapIcons(this.questMapIcons, this.questMapPoints);
        if (this.questMapIcons.Length > 0)
        {
            base.StartCoroutine(this.ShowCurrentQuest(this.questMapIcons[0]));
        }
        Tutorial.Notify(TutorialEventType.ScreenQuestShown);
    }

    public override WindowType Type =>
        WindowType.Quest;

    [CompilerGenerated]
    private sealed class <ShowCurrentQuest>c__Iterator8C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal QuestMapIcon <$>icon;
        internal GuiWindowQuest <>f__this;
        internal QuestMapIcon icon;

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
                    this.<>f__this.QuestPanel.transform.position = this.<>f__this.MarkerQuestPanelHide.transform.position;
                    this.$current = new WaitForSeconds(0.8f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.ShowQuestDetails(this.icon);
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

