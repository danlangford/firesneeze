using System;
using UnityEngine;

public class GuiWindowMainMenu : GuiWindow
{
    [Tooltip("reference to the account label in our hierarchy")]
    public GuiLabel accountLabel;
    [Tooltip("reference to the create account panel in this scene")]
    public GuiPanelCreateAccount createAccountPanel;
    [Tooltip("reference to the credits button in our hierarchy")]
    public GuiButton creditsButton;
    [Tooltip("reference to the debug button in our hierarchy")]
    public GuiButton debugButton;
    [Tooltip("reference to the exit panel in this scene")]
    public GuiPanelExit exitPanel;
    private bool externalCall;
    [Tooltip("reference to the facebook button in our hierarchy")]
    public GuiButton facebookButton;
    [Tooltip("references to the fancy fire shader art in this scene")]
    public GameObject[] fireVfx;
    [Tooltip("reference to the gallery button in our hierarchy")]
    public GuiButton galleryButton;
    [Tooltip("reference to the \"load saved game\" panel in this scene")]
    public GuiPanelLoad loadPanel;
    [Tooltip("reference to the login button in our hierarchy")]
    public GuiButtonRegion loginButton;
    [Tooltip("reference to the login with existing account panel in this scene")]
    public GuiPanelLogin loginPanel;
    [Tooltip("reference to the multiplayer button")]
    public GuiButton multiplayerButton;
    [Tooltip("reference to the multplayer button label in our hierachy")]
    public GuiLabel multiplayerLabel;
    [Tooltip("reference to the multiplayer tooltip string")]
    public StrRefType MultiplayerTooltip;
    [Tooltip("reference to the network panel in this scene")]
    public GuiPanelMenuNetwork networkPanel;
    [Tooltip("reference to the new game button in our hierarchy")]
    public GuiButton newGameButton;
    [Tooltip("reference to the normal sprite for most buttons to use")]
    public Sprite normalButtonSprite;
    [Tooltip("reference to the not logged in panel in this scene")]
    public GuiPanelNotLoggedIn notLoggedInPanel;
    [Tooltip("reference to the out of date screen panel in this scene")]
    public GuiPanelOutOfDate outOfDatePanel;
    [Tooltip("reference to the promotional panel in this scene")]
    public GuiPanelPromotion promoPanel;
    [Tooltip("reference to the quest button in our hierarchy")]
    public GuiButton questButton;
    [Tooltip("reference to the quest tooltip string")]
    public StrRefType QuestTooltip;
    [Tooltip("reference to the rules button in our hierarchy")]
    public GuiButton rulesButton;
    [Tooltip("reference to the login status panel in this scene")]
    public GuiPanelLoginStatus statusPanel;
    [Tooltip("reference to the store button in our hierarchy")]
    public GuiButton storeButton;
    [Tooltip("reference to the store connection tooltip string")]
    public StrRefType StoreConnectionTooltip;
    [Tooltip("reference to the story button in our hierarchy")]
    public GuiButton storyButton;
    [Tooltip("reference to the story tooltip string")]
    public StrRefType StoryTooltip;
    [Tooltip("reference to the tutorial button in our hierarchy")]
    public GuiButton tutorialButton;

    public void ExternalClick(ButtonType bt)
    {
        this.externalCall = true;
        switch (bt)
        {
            case ButtonType.Play:
                this.OnPlayGameButtonClick();
                break;

            case ButtonType.Story:
                this.OnStoryButtonClick();
                break;

            case ButtonType.Quest:
                this.OnQuestButtonClick();
                break;

            case ButtonType.Multiplayer:
                this.OnMultiplayerButtonClick();
                break;

            case ButtonType.Store:
                this.OnStoreButtonClick();
                break;

            case ButtonType.Tutorial:
                this.OnTutorialButtonClick();
                break;
        }
        this.externalCall = false;
    }

