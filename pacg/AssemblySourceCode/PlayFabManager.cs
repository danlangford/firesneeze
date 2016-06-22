using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

public class PlayFabManager : NetworkManager
{
    private bool _applicationPaused;
    private bool _clientOutOfDate;
    private bool _hasNetworkConnection = true;
    [CompilerGenerated]
    private static ErrorCallback <>f__am$cache10;
    [CompilerGenerated]
    private static PlayFabClientAPI.RunCloudScriptCallback <>f__am$cacheC;
    [CompilerGenerated]
    private static PlayFabClientAPI.RunCloudScriptCallback <>f__am$cacheD;
    [CompilerGenerated]
    private static PlayFabClientAPI.RunCloudScriptCallback <>f__am$cacheE;
    [CompilerGenerated]
    private static PlayFabClientAPI.RunCloudScriptCallback <>f__am$cacheF;
    private List<CatalogItem> catalog = new List<CatalogItem>();
    private readonly int chunkLimit = 0x61a8;
    private User currentUser;
    private GameSaveFile latestSaveAwaitingSync;
    private float m_recheckTimer;
    private const float RecheckTime = 60f;
    private bool saveChunkLock;
    private bool saveLock;
    private float saveTimer = 1f;
    private readonly float saveTimerThreshold = 1f;

