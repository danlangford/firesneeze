using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GuiPanelStoreAdventures : GuiPanelBackStack
{
    private Dictionary<string, Adventure> adventures;
    [Tooltip("reference to the buy button in this panel")]
    public GuiButton buyButton;
    private Box currentBox;
    private LicenseOperationType currentOperation;
    [Tooltip("reference to the discount object in this panel")]
    public GameObject discountObject;
    [Tooltip("reference to the ezren sidebar art renderer")]
    public SpriteRenderer ezrenSpriteRenderer;
    [Tooltip("reference to the gold sprite art renderer")]
    public SpriteRenderer goldSpriteRenderer;
    [Tooltip("reference to the hilite object in this panel")]
    public GameObject hiliteObject;
    private List<LicenseIcon> icons;
    private bool isClosing;
    [Tooltip("reference to the selected license description field on this panel")]
    public GuiLabel licenseDescriptionLabel;
    [Tooltip("reference to the selected license image on this panel")]
    public GuiImage licenseImage;
    [Tooltip("reference to the selected license name field on this panel")]
    public GuiLabel licenseNameLabel;
    [Tooltip("reference to the selected license price field on this panel")]
    public GuiLabel licensePriceLabel;
    [Tooltip("left margin of items in scroll list (half of the item width)")]
    public float listMarginLeft;
    [Tooltip("top margin of items in the scroll list (half of the item height)")]
    public float listMarginTop;
    [Tooltip("reference to the message label in this panel")]
    public GuiLabel messageLine;
    [Tooltip("reference to the icon and label pairs in this panel")]
    public List<CardTypePair> pairs;
    [Tooltip("reference to the preview button in this panel")]
    public GuiButton previewButton;
    [Tooltip("reference to the preview panel in this scene")]
    public GuiPanelStorePreview previewPanel;
    [Tooltip("reference to the scrolling region on this panel")]
    public GuiScrollRegion scroller;
    private static LicenseType selectedFilter;
    private static string selectedLicense;
    [Tooltip("reference to the store manager window from this panel")]
    public GuiWindowStore StoreManager;
    private TKTapRecognizer tapRecognizer;

    public override void Clear()
    {
        for (int i = 0; i < this.icons.Count; i++)
        {
            UnityEngine.Object.Destroy(this.icons[i].gameObject);
        }
        this.icons.Clear();
    }

    private void ClearSelection()
    {
        selectedLicense = null;
        this.licenseNameLabel.Clear();
        this.licenseDescriptionLabel.Clear();
        this.previewButton.Show(false);
        this.hiliteObject.SetActive(false);
        this.messageLine.Clear();
    }

    private void DisplayAdventureDetails(string productCode)
    {
        if ((productCode == Constants.IAP_LICENSE_BUNDLE_ROTR) || (productCode == Constants.IAP_LICENSE_DUMMY))
        {
            this.ShowPairs(false);
        }
        else
        {
            this.ShowPairs(true);
            this.currentBox.Clear();
            string str = "Tables/BoxTable_";
            if (productCode == Constants.IAP_LICENSE_AD11)
            {
                this.currentBox.Load(str + "11", "1");
            }
            else if (productCode == Constants.IAP_LICENSE_AD12)
            {
                this.currentBox.Load(str + "12", "2");
            }
            else if (productCode == Constants.IAP_LICENSE_AD13)
            {
                this.currentBox.Load(str + "13", "3");
            }
            else if (productCode == Constants.IAP_LICENSE_AD14)
            {
                this.currentBox.Load(str + "14", "4");
            }
            else if (productCode == Constants.IAP_LICENSE_AD15)
            {
                this.currentBox.Load(str + "15", "5");
            }
            else if (productCode == Constants.IAP_LICENSE_AD16)
            {
                this.currentBox.Load(str + "16", "6");
            }
            for (CardType type = CardType.Ally; type <= CardType.Scenario; type += 1)
            {
                this.SetCardTypeQuantity(type);
            }
        }
    }

    private string GetUpperCaseKey(string key)
    {
        if (key.Equals(Constants.IAP_LICENSE_AD11))
        {
            return "AD11_BurntOfferings";
        }
        if (key.Equals(Constants.IAP_LICENSE_AD12))
        {
            return "AD12_TheSkinsawMurders";
        }
        if (key.Equals(Constants.IAP_LICENSE_AD13))
        {
            return "AD13_TheHookMountainMassacre";
        }
        if (key.Equals(Constants.IAP_LICENSE_AD14))
        {
            return "AD14_FortressOfTheStoneGiants";
        }
        if (key.Equals(Constants.IAP_LICENSE_AD15))
        {
            return "AD15_SinsOfTheSaviors";
        }
        if (key.Equals(Constants.IAP_LICENSE_AD16))
        {
            return "AD16_SpiresOfXin-Shalast";
        }
        return string.Empty;
    }

    private void HilightLicenseIcon(string id)
    {
        this.hiliteObject.SetActive(false);
        for (int i = 0; i < this.icons.Count; i++)
        {
            if (this.icons[i].ID == id)
            {
                this.hiliteObject.transform.position = this.icons[i].transform.position;
                this.hiliteObject.SetActive(true);
            }
        }
    }

    public override void Initialize()
    {
        this.currentBox = new Box("StoreAdventureCurrentBox");
        this.icons = new List<LicenseIcon>(Constants.NUM_IAP_ADVENTURES);
        this.adventures = new Dictionary<string, Adventure>();
        this.scroller.Initialize();
        this.previewPanel.Initialize();
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 3;
        this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.Show(false);
    }

    private Sprite LoadLicenseArt(string id)
    {
        string path = "Blueprints/Icons/Licenses/" + id;
        SpriteRenderer renderer = Game.Cache.Get<SpriteRenderer>(path);
        if (renderer != null)
        {
            return renderer.sprite;
        }
        return null;
    }

    private void LoadLicenses(LicenseType licenseType, ref float totalHeight)
    {
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            string iD = LicenseTable.Key(i);
            LicenseTableEntry entry = LicenseTable.Get(iD);
            if (((entry != null) && (entry.Type == licenseType)) && ((iD != Constants.IAP_LICENSE_DUMMY) || Settings.DebugMode))
            {
                LicenseIcon item = LicenseIcon.Create(entry);
                if (item != null)
                {
                    item.ID = iD;
                    item.LoadImage(entry.Icon);
                    item.SetBackground();
                    this.icons.Add(item);
                    Adventure adventure = null;
                    if (!this.adventures.TryGetValue(iD, out adventure) && (entry.Type == LicenseType.Adventure))
                    {
                        adventure = AdventureTable.Create(this.GetUpperCaseKey(iD));
                        this.adventures.Add(iD, adventure);
                    }
                    this.scroller.Add(item.transform, this.listMarginLeft, totalHeight);
                    if (iD == Constants.IAP_LICENSE_AD11)
                    {
                        this.ShowDiscountIcon(iD);
                    }
                    totalHeight += item.Height;
                }
            }
        }
    }

    private void OnBuyButtonPushed()
    {
        if (!UI.Busy)
        {
            if ((selectedLicense == Constants.IAP_LICENSE_BUNDLE_ROTR) || (selectedLicense == Constants.IAP_LICENSE_DUMMY))
            {
                if ((selectedLicense == Constants.IAP_LICENSE_BUNDLE_ROTR) && Store.CanPurchase(selectedLicense, Store.CurrencyCategory.RM, true, 1))
                {
                    this.StoreManager.ShowPendingPanelType(this.licenseNameLabel.Text, LicenseManager.GetProduct(selectedLicense).Price, 1, GuiPanelStorePendingPurchase.PendingType.ConfirmationBundle, this);
                }
                else if ((selectedLicense == Constants.IAP_LICENSE_DUMMY) && Store.CanPurchase(selectedLicense, Store.CurrencyCategory.RM, false, 1))
                {
                    this.StoreManager.ShowPendingPanelType(this.licenseNameLabel.Text, LicenseManager.GetProduct(selectedLicense).Price, 1, GuiPanelStorePendingPurchase.PendingType.Confirmation, this);
                }
            }
            else if (Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold)
            {
                this.StoreManager.ShowInsufficientGoldPanel();
            }
            else if (Store.CanPurchase(selectedLicense, Store.CurrencyCategory.Gold, true, 1))
            {
                this.StoreManager.ShowPendingPanelType(this.licenseNameLabel.Text, string.Empty + Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold), 1, GuiPanelStorePendingPurchase.PendingType.Confirmation, this);
            }
        }
    }

    private void OnCloseButtonPushed()
    {
        if (!this.isClosing)
        {
            this.isClosing = true;
            UI.Busy = true;
            this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Start);
        }
    }

    private void OnExitButtonPushed()
    {
        if (!this.isClosing)
        {
            this.isClosing = true;
            UI.Busy = true;
            this.StoreManager.CloseStore();
        }
    }

    protected virtual void OnGuiTap(Vector2 touchPos)
    {
        if (!UI.Busy)
        {
            Vector2 origin = base.ScreenToWorldPoint(touchPos);
            if (this.scroller.Contains((Vector3) origin))
            {
                RaycastHit2D hitd = Physics2D.Raycast(origin, Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_ICON);
                if (hitd != 0)
                {
                    LicenseIcon component = hitd.collider.transform.GetComponent<LicenseIcon>();
                    if (component != null)
                    {
                        component.Tap();
                        this.Select(component.ID);
                    }
                }
            }
        }
    }

    private void OnPreviewButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.previewPanel.Show(selectedLicense);
        }
    }

    private void OnRestoreButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.messageLine.Text = StringTableManager.GetHelperText(0x37);
            this.currentOperation = LicenseOperationType.Restore;
            LicenseManager.RestoreLicenses();
        }
    }

    public void ProceedWithPurchase()
    {
        UI.Busy = true;
        this.messageLine.Text = StringTableManager.GetHelperText(0x38);
        this.currentOperation = LicenseOperationType.Purchase;
        if ((selectedLicense == Constants.IAP_LICENSE_BUNDLE_ROTR) || (selectedLicense == Constants.IAP_LICENSE_DUMMY))
        {
            Game.Network.PurchaseItemUsingRealMoney(selectedLicense);
        }
        else
        {
            Store.PurchaseItem(selectedLicense, Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold));
        }
    }

    public override void Refresh()
    {
        this.Clear();
        float listMarginTop = this.listMarginTop;
        this.LoadLicenses(LicenseType.Special, ref listMarginTop);
        this.LoadLicenses(LicenseType.Adventure, ref listMarginTop);
        float y = (this.scroller.Min.y + listMarginTop) - this.scroller.Size.y;
        this.scroller.Max = new Vector2(0f, y);
        this.scroller.Top();
        this.buyButton.Show(true);
        this.buyButton.Disable(!LicenseManager.GetIsAvailable(selectedLicense, false) || LicenseManager.GetIsLicensed(selectedLicense));
        if (selectedLicense == Constants.IAP_LICENSE_BUNDLE_ROTR)
        {
            this.buyButton.TextTint(this.StoreManager.ColorSufficientGold);
        }
        else
        {
            this.buyButton.TextTint((Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold) ? this.StoreManager.ColorInsufficientGold : this.StoreManager.ColorSufficientGold);
        }
        AlertManager.SeenAlert(AlertManager.AlertType.SeenStoreAdventures);
        AlertManager.HandleAlerts();
    }

    private string Select(LicenseType category)
    {
        selectedFilter = category;
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            string iD = LicenseTable.Key(i);
            LicenseTableEntry entry = LicenseTable.Get(iD);
            if ((entry != null) && (entry.Type == selectedFilter))
            {
                return iD;
            }
        }
        return null;
    }

    private void Select(string productCode)
    {
        LicenseTableEntry entry = LicenseTable.Get(productCode);
        if (entry != null)
        {
            selectedLicense = productCode;
            this.HilightLicenseIcon(selectedLicense);
            this.licenseImage.Image = this.LoadLicenseArt(entry.Art);
            this.licenseNameLabel.Text = entry.Name;
            this.licenseDescriptionLabel.Text = entry.Description;
            LicenseProduct product = LicenseManager.GetProduct(productCode);
            if (product != null)
            {
                this.licensePriceLabel.Text = product.Price;
            }
            else
            {
                this.licensePriceLabel.Clear();
            }
            if ((productCode != Constants.IAP_LICENSE_BUNDLE_ROTR) && (productCode != Constants.IAP_LICENSE_DUMMY))
            {
                this.licensePriceLabel.Text = Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) + " Gold";
            }
            if (LicenseManager.GetIsLicensed(productCode) && (productCode != Constants.IAP_LICENSE_DUMMY))
            {
                this.licensePriceLabel.Text = "Owned";
            }
            if (!LicenseManager.GetIsAvailable(productCode, false))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Coming");
                builder.Append("Soon");
                this.licensePriceLabel.Text = builder.ToString();
            }
            this.buyButton.Show(true);
            if (productCode == Constants.IAP_LICENSE_DUMMY)
            {
                this.buyButton.Disable(!LicenseManager.GetIsAvailable(productCode, false));
            }
            else
            {
                this.buyButton.Disable(!LicenseManager.GetIsAvailable(productCode, false) || LicenseManager.GetIsLicensed(productCode));
            }
            if ((productCode != Constants.IAP_LICENSE_BUNDLE_ROTR) && (productCode != Constants.IAP_LICENSE_DUMMY))
            {
                this.buyButton.TextTint((Store.GetPrice(productCode, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold) ? this.StoreManager.ColorInsufficientGold : this.StoreManager.ColorSufficientGold);
            }
            else
            {
                this.buyButton.TextTint(this.StoreManager.ColorSufficientGold);
            }
            this.previewButton.Show(entry.Preview.Length > 0);
            this.DisplayAdventureDetails(productCode);
            for (int i = 0; i < LicenseTable.Count; i++)
            {
                string iD = LicenseTable.Key(i);
                LicenseTableEntry entry2 = LicenseTable.Get(iD);
                if (((entry2 != null) && (entry2.Type == LicenseType.Adventure)) && !entry2.Available)
                {
                    if (iD.ToLower().Equals(productCode.ToLower()))
                    {
                        StringBuilder builder2 = new StringBuilder();
                        builder2.AppendLine("Coming");
                        if ((entry2.Date != null) && (entry2.Date.Length > 0))
                        {
                            builder2.Append(entry2.Date);
                        }
                        else
                        {
                            builder2.Append("Soon");
                        }
                        this.licensePriceLabel.Text = builder2.ToString();
                    }
                    break;
                }
            }
        }
        else
        {
            this.ClearSelection();
        }
    }

    private void SetCardTypeQuantity(CardType ct)
    {
        if (this.pairs != null)
        {
            int length = this.currentBox.GetCardList(ct, CardRankType.None).Length;
            for (int i = 0; i < this.pairs.Count; i++)
            {
                if (this.pairs[i].TypeOfCard == ct)
                {
                    this.pairs[i].Label.Text = length + string.Empty;
                    break;
                }
            }
        }
    }

    public void SetInitialProduct(string productCode)
    {
        this.Select(productCode);
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.scroller.Pause(!isVisible);
        this.tapRecognizer.enabled = isVisible;
        this.currentOperation = LicenseOperationType.None;
        if (isVisible)
        {
            if (selectedLicense == null)
            {
                selectedFilter = LicenseType.Special;
                selectedLicense = this.Select(selectedFilter);
                selectedFilter = LicenseType.Adventure;
            }
            this.Refresh();
            this.Select(selectedLicense);
        }
        else
        {
            this.previewPanel.Show(false);
            this.ClearSelection();
            this.Clear();
            UI.Busy = false;
        }
    }

    public void Show(string productCode, LicenseType category)
    {
        selectedLicense = productCode;
        selectedFilter = category;
        this.Show(true);
    }

    private void ShowDiscountIcon(string id)
    {
        if (this.discountObject != null)
        {
            this.discountObject.SetActive(false);
            for (int i = 0; i < this.icons.Count; i++)
            {
                if (this.icons[i].ID == id)
                {
                    this.discountObject.transform.position = this.icons[i].transform.position;
                    this.discountObject.SetActive(true);
                }
            }
        }
    }

    public static void ShowLicense(string license, LicenseType filter)
    {
        selectedLicense = license;
        selectedFilter = filter;
    }

    private void ShowPairs(bool isVisible)
    {
        this.goldSpriteRenderer.gameObject.SetActive(isVisible);
        this.ezrenSpriteRenderer.gameObject.SetActive(!isVisible);
        for (int i = 0; i < this.pairs.Count; i++)
        {
            this.pairs[i].Parent.SetActive(isVisible);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (this.currentOperation != LicenseOperationType.None)
        {
            LicenseResult result = LicenseManager.GetResult(this.currentOperation);
            if (result != null)
            {
                this.messageLine.Text = result.Message;
                this.currentOperation = LicenseOperationType.None;
                UI.Busy = false;
            }
        }
    }

    public LicenseOperationType CurrentOperation
    {
        get => 
            this.currentOperation;
        set
        {
            this.currentOperation = value;
        }
    }

    [Serializable]
    public class CardTypePair
    {
        public GuiLabel Label;
        public GameObject Parent;
        public CardType TypeOfCard;
    }
}

