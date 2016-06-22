using System;
using UnityEngine;

public class GuiPanelSalvage : GuiPanelBackStack
{
    [Tooltip("label on this panel that stores the salvage amount")]
    public GuiLabel AmountLabel;
    private Card myCard;
    [Tooltip("button on this panel that means: do not salvage")]
    public GuiButton NoButton;
    [Tooltip("button on this panel that means: salvage the card")]
    public GuiButton YesButton;

    public override void Initialize()
    {
        this.Show(false);
    }

    private void OnCloseButtonPushed()
    {
        this.OnNoButtonPushed();
    }

    private void OnNoButtonPushed()
    {
        this.Show(false);
        this.UnZoomCard();
        this.myCard = null;
    }

    private void OnYesButtonPushed()
    {
        this.Show(false);
        if (this.Salvage())
        {
            base.transform.parent.SendMessage("DestroyZoomCard");
        }
        else
        {
            this.UnZoomCard();
        }
        this.myCard = null;
    }

    private bool Salvage() => 
        ((this.myCard != null) && Collection.Remove(this.myCard.ID));

    public void Show(Card card)
    {
        if (card != null)
        {
            this.myCard = card;
            object[] objArray1 = new object[] { "+ ", this.myCard.Cost, " ", UI.Text(0x1e7) };
            this.AmountLabel.Text = string.Concat(objArray1);
            base.Show(true);
        }
    }

    private void UnZoomCard()
    {
        base.transform.parent.SendMessage("UnZoomCard");
    }

    public override bool Fullscreen =>
        true;
}

