using System;

public class LicenseProduct
{
    public string Description;
    public string Id;
    public string Price;
    public string Title;

    public bool Consumable
    {
        get
        {
            if (((this.Id != Constants.IAP_LICENSE_GOLD_TIER1) && (this.Id != Constants.IAP_LICENSE_GOLD_TIER2)) && (this.Id != Constants.IAP_LICENSE_GOLD_TIER3))
            {
                return false;
            }
            return true;
        }
    }

    public bool Subscription =>
        ((this.Id != null) && (this.Id == Constants.IAP_LICENSE_GOLD_SUBSCRIPTION_TIER1));
}

