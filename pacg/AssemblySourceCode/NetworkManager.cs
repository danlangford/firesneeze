using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Purchasing;

public abstract class NetworkManager : MonoBehaviour
{
    protected NetworkManager()
    {
    }

    public abstract void AddCardToCollection(string cardId, int quantity);
    public abstract void DebugBuyGoldSub();
    public abstract void DebugPCPurchase(string productIdentifier);
    public abstract void DeleteGameSave(GameSaveFile gameSave);
    public abstract void GetQuests(int partyLevel);
    public abstract void GetScenarioGold(string scenarioId, int difficulty, GetScenarioGoldCallBack callback);
    public abstract void GiveGold(int amount);
    public abstract void Login();
    public abstract void LogPurchase(string productIdentifier, string intent);
    protected abstract void OnApplicationPause(bool pauseStatus);
    protected abstract void OnApplicationResume();
    public abstract void OnCardDefeated(Card card);
    public abstract void OnCardUndefeated(Card card);
    public abstract void OnLocationClosed(Location location);
    public abstract void OnLogin();
    public abstract void OnSalvageCards(Deck deck);
    public abstract void OnScenarioComplete(Scenario scenario);
    public abstract void OpenTreasureChest(AudioClip clip);
    public virtual void PurchaseItemUsingRealMoney(string itemId)
    {
        LicenseManager.BuyLicense(itemId);
    }

    public abstract void PurchaseItemUsingVirtualCurrency(string itemId, int price);
    public abstract void RedeemGoldSubscription();
    public abstract void RedeemVoucher(string voucher, string catalog = "Purchasable");
    public abstract void RefreshCatalog();
    public virtual void RefreshUserData()
    {
        for (UserField field = UserField.DisplayName; field < UserField.Invalid; field += 1)
        {
            this.RefreshUserData(field);
        }
    }

    public abstract void RefreshUserData(UserField field);
    public abstract void RemoveCardFromCollection(string cardId, int quantity);
    protected abstract void Start();
    public abstract void SynchronizeConquests(Dictionary<string, int> conquestTable);
    public abstract void SynchronizeGameSave(GameSaveFile gameSave);
    protected abstract void Update();
    public abstract void UpdateNetworkConnection();
    public abstract void UpdateNetworkConnection(string str);
    public abstract void ValidateReceipt(Product product);

    public virtual List<CatalogItem> Catalog { get; protected set; }

    public virtual bool Connected { get; protected set; }

    public abstract User CurrentUser { get; }

    public virtual bool HasNetworkConnection { get; protected set; }

    public virtual bool OutOfDate { get; protected set; }

    public delegate void GetScenarioGoldCallBack(int gold);

    public enum UserField
    {
        DisplayName,
        FriendsList,
        Licenses,
        Collection,
        ChestsAndGold,
        GameSaves,
        Invalid
    }
}

