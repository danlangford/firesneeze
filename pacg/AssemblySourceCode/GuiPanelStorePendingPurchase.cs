using System;
using UnityEngine;

public class GuiPanelStorePendingPurchase : GuiPanelBackStack
{
    private PendingType activePendingType;
    [Tooltip("reference to the cancel button in this panel")]
    public GuiButton CancelButton;
    [Tooltip("referecnce to the close button in this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the confirm button in this panel")]
    public GuiButton ConfirmButton;
    public GuiButton CornerCloseButton;
    private readonly string f_blankSuccessfulPrompt = "Congratulations! Your purchase was successful!";
    private readonly string f_confirmationBundlePrompt = "Do you wish to purchase {0}{1} for {2}? Some content may already have been purchased seperately.";
    private readonly string f_confirmationPrompt = "Do you wish to purchase {0}{1} for {2} gold?";
    private readonly string f_failedPrompt = "Sorry, the attempted purchase of {0}{1} did not go through!";
    private readonly string f_pendingPrompt = "Please wait while we attempt to purchase {0}{1}...";
    private readonly string f_successfulPrompt = "Congratulations! You just purchased {0}{1}!";
    private string productName;
    private GuiPanel productPanel;
    private int productQuantity;
    private string productTotalPrice;
    [Tooltip("Reference to the prompt label in this panel")]
    public GuiLabel PromptLabel;
    [Tooltip("reference to the store manager window in the scene")]
    public GuiWindowStore StoreManager;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the title label in this panel")]
    public GuiLabel TitleLabel;

    public override void Clear()
    {
        this.productName = string.Empty;
        this.productTotalPrice = string.Empty + 0;
        this.productQuantity = 1;
    }

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

    private void OnConfirmButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.StoreManager.ShowPurchasePending();
            if (this.productPanel is GuiPanelStoreAdventures)
            {
                this.StoreManager.adventuresPanel.ProceedWithPurchase();
            }
            else if (this.productPanel is GuiPanelStoreCharacters)
            {
                this.StoreManager.charactersPanel.ProceedWithPurchase();
            }
            else if (this.productPanel is GuiPanelStoreTreasurePurchase)
            {
                this.StoreManager.treasurePurchasePanel.ProceedWithPurchase();
            }
            else if (this.productPanel is GuiPanelStoreSpecials)
            {
                this.StoreManager.specialsPanel.ProceedWithPurchase();
            }
        }
    }

    public override void Refresh()
    {
        if (this.ActivePendingType != PendingType.Purchasing)
        {
            UI.Busy = false;
        }
        switch (this.ActivePendingType)
        {
            case PendingType.Confirmation:
                this.PromptLabel.Text = string.Format(this.f_confirmationPrompt, (this.productQuantity != 1) ? (this.productQuantity + " ") : string.Empty, this.productName, this.productTotalPrice);
                this.CancelButton.Show(true);
                this.ConfirmButton.Show(true);
                this.CloseButton.Show(false);
                break;

            case PendingType.ConfirmationBundle:
                this.PromptLabel.Text = string.Format(this.f_confirmationBundlePrompt, string.Empty, this.productName, this.productTotalPrice);
                this.CancelButton.Show(true);
                this.ConfirmButton.Show(true);
                this.CloseButton.Show(false);
                break;

            case PendingType.Purchasing:
                this.PromptLabel.Text = string.Format(this.f_pendingPrompt, (this.productQuantity != 1) ? (this.productQuantity + " ") : string.Empty, this.productName);
                this.CancelButton.Show(false);
                this.ConfirmButton.Show(false);
                this.CloseButton.Show(false);
                break;

            case PendingType.Successful:
                this.PromptLabel.Text = string.Format(this.f_successfulPrompt, (this.productQuantity != 1) ? (this.productQuantity + " ") : string.Empty, this.productName);
                this.CancelButton.Show(false);
                this.ConfirmButton.Show(false);
                this.CloseButton.Show(true);
                break;

            case PendingType.BlankSuccessful:
                this.PromptLabel.Text = string.Format(this.f_blankSuccessfulPrompt, new object[0]);
                this.CancelButton.Show(false);
                this.ConfirmButton.Show(false);
                this.CloseButton.Show(true);
                break;

            case PendingType.Failed:
                this.PromptLabel.Text = string.Format(this.f_failedPrompt, (this.productQuantity != 1) ? (this.productQuantity + " ") : string.Empty, this.productName);
                this.CancelButton.Show(false);
                this.ConfirmButton.Show(false);
                this.CloseButton.Show(true);
                break;

            case PendingType.Debug:
                this.CancelButton.Show(false);
                this.ConfirmButton.Show(false);
                this.CloseButton.Show(true);
                break;
        }
    }

    public void SetProduct(string ProductName, string TotalPrice, int ProductQuantity, GuiPanel panel)
    {
        this.productName = ProductName;
        this.productTotalPrice = TotalPrice;
        this.productQuantity = ProductQuantity;
        this.PromptLabel.Text = string.Format(this.f_confirmationPrompt, (this.productQuantity != 1) ? (this.productQuantity + " ") : string.Empty, this.productName, this.productTotalPrice);
        this.productPanel = panel;
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
            this.CloseButton.Show(false);
            this.ConfirmButton.Show(false);
            this.CancelButton.Show(false);
            this.Clear();
            UI.Busy = false;
        }
    }

    public PendingType ActivePendingType
    {
        get => 
            this.activePendingType;
        set
        {
            this.activePendingType = value;
        }
    }

    public override bool Fullscreen =>
        true;

    public enum PendingType
    {
        Confirmation,
        ConfirmationBundle,
        Purchasing,
        Successful,
        BlankSuccessful,
        Failed,
        Debug
    }
}

