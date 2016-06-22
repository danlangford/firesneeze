using System;
using UnityEngine;

public class GuiWindowCreateParty : GuiWindow
{
    [Tooltip("reference to the avatar animator in this scene")]
    public Animator AvatarAnimator;
    [Tooltip("reference to the cancel button contained in the scene")]
    public GuiButton CancelButton;
    [Tooltip("reference to the character description label in the scene")]
    public GuiLabel CharacterDescriptionLabel;
    [Tooltip("reference to the character name label in the scene")]
    public GuiLabel CharacterNameLabel;
    [Tooltip("reference to the character traits label in the scene")]
    public GuiLabel CharacterTraitsLabel;
    private int currentPane;
    private CharacterToken currentToken;
    private CharacterToken draggedToken;
    private Vector2 dragLocation;
    private TKPanRecognizer dragRecognizer;
    private Vector2 dragStartLocation;
    [Tooltip("reference to the XP bar in our hierarchy")]
    public GuiPanelExperienceBar ExperienceBar;
    private FingerGesture finger;
    private bool isDragStarted;
    [Tooltip("reference to biography sub-panel in our hierarchy")]
    public GuiPanelCharacterBio Pane0;
    [Tooltip("reference to skills sub-panel 1 in our hierarchy")]
    public GuiPanelCharacterSkills Pane1;
    [Tooltip("reference to powers sub-panel 2 in our hierarchy")]
    public GuiPanelCharacterPowers Pane2;
    [Tooltip("reference to cards sub-panel 3 in our hierarchy")]
    public GuiPanelCharacterCards Pane3;
    [Tooltip("reference to completion sub-panel 4 in our hierachy")]
    public GuiPanelCharacterComplete Pane4;
    [Tooltip("reference to quests sub-panel 5 in our hierarchy")]
    public GuiPanelCharacterQuests Pane5;
    [Tooltip("reference to the texture used to block unavailable slots in this scene")]
    public GameObject PartySlotBlocker;
    [Tooltip("reference to the party slot layout in this scene")]
    public CharacterTokenSlotLayout PartySlotLayout;
    [Tooltip("references to the party drop slots contained in the scene (ordered)")]
    public CharacterTokenSlot[] PartySlots;
    [Tooltip("reference to the \"pass and play\" toggle button")]
    public GuiButton PassAndPlayButton;
    [Tooltip("reference to the \"pass and play\" tooltip in the scene")]
    public GuiPanelTooltip PassAndPlayTooltip;
    [Tooltip("reference to the \"perma death\" toggle button")]
    public GuiButton PermaDeathButton;
    [Tooltip("reference to the \"perma death\" tooltip in the scene")]
    public GuiPanelTooltip PermaDeathTooltip;
    [Tooltip("reference to the \"yes/no\" popup in this scene")]
    public GuiPanelMenuAsk Popup;
    [Tooltip("reference to the proceed button contained in the scene")]
    public GuiButton ProceedButton;
    [Tooltip("sound played when a character is selected via the tokens")]
    public AudioClip SwitchCharacterSound;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the token slot layout in the scene")]
    public CharacterTokenSlotLayoutLine TokenLayout;
    [Tooltip("references to the token slots containined in the scene (ordered)")]
    public CharacterTokenSlot[] TokenSlots;
    [Tooltip("reference to the vault character panel in the scene")]
    public GuiPanelVaultCharacters VaultPanel;

