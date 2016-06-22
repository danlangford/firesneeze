using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LicenseManager
{
    private static readonly string encryptionPassword = "xLgsV[6]c";
    private static LicenseResult lastResult;
    private static List<LicenseProduct> productList;
    private static List<string> rotrList;
    private static LicenseAdapter store;

    public static void AvailableProducts(List<LicenseProduct> list)
    {
        lastResult = null;
        productList = list;
        productList.Sort(LicenseSorter.SortByPrice());
    }

    public static void BuyLicense(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        if (!store.IsPurchasePossible())
        {
            string helperText = StringTableManager.GetHelperText(0x39);
            lastResult = new LicenseResult(LicenseOperationType.Purchase, productIdentifier, false, helperText);
        }
        else if (!GetIsSupported(productIdentifier))
        {
            string text = StringTableManager.GetHelperText(0x3a);
            lastResult = new LicenseResult(LicenseOperationType.Purchase, productIdentifier, false, text);
        }
        else
        {
            lastResult = null;
            store.Purchase(productIdentifier);
        }
    }

    private static string CreatePlayerID()
    {
        string[] textArray1 = new string[] { S4(), "-", S4(), "-", S4(), "-", S4(), S4(), S4(), S4() };
        return string.Concat(textArray1);
    }

    private static string DecryptString(string message, string password) => 
        XorString(message, password);

    private static string EncryptString(string message, string password) => 
        XorString(message, password);

    public static string GetBundle(string productIdentifier)
    {
        string item = productIdentifier.ToLower();
        if (RiseOfTheRunelordsList.Contains(item))
        {
            return Constants.IAP_LICENSE_BUNDLE_ROTR;
        }
        return null;
    }

    public static bool GetIsAvailable(string productIdentifier, bool usingRealMoney)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        if (!usingRealMoney)
        {
            return GetIsSupported(productIdentifier);
        }
        for (int i = 0; i < productList.Count; i++)
        {
            if (productList[i].Id == productIdentifier)
            {
                return GetIsSupported(productIdentifier);
            }
        }
        return false;
    }

    public static bool GetIsBundle(string productIdentifier) => 
        productIdentifier.ToLower().Equals(Constants.IAP_LICENSE_BUNDLE_ROTR);

    public static bool GetIsLicensed(string productIdentifier)
    {
        productIdentifier = productIdentifier.ToLower();
        string bundle = GetBundle(productIdentifier);
        if (((bundle != null) && GetIsLicensed(bundle)) && GetIsLicensed(GetBundle(productIdentifier)))
        {
            return true;
        }
        string licenseName = GetLicenseName(productIdentifier);
        if (PlayerPrefs.HasKey(licenseName))
        {
            string message = PlayerPrefs.GetString(licenseName);
            if ((message != null) && (message.Length > 0))
            {
                string str4 = DecryptString(message, encryptionPassword);
                string str5 = GetPlayerID() + "/" + productIdentifier;
                return (str4 == str5);
            }
        }
        return false;
    }

    public static bool GetIsSupported(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        LicenseTableEntry entry = LicenseTable.Get(productIdentifier);
        return ((entry != null) && entry.Available);
    }

    public static string GetLicenseIdentifierForDeck(string set)
    {
        if (set == "1")
        {
            return Constants.IAP_LICENSE_AD11;
        }
        if (set == "2")
        {
            return Constants.IAP_LICENSE_AD12;
        }
        if (set == "3")
        {
            return Constants.IAP_LICENSE_AD13;
        }
        if (set == "4")
        {
            return Constants.IAP_LICENSE_AD14;
        }
        if (set == "5")
        {
            return Constants.IAP_LICENSE_AD15;
        }
        if (set == "6")
        {
            return Constants.IAP_LICENSE_AD16;
        }
        if (set == "C")
        {
            return Constants.IAP_LICENSE_CH11;
        }
        if (set == "P")
        {
            return Constants.IAP_LICENSE_BUNDLE_ROTR;
        }
        return null;
    }

    private static string GetLicenseName(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        return ("License-" + productIdentifier);
    }

    public static GuiWindowStore.StorePanelType GetLicensePanelType(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        if (productIdentifier.StartsWith(Constants.CHEST_IAP_STARTS_WITH))
        {
            return GuiWindowStore.StorePanelType.Treasure_Buy;
        }
        if (productIdentifier.StartsWith(Constants.AD_IAP_STARTS_WITH))
        {
            return GuiWindowStore.StorePanelType.Adventures;
        }
        if (productIdentifier.StartsWith(Constants.CH_IAP_STARTS_WITH))
        {
            return GuiWindowStore.StorePanelType.Characters;
        }
        if (productIdentifier.StartsWith(Constants.GOLD_IAP_STARTS_WITH))
        {
            return GuiWindowStore.StorePanelType.Gold;
        }
        if (productIdentifier.StartsWith(Constants.BUNDLE_IAP_STARTS_WITH))
        {
            return GuiWindowStore.StorePanelType.Specials;
        }
        return GuiWindowStore.StorePanelType.Start;
    }

    public static string GetPlayerID()
    {
        string str = PlayerPrefs.GetString("License-ID");
        if (str == string.Empty)
        {
            str = CreatePlayerID();
            PlayerPrefs.SetString("License-ID", str);
            PlayerPrefs.Save();
        }
        return str;
    }

    public static LicenseProduct GetProduct(int i)
    {
        if ((i >= 0) && (i < productList.Count))
        {
            return productList[i];
        }
        return null;
    }

    public static LicenseProduct GetProduct(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        for (int i = 0; i < productList.Count; i++)
        {
            if (productList[i].Id == productIdentifier)
            {
                return productList[i];
            }
        }
        return null;
    }

    public static int GetProductCount() => 
        productList.Count;

    public static LicenseResult GetResult(LicenseOperationType type)
    {
        if ((lastResult != null) && (lastResult.Operation == type))
        {
            return lastResult;
        }
        return null;
    }

    public static void GrantBundle(string productIdentifier)
    {
        if (productIdentifier.ToLower().Equals(Constants.IAP_LICENSE_BUNDLE_ROTR))
        {
            GrantLicenses(RiseOfTheRunelordsList);
        }
    }

    public static void GrantLicense(string productIdentifier)
    {
        productIdentifier = productIdentifier.ToLower();
        string licenseName = GetLicenseName(productIdentifier);
        string str3 = EncryptString(GetPlayerID() + "/" + productIdentifier, encryptionPassword);
        PlayerPrefs.SetString(licenseName, str3);
        PlayerPrefs.Save();
        if (GetIsBundle(productIdentifier))
        {
            GrantBundle(productIdentifier);
        }
    }

    public static void GrantLicenses(List<string> productIdentifiers)
    {
        if ((productIdentifiers != null) && (productIdentifiers.Count > 0))
        {
            for (int i = 0; i < productIdentifiers.Count; i++)
            {
                if (!GetIsLicensed(productIdentifiers[i]))
                {
                    GrantLicense(productIdentifiers[i]);
                }
            }
        }
    }

    public static void PurchaseCancelled(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        string helperText = StringTableManager.GetHelperText(60);
        lastResult = new LicenseResult(LicenseOperationType.Purchase, productIdentifier, false, helperText);
    }

    public static void PurchaseDeferred(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        string helperText = StringTableManager.GetHelperText(0x3d);
        lastResult = new LicenseResult(LicenseOperationType.Purchase, productIdentifier, false, helperText);
    }

    public static void PurchaseFailed(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        string helperText = StringTableManager.GetHelperText(0x3b);
        lastResult = new LicenseResult(LicenseOperationType.Purchase, productIdentifier, false, helperText);
    }

    public static void PurchaseRefunded(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        string helperText = StringTableManager.GetHelperText(0x3f);
        lastResult = new LicenseResult(LicenseOperationType.Refund, productIdentifier, true, helperText);
        RevokeLicense(productIdentifier);
    }

    public static void PurchaseSuccessful(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        string helperText = StringTableManager.GetHelperText(0x3e);
        lastResult = new LicenseResult(LicenseOperationType.Purchase, productIdentifier, true, helperText);
        GrantLicense(productIdentifier);
        License license = LicenseFactory.Create(productIdentifier);
        if (license != null)
        {
            license.Deliver();
        }
    }

    public static void RestoreFinished(bool success)
    {
        lastResult = new LicenseResult(LicenseOperationType.Restore, success);
        Game.UI.Toast.Show("Restore " + (!success ? "failed!" : "successful!"));
    }

    public static void RestoreLicenses()
    {
        lastResult = null;
        store.Restore();
    }

    public static void RestoreLicenses(List<string> purchasedLicenses)
    {
        for (int i = 0; i < purchasedLicenses.Count; i++)
        {
            string productIdentifier = purchasedLicenses[i].ToLower();
            if (!GetIsLicensed(productIdentifier))
            {
                RestoreSuccessful(productIdentifier);
            }
        }
    }

    public static void RestoreSuccessful(string productIdentifier)
    {
        if (productIdentifier != null)
        {
            productIdentifier = productIdentifier.ToLower();
        }
        string helperText = StringTableManager.GetHelperText(0x37);
        lastResult = new LicenseResult(LicenseOperationType.Restore, productIdentifier, true, helperText);
        GrantLicense(productIdentifier);
        License license = LicenseFactory.Create(productIdentifier);
        if (license != null)
        {
            license.Deliver();
        }
    }

    public static void RevokeLicense(string productIdentifier)
    {
        productIdentifier = productIdentifier.ToLower();
        PlayerPrefs.SetString(GetLicenseName(productIdentifier), string.Empty);
        PlayerPrefs.Save();
    }

    private static string S4() => 
        UnityEngine.Random.Range(0, 0x10000).ToString("X4").ToLower();

    public static void Start()
    {
        productList = new List<LicenseProduct>(Constants.NUM_IAP_PRODUCTS);
        store = new LicenseAdapterUnibill();
        store.Start();
    }

    private static string XorString(string message, string key)
    {
        StringBuilder builder = new StringBuilder(message.Length);
        for (int i = 0; i < message.Length; i++)
        {
            uint num2 = key[i % key.Length] + '\x0080';
            uint num3 = message[i];
            builder.Append((char) (num3 ^ num2));
        }
        return builder.ToString();
    }

    public static List<string> RiseOfTheRunelordsList
    {
        get
        {
            if ((rotrList == null) || (rotrList.Count == 0))
            {
                List<string> list = new List<string> {
                    Constants.IAP_LICENSE_CH11,
                    Constants.IAP_LICENSE_CH1B_EZREN,
                    Constants.IAP_LICENSE_CH1B_HARSK,
                    Constants.IAP_LICENSE_CH1B_KYRA,
                    Constants.IAP_LICENSE_CH1B_LEM,
                    Constants.IAP_LICENSE_CH1B_MERISIEL,
                    Constants.IAP_LICENSE_CH1B_SEONI,
                    Constants.IAP_LICENSE_CH1B_VALEROS,
                    Constants.IAP_LICENSE_CH1C_AMIRI,
                    Constants.IAP_LICENSE_CH1C_LINI,
                    Constants.IAP_LICENSE_CH1C_SAJAN,
                    Constants.IAP_LICENSE_CH1C_SEELAH,
                    Constants.IAP_LICENSE_AD11,
                    Constants.IAP_LICENSE_AD12,
                    Constants.IAP_LICENSE_AD13,
                    Constants.IAP_LICENSE_AD14,
                    Constants.IAP_LICENSE_AD15,
                    Constants.IAP_LICENSE_AD16
                };
                rotrList = list;
            }
            return rotrList;
        }
    }
}

