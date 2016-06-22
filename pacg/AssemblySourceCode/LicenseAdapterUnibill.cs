using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class LicenseAdapterUnibill : LicenseAdapter, IStoreListener
{
    private IStoreController storeController;
    private IExtensionProvider storeExtensions;

    public override bool IsPurchasePossible() => 
        (this.storeController != null);

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.storeController = controller;
        this.storeExtensions = extensions;
        List<LicenseProduct> list = new List<LicenseProduct>(Constants.NUM_IAP_PRODUCTS);
        foreach (Product product in this.storeController.products.all)
        {
            if (product.availableToPurchase)
            {
                LicenseProduct item = new LicenseProduct {
                    Id = product.definition.id,
                    Title = product.metadata.localizedTitle,
                    Description = product.metadata.localizedDescription,
                    Price = product.metadata.localizedPriceString
                };
                list.Add(item);
            }
        }
        LicenseManager.AvailableProducts(list);
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IAppleExtensions extension = this.storeExtensions.GetExtension<IAppleExtensions>();
            if (extension != null)
            {
                extension.RegisterPurchaseDeferredListener(new Action<Product>(this.OnPurchaseDeferred));
            }
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        this.storeController = null;
        this.storeExtensions = null;
    }

    private void OnPurchaseDeferred(Product item)
    {
        LicenseManager.PurchaseDeferred(item.definition.id);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason error)
    {
        if (((GuiWindow.Current != null) && (GuiWindow.Current is GuiWindowStore)) && (GuiWindow.Current as GuiWindowStore).pendingPurchasePanel.Visible)
        {
            (GuiWindow.Current as GuiWindowStore).ShowPurchaseFailed();
        }
        LicenseManager.PurchaseFailed(product.definition.id);
    }

    private void OnTransactionsRestored(bool success)
    {
        LicenseManager.RestoreFinished(success);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        LicenseManager.PurchaseSuccessful(args.purchasedProduct.definition.id);
        Game.Network.ValidateReceipt(args.purchasedProduct);
        return PurchaseProcessingResult.Complete;
    }

    public override void Purchase(string productIdentifier)
    {
        if (this.storeController != null)
        {
            Game.Network.LogPurchase(productIdentifier, "Attempt");
            this.storeController.InitiatePurchase(productIdentifier);
        }
    }

    public override void Restore()
    {
        if ((this.storeExtensions != null) && (Application.platform == RuntimePlatform.IPhonePlayer))
        {
            IAppleExtensions extension = this.storeExtensions.GetExtension<IAppleExtensions>();
            if (extension != null)
            {
                extension.RestoreTransactions(new Action<bool>(this.OnTransactionsRestored));
            }
        }
    }

    public override void Start()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(), new IPurchasingModule[0]);
        builder.Configure<IGooglePlayConfiguration>().SetPublicKey(Constants.IAP_KEY_GOOGLE);
        IDs storeIDs = new IDs();
        string[] stores = new string[] { "GooglePlay" };
        storeIDs.Add("net.obsidian.pacg1.gold.sub1", stores);
        string[] textArray2 = new string[] { "AppleAppStore" };
        storeIDs.Add("net.obsidian.pacg1.gold.sub1", textArray2);
        builder.AddProduct(Constants.IAP_LICENSE_GOLD_SUBSCRIPTION_TIER1, ProductType.Consumable, storeIDs);
        storeIDs = new IDs();
        string[] textArray3 = new string[] { "GooglePlay" };
        storeIDs.Add("net.obsidian.pacg1.gold.tier1", textArray3);
        string[] textArray4 = new string[] { "AppleAppStore" };
        storeIDs.Add("net.obsidian.pacg1.gold.tier1", textArray4);
        builder.AddProduct(Constants.IAP_LICENSE_GOLD_TIER1, ProductType.Consumable, storeIDs);
        storeIDs = new IDs();
        string[] textArray5 = new string[] { "GooglePlay" };
        storeIDs.Add("net.obsidian.pacg1.gold.tier2", textArray5);
        string[] textArray6 = new string[] { "AppleAppStore" };
        storeIDs.Add("net.obsidian.pacg1.gold.tier2", textArray6);
        builder.AddProduct(Constants.IAP_LICENSE_GOLD_TIER2, ProductType.Consumable, storeIDs);
        storeIDs = new IDs();
        string[] textArray7 = new string[] { "GooglePlay" };
        storeIDs.Add("net.obsidian.pacg1.gold.tier3", textArray7);
        string[] textArray8 = new string[] { "AppleAppStore" };
        storeIDs.Add("net.obsidian.pacg1.gold.tier3", textArray8);
        builder.AddProduct(Constants.IAP_LICENSE_GOLD_TIER3, ProductType.Consumable, storeIDs);
        storeIDs = new IDs();
        string[] textArray9 = new string[] { "GooglePlay" };
        storeIDs.Add("net.obsidian.pacg1.bundle.rotr", textArray9);
        string[] textArray10 = new string[] { "AppleAppStore" };
        storeIDs.Add("net.obsidian.pacg1.bundle.rotr", textArray10);
        builder.AddProduct(Constants.IAP_LICENSE_BUNDLE_ROTR, ProductType.NonConsumable, storeIDs);
        if (Settings.DebugMode)
        {
            storeIDs = new IDs();
            string[] textArray11 = new string[] { "GooglePlay" };
            storeIDs.Add("net.obsidian.pacg1.dummy", textArray11);
            string[] textArray12 = new string[] { "AppleAppStore" };
            storeIDs.Add("net.obsidian.pacg1.dummy", textArray12);
            builder.AddProduct(Constants.IAP_LICENSE_DUMMY, ProductType.Consumable, storeIDs);
        }
        UnityPurchasing.Initialize(this, builder);
    }
}

