using System;
using UnityEngine;

public class GuiPanelStoreInsufficientGold : GuiPanelBackStack
{
    private readonly string confirmationPrompt = "You do not have enough gold for this purchase. Would you like to purchase more gold?";
    [Tooltip("referecnce to the close button in this panel")]
    public GuiButton CornerCloseButton;
    [Tooltip("reference to the cancel button in this panel")]
    public GuiButton NoButton;
    [Tooltip("Reference to the prompt label in this panel")]
    public GuiLabel PromptLabel;
    [Tooltip("reference to the store manager window in the scene")]
    public GuiWindowStore StoreManager;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the title label in this panel")]
    public GuiLabel TitleLabel;
    [Tooltip("reference to the confirm button in this panel")]
    public GuiButton YesButton;

    public override void Initialize()
    {
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 3;
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.Show(false);
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    private void OnNoButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    private void OnYesButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.Show(false);
            this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Gold);
            this.StoreManager.overlayPanel.ButtonGlow(GuiWindowStore.StorePanelType.Gold);
        }
    }

    public override void Refresh()
    {
        this.PromptLabel.Text = this.confirmationPrompt;
        this.NoButton.Show(true);
        this.YesButton.Show(true);
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.tapRecognizer.enabled = isVisible;
        if (isVisible)
        {
            this.Refresh();
        }
        else
        {
            this.Clear();
            UI.Busy = false;
        }
    }

    public override bool Fullscreen =>
        true;
}

