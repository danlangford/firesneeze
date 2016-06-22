using System;

public class License
{
    private string ID;

    public License(string id)
    {
        this.ID = id;
    }

    public virtual void Deliver()
    {
        if (UI.Window != null)
        {
            this.UnlockLicense();
            UI.Window.Refresh();
        }
    }

    private void UnlockLicense()
    {
        if (this.ID == Constants.IAP_LICENSE_AD11)
        {
            Campaign.SetAdventureUnlocked("AD11_BurntOfferings", true);
        }
        else if (this.ID == Constants.IAP_LICENSE_AD12)
        {
            Campaign.SetAdventureUnlocked("AD12_TheSkinsawMurders", true);
        }
        else if (this.ID == Constants.IAP_LICENSE_AD13)
        {
            Campaign.SetAdventureUnlocked("AD13_TheHookMountainMassacre", true);
        }
        else if (this.ID == Constants.IAP_LICENSE_AD14)
        {
            Campaign.SetAdventureUnlocked("AD14_FortressOfTheStoneGiants", true);
        }
        else if (this.ID == Constants.IAP_LICENSE_AD15)
        {
            Campaign.SetAdventureUnlocked("AD15_SinsOfTheSaviors", true);
        }
        else if (this.ID == Constants.IAP_LICENSE_AD16)
        {
            Campaign.SetAdventureUnlocked("AD16_SpiresOfXin-Shalast", true);
        }
        else if (this.ID == Constants.IAP_LICENSE_BUNDLE_ROTR)
        {
            Campaign.SetAdventureUnlocked("AD11_BurntOfferings", true);
            Campaign.SetAdventureUnlocked("AD12_TheSkinsawMurders", true);
            Campaign.SetAdventureUnlocked("AD13_TheHookMountainMassacre", true);
            Campaign.SetAdventureUnlocked("AD14_FortressOfTheStoneGiants", true);
            Campaign.SetAdventureUnlocked("AD15_SinsOfTheSaviors", true);
            Campaign.SetAdventureUnlocked("AD16_SpiresOfXin-Shalast", true);
        }
    }
}

