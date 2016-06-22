using System;
using System.Runtime.CompilerServices;

public class CardPlayedInfo
{
    private Card _PlayedPowerOwner;
    public int PlayedPower = -1;
    public Guid PlayedPowerOwnerGuid = Guid.Empty;

    private Card FindCard(Guid guid)
    {
        Card card = null;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (card == null)
            {
                card = window.layoutDiscard.Deck[guid];
            }
            if (card == null)
            {
                card = window.layoutBanish.Deck[guid];
            }
            if (card == null)
            {
                card = window.layoutBury.Deck[guid];
            }
            if (card == null)
            {
                card = window.layoutRecharge.Deck[guid];
            }
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (card == null)
                {
                    card = Party.Characters[i].Discard[guid];
                }
                if (card == null)
                {
                    card = Party.Characters[i].Recharge[guid];
                }
                if (card == null)
                {
                    card = Party.Characters[i].Bury[guid];
                }
                if (card == null)
                {
                    card = Party.Characters[i].Hand[guid];
                }
                if (card == null)
                {
                    card = Party.Characters[i].Deck[guid];
                }
            }
        }
        return card;
    }

    public Card PlayedPowerOwner
    {
        get
        {
            if (this._PlayedPowerOwner == null)
            {
                this._PlayedPowerOwner = this.FindCard(this.PlayedPowerOwnerGuid);
            }
            return this._PlayedPowerOwner;
        }
        set
        {
            if (value == null)
            {
                this.PlayedPowerOwnerGuid = Guid.Empty;
            }
            else
            {
                this.PlayedPowerOwnerGuid = value.GUID;
            }
            this._PlayedPowerOwner = value;
        }
    }

    public string PlayedPowerOwnerID { get; set; }
}

