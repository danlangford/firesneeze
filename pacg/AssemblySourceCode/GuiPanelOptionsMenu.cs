using System;
using UnityEngine;

public class GuiPanelOptionsMenu : GuiPanelBackStack
{
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the debug button on this panel")]
    public GuiButton DebugButton;
    [Tooltip("reference to the debug history button on this panel")]
    public GuiButton DebugHistoryButton;
    [Tooltip("reference to the debug panel in our hierarchy")]
    public GuiPanelDebug DebugPanel;
    [Tooltip("reference to the forfeit button on this panel")]
    public GuiButton ForfeitButton;
    [Tooltip("reference to the forfeit panel")]
    public GuiPanelForfeit ForfeitPanel;
    [Tooltip("reference to the gold button on this panel")]
    public GuiButton GoldButton;
    [Tooltip("reference to the gold quantity label on this panel")]
    public GuiLabel GoldLabel;
    [Tooltip("reference to the quit button on this panel")]
    public GuiButton QuitButton;
    [Tooltip("reference to the rules button on this panel")]
    public GuiButton RulesButton;
    [Tooltip("reference to the rules panel")]
    public GuiPanelRules RulesPanel;
    [Tooltip("reference to the settings button on this panel")]
    public GuiButton SettingsButton;
    [Tooltip("reference to the settings panel")]
    public GuiPanelSettings SettingsPanel;
    [Tooltip("reference to the treasure button on this panel")]
    public GuiButton TreasureButton;
    [Tooltip("reference to the treasure quantity label on this panel")]
    public GuiLabel TreasureLabel;
    [Tooltip("reference to the vault button on this panel")]
    public GuiButton VaultButton;

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    private void OnDebugButtonPushed()
    {
        if (!UI.Busy)
        {
            this.DebugPanel.Show(true);
            this.DebugButton.Glow(true);
            this.DebugHistoryButton.gameObject.SetActive(true);
        }
    }

    private void OnDebugHistoryButtonPushed()
    {
        if (this.DebugPanel.Visible)
        {
            this.DebugPanel.ShowPreviousCommand();
        }
    }

    private void OnForfeitButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
            UI.Window.Pause(true);
            this.ForfeitPanel.Show(true);
        }
    }

    private void OnGoldButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.Show(false);
            Game.UI.ShowStoreWindow(LicenseType.Gold);
        }
    }

    private void OnQuitButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            Game.Restart();
        }
    }

    public void OnRulesSheetButtonPushed()
    {
        this.Show(false);
        this.RulesPanel.Initialize();
        UI.Window.Pause(true);
        this.RulesPanel.Show(true);
    }

    private void OnSettingsButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
            UI.Window.Pause(true);
            this.SettingsPanel.Show(true);
        }
    }

    private void OnTreasureChestButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.Show(false);
            Game.UI.ShowStoreWindow(LicenseType.TreasureOpen);
        }
    }

    private void OnVaultButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.Show(false);
            Game.UI.ShowCollectionScene();
        }
    }

    public override void Rebind()
    {
        this.QuitButton.Rebind();
        this.CloseButton.Rebind();
        this.DebugButton.Rebind();
        this.DebugHistoryButton.Rebind();
        this.RulesButton.Rebind();
        this.SettingsButton.Rebind();
        this.VaultButton.Rebind();
        this.GoldButton.Rebind();
        this.TreasureButton.Rebind();
        this.ForfeitButton.Rebind();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (base.Owner != null)
        {
            base.Owner.Pause(isVisible);
        }
        else
        {
            UI.Window.Pause(isVisible);
        }
        this.DebugButton.Glow(false);
        this.DebugPanel.Show(false);
        this.DebugHistoryButton.gameObject.SetActive(false);
        this.GoldLabel.Text = string.Empty + Game.Network.CurrentUser.Gold;
        this.TreasureLabel.Text = string.Empty + Game.Network.CurrentUser.Chests;
        if (!isVisible)
        {
            base.Owner = null;
        }
        if (!Settings.DebugMode)
        {
            this.DebugButton.Show(false);
            this.DebugHistoryButton.Show(false);
            this.DebugPanel.Show(false);
        }
        if (isVisible)
        {
            bool flag = true;
            if (((UI.Window.Type == WindowType.Reward) || (UI.Window.Type == WindowType.SelectCards)) || (((UI.Window.Type == WindowType.Collection) || (UI.Window.Type == WindowType.Cutscene)) || Tutorial.Running))
            {
                flag = false;
            }
            this.VaultButton.Disable(!flag);
            this.GoldButton.Disable(!flag);
            this.TreasureButton.Disable(!flag);
        }
        if (isVisible)
        {
            bool flag2 = UI.Window.Type == WindowType.Location;
            this.ForfeitButton.Disable(!flag2);
        }
    }

    public override bool Fullscreen =>
        true;
}

