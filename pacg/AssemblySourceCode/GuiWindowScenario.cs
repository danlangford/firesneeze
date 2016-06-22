using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiWindowScenario : GuiWindow
{
    [Tooltip("reference to the character sheet panel in this scene")]
    public GuiPanelCharacter characterPanel;
    private CharacterToken draggedToken;
    private Vector2 dragLocation;
    private TKPanRecognizer dragRecognizer;
    [Tooltip("reference to the favored card panel in this scene")]
    public GuiPanelChooseFavoredCard FavoredCardPanel;
    [Tooltip("reference to the finish button contained in this hierarchy")]
    public GuiButton FinishButton;
    [Tooltip("reference to the help label contained in this hierarchy")]
    public GuiLabel HelpLabel;
    [Tooltip("reference to the map icon layout grid")]
    public ScenarioMapIconLayout LayoutIcons;
    [Tooltip("reference to the start location token layout along the right")]
    public CharacterTokenLayout LayoutStartLocation;
    [Tooltip("reference to the turn order token layout along the bottom")]
    public CharacterTokenLayout LayoutTurnOrder;
    [Tooltip("reference to the map location hilite in this scene")]
    public GameObject LocationHilite;
    [Tooltip("reference to the location panel in this scene")]
    public GuiPanelLocation LocationPanel;
    [Tooltip("reference to the map panel in this scene")]
    public GuiPanelMap MapPanel;
    [Tooltip("reference to the map title label in this scene")]
    public GuiLabel MapTitle;
    [Tooltip("reference to the menu button contained in this hierarchy")]
    public GuiButton MenuButton;
    private List<CharacterToken> partyTokens;
    [Tooltip("reference to the scenario preview panel in this scene")]
    public GuiPanelScenarioPreview ScenarioPanel;
    private int screenNumber = 1;
    [Tooltip("reference to the party character slots along the top")]
    public CharacterTokenSlot[] SlotsParty;
    [Tooltip("reference to the turn order slots in this scene")]
    public CharacterTokenSlot[] SlotsTurnOrder;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the background frame for turn order selection")]
    public GameObject TurnOrderBackground;
    [Tooltip("reference to the container for the view character and deck buttons")]
    public GameObject ViewButtons;
    [Tooltip("reference to the button that shows the character sheet")]
    public GuiButton ViewCharacterButton;
    [Tooltip("reference to the button that starts card selection")]
    public GuiButton ViewCharacterDeckButton;
    [Tooltip("reference to a screen overlay used in the tutorial")]
    public GameObject ViewCharacterOverlay;

    private void AssignPartySlots(CharacterTokenSlot[] slots, bool owner)
    {
        if (this.partyTokens == null)
        {
            this.CreatePartyTokens();
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < this.partyTokens.Count)
            {
                if (owner)
                {
                    slots[i].Owner = this.partyTokens[i];
                }
                slots[i].Token = this.partyTokens[i];
                this.partyTokens[i].Slot = slots[i];
                this.partyTokens[i].Home = slots[i];
            }
            else if (slots[i] != null)
            {
                UnityEngine.Object.Destroy(slots[i].gameObject);
                slots[i] = null;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.dragRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.dragRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy)
            {
                this.dragLocation = this.dragRecognizer.touchLocation();
                if (this.draggedToken == null)
                {
                    this.OnGuiDragStart(this.dragLocation);
                }
                else
                {
                    this.OnGuiDrag(this.dragRecognizer.deltaTranslation);
                }
            }
        };
        this.dragRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiDragEnd(this.dragLocation);
            }
        };
        TouchKit.addGestureRecognizer(this.dragRecognizer);
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.characterPanel.Initialize();
        this.ClearPartyLocations();
        this.TurnOrderBackground.SetActive(false);
    }

    private void ClearPartyLocations()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].Location = null;
        }
    }

    private void CreatePartyTokens()
    {
        this.partyTokens = new List<CharacterToken>(Constants.MAX_PARTY_MEMBERS);
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            CharacterToken item = CharacterToken.Create(Party.Characters[i].ID);
            if (item != null)
            {
                this.partyTokens.Add(item);
            }
        }
    }

    private ScenarioMapIcon FindMapIcon(string ID)
    {
        ScenarioMapIcon[] iconArray = UnityEngine.Object.FindObjectsOfType<ScenarioMapIcon>();
        for (int i = 0; i < iconArray.Length; i++)
        {
            if (iconArray[i].ID == ID)
            {
                return iconArray[i];
            }
        }
        return null;
    }

    public void FinishButtonPushed()
    {
        UI.Busy = false;
        if (this.screenNumber <= 1)
        {
            this.HideScenarioPreviewScreen();
            this.screenNumber = (Party.Find("CH1B_Lem") == null) ? 3 : 2;
            if (this.screenNumber == 3)
            {
                this.HideFavoredCardScreen();
            }
        }
        else if (this.screenNumber == 2)
        {
            this.HideFavoredCardScreen();
            this.screenNumber = 3;
        }
        else if (this.screenNumber == 3)
        {
            this.HideStartLocationScreen();
            this.screenNumber = (Party.Characters.Count < 2) ? 5 : 4;
        }
        else if (this.screenNumber == 4)
        {
            this.HideTurnOrderScreen();
            this.screenNumber = 5;
        }
        this.ScreenControllerUpdate();
    }

    private void HideFavoredCardScreen()
    {
        this.ShowPartyTokens();
        this.FinishButton.Show(true);
        this.FavoredCardPanel.Show(false);
    }

    private void HideMapIcons()
    {
        ScenarioMapIcon[] iconArray = UnityEngine.Object.FindObjectsOfType<ScenarioMapIcon>();
        for (int i = 0; i < iconArray.Length; i++)
        {
            iconArray[i].gameObject.SetActive(false);
        }
    }

    private void HidePartyTokens()
    {
        for (int i = 0; i < this.partyTokens.Count; i++)
        {
            this.partyTokens[i].gameObject.SetActive(false);
        }
    }

    private void HideScenarioPreviewScreen()
    {
        this.FinishButton.Show(true);
        this.CreatePartyTokens();
        this.HideMapIcons();
        this.MapPanel.ChangeBackgroundColor(Color.gray, 0f);
        this.MapPanel.Pause(true);
    }

    private void HideStartLocationScreen()
    {
        this.MapTitle.Show(false);
        this.LocationPanel.Show(false);
        this.LocationHilite.SetActive(false);
        this.LayoutStartLocation.Layout();
        for (int i = 0; i < this.LayoutStartLocation.Tokens.Count; i++)
        {
            this.LayoutStartLocation.Tokens[i].Reset();
        }
        this.ShowPartySlots(this.SlotsParty, this.LayoutStartLocation, false);
        this.LayoutIcons.Show(false);
        for (int j = 0; j < this.MapPanel.Links.Count; j++)
        {
            this.MapPanel.Links[j].Show(false);
        }
    }

    private void HideTurnOrderScreen()
    {
        this.TurnOrderBackground.SetActive(false);
        this.ShowPartySlots(this.SlotsTurnOrder, this.LayoutTurnOrder, false);
    }

    private bool IsLocationSelectionAllowed()
    {
        if (!string.IsNullOrEmpty(Scenario.Current.StartLocation))
        {
            return false;
        }
        if (Tutorial.Running)
        {
            return false;
        }
        return true;
    }

    private void LockPartyTokens(bool isLocked)
    {
        for (int i = 0; i < this.partyTokens.Count; i++)
        {
            this.partyTokens[i].Locked = isLocked;
        }
    }

    private void OnCardsButtonPushed()
    {
        if (Tutorial.Running)
        {
            Tutorial.Notify(TutorialEventType.ScenarioSetupCardButton);
        }
        else if (!UI.Busy && !UI.Window.Paused)
        {
            Game.UI.ShowSelectCardScene();
        }
    }

    private void OnCharacterSheetButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Window.Pause(true);
            UI.Window.Show(false);
            this.characterPanel.Show(true);
        }
    }

    public void OnFinishButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.FinishButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.FinishButtonPushed));
        }
    }

    private void OnGuiDrag(Vector2 deltaTranslation)
    {
        if (this.draggedToken != null)
        {
            Vector3 screenPoint = (Vector3) (base.WorldToScreenPoint(this.draggedToken.transform.position) + deltaTranslation);
            this.draggedToken.transform.position = (Vector3) base.ScreenToWorldPoint(screenPoint);
        }
    }

    private void OnGuiDragEnd(Vector2 touchPos)
    {
        if (this.draggedToken != null)
        {
            CharacterTokenSlot component = null;
            RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT | Constants.LAYER_MASK_MAP);
            if (hitd != 0)
            {
                component = hitd.collider.transform.GetComponent<CharacterTokenSlot>();
                if ((component != null) && !component.OnDrop(this.draggedToken))
                {
                    component = null;
                }
            }
            this.draggedToken.OnGuiDrop(component);
            this.draggedToken = null;
        }
    }

    private void OnGuiDragStart(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD);
        if (hitd != 0)
        {
            CharacterToken component = hitd.collider.transform.GetComponent<CharacterToken>();
            if (component != null)
            {
                component.OnGuiDrag();
                this.draggedToken = component;
            }
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (this.screenNumber == 3)
        {
            RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_MAP);
            if (hitd != 0)
            {
                ScenarioMapIcon component = hitd.collider.transform.GetComponent<ScenarioMapIcon>();
                if (component != null)
                {
                    UI.Sound.Play(SoundEffectType.GenericClick);
                    this.LocationPanel.Show(component.ID);
                }
            }
        }
    }

    private void OnLocationChanged(string ID)
    {
        ScenarioMapIcon icon = this.FindMapIcon(ID);
        if (icon != null)
        {
            this.LocationHilite.transform.parent = icon.transform;
            this.LocationHilite.transform.localPosition = Vector3.zero;
            this.LocationHilite.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            this.LocationHilite.SetActive(true);
            for (int i = 0; i < this.MapPanel.Links.Count; i++)
            {
                this.MapPanel.Links[i].Glow(false);
            }
            for (int j = 0; j < this.MapPanel.Links.Count; j++)
            {
                if (this.MapPanel.Links[j].Match(icon))
                {
                    this.MapPanel.Links[j].Glow(true);
                }
            }
        }
    }

    private void OnMenuButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnTokenDropComplete()
    {
        if (this.screenNumber == 3)
        {
            bool flag = true;
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (string.IsNullOrEmpty(Party.Characters[i].Location))
                {
                    flag = false;
                }
            }
            if (flag && !this.FinishButton.Visible)
            {
                this.FinishButton.Fade(true, 0.25f);
            }
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.tapRecognizer.enabled = !isPaused;
        this.dragRecognizer.enabled = !isPaused;
    }

    private void ScreenControllerUpdate()
    {
        if (this.screenNumber <= 1)
        {
            this.ShowScenarioPreviewScreen();
        }
        if (this.screenNumber == 2)
        {
            this.ShowFavoredCardScreen();
        }
        if (this.screenNumber == 3)
        {
            this.ShowStartLocationScreen();
        }
        if (this.screenNumber == 4)
        {
            this.ShowTurnOrderScreen();
        }
        if (this.screenNumber >= 5)
        {
            this.ShowMapScreen();
        }
    }

    private void SetupPartyHands()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].BuildHand();
        }
    }

    [DebuggerHidden]
    private IEnumerator SetupStartLocation() => 
        new <SetupStartLocation>c__Iterator90 { <>f__this = this };

    private void ShowFavoredCardScreen()
    {
        this.HidePartyTokens();
        this.FinishButton.Show(false);
        this.HelpLabel.Text = StringTableManager.GetHelperText(0x31);
        this.FavoredCardPanel.Show(Party.Find("CH1B_Lem"));
    }

    private void ShowLoadScreen()
    {
        this.SetupPartyHands();
        StartScreen = 0;
        this.screenNumber = 0;
        Turn.Number = 0;
        Turn.Switch = 0;
        Turn.Current = 0;
        Game.UI.ShowLocationScene(Party.Characters[0].Location, false);
    }

    private void ShowMapScreen()
    {
        this.HidePartyTokens();
        this.MapPanel.Show(false);
        this.ViewButtons.SetActive(false);
        this.FinishButton.Show(false);
        this.MenuButton.Show(false);
        UI.Window.Pause(true);
        Tutorial.Hide();
        this.HelpLabel.Clear();
        GameObject original = (GameObject) Resources.Load("Art/MapIntros/" + Scenario.Current.Intro, typeof(GameObject));
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
            if (obj3 != null)
            {
                obj3.name = original.name;
                Transform transform = obj3.transform.FindChild("MapHolder");
                if (transform != null)
                {
                    this.MapPanel.transform.parent = transform;
                    this.MapPanel.transform.localPosition = Vector3.zero;
                    this.MapPanel.Reset();
                    this.MapPanel.ChangeBackgroundColor(Color.white, 0f);
                    this.MapPanel.Show(true);
                    this.MapPanel.Pause(true);
                    LeanTween.delayedCall(0.3f, () => this.MapPanel.Reset());
                }
            }
        }
        else
        {
            this.ShowLoadScreen();
        }
    }

    private void ShowPartySlots(CharacterTokenSlot[] slots, CharacterTokenLayout layout, bool isVisible)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].gameObject.SetActive(isVisible);
                if (isVisible)
                {
                    slots[i].transform.position = layout.GetTokenPosition(i);
                }
            }
        }
    }

    private void ShowPartyTokens()
    {
        for (int i = 0; i < this.partyTokens.Count; i++)
        {
            this.partyTokens[i].gameObject.SetActive(true);
        }
    }

    private void ShowScenarioPreviewScreen()
    {
        this.MapTitle.Show(false);
        this.FinishButton.Show(false);
        this.MapPanel.Show(true);
        this.MapPanel.Pause(true);
        this.ScenarioPanel.Show(true);
    }

    private void ShowStartLocationScreen()
    {
        StartScreen = 3;
        this.AssignPartySlots(this.SlotsParty, true);
        Tutorial.Hide();
        this.HelpLabel.Clear();
        if (this.IsLocationSelectionAllowed())
        {
            this.HelpLabel.Text = StringTableManager.GetHelperText(50);
        }
        this.HideTurnOrderScreen();
        this.LayoutStartLocation.Tokens = this.SortPartyTokens();
        this.LayoutStartLocation.Refresh();
        this.ShowPartySlots(this.SlotsParty, this.LayoutStartLocation, true);
        this.MapTitle.Show(true);
        this.MapPanel.Show(true);
        this.MapPanel.Pause(true);
        this.MapPanel.ChangeBackgroundColor(Color.gray, 0f);
        this.LayoutIcons.Show(true);
        this.FinishButton.Show(false);
        this.LocationPanel.Show(Scenario.Current.FirstLocation);
        this.ViewButtons.SetActive(true);
        base.StartCoroutine(this.SetupStartLocation());
        Tutorial.Notify(TutorialEventType.ScenarioSetupLocation);
    }

    private void ShowTurnOrderScreen()
    {
        this.AssignPartySlots(this.SlotsTurnOrder, false);
        this.MapTitle.Show(false);
        this.MapPanel.ChangeBackgroundColor(Color.gray, 0f);
        this.ViewButtons.SetActive(true);
        Tutorial.Notify(TutorialEventType.ScenarioSetupTurnOrder);
        this.HelpLabel.Clear();
        this.LayoutTurnOrder.Initialize(this.partyTokens);
        this.ShowPartySlots(this.SlotsTurnOrder, this.LayoutTurnOrder, true);
        this.TurnOrderBackground.SetActive(true);
        this.LayoutTurnOrder.Refresh();
        this.FinishButton.Show(true);
        this.HideMapIcons();
    }

    private List<CharacterToken> SortPartyTokens()
    {
        if (this.partyTokens == null)
        {
            this.CreatePartyTokens();
        }
        List<CharacterToken> list = new List<CharacterToken>(Constants.MAX_PARTY_MEMBERS);
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = 0; j < this.partyTokens.Count; j++)
            {
                if (this.partyTokens[j].ID == Party.Characters[i].ID)
                {
                    list.Add(this.partyTokens[j]);
                    break;
                }
            }
        }
        return list;
    }

    protected override void Start()
    {
        base.Start();
        this.ViewButtons.SetActive(false);
        this.ScenarioPanel.Initialize();
        this.MapPanel.Initialize();
        this.MapPanel.Show(true);
        this.MapPanel.CenterAllIcons();
        this.MapPanel.Pause(true);
        this.FinishButton.Show(false);
        base.StartCoroutine(this.StartAfterDelay(UI.LoadScreenAnimationLength));
    }

    [DebuggerHidden]
    private IEnumerator StartAfterDelay(float delay) => 
        new <StartAfterDelay>c__Iterator8F { 
            delay = delay,
            <$>delay = delay,
            <>f__this = this
        };

    public static int StartScreen
    {
        [CompilerGenerated]
        get => 
            <StartScreen>k__BackingField;
        [CompilerGenerated]
        set
        {
            <StartScreen>k__BackingField = value;
        }
    }

    public override WindowType Type =>
        WindowType.Scenario;

    [CompilerGenerated]
    private sealed class <SetupStartLocation>c__Iterator90 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowScenario <>f__this;
        internal int <i>__2;
        internal int <i>__3;
        internal string <ID>__0;
        internal CharacterTokenSlotPartyLocation <slot>__1;

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
                    this.<ID>__0 = Scenario.Current.StartLocation;
                    if (this.<ID>__0 == null)
                    {
                        goto Label_019F;
                    }
                    this.<>f__this.LockPartyTokens(true);
                    this.$current = new WaitForSeconds(0.35f);
                    this.$PC = 1;
                    goto Label_01A8;

                case 1:
                    this.<slot>__1 = null;
                    this.<i>__2 = 0;
                    while (this.<i>__2 < this.<>f__this.MapPanel.Icons.Count)
                    {
                        if (this.<>f__this.MapPanel.Icons[this.<i>__2].ID == this.<ID>__0)
                        {
                            this.<slot>__1 = this.<>f__this.MapPanel.Icons[this.<i>__2].GetComponent<CharacterTokenSlotPartyLocation>();
                            break;
                        }
                        this.<i>__2++;
                    }
                    break;

                case 2:
                    goto Label_0176;

                default:
                    goto Label_01A6;
            }
            if (this.<slot>__1 != null)
            {
                this.<i>__3 = 0;
                while (this.<i>__3 < this.<>f__this.partyTokens.Count)
                {
                    this.<>f__this.LockPartyTokens(false);
                    this.<slot>__1.OnDrop(this.<>f__this.partyTokens[this.<i>__3]);
                    this.<>f__this.LockPartyTokens(true);
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 2;
                    goto Label_01A8;
                Label_0176:
                    this.<i>__3++;
                }
            }
        Label_019F:
            this.$PC = -1;
        Label_01A6:
            return false;
        Label_01A8:
            return true;
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
    private sealed class <StartAfterDelay>c__Iterator8F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>delay;
        internal GuiWindowScenario <>f__this;
        internal float delay;

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
                    this.$current = new WaitForSeconds(this.delay);
                    this.$PC = 1;
                    return true;

                case 1:
                    if (GuiWindowScenario.StartScreen > 0)
                    {
                        this.<>f__this.screenNumber = GuiWindowScenario.StartScreen;
                    }
                    this.<>f__this.ScreenControllerUpdate();
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

