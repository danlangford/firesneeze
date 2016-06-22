using System;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [Tooltip("static instance of this object")]
    public static Store Instance;

    private void Awake()
    {
        Instance = this;
        Game.Network.RefreshCatalog();
        Game.Network.RefreshUserData();
    }

    public static bool CanPurchase(string selectedLicense, CurrencyCategory category, bool unique, int quantity)
    {
        if (!selectedLicense.Equals(Constants.IAP_LICENSE_BUNDLE_ROTR) && !Game.Network.Connected)
        {
            return false;
        }
        if (Game.Network.OutOfDate)
        {
            return false;
        }
        if (category == CurrencyCategory.Gold)
        {
            List<CatalogItem> catalog = Game.Network.Catalog;
            for (int i = 0; i < catalog.Count; i++)
            {
                if (selectedLicense == catalog[i].Id)
                {
                    return ((catalog[i].VirtualCurrencyPrices["GO"] * quantity) <= Game.Network.CurrentUser.Gold);
                }
            }
            return true;
        }
        return (!unique || !OwnsProduct(selectedLicense));
    }

    public static int GetPrice(string itemId, CurrencyCategory category)
    {
        List<CatalogItem> catalog = Game.Network.Catalog;
        for (int i = 0; i < catalog.Count; i++)
        {
            if (itemId == catalog[i].Id)
            {
                int num2 = 0;
                if ((category == CurrencyCategory.Gold) && catalog[i].VirtualCurrencyPrices.TryGetValue("GO", out num2))
                {
                    return num2;
                }
                if ((category == CurrencyCategory.RM) && catalog[i].VirtualCurrencyPrices.TryGetValue("RM", out num2))
                {
                    return num2;
                }
                if ((category == CurrencyCategory.RM) && catalog[i].RealCurrencyPrices.TryGetValue("USD", out num2))
                {
                    return num2;
                }
            }
        }
        return -1;
    }

    public static bool OwnsProduct(string selectedLicense)
    {
        List<string> licences = Game.Network.CurrentUser.Licences;
        if ((licences != null) && (licences.Count > 0))
        {
            for (int i = 0; i < licences.Count; i++)
            {
                if (licences[i] == selectedLicense)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static void PurchaseItem(string itemId, int pricePerUnit)
    {
        Game.Network.PurchaseItemUsingVirtualCurrency(itemId, pricePerUnit);
    }

    public enum CurrencyCategory
    {
        Gold,
        RM
    }

    public enum ItemCategory
    {
        SeasonPass,
        SubscriptionGold,
        InstantGold,
        Adventure,
        Character,
        Treasure,
        Promotional,
        Invalid
    }
}