    private void OnCreditsButtonClick()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            Game.UI.ShowCreditsScene();
        }
    }

    private void OnDebugButtonClick()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            Game.GameMode = GameModeType.Story;
            Campaign.Start("1B");
            Settings.Debug.SetupGame();
            if (!Settings.Debug.Play())
            {
                Game.Play(Settings.Debug.GameType, Constants.SAVE_SLOT_DEBUG, WindowType.Location, Scenario.Current.FirstLocation, false);
                Scenario.Current.Initialize();
            }
        }
    }

    private void OnGalleryButtonClick()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            Game.UI.ShowCollectionScene();
        }
    }

    private void OnLoginButtonClick()
    {
        if (!UI.Busy)
        {
            if (Game.Network.HasNetworkConnection)
            {
                Game.Network.Login();
            }
            else
            {
                Game.UI.Toast.Show(this.StoreConnectionTooltip.ToString());
            }
        }
    }

    private void OnMultiplayerButtonClick()
    {
        if (!UI.Busy)
        {
            Game.UI.Toast.Text = this.MultiplayerTooltip.ToString();
            Game.UI.Toast.Show(true);
        }
    }

    private void OnPlayGameButtonClick()
    {
        if (!UI.Busy)
        {
            if (!this.externalCall && (!Game.Network.Connected || !Game.Network.HasNetworkConnection))
            {
                this.notLoggedInPanel.CallbackButtonType = ButtonType.Play;
                this.notLoggedInPanel.Show(true);
            }
            else if ((Settings.ActiveSaveSlot > 0) && !GameDirectory.Empty(Settings.ActiveSaveSlot))
            {
                Game.Load(GameDirectory.ActiveSlot);
            }
            else if (Conquests.IsComplete(Constants.STORY_MODE_UNLOCKED))
            {
                Game.GameMode = GameModeType.Story;
                this.Show(false);
                this.loadPanel.Show(true);
                this.networkPanel.Show(false);
            }
            else
            {
                Game.GameMode = GameModeType.Story;
                Tutorial.Run(Constants.SAVE_SLOT_TUTORIAL);
            }
        }
    }

    private void OnQuestButtonClick()
    {
        if (!UI.Busy)
        {
            if (!Conquests.IsComplete(Constants.QUEST_MODE_UNLOCKED))
            {
                Game.UI.Toast.Show(this.QuestTooltip.ToString());
            }
            else if (!this.externalCall && (!Game.Network.Connected || !Game.Network.HasNetworkConnection))
            {
                this.notLoggedInPanel.CallbackButtonType = ButtonType.Quest;
                this.notLoggedInPanel.Show(true);
            }
            else
            {
                UI.Busy = true;
                if (GameDirectory.Empty(Constants.SAVE_SLOT_QUEST))
                {
                    Game.GameMode = GameModeType.Quest;
                    Game.Play(GameType.LocalSinglePlayer, Constants.SAVE_SLOT_QUEST, WindowType.Quest, null, false);
                }
                else if (!Game.Load(Constants.SAVE_SLOT_QUEST))
                {
                    GameDirectory.Clear(Constants.SAVE_SLOT_QUEST);
                    Game.Restart();
                }
            }
        }
    }

    private void OnRulesButtonClick()
    {
        if (!UI.Busy)
        {
            GuiPanelOptionsMenu optionsPanel = Game.UI.OptionsPanel as GuiPanelOptionsMenu;
            if (optionsPanel != null)
            {
                optionsPanel.RulesPanel.Initialize();
                UI.Window.Pause(true);
                optionsPanel.RulesPanel.Show(true);
            }
        }
    }

    private void OnSettingsButtonClick()
    {
        if (!UI.Busy)
        {
            GuiPanelOptionsMenu optionsPanel = Game.UI.OptionsPanel as GuiPanelOptionsMenu;
            if (optionsPanel != null)
            {
                UI.Window.Pause(true);
                optionsPanel.SettingsPanel.Show(true);
            }
        }
    }

    private void OnStoreButtonClick()
    {
        if (!UI.Busy)
        {
            if (!Game.Network.HasNetworkConnection)
            {
                Game.UI.Toast.Show(this.StoreConnectionTooltip.ToString());
            }
            else
            {
                UI.Busy = true;
                if (Game.Network.Connected)
                {
                    Game.UI.ShowStoreWindow();
                }
                else
                {
                    Game.UI.ShowStoreWindow(LicenseType.Special);
                }
            }
        }
    }

    private void OnStoryButtonClick()
    {
        if (!UI.Busy)
        {
            if (!Conquests.IsComplete(Constants.STORY_MODE_UNLOCKED))
            {
                Game.UI.Toast.Show(this.StoryTooltip.ToString());
            }
            else if (!this.externalCall && (!Game.Network.Connected || !Game.Network.HasNetworkConnection))
            {
                this.notLoggedInPanel.CallbackButtonType = ButtonType.Story;
                this.notLoggedInPanel.Show(true);
            }
            else
            {
                Game.GameMode = GameModeType.Story;
                this.Show(false);
                this.loadPanel.Show(true);
                this.networkPanel.Show(false);
            }
        }
    }

    private void OnTutorialButtonClick()
    {
        if (!UI.Busy)
        {
            if (!this.externalCall && (!Game.Network.Connected || !Game.Network.HasNetworkConnection))
            {
                this.notLoggedInPanel.CallbackButtonType = ButtonType.Tutorial;
                this.notLoggedInPanel.Show(true);
            }
            else
            {
                Game.GameMode = GameModeType.Story;
                Tutorial.Run(Constants.SAVE_SLOT_TUTORIAL);
            }
        }
    }

    public override void Refresh()
    {
        if (Game.Network.Connected)
        {
            PlayFabLoginCalls.GetAccountInfo(null);
        }
        this.networkPanel.Show(true);
        this.loginButton.Show(!Game.Network.Connected);
        bool flag = Game.Network.HasNetworkConnection && !Game.Network.OutOfDate;
        this.storeButton.Disable(!flag);
        if ((Settings.ActiveSaveSlot > 0) || Conquests.IsComplete(Constants.STORY_MODE_UNLOCKED))
        {
            this.newGameButton.Text = UI.Text(0x153);
        }
        else
        {
            this.newGameButton.Text = UI.Text(0x17b);
        }
        if (!Conquests.IsComplete(Constants.STORY_MODE_UNLOCKED))
        {
            this.storyButton.Image = this.storyButton.ImageDisabled;
        }
        else
        {
            this.storyButton.Image = this.normalButtonSprite;
        }
        if (!Conquests.IsComplete(Constants.QUEST_MODE_UNLOCKED))
        {
            this.questButton.Image = this.questButton.ImageDisabled;
        }
        else
        {
            this.questButton.Image = this.normalButtonSprite;
        }
        for (int i = 0; i < this.fireVfx.Length; i++)
        {
            this.fireVfx[i].SetActive(Settings.GraphicsLevel > 0);
        }
        AlertManager.HandleAlerts();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.networkPanel.Show(true);
        AlertManager.HandleAlerts();
    }

    public static void ShowFacebookButton(bool isVisible)
    {
        GuiWindow current = GuiWindow.Current;
        if (current != null)
        {
            GuiWindowMainMenu menu = current as GuiWindowMainMenu;
            if (menu != null)
            {
                menu.facebookButton.Show(isVisible);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        this.loadPanel.Initialize();
        Game.UI.OptionsPanel.Show(false);
        this.loginPanel.Show(false);
        this.statusPanel.Show(false);
        this.createAccountPanel.Show(false);
        this.notLoggedInPanel.Show(false);
        this.outOfDatePanel.Show(Game.Network.OutOfDate);
        this.loginButton.Show(!Game.Network.Connected);
        this.storeButton.Disable(!Game.Network.HasNetworkConnection || Game.Network.OutOfDate);
        this.networkPanel.Show(true);
        this.Refresh();
        if (!Settings.DebugMode)
        {
            this.debugButton.Show(false);
        }
        if (Settings.Debug.DemoMode)
        {
            this.debugButton.Show(false);
            this.storeButton.Show(false);
            this.questButton.Show(false);
        }
        Tutorial.Notify(TutorialEventType.ScreenMainMenuShown);
    }

    private void Update()
    {
        if (Settings.DebugMode)
        {
            if (Game.Network.HasNetworkConnection)
            {
                if (this.storeButton.Disabled)
                {
                    this.storeButton.Disable(false);
                }
            }
            else if (!this.storeButton.Disabled)
            {
                this.storeButton.Disable(true);
            }
        }
        else if (!Game.Network.HasNetworkConnection && !this.storeButton.Disabled)
        {
            this.storeButton.Disable(true);
        }
        else if (Game.Network.HasNetworkConnection && this.storeButton.Disabled)
        {
            this.storeButton.Disable(false);
        }
        if (!GuiPanel.IsFullScreenPanelShowing() && Device.GetIsBackButtonPushed())
        {
            this.exitPanel.Show(true);
        }
        if (Game.Network.Connected)
        {
            if ((this.loginButton != null) && this.loginButton.Visible)
            {
                this.loginButton.Show(false);
            }
        }
        else if ((this.loginButton != null) && this.loginButton.Visible)
        {
            this.loginButton.Show(true);
        }
    }

    public override WindowType Type =>
        WindowType.MainMenu;

    public enum ButtonType
    {
        Play,
        Story,
        Quest,
        Multiplayer,
        Store,
        Tutorial
    }
}

