using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class GuiWindowStore : GuiWindow
{
    private static GuiPanel activePanel;
    private static StorePanelType activePanelType;
    [Tooltip("reference to the adventures panel in this scene")]
    public GuiPanelStoreAdventures adventuresPanel;
    [Tooltip("reference to the characters panel in this scene")]
    public GuiPanelStoreCharacters charactersPanel;
    [Tooltip("reference to the bottom lying chest button in this scene")]
    public GuiButton ChestButton;
    [Tooltip("reference to the bottom lying chest quantity label in this scene")]
    public GuiLabel ChestLabel;
    [Tooltip("color used for available product background tinting")]
    public Color ColorAvailable;
    [Tooltip("color used for buy button text when gold is insufficient")]
    public Color ColorInsufficientGold;
    [Tooltip("color used for owned product background tinting")]
    public Color ColorOwned;
    [Tooltip("color used for buy button text when gold is sufficient")]
    public Color ColorSufficientGold;
    [Tooltip("color used for unavailable product background tinting")]
    public Color ColorUnavailable;
    [Tooltip("reference to the bottom lying gold button in this scene")]
    public GuiButton GoldButton;
    [Tooltip("reference to the bottom lying gold quantity label in this scene")]
    public GuiLabel GoldLabel;
    [Tooltip("reference to the gold panel in this scene")]
    public GuiPanelStoreGold goldPanel;
    private static string initialProduct;
    [Tooltip("reference to the insufficient gold panel in this scene")]
    public GuiPanelStoreInsufficientGold insufficientGoldPanel;
    private bool isClosing;
    [Tooltip("reference to the overlay panel in this scene")]
    public GuiPanelStoreOverlay overlayPanel;
    [Tooltip("reference to the pending purchase panel in this scene")]
    public GuiPanelStorePendingPurchase pendingPurchasePanel;
    [Tooltip("reference to the specials panel in this scene")]
    public GuiPanelStoreSpecials specialsPanel;
    [Tooltip("reference to the start panel in this scene")]
    public GuiPanelStoreStart startPanel;
    [Tooltip("reference to the treasure purchase panel in this scene")]
    public GuiPanelStoreTreasurePurchase treasurePurchasePanel;
    [Tooltip("reference to the treasure reveal panel in this scene")]
    public GuiPanelStoreTreasureReveal treasureRevealPanel;

    public void CloseStore()
    {
        if (!this.isClosing)
        {
            UI.Busy = true;
            this.isClosing = true;
            WindowType exitScene = ExitScene;
            switch (exitScene)
            {
                case WindowType.Adventure:
                    Game.UI.ShowAdventureScene();
                    break;

                case WindowType.Scenario:
                    Game.UI.ShowSetupScene();
                    break;

                case WindowType.CreateParty:
                    Game.UI.ShowCreatePartyScene();
                    break;

                case WindowType.Location:
                    if (!Game.Instance.Reload())
                    {
                        Game.UI.ShowMainMenu();
                    }
                    break;

                default:
                    if (exitScene == WindowType.Collection)
                    {
                        Game.UI.ShowCollectionScene();
                    }
                    else
                    {
                        Game.UI.ShowMainMenu();
                    }
                    break;
            }
            ExitScene = WindowType.None;
        }
    }

    [DebuggerHidden]
    private IEnumerator PanelShowDelayed(GuiPanel panel, float waitTime, bool isVisible) => 
        new <PanelShowDelayed>c__Iterator92 { 
            waitTime = waitTime,
            isVisible = isVisible,
            panel = panel,
            <$>waitTime = waitTime,
            <$>isVisible = isVisible,
            <$>panel = panel
        };

    public override void Refresh()
    {
        if (activePanel != null)
        {
            activePanel.Refresh();
        }
        this.overlayPanel.Refresh();
        this.GoldLabel.Text = string.Empty + Game.Network.CurrentUser.Gold;
        if (this.GoldButton.Visible)
        {
            this.GoldButton.Refresh();
        }
        else
        {
            this.GoldButton.Show(true);
        }
        this.ChestLabel.Text = string.Empty + Game.Network.CurrentUser.Chests;
        if (this.ChestButton.Visible)
        {
            this.ChestButton.Refresh();
        }
        else
        {
            this.ChestButton.Show(true);
        }
        AlertManager.HandleAlerts();
    }

    public void SetActivePanel()
    {
        switch (activePanelType)
        {
            case StorePanelType.Specials:
                activePanel = this.specialsPanel;
                this.specialsPanel.SetInitialProduct(initialProduct);
                break;

            case StorePanelType.Adventures:
                activePanel = this.adventuresPanel;
                this.adventuresPanel.SetInitialProduct(initialProduct);
                break;

            case StorePanelType.Characters:
                activePanel = this.charactersPanel;
                this.charactersPanel.SetInitialProduct(initialProduct);
                break;

            case StorePanelType.Treasure_Buy:
                activePanel = this.treasurePurchasePanel;
                this.treasurePurchasePanel.SetInitialProduct(initialProduct);
                break;

            case StorePanelType.Treasure_Open:
                activePanel = this.treasureRevealPanel;
                break;

            case StorePanelType.Gold:
                activePanel = this.goldPanel;
                this.goldPanel.SetInitialProduct(initialProduct);
                break;
        }
        if ((activePanel == null) && (this.startPanel != null))
        {
            activePanel = this.startPanel;
        }
        if ((activePanel != null) && !activePanel.Visible)
        {
            activePanel.Show(true);
        }
    }

    public void Show(StorePanelType panelType)
    {
        this.SwitchTo(panelType);
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.SetActivePanel();
            this.Refresh();
        }
        else
        {
            UI.Busy = false;
            activePanel = null;
            this.GoldButton.Show(false);
            this.ChestButton.Show(false);
        }
    }

    public void ShowInsufficientGoldPanel()
    {
        this.insufficientGoldPanel.Show(true);
    }

    public static void ShowLicenses(StorePanelType windowType, string product = null)
    {
        activePanelType = windowType;
        initialProduct = (product == null) ? product : product.ToLower();
    }

    public static void ShowLicenses(LicenseType licenseType, string product = null)
    {
        if (!Game.Network.OutOfDate && Game.Network.HasNetworkConnection)
        {
            if ((!Game.Network.Connected && (licenseType != LicenseType.None)) && (licenseType != LicenseType.Special))
            {
                licenseType = LicenseType.Special;
            }
            switch (licenseType)
            {
                case LicenseType.None:
                    ShowLicenses(StorePanelType.Start, null);
                    break;

                case LicenseType.Adventure:
                    ShowLicenses(StorePanelType.Adventures, product);
                    break;

                case LicenseType.Character:
                    ShowLicenses(StorePanelType.Characters, product);
                    break;

                case LicenseType.Gold:
                    ShowLicenses(StorePanelType.Gold, product);
                    break;

                case LicenseType.Special:
                    ShowLicenses(StorePanelType.Specials, product);
                    break;

                case LicenseType.TreasurePurchase:
                    ShowLicenses(StorePanelType.Treasure_Buy, product);
                    break;

                case LicenseType.TreasureOpen:
                    ShowLicenses(StorePanelType.Treasure_Open, null);
                    break;
            }
        }
    }

    public void ShowPendingPanelType(string productName, string totalPrice, int quantity, GuiPanelStorePendingPurchase.PendingType pendingType, GuiPanel panel)
    {
        this.pendingPurchasePanel.SetProduct(productName, totalPrice, quantity, panel);
        this.pendingPurchasePanel.ActivePendingType = pendingType;
        this.ShowPendingPurchasePanel();
    }

    private void ShowPendingPurchasePanel()
    {
        if (this.pendingPurchasePanel.Visible)
        {
            this.pendingPurchasePanel.Refresh();
        }
        else
        {
            this.pendingPurchasePanel.Show(true);
        }
    }

    public void ShowPurchaseDebug(string msg)
    {
        this.pendingPurchasePanel.ActivePendingType = GuiPanelStorePendingPurchase.PendingType.Debug;
        this.pendingPurchasePanel.PromptLabel.Text = msg;
        this.ShowPendingPurchasePanel();
    }

    public void ShowPurchaseFailed()
    {
        this.pendingPurchasePanel.ActivePendingType = GuiPanelStorePendingPurchase.PendingType.Failed;
        this.ShowPendingPurchasePanel();
    }

    public void ShowPurchasePending()
    {
        this.pendingPurchasePanel.ActivePendingType = GuiPanelStorePendingPurchase.PendingType.Purchasing;
        this.ShowPendingPurchasePanel();
    }

    public void ShowPurchaseSuccessful()
    {
        this.pendingPurchasePanel.ActivePendingType = GuiPanelStorePendingPurchase.PendingType.Successful;
        this.ShowPendingPurchasePanel();
    }

    public void ShowPurchaseSuccessfulBlank()
    {
        this.pendingPurchasePanel.ActivePendingType = GuiPanelStorePendingPurchase.PendingType.BlankSuccessful;
        this.ShowPendingPurchasePanel();
    }

    protected override void Start()
    {
        base.Start();
        if (this.overlayPanel != null)
        {
            this.overlayPanel.Initialize();
        }
        if (this.adventuresPanel != null)
        {
            this.adventuresPanel.Initialize();
        }
        if (this.charactersPanel != null)
        {
            this.charactersPanel.Initialize();
        }
        if (this.goldPanel != null)
        {
            this.goldPanel.Initialize();
        }
        if (this.specialsPanel != null)
        {
            this.specialsPanel.Initialize();
        }
        if (this.startPanel != null)
        {
            this.startPanel.Initialize();
        }
        if (this.treasurePurchasePanel != null)
        {
            this.treasurePurchasePanel.Initialize();
        }
        if (this.treasureRevealPanel != null)
        {
            this.treasureRevealPanel.Initialize();
        }
        if (this.pendingPurchasePanel != null)
        {
            this.pendingPurchasePanel.Initialize();
        }
        if (this.insufficientGoldPanel != null)
        {
            this.insufficientGoldPanel.Initialize();
        }
        this.Show(true);
    }

    public void SwitchTo(StorePanelType windowType)
    {
        bool flag = true;
        switch (windowType)
        {
            case StorePanelType.Start:
                if (this.startPanel != null)
                {
                    flag = false;
                    activePanel.Show(false);
                    activePanel = this.startPanel;
                    this.startPanel.Show(true);
                    activePanelType = StorePanelType.Start;
                }
                break;

            case StorePanelType.Specials:
                if (this.specialsPanel != null)
                {
                    flag = false;
                    activePanel.Show(false);
                    activePanel = this.specialsPanel;
                    activePanel.Show(true);
                    activePanelType = StorePanelType.Specials;
                }
                break;

            case StorePanelType.Adventures:
                if (this.adventuresPanel != null)
                {
                    flag = false;
                    activePanel.Show(false);
                    activePanel = this.adventuresPanel;
                    activePanel.Show(true);
                    activePanelType = StorePanelType.Adventures;
                }
                break;

            case StorePanelType.Characters:
                if (this.charactersPanel != null)
                {
                    flag = false;
                    activePanel.Show(false);
                    activePanel = this.charactersPanel;
                    activePanel.Show(true);
                    activePanelType = StorePanelType.Characters;
                }
                break;

            case StorePanelType.Treasure_Buy:
                if (this.treasurePurchasePanel != null)
                {
                    flag = false;
                    activePanel.Show(false);
                    activePanel = this.treasurePurchasePanel;
                    activePanel.Show(true);
                    activePanelType = StorePanelType.Treasure_Buy;
                }
                break;

            case StorePanelType.Treasure_Open:
                if (this.treasureRevealPanel != null)
                {
                    flag = false;
                    activePanel.Show(false);
                    activePanel = this.treasureRevealPanel;
                    activePanel.Show(true);
                    activePanelType = StorePanelType.Treasure_Open;
                }
                break;

            case StorePanelType.Gold:
                if (this.goldPanel != null)
                {
                    flag = false;
                    activePanel.Show(false);
                    activePanel = this.goldPanel;
                    activePanel.Show(true);
                    activePanelType = StorePanelType.Gold;
                }
                break;
        }
        if (flag)
        {
            UI.Busy = false;
        }
        AlertManager.HandleAlerts();
    }

    public StorePanelType ActivePanelType =>
        activePanelType;

    public static WindowType ExitScene
    {
        [CompilerGenerated]
        get => 
            <ExitScene>k__BackingField;
        [CompilerGenerated]
        set
        {
            <ExitScene>k__BackingField = value;
        }
    }

    public override WindowType Type =>
        WindowType.Store;

    [CompilerGenerated]
    private sealed class <PanelShowDelayed>c__Iterator92 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal bool <$>isVisible;
        internal GuiPanel <$>panel;
        internal float <$>waitTime;
        internal bool isVisible;
        internal GuiPanel panel;
        internal float waitTime;

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
                    this.$current = new WaitForSeconds(this.waitTime);
                    this.$PC = 1;
                    return true;

                case 1:
                    if (this.isVisible)
                    {
                        Game.UI.ShowLoadScreen(false);
                    }
                    this.panel.Show(this.isVisible);
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

    public enum StorePanelType
    {
        Start,
        Specials,
        Adventures,
        Characters,
        Treasure_Buy,
        Treasure_Open,
        Gold
    }
}