    public override void AddCardToCollection(string cardID, int quantity)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "CardId",
                    cardID
                },
                { 
                    "Quantity",
                    quantity
                }
            };
            this.RunCloudScript("addCardToCollection", o, delegate (RunCloudScriptResult result) {
                Game.Network.UpdateNetworkConnection();
                UnityEngine.Debug.Log("Got log entries:");
                UnityEngine.Debug.Log(result.ActionLog);
                UnityEngine.Debug.Log("and return value: " + result.Results.ToString());
                this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
                this.RefreshUserData(NetworkManager.UserField.Collection);
            }, null);
        }
    }

    private void AddUserVirtualCurrency(string currencyName, int amount)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, string> o = new Dictionary<string, string> {
                { 
                    "VirtualCurrency",
                    currencyName
                },
                { 
                    "Amount",
                    string.Empty + amount
                }
            };
            this.RunCloudScript("addUserVirtualCurrency", o, delegate (RunCloudScriptResult result) {
                Game.Network.UpdateNetworkConnection();
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                if (result.ResultsEncoded != null)
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ResultsEncoded);
                }
                else
                {
                    UnityEngine.Debug.Log("Results were null!");
                    return;
                }
                UnityEngine.Debug.Log("addUserVirtualCurrency(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                string str = string.Empty;
                if (dictionary.TryGetValue("messageValue", out str))
                {
                    int num = 0;
                    char[] separator = new char[] { ' ' };
                    string[] strArray = str.Split(separator);
                    string str2 = strArray[strArray.Length - 1];
                    int.TryParse(str2.Substring(0, str2.Length - 1), out num);
                    UnityEngine.Debug.Log("New Gold Value: " + num);
                    this.CurrentUser.Gold = num;
                    UnityEngine.Debug.Log($"Cloud Script -- Version: {result.Version}, Revision: {result.Revision} 
Response: {str}");
                }
                this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
                if ((GuiWindow.Current is GuiWindowStore) && ((GuiWindow.Current as GuiWindowStore).ActivePanelType == GuiWindowStore.StorePanelType.Gold))
                {
                    (GuiWindow.Current as GuiWindowStore).overlayPanel.ShowGoldVFX();
                }
            }, null);
        }
    }

    private void AlertNetworkConnectionFound()
    {
        if (Time.realtimeSinceStartup >= 15f)
        {
            if (!Game.Network.Connected)
            {
                this.Login();
            }
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.NetworkConnectionFound);
        }
    }

    private void AlertNetworkConnectionLost()
    {
        Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.NetworkConnectionLost);
        if (GuiWindow.Current is GuiWindowStore)
        {
            (GuiWindow.Current as GuiWindowStore).CloseStore();
            base.StartCoroutine(this.ShowStoreKick());
        }
    }

    private void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        if (Settings.DebugMode)
        {
            PlayFabSettings.TitleId = "8834";
        }
        else
        {
            PlayFabSettings.TitleId = "E1A5";
        }
    }

    public override void DebugBuyGoldSub()
    {
        this.RunCloudScript("dubugBuyGoldSub", null, delegate (RunCloudScriptResult result) {
            Game.Network.UpdateNetworkConnection();
            this.ProcessPurchases(null);
        }, null);
    }

    public override void DebugPCPurchase(string productIdentifier)
    {
        Dictionary<string, object> o = new Dictionary<string, object> {
            { 
                "ItemId",
                productIdentifier
            }
        };
        this.RunCloudScript("debugPurchase", o, delegate (RunCloudScriptResult result) {
            Game.Network.UpdateNetworkConnection();
            this.ProcessPurchases(null);
        }, null);
    }

    public override void DeleteGameSave(GameSaveFile save)
    {
        if ((this.Connected && !this.OutOfDate) && (this.HasNetworkConnection && (save.Header != null)))
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "Saves",
                    save.Header.GUID
                }
            };
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = delegate (RunCloudScriptResult result) {
                    Game.Network.UpdateNetworkConnection();
                    UnityEngine.Debug.Log("deleteGameSave(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                };
            }
            this.RunCloudScript("deleteGameSave", o, <>f__am$cacheC, null);
        }
    }

    private void FailedDebug(PlayFabError error)
    {
        UI.Busy = false;
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
        GuiWindow current = GuiWindow.Current;
        if ((current != null) && (current is GuiWindowStore))
        {
            (current as GuiWindowStore).ShowPurchaseDebug(error.Error + " " + error.ErrorMessage);
        }
    }

    private void GetCloudScriptEndpoint(UnityAction callback = null)
    {
        <GetCloudScriptEndpoint>c__AnonStorey128 storey = new <GetCloudScriptEndpoint>c__AnonStorey128 {
            callback = callback
        };
        if (string.IsNullOrEmpty(PlayFabSettings.LogicServerURL))
        {
            PlayFabClientAPI.GetCloudScriptUrl(new GetCloudScriptUrlRequest(), new PlayFabClientAPI.GetCloudScriptUrlCallback(storey.<>m__162), new ErrorCallback(PlayFabManager.OnPlayFabError), null);
        }
        else
        {
            storey.callback();
        }
    }

    public override void GetQuests(int partyLevel)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "PartyLevel",
                    partyLevel
                }
            };
            this.RunCloudScript("getQuests", o, delegate (RunCloudScriptResult result) {
                object obj2;
                object obj3;
                object obj4;
                object obj5;
                Game.Network.UpdateNetworkConnection();
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                if (result.ResultsEncoded != null)
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                }
                else
                {
                    UnityEngine.Debug.Log("Results were null!");
                    return;
                }
                UnityEngine.Debug.Log("getQuests(result.ActionLog:" + result.ActionLog + ") AND (result.Results:" + result.Results.ToString() + ")");
                if (dictionary.TryGetValue("dailyQuest", out obj2) && !string.IsNullOrEmpty((string) obj2))
                {
                    this.CurrentUser.DailyQuestCurrent = (string) obj2;
                }
                if (dictionary.TryGetValue("promoQuest", out obj3) && !string.IsNullOrEmpty((string) obj3))
                {
                    this.CurrentUser.PromotionalQuestCurrent = (string) obj3;
                }
                if (dictionary.TryGetValue("completedDailyQuest", out obj4))
                {
                    this.CurrentUser.DailyQuestAvailable = !((bool) obj4);
                }
                if (dictionary.TryGetValue("dailyQuestTimeTillReset", out obj5))
                {
                    this.CurrentUser.DailyQuestTimeTillReset = (double) obj5;
                    this.CurrentUser.DailyQuestTimeSet = DateTime.Now.ToFileTimeUtc();
                }
            }, null);
        }
    }

    public override void GetScenarioGold(string scenarioId, int difficulty, NetworkManager.GetScenarioGoldCallBack callback = null)
    {
        <GetScenarioGold>c__AnonStorey126 storey = new <GetScenarioGold>c__AnonStorey126 {
            callback = callback,
            <>f__this = this
        };
        if ((!this.Connected || this.OutOfDate) || !this.HasNetworkConnection)
        {
            if (storey.callback != null)
            {
                storey.callback(-1);
            }
        }
        else
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "Difficulty",
                    difficulty
                },
                { 
                    "Scenario",
                    scenarioId
                }
            };
            this.RunCloudScript("getScenarioGold", o, new PlayFabClientAPI.RunCloudScriptCallback(storey.<>m__15C), null);
        }
    }

    public override void GiveGold(int amount)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            this.AddUserVirtualCurrency("GO", amount);
        }
    }

    public override void Login()
    {
        PlayFabLoginCalls.StartGooglePlayGamesLogin();
    }

    public override void LogPurchase(string product, string intent)
    {
        Dictionary<string, object> o = new Dictionary<string, object> {
            { 
                "Intent",
                intent
            },
            { 
                "ItemId",
                product
            }
        };
        if (<>f__am$cacheF == null)
        {
            <>f__am$cacheF = result => Game.Network.UpdateNetworkConnection();
        }
        this.RunCloudScript("logPurchase", o, <>f__am$cacheF, null);
    }

    private void MakeTempSavesOfficial()
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            this.RunCloudScript("makeTempSavesOfficial", null, delegate (RunCloudScriptResult result) {
                this.saveLock = false;
                Game.Network.UpdateNetworkConnection();
                UnityEngine.Debug.Log("saveChunk(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
            }, null);
        }
    }

    protected override void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && (this._applicationPaused != pauseStatus))
        {
            this.OnApplicationResume();
        }
        this._applicationPaused = pauseStatus;
    }

    protected override void OnApplicationResume()
    {
        if (Game.UI.NetworkTooltip.Visible)
        {
            Game.UI.NetworkTooltip.Rebind();
        }
        if (!Game.Network.Connected)
        {
            this.Login();
        }
    }

    public override void OnCardDefeated(Card card)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "Type",
                    card.Type
                },
                { 
                    "Traits",
                    card.Traits
                },
                { 
                    "Tier",
                    Party.Tier
                }
            };
            this.RunCloudScript("onCardDefeated", o, delegate (RunCloudScriptResult result) {
                object obj2;
                Game.Network.UpdateNetworkConnection();
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                if (result.ResultsEncoded != null)
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                }
                else
                {
                    UnityEngine.Debug.Log("Results were null!");
                    return;
                }
                UnityEngine.Debug.Log("onCardDefeated(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                if (dictionary.TryGetValue("gold", out obj2))
                {
                    int amount = Convert.ToInt32(obj2);
                    if (amount > 0)
                    {
                        GuiWindowLocation window = UI.Window as GuiWindowLocation;
                        if (window != null)
                        {
                            window.statusPanel.ShowGold(amount);
                        }
                        this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
                        Party.RewardGold(amount);
                    }
                }
            }, null);
        }
    }

    public override void OnCardUndefeated(Card card)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
        }
    }

    public override void OnLocationClosed(Location location)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "Tier",
                    Party.Tier
                }
            };
            this.RunCloudScript("onLocationClosed", o, delegate (RunCloudScriptResult result) {
                object obj2;
                Game.Network.UpdateNetworkConnection();
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                if (result.ResultsEncoded != null)
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                }
                else
                {
                    UnityEngine.Debug.Log("Results were null!");
                    return;
                }
                UnityEngine.Debug.Log("onLocationClosed(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                if (dictionary.TryGetValue("gold", out obj2))
                {
                    int amount = Convert.ToInt32(obj2);
                    if (amount > 0)
                    {
                        GuiWindowLocation window = UI.Window as GuiWindowLocation;
                        if (window != null)
                        {
                            window.statusPanel.ShowGold(amount);
                        }
                        this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
                        Party.RewardGold(amount);
                    }
                }
            }, null);
        }
    }

    public override void OnLogin()
    {
        Dictionary<string, object> o = new Dictionary<string, object> {
            { 
                "BuildNumber",
                Game.Instance.BuildNumber
            }
        };
        if (!string.IsNullOrEmpty(PlayFabLoginCalls.AndroidID))
        {
            o.Add("DeviceID", PlayFabLoginCalls.AndroidID);
        }
        else if (!string.IsNullOrEmpty(PlayFabLoginCalls.IOSID))
        {
            o.Add("DeviceID", PlayFabLoginCalls.IOSID);
        }
        this.RunCloudScript("onLogin", o, delegate (RunCloudScriptResult result) {
            object obj2;
            object obj3;
            object obj4;
            Game.Network.UpdateNetworkConnection();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            if (result.ResultsEncoded != null)
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
            }
            else
            {
                return;
            }
            if (dictionary.TryGetValue("conquest", out obj2))
            {
                if (obj2 != null)
                {
                    Conquests.Synchronize(JsonConvert.DeserializeObject<Dictionary<string, int>>((string) obj2));
                }
            }
            else
            {
                UnityEngine.Debug.Log("Could not obtain conquest data");
            }
            if (dictionary.TryGetValue("promotion", out obj3))
            {
                if (obj3 != null)
                {
                    GuiPanelPromotion.Synchronize(JsonConvert.DeserializeObject<Dictionary<string, string>>((string) obj3));
                }
            }
            else
            {
                UnityEngine.Debug.Log("Could not obtain promotional data");
            }
            if (dictionary.TryGetValue("outOfDate", out obj4))
            {
                this._clientOutOfDate = (bool) obj4;
                if (Application.isEditor)
                {
                    this._clientOutOfDate = false;
                }
                if (this._clientOutOfDate)
                {
                    GuiWindow current = GuiWindow.Current;
                    if ((current != null) && (current is GuiWindowMainMenu))
                    {
                        GuiWindowMainMenu menu = current as GuiWindowMainMenu;
                        if ((menu.promoPanel != null) && menu.promoPanel.Visible)
                        {
                            menu.promoPanel.Show(false);
                        }
                        menu.outOfDatePanel.Show(this._clientOutOfDate);
                    }
                }
            }
            UI.Window.Refresh();
            this.RefreshCatalog();
            this.RefreshUserData();
        }, null);
    }

    private void OnNetworkFail(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
    }

    private void OnNetworkSucceed(PingResult result)
    {
        this.UpdateNetworkConnection();
    }

    public static void OnPlayFabError(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
        UnityEngine.Debug.Log($"{error.actionId} : Error {error.HttpCode} : {error.ErrorMessage}");
        UI.Busy = false;
        GuiWindow current = GuiWindow.Current;
        if ((current != null) && (current is GuiWindowStore))
        {
            GuiWindowStore store = current as GuiWindowStore;
            if (store.ActivePanelType == GuiWindowStore.StorePanelType.Gold)
            {
                store.goldPanel.UnlockBuyButton();
            }
        }
    }

    private void OnPurchasedItem(PurchaseItemResult result)
    {
        Game.Network.UpdateNetworkConnection();
        this.ProcessPurchases(result.Items);
    }

    private void OnPurchaseFailed(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
        GuiWindow current = GuiWindow.Current;
        if (current != null)
        {
            (current as GuiWindowStore).ShowPurchaseFailed();
        }
        UI.Busy = false;
        UnityEngine.Debug.Log($"Error {error.HttpCode}: {error.ErrorMessage}");
    }

    public override void OnSalvageCards(Deck deck)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "Cards",
                    deck.GetCardList()
                },
                { 
                    "Tier",
                    Party.Tier
                }
            };
            this.RunCloudScript("onSalvageCards", o, delegate (RunCloudScriptResult result) {
                object obj2;
                Game.Network.UpdateNetworkConnection();
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                if (result.ResultsEncoded != null)
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                }
                else
                {
                    UnityEngine.Debug.Log("Results were null!");
                    return;
                }
                UnityEngine.Debug.Log("onSalvageCards(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                if (dictionary.TryGetValue("gold", out obj2))
                {
                    int gp = Convert.ToInt32(obj2);
                    if (gp > 0)
                    {
                        this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
                        Game.UI.GoldPanel.Show(gp);
                        Party.RewardGold(gp);
                    }
                }
            }, null);
        }
    }

    public override void OnScenarioComplete(Scenario scenario)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "Difficulty",
                    scenario.Difficulty
                },
                { 
                    "Scenario",
                    scenario.ID
                }
            };
            this.RunCloudScript("onScenarioComplete", o, delegate (RunCloudScriptResult result) {
                object obj2;
                Game.Network.UpdateNetworkConnection();
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                if (result.ResultsEncoded != null)
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                }
                else
                {
                    UnityEngine.Debug.Log("Results were null!");
                    return;
                }
                UnityEngine.Debug.Log("onScenarioComplete(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                if (dictionary.TryGetValue("gold", out obj2))
                {
                    int amount = Convert.ToInt32(obj2);
                    if (amount > 0)
                    {
                        this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
                        Party.RewardGold(amount);
                    }
                }
            }, null);
        }
    }

    private void OnValidateGooglePlayReceipt(ValidateGooglePlayPurchaseResult result)
    {
        Game.Network.UpdateNetworkConnection();
        this.ProcessPurchases(null);
    }

    private void OnValidateIOSReceipt(ValidateIOSReceiptResult result)
    {
        Game.Network.UpdateNetworkConnection();
        this.ProcessPurchases(null);
    }

    public override void OpenTreasureChest(AudioClip clip)
    {
        <OpenTreasureChest>c__AnonStorey125 storey = new <OpenTreasureChest>c__AnonStorey125 {
            clip = clip,
            <>f__this = this
        };
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            this.RunCloudScript("openTreasureChest", null, new PlayFabClientAPI.RunCloudScriptCallback(storey.<>m__155), null);
        }
    }

    public void PollNetworkConnection()
    {
        PingRequest request = new PingRequest();
        PlayFabClientAPI.Ping(request, new PlayFabClientAPI.PingCallback(this.OnNetworkSucceed), new ErrorCallback(this.OnNetworkFail), null);
    }

    private void ProcessPurchases(List<ItemInstance> itemsPurchased = null)
    {
        this.RunCloudScript("processPurchases", null, delegate (RunCloudScriptResult result) {
            UI.Busy = false;
            Game.Network.UpdateNetworkConnection();
            if (GuiWindow.Current is GuiWindowStore)
            {
                GuiWindowStore current = GuiWindow.Current as GuiWindowStore;
                if (current.ActivePanelType == GuiWindowStore.StorePanelType.Gold)
                {
                    current.goldPanel.UnlockBuyButton();
                }
                current.Refresh();
                if (current.ActivePanelType == GuiWindowStore.StorePanelType.Gold)
                {
                    current.ShowPurchaseSuccessfulBlank();
                }
                else
                {
                    current.ShowPurchaseSuccessful();
                }
                this.RefreshUserData(NetworkManager.UserField.Licenses);
                this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
            }
        }, null);
    }

    public override void PurchaseItemUsingVirtualCurrency(string itemId, int price)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            string str = "Purchasable";
            string str2 = "GO";
            PurchaseItemRequest request = new PurchaseItemRequest {
                CatalogVersion = str,
                ItemId = itemId,
                VirtualCurrency = str2,
                Price = price
            };
            PlayFabClientAPI.PurchaseItem(request, new PlayFabClientAPI.PurchaseItemCallback(this.OnPurchasedItem), new ErrorCallback(this.OnPurchaseFailed), null);
        }
    }

    public static void PurchaseSuccessful(PurchaseEventArgs itemArgs)
    {
    }

    public override void RedeemGoldSubscription()
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            this.RunCloudScript("redeemGoldSubcription", null, delegate (RunCloudScriptResult result) {
                Game.Network.UpdateNetworkConnection();
                UnityEngine.Debug.Log("Got log entries:");
                UnityEngine.Debug.Log(result.ActionLog);
                UnityEngine.Debug.Log("and return value: " + result.Results.ToString());
                this.RefreshUserData(NetworkManager.UserField.Licenses);
                this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
            }, null);
        }
    }

    public override void RedeemVoucher(string voucher, string catalog = "Purchasable")
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, string> o = new Dictionary<string, string> {
                { 
                    "CouponCode",
                    voucher
                },
                { 
                    "CatalogVersion",
                    catalog
                }
            };
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = delegate (PlayFabError error) {
                    GuiPanelVoucher.Unlock();
                    GuiPanelVoucher.SetMessage(GuiPanelVoucher.ResultType.InvalidCode);
                    if (error.ErrorMessage.Contains("Coupon already redeemed"))
                    {
                        GuiPanelVoucher.SetMessage(GuiPanelVoucher.ResultType.AlreadyRedeemed);
                    }
                    if (error.ErrorMessage.Contains("Coupon code not found"))
                    {
                        GuiPanelVoucher.SetMessage(GuiPanelVoucher.ResultType.CouldntFindCode);
                    }
                    OnPlayFabError(error);
                };
            }
            this.RunCloudScript("redeemVoucher", o, delegate (RunCloudScriptResult result) {
                UnityEngine.Debug.Log("Successfully redeemed a code with (" + result.Results.ToString() + ")");
                GuiPanelVoucher.Unlock();
                GuiPanelVoucher.SetMessage(GuiPanelVoucher.ResultType.Success);
                Game.Network.UpdateNetworkConnection();
                this.RefreshUserData(NetworkManager.UserField.Licenses);
                this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
            }, <>f__am$cache10);
        }
    }

    public override void RefreshCatalog()
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            this.RunCloudScript("getCatalog", null, delegate (RunCloudScriptResult result) {
                Game.Network.UpdateNetworkConnection();
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                if (result.ResultsEncoded != null)
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ResultsEncoded);
                }
                else
                {
                    UnityEngine.Debug.Log("Results were null!");
                    return;
                }
                UnityEngine.Debug.Log("getCatalog(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                string str = string.Empty;
                if (dictionary.TryGetValue("catalog", out str))
                {
                    List<CatalogItem> list = JsonConvert.DeserializeObject<List<CatalogItem>>(str);
                    this.Catalog = list;
                }
            }, null);
        }
    }

    public override void RefreshUserData(NetworkManager.UserField field)
    {
        switch (field)
        {
            case NetworkManager.UserField.DisplayName:
                this.currentUser.DisplayName = this.PlayFabDisplayName;
                this.currentUser.Id = this.PlayFabID;
                break;

            case NetworkManager.UserField.Licenses:
                this.RunCloudScript("getLicenses", null, delegate (RunCloudScriptResult result) {
                    object obj2;
                    object obj3;
                    object obj4;
                    object obj5;
                    Game.Network.UpdateNetworkConnection();
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    if (result.ResultsEncoded != null)
                    {
                        dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Results were null!");
                        return;
                    }
                    UnityEngine.Debug.Log("getLicenses(result.ActionLog:" + result.ActionLog + ") AND (result.Results:" + result.Results.ToString() + ")");
                    if (dictionary.TryGetValue("licenses", out obj2) && (obj2 != null))
                    {
                        List<string> purchasedLicenses = JsonConvert.DeserializeObject<List<string>>((string) obj2);
                        this.CurrentUser.Licences = purchasedLicenses;
                        LicenseManager.RestoreLicenses(purchasedLicenses);
                    }
                    if (dictionary.TryGetValue("hasGoldSub", out obj3) && (obj3 != null))
                    {
                        this.CurrentUser.GoldSubAvailable = (bool) obj3;
                    }
                    if (dictionary.TryGetValue("numDaysRemaining", out obj4) && (obj4 != null))
                    {
                        this.CurrentUser.GoldSubDaysRemaining = int.Parse(obj4 + string.Empty);
                    }
                    if (dictionary.TryGetValue("timeTillReset", out obj5) && (obj5 != null))
                    {
                        this.CurrentUser.GoldSubTimeTillReset = (double) obj5;
                        this.CurrentUser.GoldResetTimeSet = DateTime.Now.ToFileTimeUtc();
                    }
                    GuiWindow current = GuiWindow.Current;
                    if ((current != null) && (current is GuiWindowStore))
                    {
                        GuiWindowStore store = current as GuiWindowStore;
                        if (store.ActivePanelType == GuiWindowStore.StorePanelType.Start)
                        {
                            store.startPanel.Refresh();
                        }
                        else if (store.ActivePanelType == GuiWindowStore.StorePanelType.Specials)
                        {
                            store.specialsPanel.Refresh();
                        }
                        else if (store.ActivePanelType == GuiWindowStore.StorePanelType.Gold)
                        {
                            store.goldPanel.Refresh();
                        }
                    }
                }, null);
                break;

            case NetworkManager.UserField.Collection:
            {
                Dictionary<string, object> o = new Dictionary<string, object> {
                    { 
                        "Tier",
                        Party.Tier
                    }
                };
                if (<>f__am$cacheD == null)
                {
                    <>f__am$cacheD = delegate (RunCloudScriptResult result) {
                        Game.Network.UpdateNetworkConnection();
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        if (result.ResultsEncoded != null)
                        {
                            dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ResultsEncoded);
                        }
                        else
                        {
                            UnityEngine.Debug.Log("Results were null!");
                            return;
                        }
                        UnityEngine.Debug.Log("getCardCollection(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                        string str = string.Empty;
                        if (dictionary.TryGetValue("collection", out str))
                        {
                            Collection.Load(JsonConvert.DeserializeObject<List<CollectionEntry>>(str));
                        }
                        GuiWindow current = GuiWindow.Current;
                        if (current != null)
                        {
                            current.Refresh();
                        }
                    };
                }
                this.RunCloudScript("getCardCollection", o, <>f__am$cacheD, null);
                break;
            }
            case NetworkManager.UserField.ChestsAndGold:
                this.RunCloudScript("getChestsAndGold", null, delegate (RunCloudScriptResult result) {
                    object obj2;
                    object obj3;
                    Game.Network.UpdateNetworkConnection();
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    if (result.ResultsEncoded != null)
                    {
                        dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Results were null!");
                        return;
                    }
                    UnityEngine.Debug.Log("getChestsAndGold(result.ActionLog:" + result.ActionLog + ") AND (result.Results:" + result.Results.ToString() + ")");
                    bool flag = false;
                    bool flag2 = false;
                    GuiWindowStore window = UI.Window as GuiWindowStore;
                    if (dictionary.TryGetValue("chests", out obj2))
                    {
                        int num = Convert.ToInt32(obj2);
                        if (this.CurrentUser.Chests < num)
                        {
                            flag2 = true;
                        }
                        this.CurrentUser.Chests = num;
                    }
                    if (dictionary.TryGetValue("gold", out obj3))
                    {
                        int num2 = Convert.ToInt32(obj3);
                        if (this.CurrentUser.Gold < num2)
                        {
                            flag = true;
                        }
                        this.CurrentUser.Gold = num2;
                    }
                    if (window != null)
                    {
                        window.Refresh();
                        if (flag)
                        {
                            window.overlayPanel.ShowGoldVFX();
                        }
                        if (flag2)
                        {
                            window.overlayPanel.ShowChestVFX();
                        }
                    }
                }, null);
                break;

            case NetworkManager.UserField.GameSaves:
                this.RunCloudScript("getGameSaves", null, delegate (RunCloudScriptResult result) {
                    object obj2;
                    Game.Network.UpdateNetworkConnection();
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    if (result.ResultsEncoded != null)
                    {
                        dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Results were null!");
                        return;
                    }
                    UnityEngine.Debug.Log("getGameSaves(result.ActionLog:" + result.ActionLog + ") AND (result.Results:" + result.Results.ToString() + ")");
                    if (dictionary.TryGetValue("savedGames", out obj2))
                    {
                        List<int> list = new List<int>();
                        for (int i = GameDirectory.FirstSlot; i <= GameDirectory.LastSlot; i++)
                        {
                            list.Add(i);
                        }
                        list.Add(Constants.SAVE_SLOT_QUEST);
                        List<GameSaveItem> list2 = JsonConvert.DeserializeObject<List<GameSaveItem>>((string) obj2);
                        List<GameSaveFile> saveFiles = new List<GameSaveFile>();
                        Dictionary<string, int> saveGamesToBeSyncedLocally = new Dictionary<string, int>();
                        if (list2.Count > 0)
                        {
                            for (int j = list.Count - 1; j >= 0; j--)
                            {
                                if (GameDirectory.Empty(list[j]))
                                {
                                    continue;
                                }
                                GameSaveFile file = new GameSaveFile(list[j]);
                                if (file.Header != null)
                                {
                                    bool flag = false;
                                    for (int m = 0; m < list2.Count; m++)
                                    {
                                        if (list2[m].GUID == file.Header.GUID)
                                        {
                                            flag = true;
                                            if (list2[m].VersionStamp > file.Header.VersionStamp)
                                            {
                                                saveGamesToBeSyncedLocally.Add(list2[m].GUID, list[j]);
                                            }
                                            else if (list2[m].VersionStamp < file.Header.VersionStamp)
                                            {
                                                saveFiles.Add(file);
                                            }
                                            list2.RemoveAt(m);
                                            break;
                                        }
                                    }
                                    if (!flag)
                                    {
                                        saveFiles.Add(file);
                                    }
                                    list.RemoveAt(j);
                                }
                            }
                            for (int k = 0; k < list.Count; k++)
                            {
                                if (list[k] == Constants.SAVE_SLOT_QUEST)
                                {
                                    GameSaveItem item = new GameSaveItem();
                                    for (int n = 0; n < list2.Count; n++)
                                    {
                                        if ((list2[n].TimeStamp > item.TimeStamp) && (list2[n].GameMode == 1))
                                        {
                                            item = list2[n];
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(item.GUID))
                                    {
                                        saveGamesToBeSyncedLocally.Add(item.GUID, list[k]);
                                        list2.Remove(item);
                                    }
                                }
                                else
                                {
                                    GameSaveItem item2 = new GameSaveItem();
                                    for (int num6 = 0; num6 < list2.Count; num6++)
                                    {
                                        if ((list2[num6].TimeStamp > item2.TimeStamp) && (list2[num6].GameMode == 0))
                                        {
                                            item2 = list2[num6];
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(item2.GUID) && (list[k] != Constants.SAVE_SLOT_QUEST))
                                    {
                                        saveGamesToBeSyncedLocally.Add(item2.GUID, list[k]);
                                        list2.Remove(item2);
                                    }
                                }
                            }
                            base.StartCoroutine(this.SynchronizeGameSavesOnline(saveFiles));
                            this.SynchronizeGameSaveLocally(saveGamesToBeSyncedLocally);
                        }
                        else
                        {
                            for (int num7 = 0; num7 < list.Count; num7++)
                            {
                                if (!GameDirectory.Empty(list[num7]))
                                {
                                    GameSaveFile file2 = new GameSaveFile(list[num7]);
                                    if (file2.Header != null)
                                    {
                                        saveFiles.Add(file2);
                                    }
                                }
                            }
                            base.StartCoroutine(this.SynchronizeGameSavesOnline(saveFiles));
                        }
                    }
                }, null);
                break;
        }
    }

    public override void RemoveCardFromCollection(string cardId, int quantity)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "CardId",
                    cardId
                },
                { 
                    "Quantity",
                    quantity
                },
                { 
                    "Tier",
                    Party.Tier
                }
            };
            this.RunCloudScript("removeCardFromCollection", o, delegate (RunCloudScriptResult result) {
                object obj2;
                Game.Network.UpdateNetworkConnection();
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                if (result.ResultsEncoded != null)
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
                }
                else
                {
                    UnityEngine.Debug.Log("Results were null!");
                    return;
                }
                UnityEngine.Debug.Log("onLogin(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                if (dictionary.TryGetValue("gold", out obj2))
                {
                    int gp = Convert.ToInt32(obj2);
                    if (gp > 0)
                    {
                        Game.UI.GoldPanel.Show(gp);
                    }
                }
                this.RefreshUserData(NetworkManager.UserField.Collection);
                this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
            }, null);
        }
    }

    private void RunCloudScript(string actionId)
    {
        this.RunCloudScript(actionId, null, null, null);
    }

    private void RunCloudScript(string actionId, object o, PlayFabClientAPI.RunCloudScriptCallback callback = null, ErrorCallback errorCallback = null)
    {
        <RunCloudScript>c__AnonStorey127 storey = new <RunCloudScript>c__AnonStorey127 {
            actionId = actionId,
            o = o,
            callback = callback,
            errorCallback = errorCallback
        };
        UnityAction action = new UnityAction(storey.<>m__161);
        this.GetCloudScriptEndpoint(action);
    }

    private void SaveChunk(string guid, string chunk)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            this.saveChunkLock = true;
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "GUID",
                    guid
                },
                { 
                    "Chunk",
                    chunk
                }
            };
            this.RunCloudScript("saveChunk", o, delegate (RunCloudScriptResult result) {
                this.saveChunkLock = false;
                Game.Network.UpdateNetworkConnection();
                UnityEngine.Debug.Log("saveChunk(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
            }, null);
        }
    }

    [DebuggerHidden]
    private IEnumerator SendSaveGameByChunks(GameSaveFile saveFile) => 
        new <SendSaveGameByChunks>c__Iterator97 { 
            saveFile = saveFile,
            <$>saveFile = saveFile,
            <>f__this = this
        };

    [DebuggerHidden]
    private IEnumerator ShowStoreKick() => 
        new <ShowStoreKick>c__Iterator96();

    protected override void Start()
    {
        this.Login();
    }

    public override void SynchronizeConquests(Dictionary<string, int> conquestTable)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "Conquest",
                    JsonConvert.SerializeObject(conquestTable)
                }
            };
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = delegate (RunCloudScriptResult result) {
                    Game.Network.UpdateNetworkConnection();
                    UnityEngine.Debug.Log("syncConquestData(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
                };
            }
            this.RunCloudScript("syncConquestData", o, <>f__am$cacheE, null);
        }
    }

    public override void SynchronizeGameSave(GameSaveFile save)
    {
        if ((this.Connected && !this.OutOfDate) && this.HasNetworkConnection)
        {
            this.latestSaveAwaitingSync = save;
            this.saveTimer = this.saveTimerThreshold;
        }
    }

    private void SynchronizeGameSaveLocally(Dictionary<string, int> saveGamesToBeSyncedLocally)
    {
        <SynchronizeGameSaveLocally>c__AnonStorey124 storey = new <SynchronizeGameSaveLocally>c__AnonStorey124 {
            saveGamesToBeSyncedLocally = saveGamesToBeSyncedLocally
        };
        if ((this.Connected && !this.OutOfDate) && (this.HasNetworkConnection && (storey.saveGamesToBeSyncedLocally.Count > 0)))
        {
            Dictionary<string, object> o = new Dictionary<string, object> {
                { 
                    "Saves",
                    JsonConvert.SerializeObject(storey.saveGamesToBeSyncedLocally)
                }
            };
            this.RunCloudScript("synchronizeGameSaveLocally", o, new PlayFabClientAPI.RunCloudScriptCallback(storey.<>m__149), null);
        }
    }

    [DebuggerHidden]
    private IEnumerator SynchronizeGameSavesOnline(List<GameSaveFile> saveFiles) => 
        new <SynchronizeGameSavesOnline>c__Iterator98 { 
            saveFiles = saveFiles,
            <$>saveFiles = saveFiles,
            <>f__this = this
        };

    protected override void Update()
    {
        this.m_recheckTimer -= Time.deltaTime;
        if (this.m_recheckTimer <= 0f)
        {
            this.m_recheckTimer = 60f;
            this.PollNetworkConnection();
        }
        this.saveTimer -= Time.deltaTime;
        if ((this.saveTimer <= 0f) && (this.latestSaveAwaitingSync != null))
        {
            base.StartCoroutine(this.SendSaveGameByChunks(this.latestSaveAwaitingSync));
            this.latestSaveAwaitingSync = null;
        }
    }

    public void UpdateDisplayName(string name)
    {
        PlayFabLoginCalls.UpdateDisplayName(name);
    }

    public override void UpdateNetworkConnection()
    {
        if (!this._hasNetworkConnection)
        {
            this.AlertNetworkConnectionFound();
        }
        this._hasNetworkConnection = true;
    }

    public override void UpdateNetworkConnection(string error)
    {
        if ((error == null) || (error == string.Empty))
        {
            if (!this._hasNetworkConnection)
            {
                this.AlertNetworkConnectionFound();
            }
            this._hasNetworkConnection = true;
        }
        error = error.ToLower();
        if ((error.Contains("failed to connect") || error.Contains("timeout")) || ((error.Contains("resolve host") || error.Contains("503")) || error.Contains("nsurler")))
        {
            if (this._hasNetworkConnection)
            {
                this.AlertNetworkConnectionLost();
            }
            this._hasNetworkConnection = false;
        }
        else
        {
            if (!this._hasNetworkConnection)
            {
                this.AlertNetworkConnectionFound();
            }
            this._hasNetworkConnection = true;
        }
    }

    public override void ValidateReceipt(Product product)
    {
        if (this.HasNetworkConnection)
        {
            UI.Busy = true;
            if (!this.Connected)
            {
                if (GuiWindow.Current is GuiWindowStore)
                {
                    (GuiWindow.Current as GuiWindowStore).Refresh();
                }
                if (LicenseManager.GetIsBundle(product.definition.id))
                {
                    LicenseManager.GrantBundle(product.definition.id);
                }
                UI.Busy = false;
            }
            else
            {
                this.LogPurchase(product.definition.id, "Receipt");
                JObject obj2 = JObject.Parse(product.receipt);
                ValidateGooglePlayPurchaseRequest request = new ValidateGooglePlayPurchaseRequest();
                JObject obj3 = JObject.Parse(obj2["Payload"].ToString());
                request.ReceiptJson = obj3["json"].ToString();
                request.CurrencyCode = "USD";
                request.Signature = obj3["signature"].ToString();
                if (Settings.DebugMode)
                {
                    PlayFabClientAPI.ValidateGooglePlayPurchase(request, new PlayFabClientAPI.ValidateGooglePlayPurchaseCallback(this.OnValidateGooglePlayReceipt), new ErrorCallback(this.FailedDebug), null);
                }
                else
                {
                    PlayFabClientAPI.ValidateGooglePlayPurchase(request, new PlayFabClientAPI.ValidateGooglePlayPurchaseCallback(this.OnValidateGooglePlayReceipt), new ErrorCallback(PlayFabManager.OnPlayFabError), null);
                }
            }
        }
    }

    public override List<CatalogItem> Catalog
    {
        get
        {
            if (this.catalog == null)
            {
                this.catalog = new List<CatalogItem>();
            }
            return this.catalog;
        }
        protected set
        {
            this.catalog = value;
        }
    }

    public override bool Connected =>
        PlayFabClientAPI.IsClientLoggedIn();

    public override User CurrentUser
    {
        get
        {
            if (this.currentUser == null)
            {
                this.currentUser = new PlayFabUser();
            }
            return this.currentUser;
        }
    }

    public override bool HasNetworkConnection =>
        this._hasNetworkConnection;

    public override bool OutOfDate
    {
        get => 
            this._clientOutOfDate;
        protected set
        {
            this._clientOutOfDate = value;
        }
    }

    private string PlayFabDisplayName
    {
        get
        {
            if (PlayFabLoginCalls.LoggedInUserInfo != null)
            {
                return PlayFabLoginCalls.LoggedInUserInfo.TitleInfo.DisplayName;
            }
            return "User Not Logged In Yet";
        }
    }

    private string PlayFabID
    {
        get
        {
            if (PlayFabLoginCalls.LoggedInUserInfo != null)
            {
                return PlayFabLoginCalls.LoggedInUserInfo.PlayFabId;
            }
            return "User Not Logged In Yet";
        }
    }

    [CompilerGenerated]
    private sealed class <GetCloudScriptEndpoint>c__AnonStorey128
    {
        internal UnityAction callback;

        internal void <>m__162(GetCloudScriptUrlResult result)
        {
            if (this.callback != null)
            {
                this.callback();
            }
        }
    }

    [CompilerGenerated]
    private sealed class <GetScenarioGold>c__AnonStorey126
    {
        internal PlayFabManager <>f__this;
        internal NetworkManager.GetScenarioGoldCallBack callback;

        internal void <>m__15C(RunCloudScriptResult result)
        {
            object obj2;
            Game.Network.UpdateNetworkConnection();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            if (result.ResultsEncoded != null)
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ResultsEncoded);
            }
            else
            {
                UnityEngine.Debug.Log("Results were null!");
                return;
            }
            UnityEngine.Debug.Log("onLogin..Got log entries:");
            UnityEngine.Debug.Log(result.ActionLog);
            UnityEngine.Debug.Log("and return value: " + result.Results.ToString());
            if (dictionary.TryGetValue("gold", out obj2))
            {
                int gold = Convert.ToInt32(obj2);
                this.<>f__this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
                if (this.callback != null)
                {
                    this.callback(gold);
                }
            }
        }
    }

    [CompilerGenerated]
    private sealed class <OpenTreasureChest>c__AnonStorey125
    {
        internal PlayFabManager <>f__this;
        internal AudioClip clip;

        internal void <>m__155(RunCloudScriptResult result)
        {
            UI.Sound.Stop(this.clip);
            Game.Network.UpdateNetworkConnection();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (result.ResultsEncoded != null)
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ResultsEncoded);
            }
            else
            {
                UnityEngine.Debug.Log("Results were null!");
                UI.Busy = false;
                return;
            }
            UnityEngine.Debug.Log("openTreasureChest(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
            string str = string.Empty;
            if (dictionary.TryGetValue("rewards", out str))
            {
                string[] cards = JsonConvert.DeserializeObject<string[]>(str);
                GuiWindowStore window = UI.Window as GuiWindowStore;
                if ((window != null) && (window.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open))
                {
                    window.treasureRevealPanel.ProcessRewards(cards);
                }
            }
            this.<>f__this.RefreshUserData(NetworkManager.UserField.ChestsAndGold);
            this.<>f__this.RefreshUserData(NetworkManager.UserField.Collection);
        }
    }

    [CompilerGenerated]
    private sealed class <RunCloudScript>c__AnonStorey127
    {
        internal string actionId;
        internal PlayFabClientAPI.RunCloudScriptCallback callback;
        internal ErrorCallback errorCallback;
        internal object o;

        internal void <>m__161()
        {
            RunCloudScriptRequest request = new RunCloudScriptRequest {
                ActionId = this.actionId
            };
            if (this.o != null)
            {
                request.Params = this.o;
            }
            if (this.callback == null)
            {
                if (this.errorCallback == null)
                {
                    PlayFabClientAPI.RunCloudScript(request, result => UnityEngine.Debug.Log(this.actionId + "(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")"), new ErrorCallback(PlayFabManager.OnPlayFabError), null);
                }
                else
                {
                    PlayFabClientAPI.RunCloudScript(request, result => UnityEngine.Debug.Log(this.actionId + "(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")"), this.errorCallback, null);
                }
            }
            else if (this.errorCallback == null)
            {
                PlayFabClientAPI.RunCloudScript(request, this.callback, new ErrorCallback(PlayFabManager.OnPlayFabError), null);
            }
            else
            {
                PlayFabClientAPI.RunCloudScript(request, this.callback, this.errorCallback, null);
            }
        }

        internal void <>m__164(RunCloudScriptResult result)
        {
            UnityEngine.Debug.Log(this.actionId + "(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
        }

        internal void <>m__165(RunCloudScriptResult result)
        {
            UnityEngine.Debug.Log(this.actionId + "(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
        }
    }

    [CompilerGenerated]
    private sealed class <SendSaveGameByChunks>c__Iterator97 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameSaveFile <$>saveFile;
        internal PlayFabManager <>f__this;
        internal int <i>__2;
        internal int <i>__3;
        internal string <save>__0;
        internal List<string> <saveChunks>__1;
        internal GameSaveFile saveFile;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if ((((this.<>f__this.Connected && !this.<>f__this.OutOfDate) && (this.<>f__this.HasNetworkConnection && !this.<>f__this.saveLock)) && (this.saveFile != null)) && (this.saveFile.Header != null))
                    {
                        this.<>f__this.saveLock = true;
                        this.<save>__0 = JsonConvert.SerializeObject(this.saveFile);
                        this.<saveChunks>__1 = new List<string>();
                        this.<i>__2 = 0;
                        while (this.<i>__2 < this.<save>__0.Length)
                        {
                            if ((this.<i>__2 + this.<>f__this.chunkLimit) < this.<save>__0.Length)
                            {
                                this.<saveChunks>__1.Add(this.<save>__0.Substring(this.<i>__2, this.<>f__this.chunkLimit));
                            }
                            else
                            {
                                this.<saveChunks>__1.Add(this.<save>__0.Substring(this.<i>__2));
                            }
                            this.<i>__2 += this.<>f__this.chunkLimit;
                        }
                        this.<i>__3 = 0;
                        goto Label_01BF;
                    }
                    goto Label_020F;

                case 1:
                    break;

                case 2:
                    goto Label_01ED;

                default:
                    goto Label_020F;
            }
        Label_0175:
            if (this.<>f__this.saveChunkLock)
            {
                this.$current = null;
                this.$PC = 1;
                goto Label_0211;
            }
            this.<>f__this.SaveChunk(this.saveFile.Header.GUID, this.<saveChunks>__1[this.<i>__3]);
            this.<i>__3++;
        Label_01BF:
            if (this.<i>__3 < this.<saveChunks>__1.Count)
            {
                goto Label_0175;
            }
        Label_01ED:
            while (this.<>f__this.saveChunkLock)
            {
                this.$current = null;
                this.$PC = 2;
                goto Label_0211;
            }
            this.<>f__this.MakeTempSavesOfficial();
            this.$PC = -1;
        Label_020F:
            return false;
        Label_0211:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }

    [CompilerGenerated]
    private sealed class <ShowStoreKick>c__Iterator96 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.$current = new WaitForSeconds(3f);
                    this.$PC = 1;
                    return true;

                case 1:
                    Game.UI.Toast.Show("Kicked from the store due to losing connection!");
                    this.$PC = -1;
                    break;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }

    [CompilerGenerated]
    private sealed class <SynchronizeGameSaveLocally>c__AnonStorey124
    {
        internal Dictionary<string, int> saveGamesToBeSyncedLocally;

        internal void <>m__149(RunCloudScriptResult result)
        {
            string str;
            Game.Network.UpdateNetworkConnection();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (result.ResultsEncoded != null)
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ResultsEncoded);
            }
            else
            {
                UnityEngine.Debug.Log("Results were null!");
                return;
            }
            if (dictionary.TryGetValue("saveFiles", out str))
            {
                List<GameSaveFile> list = JsonConvert.DeserializeObject<List<GameSaveFile>>(str);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Header != null)
                    {
                        string gUID = list[i].Header.GUID;
                        int slot = this.saveGamesToBeSyncedLocally[gUID];
                        if (list[i].Header.GameMode == GameModeType.Quest)
                        {
                            GameSaveFile file = new GameSaveFile(Constants.SAVE_SLOT_QUEST);
                            if (file.Header != null)
                            {
                                file.LoadFromOnlineSave(list[i]);
                                file.SaveInternal();
                            }
                        }
                        else if (list[i].Header.GameMode == GameModeType.Story)
                        {
                            GameSaveFile file2 = new GameSaveFile(slot);
                            if (file2.Header != null)
                            {
                                file2.LoadFromOnlineSave(list[i]);
                                file2.SaveInternal();
                            }
                        }
                    }
                }
            }
            UnityEngine.Debug.Log("synchronizeGameSaveLocally(result.ActionLog:" + result.ActionLog + ") AND return value(result.Results:" + result.Results.ToString() + ")");
        }
    }

    [CompilerGenerated]
    private sealed class <SynchronizeGameSavesOnline>c__Iterator98 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal List<GameSaveFile> <$>saveFiles;
        internal PlayFabManager <>f__this;
        internal int <i>__0;
        internal List<GameSaveFile> saveFiles;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if ((this.<>f__this.Connected && !this.<>f__this.OutOfDate) && this.<>f__this.HasNetworkConnection)
                    {
                        this.<i>__0 = 0;
                        goto Label_00C0;
                    }
                    goto Label_00DD;

                case 1:
                    break;

                default:
                    goto Label_00DD;
            }
        Label_007A:
            if (this.<>f__this.saveLock)
            {
                this.$current = null;
                this.$PC = 1;
                return true;
            }
            this.<>f__this.StartCoroutine(this.<>f__this.SendSaveGameByChunks(this.saveFiles[this.<i>__0]));
            this.<i>__0++;
        Label_00C0:
            if (this.<i>__0 < this.saveFiles.Count)
            {
                goto Label_007A;
            }
            this.$PC = -1;
        Label_00DD:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