    private void AssignPartyToSlots()
    {
        int index = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Alive)
            {
                CharacterToken token = this.VaultPanel.FindToken(Party.Characters[i]);
                if ((token != null) && (index < this.PartySlots.Length))
                {
                    token.Show(true);
                    bool showShadow = false;
                    for (int j = 0; j < this.TokenSlots.Length; j++)
                    {
                        if (this.TokenSlots[j].Token == token)
                        {
                            showShadow = true;
                        }
                    }
                    token.OnGuiSlot(this.PartySlots[index], showShadow);
                    index++;
                }
            }
        }
        this.UpdateButtonStates();
        this.PartySlotLayout.Refresh();
    }

    private void CancelButtonPushed()
    {
        Game.Restart();
    }

    private void ConfigureGestureRecognizers()
    {
        if (!this.TokenLayout.Scrolling)
        {
            this.TokenLayout.Pause(true);
            this.finger.Lock(FingerGesture.FingerState.Drag);
        }
        else
        {
            this.TokenLayout.Pause(false);
            this.finger.Locked = false;
            this.finger.Reset();
        }
    }

    private void CreatePartyFromSlots()
    {
        Party.Clear();
        for (int i = 0; i < this.PartySlots.Length; i++)
        {
            if (this.PartySlots[i].Token != null)
            {
                Party.Add(this.PartySlots[i].Token.Character);
            }
        }
    }

    private void DisplayTokenDetails(CharacterToken token)
    {
        if ((this.currentToken != token) && (token != null))
        {
            if (this.currentToken != null)
            {
                this.currentToken.Select(false);
            }
            token.Select(true);
            this.VaultPanel.HighlightToken(token);
            if (this.currentToken != null)
            {
                this.AvatarAnimator.SetTrigger("SwitchCharacter");
                UI.Sound.Play(this.SwitchCharacterSound);
            }
            Character character = token.Character;
            if (character != null)
            {
                this.CharacterNameLabel.Text = character.DisplayName;
                this.CharacterDescriptionLabel.Text = character.DisplayText;
                string[] textArray1 = new string[] { character.Gender.ToText(), " \x00b7 ", character.Race.ToText(), " \x00b7 ", character.Class.ToText() };
                this.CharacterTraitsLabel.Text = string.Concat(textArray1);
                this.ExperienceBar.Show(token);
                this.Pane0.Character = character;
                this.Pane1.Character = character;
                this.Pane2.Character = character;
                this.Pane3.Character = character;
                this.Pane4.Character = character;
                this.Pane5.Character = character;
                if (this.currentPane == 0)
                {
                    this.Pane0.Refresh();
                }
                if (this.currentPane == 1)
                {
                    this.Pane1.Refresh();
                }
                this.Pane2.Show(this.currentPane == 2);
                this.Pane2.Refresh();
                if (this.currentPane == 3)
                {
                    this.Pane3.Refresh();
                }
                if (this.currentPane == 4)
                {
                    this.Pane4.Refresh();
                }
                if (this.currentPane == 5)
                {
                    this.Pane5.Refresh();
                }
            }
            this.currentToken = token;
        }
    }

    private int GetTokenIndex(CharacterToken token)
    {
        for (int i = 0; i < this.TokenSlots.Length; i++)
        {
            if ((this.TokenSlots[i].Owner != null) && (this.TokenSlots[i].Owner.ID == token.ID))
            {
                return i;
            }
        }
        return -1;
    }

    private bool IsDropPossible(CharacterToken token, CharacterTokenSlot slot)
    {
        if ((token == null) || (slot == null))
        {
            return false;
        }
        for (int i = 0; i < this.PartySlots.Length; i++)
        {
            if ((this.PartySlots[i].Token != null) && (this.PartySlots[i].Token.ID == token.ID))
            {
                return false;
            }
        }
        return slot.OnDrop(token);
    }

    private void OnCancelButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.CancelButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.CancelButtonPushed));
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
            RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT);
            if (hitd != 0)
            {
                component = hitd.collider.transform.GetComponent<CharacterTokenSlot>();
                if ((component != null) && !this.IsDropPossible(this.draggedToken, component))
                {
                    component = null;
                }
            }
            this.draggedToken.OnGuiDrop(component);
            this.PartySlotLayout.Refresh();
            if ((component != null) && !(component is CharacterTokenSlotPartyMember))
            {
                Vault.Lock(this.draggedToken.Character.NickName, false);
            }
            this.draggedToken = null;
            this.UpdateButtonStates();
        }
        this.finger.Reset();
    }

    private void OnGuiDragStart(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD);
        if (hitd != 0)
        {
            CharacterToken component = hitd.collider.transform.GetComponent<CharacterToken>();
            if (component != null)
            {
                this.DisplayTokenDetails(component);
                component.OnGuiDrag();
                this.draggedToken = component;
            }
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD);
        if (hitd != 0)
        {
            CharacterToken component = hitd.collider.transform.GetComponent<CharacterToken>();
            this.DisplayTokenDetails(component);
        }
    }

    private void OnLicenseYesButtonPushed()
    {
        this.Popup.Show(false);
        this.CreatePartyFromSlots();
        this.currentToken.Buy();
    }

    private void OnMenuButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.Pause(true);
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnNextCharacterButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            int tokenIndex = this.GetTokenIndex(this.currentToken);
            if (tokenIndex >= 0)
            {
                if ((++tokenIndex >= this.TokenSlots.Length) || (this.TokenSlots[tokenIndex].Owner == null))
                {
                    tokenIndex = 0;
                }
                this.DisplayTokenDetails(this.TokenSlots[tokenIndex].Owner);
            }
        }
    }

    private void OnPane0ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.currentPane = 0;
            this.Pane0.Show(true);
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(false);
            this.Pane4.Show(false);
            this.Pane5.Show(false);
        }
    }

    private void OnPane1ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.currentPane = 1;
            this.Pane0.Show(false);
            this.Pane1.Show(true);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(false);
            this.Pane4.Show(false);
            this.Pane5.Show(false);
        }
    }

    private void OnPane2ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.currentPane = 2;
            this.Pane0.Show(false);
            this.Pane1.Show(false);
            this.Pane2.Show(true);
            this.Pane2.RolePanel.gameObject.SetActive(true);
            this.Pane3.Show(false);
            this.Pane4.Show(false);
            this.Pane5.Show(false);
        }
    }

    private void OnPane3ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.currentPane = 3;
            this.Pane0.Show(false);
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(true);
            this.Pane4.Show(false);
            this.Pane5.Show(false);
        }
    }

    private void OnPane4ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.Pane0.Show(false);
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(false);
            if (!Rules.IsQuestRewardAllowed())
            {
                this.Pane4.Show(true);
                this.currentPane = 4;
            }
            else
            {
                this.Pane5.Show(true);
                this.currentPane = 5;
            }
        }
    }

    private void OnPassAndPlayButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            if (Game.GameType == GameType.LocalSinglePlayer)
            {
                Game.GameType = GameType.LocalMultiPlayer;
            }
            else if (Game.GameType == GameType.LocalMultiPlayer)
            {
                Game.GameType = GameType.LocalSinglePlayer;
            }
            this.PassAndPlayButton.Glow(Game.GameType == GameType.LocalMultiPlayer);
            this.PassAndPlayTooltip.Show(true);
        }
    }

    private void OnPermaDeathButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            Campaign.PermaDeath = !Campaign.PermaDeath;
            this.PermaDeathButton.Glow(Campaign.PermaDeath);
            this.PermaDeathTooltip.Show(true);
        }
    }

    private void OnPreviousCharacterButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            int tokenIndex = this.GetTokenIndex(this.currentToken);
            if (tokenIndex >= 0)
            {
                if (--tokenIndex < 0)
                {
                    for (int i = this.TokenSlots.Length - 1; i >= 0; i--)
                    {
                        if (this.TokenSlots[i].Owner != null)
                        {
                            tokenIndex = i;
                            break;
                        }
                    }
                }
                this.DisplayTokenDetails(this.TokenSlots[tokenIndex].Owner);
            }
        }
    }

    private void OnProceedButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.ProceedButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.ProceedButtonPushed));
        }
    }

    private void ProceedButtonPushed()
    {
        this.CreatePartyFromSlots();
        Campaign.Audit();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Vault.Add(Party.Characters[i].NickName, Party.Characters[i]);
            Vault.Lock(Party.Characters[i].NickName, true);
            Vault.Mode(Party.Characters[i].NickName, Game.GameMode);
        }
        for (int j = 0; j <= 1; j++)
        {
            for (int k = 0; k < this.TokenSlots.Length; k++)
            {
                if (((this.TokenSlots[k].Token != null) && (this.TokenSlots[k].Token.Character != null)) && !Party.Contains(this.TokenSlots[k].Token.Character))
                {
                    UnityEngine.Object.Destroy(this.TokenSlots[k].Token.Character.gameObject);
                }
            }
            this.VaultPanel.Toggle();
        }
        if (Campaign.Distributions.Count > 0)
        {
            for (int m = 0; m < Party.Characters.Count; m++)
            {
                if (Party.Characters[m].Deck.Count <= 0)
                {
                    Party.Characters[m].BuildDeck();
                }
            }
            Game.UI.ShowSelectCardScene();
        }
        else
        {
            if (Game.GameMode == GameModeType.Story)
            {
                Game.UI.ShowAdventureScene();
            }
            if (Game.GameMode == GameModeType.Quest)
            {
                Game.UI.ShowQuestScene();
            }
        }
    }

    public override void Refresh()
    {
        this.VaultPanel.Refresh();
        if (this.TokenSlots[0].Owner != null)
        {
            this.DisplayTokenDetails(this.TokenSlots[0].Owner);
        }
        else
        {
            this.VaultPanel.Toggle();
            this.DisplayTokenDetails(this.TokenSlots[0].Owner);
        }
        this.ConfigureGestureRecognizers();
    }

    public void ShowLicensePopup()
    {
        this.Popup.YesButtonCallback = "OnLicenseYesButtonPushed";
        this.Popup.MessageLabel.Text = this.currentToken.LicenseText;
        this.Popup.YesButton.Text = StringTableManager.GetUIText(0x1c6);
        this.Popup.Show(true);
    }

    protected override void Start()
    {
        base.Start();
        this.ProceedButton.Show(false);
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 0;
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!UI.Busy && !UI.Window.Paused)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.dragRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.dragRecognizer.zIndex = 1;
        this.dragRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy && !UI.Window.Paused)
            {
                if (!this.isDragStarted)
                {
                    this.dragStartLocation = this.dragRecognizer.touchLocation();
                    this.isDragStarted = true;
                }
                this.finger.Calculate(this.dragRecognizer.touchLocation());
                if (this.finger.State == FingerGesture.FingerState.Drag)
                {
                    this.dragLocation = this.dragRecognizer.touchLocation();
                    if (this.draggedToken == null)
                    {
                        this.OnGuiDragStart(this.dragStartLocation);
                    }
                    else
                    {
                        this.OnGuiDrag(this.dragRecognizer.deltaTranslation);
                    }
                }
            }
        };
        this.dragRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy && !UI.Window.Paused)
            {
                if (this.finger.State == FingerGesture.FingerState.Drag)
                {
                    this.OnGuiDragEnd(this.dragLocation);
                }
                this.isDragStarted = false;
            }
        };
        TouchKit.addGestureRecognizer(this.dragRecognizer);
        this.finger = new FingerGesture();
        this.finger.gestureRecognizedEvent += delegate (FingerGesture r) {
            if (!UI.Busy && !UI.Zoomed)
            {
                if (this.finger.State == FingerGesture.FingerState.Slide)
                {
                    this.TokenLayout.Pause(false);
                }
                if (this.finger.State == FingerGesture.FingerState.Drag)
                {
                    this.TokenLayout.Pause(true);
                }
            }
        };
        this.TokenLayout.Finger = this.finger;
        Vault.Audit();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Vault.Lock(Party.Characters[i].NickName, false);
        }
        this.TokenLayout.Initialize(this.TokenSlots);
        this.VaultPanel.Initialize();
        this.ExperienceBar.Initialize();
        this.Pane0.Initialize();
        this.Pane0.Show(false);
        this.Pane1.Initialize();
        this.Pane1.Show(false);
        this.Pane2.Initialize();
        this.Pane2.Show(false);
        this.Pane3.Initialize();
        this.Pane3.Show(false);
        this.Pane4.Initialize();
        this.Pane4.Show(false);
        this.Pane5.Initialize();
        this.Pane5.Show(false);
        if (Campaign.Deaths.Count > 0)
        {
            this.CancelButton.Show(false);
        }
        if (AdventurePath.Current != null)
        {
            this.CancelButton.Show(false);
        }
        this.PermaDeathButton.Glow(Campaign.PermaDeath);
        this.PassAndPlayButton.Glow(Game.GameType == GameType.LocalMultiPlayer);
        this.PassAndPlayButton.Disable(true);
        this.Refresh();
        if (Party.Characters.Count > 0)
        {
            this.AssignPartyToSlots();
        }
        Party.Rest();
        this.OnPane1ButtonPushed();
        Tutorial.Notify(TutorialEventType.ScreenPartySelectionShown);
    }

    private void UpdateButtonStates()
    {
        int num = 0;
        for (int i = 0; i < this.PartySlots.Length; i++)
        {
            if (this.PartySlots[i].Token != null)
            {
                num++;
            }
        }
        if (num > 0)
        {
            if (!this.ProceedButton.Visible)
            {
                this.ProceedButton.Fade(true, 0.25f);
            }
        }
        else if (this.ProceedButton.Visible)
        {
            this.ProceedButton.Fade(false, 0.25f);
        }
        if (num > 1)
        {
            this.PassAndPlayButton.Disable(false);
        }
        else
        {
            this.PassAndPlayButton.Disable(true);
        }
        this.ExperienceBar.Refresh();
    }

    public CharacterToken SelectedToken =>
        this.currentToken;

    public override WindowType Type =>
        WindowType.CreateParty;
}

