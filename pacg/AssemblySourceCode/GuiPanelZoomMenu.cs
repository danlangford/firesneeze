using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelZoomMenu : GuiPanel
{
    [Tooltip("reference to the banish button in our hierarchy")]
    public GuiButton zoomBanishButton;
    [Tooltip("reference to the bury button in our hierarchy")]
    public GuiButton zoomBuryButton;
    [Tooltip("reference to the discard button in our hierarchy")]
    public GuiButton zoomDiscardButton;
    [Tooltip("reference to the display button in our hierarchy")]
    public GuiButton zoomDisplayButton;
    [Tooltip("shift the zoom menu right by this world position on low res")]
    public float zoomMenuOffset;
    [Tooltip("reference to the recharge button in our hierarchy")]
    public GuiButton zoomRechargeButton;
    [Tooltip("reference to the reveal button in our hierarchy")]
    public GuiButton zoomRevealButton;
    [Tooltip("reference to the top button in our hierarchy")]
    public GuiButton zoomTopButton;

    private void HideZoomMenu()
    {
        this.zoomDisplayButton.Fade(false, 0.15f);
        this.zoomRevealButton.Fade(false, 0.15f);
        this.zoomRechargeButton.Fade(false, 0.15f);
        this.zoomDiscardButton.Fade(false, 0.15f);
        this.zoomBuryButton.Fade(false, 0.15f);
        this.zoomBanishButton.Fade(false, 0.15f);
        this.zoomTopButton.Fade(false, 0.15f);
        this.Card.Animate(AnimationType.Focus, false);
    }

    public override void Initialize()
    {
        if (Device.GetScreenProfile() == DeviceScreenType.TabletLow)
        {
            Transform transform = this.zoomDiscardButton.transform;
            transform.position += new Vector3(this.zoomMenuOffset, 0f, 0f);
            Transform transform2 = this.zoomDisplayButton.transform;
            transform2.position += new Vector3(this.zoomMenuOffset, 0f, 0f);
            Transform transform3 = this.zoomRechargeButton.transform;
            transform3.position += new Vector3(this.zoomMenuOffset, 0f, 0f);
            Transform transform4 = this.zoomRevealButton.transform;
            transform4.position += new Vector3(this.zoomMenuOffset, 0f, 0f);
            Transform transform5 = this.zoomTopButton.transform;
            transform5.position += new Vector3(this.zoomMenuOffset, 0f, 0f);
            Transform transform6 = this.zoomBuryButton.transform;
            transform6.position += new Vector3(this.zoomMenuOffset, 0f, 0f);
            Transform transform7 = this.zoomBanishButton.transform;
            transform7.position += new Vector3(this.zoomMenuOffset, 0f, 0f);
        }
    }

    private void OnZoomBanishButtonPressed()
    {
        this.ZoomButtonPressedDropCard(ActionType.Banish);
    }

    private void OnZoomBuryButtonPressed()
    {
        this.ZoomButtonPressedDropCard(ActionType.Bury);
    }

    private void OnZoomDiscardButtonPressed()
    {
        this.ZoomButtonPressedDropCard(ActionType.Discard);
    }

    private void OnZoomDisplayButtonPressed()
    {
        this.ZoomButtonPressedDropCard(ActionType.Reveal);
    }

    private void OnZoomRechargeButtonPressed()
    {
        this.ZoomButtonPressedDropCard(ActionType.Recharge);
    }

    private void OnZoomRevealButtonPressed()
    {
        this.ZoomButtonPressedDropCard(ActionType.Reveal);
    }

    private void OnZoomTopButtonPressed()
    {
        this.ZoomButtonPressedDropCard(ActionType.Recharge);
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            this.ShowZoomMenu();
        }
        else
        {
            this.HideZoomMenu();
        }
    }

    private void ShowZoomMenu()
    {
        UI.Busy = false;
        bool flag = false;
        if (this.Card != null)
        {
            flag = (this.Card.Deck != null) && (this.Card.Deck == Turn.Character.Hand);
            this.Card.Animate(AnimationType.Focus, true);
        }
        if (flag && Turn.IsActionAllowed(ActionType.Display, this.Card))
        {
            this.zoomDisplayButton.gameObject.SetActive(true);
            this.zoomDisplayButton.Fade(true, 0.15f);
        }
        if (flag && Turn.IsActionAllowed(ActionType.Reveal, this.Card))
        {
            this.zoomRevealButton.gameObject.SetActive(true);
            this.zoomRevealButton.Fade(true, 0.15f);
        }
        if (flag && Turn.IsActionAllowed(ActionType.Recharge, this.Card))
        {
            this.zoomRechargeButton.gameObject.SetActive(true);
            this.zoomRechargeButton.Fade(true, 0.15f);
        }
        if (flag && Turn.IsActionAllowed(ActionType.Discard, this.Card))
        {
            this.zoomDiscardButton.gameObject.SetActive(true);
            this.zoomDiscardButton.Fade(true, 0.15f);
        }
        if (flag && Turn.IsActionAllowed(ActionType.Bury, this.Card))
        {
            this.zoomBuryButton.gameObject.SetActive(true);
            this.zoomBuryButton.Fade(true, 0.15f);
        }
        if (flag && Turn.IsActionAllowed(ActionType.Banish, this.Card))
        {
            this.zoomBanishButton.gameObject.SetActive(true);
            this.zoomBanishButton.Fade(true, 0.15f);
        }
        if (flag && Turn.IsActionAllowed(ActionType.Top, this.Card))
        {
            this.zoomTopButton.gameObject.SetActive(true);
            this.zoomTopButton.Fade(true, 0.15f);
        }
    }

    private void ZoomButtonPressedDropCard(ActionType action)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card card = this.Card;
            window.UnZoomCard();
            GuiLayout layoutDeck = window.GetLayoutDeck(action);
            window.DropCardOnLayout(card, layoutDeck);
        }
    }

    public Card Card { get; set; }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;
}

