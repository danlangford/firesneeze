using System;
using UnityEngine;

public class GuiPanelCollectionEmpty : GuiPanel
{
    [Tooltip("reference to the buy button on this panel")]
    public GuiButton BuyButton;
    private string currentDeck;
    [Tooltip("instructions for when the deck is not owned")]
    public StrRefType MessageBuy;
    [Tooltip("instructions for when deck C is not owned")]
    public StrRefType MessageBuyC;
    [Tooltip("instructions for when deck P is not owned")]
    public StrRefType MessageBuyP;
    [Tooltip("instructions for when there are no cards")]
    public StrRefType MessageEmpty;
    [Tooltip("instructions for when the deck is not in play (vault)")]
    public StrRefType MessageUnavailable;
    [Tooltip("instructions for when the deck is not supported (because it's not for sale yet)")]
    public StrRefType MessageUnsupported;
    [Tooltip("reference to the text label on this panel")]
    public GuiLabel TextLabel;

    private string GetBuyMessage(string deck)
    {
        if (deck == "C")
        {
            return this.MessageBuyC.ToString();
        }
        if (deck == "P")
        {
            return this.MessageBuyP.ToString();
        }
        return this.MessageBuy.ToString();
    }

    private LicenseType GetDeckLicenseType(string deck)
    {
        if (deck == "C")
        {
            return LicenseType.Character;
        }
        if (deck == "P")
        {
            return LicenseType.Special;
        }
        return LicenseType.Adventure;
    }

    private string GetDeckTitle(string deck)
    {
        for (int i = 0; i < AdventureTable.Count; i++)
        {
            AdventureTableEntry entry = AdventureTable.Get(i);
            if (entry.set == deck)
            {
                return entry.Name;
            }
        }
        return ("Deck " + deck);
    }

    private string GetMessageText(string deck, bool deckLoaded)
    {
        string licenseIdentifierForDeck = LicenseManager.GetLicenseIdentifierForDeck(deck);
        if (licenseIdentifierForDeck != null)
        {
            if (!LicenseManager.GetIsSupported(licenseIdentifierForDeck))
            {
                this.BuyButton.Show(false);
                return string.Format(this.MessageUnsupported.ToString(), this.GetDeckTitle(deck));
            }
            if (!LicenseManager.GetIsLicensed(licenseIdentifierForDeck))
            {
                this.BuyButton.Show(true);
                return string.Format(this.GetBuyMessage(deck), this.GetDeckTitle(deck));
            }
        }
        this.BuyButton.Show(false);
        if (!deckLoaded)
        {
            return this.MessageUnavailable.ToString();
        }
        return this.MessageEmpty.ToString();
    }

    public override void Initialize()
    {
        this.Show(false);
    }

    private void OnStoreButtonPushed()
    {
        if ((!UI.Busy && !base.Paused) && !Settings.Debug.DemoMode)
        {
            string licenseIdentifierForDeck = LicenseManager.GetLicenseIdentifierForDeck(this.currentDeck);
            UI.Window.SendMessage("CloseSubWindows");
            LicenseType deckLicenseType = this.GetDeckLicenseType(this.currentDeck);
            Game.UI.ShowStoreWindow(licenseIdentifierForDeck, deckLicenseType);
        }
    }

    public void Show(string deck, bool deckLoaded)
    {
        this.currentDeck = deck;
        this.TextLabel.Text = this.GetMessageText(deck, deckLoaded);
        this.Show(true);
    }
}

