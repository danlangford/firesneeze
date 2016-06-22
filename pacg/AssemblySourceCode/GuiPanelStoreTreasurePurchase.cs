using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GuiPanelStoreTreasurePurchase : GuiPanelBackStack
{
    [Tooltip("reference to the bonus label on this panel")]
    public GuiLabel bonusLabel;
    [Tooltip("reference to the bonus corner sprite on this panel")]
    public SpriteRenderer bonusSprite;
    [Tooltip("reference to the buy button in this panel")]
    public GuiButton buyButton;
    private LicenseOperationType currentOperation;
    [Tooltip("reference to the hilite object in this panel")]
    public GameObject hiliteObject;
    private List<LicenseIcon> icons;
    [Tooltip("reference to the selected license description field on this panel")]
    public GuiLabel licenseDescriptionLabel;
    [Tooltip("reference to the product image on this panel")]
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
        this.hiliteObject.SetActive(false);
        this.messageLine.Clear();
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
        this.icons = new List<LicenseIcon>(Constants.NUM_IAP_ADVENTURES);
        this.scroller.Initialize();
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

    private void OnBuyButtonPushed()
    {
        if (!UI.Busy)
        {
            if (Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold)
            {
                this.StoreManager.ShowInsufficientGoldPanel();
            }
            else if (Store.CanPurchase(selectedLicense, Store.CurrencyCategory.Gold, false, 1))
            {
                this.StoreManager.ShowPendingPanelType(this.licenseNameLabel.Text, string.Empty + Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold), 1, GuiPanelStorePendingPurchase.PendingType.Confirmation, this);
            }
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
        Store.PurchaseItem(selectedLicense, Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold));
    }

    public override void Refresh()
    {
        this.Clear();
        float listMarginTop = this.listMarginTop;
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            string iD = LicenseTable.Key(i);
            LicenseTableEntry entry = LicenseTable.Get(iD);
            if ((entry != null) && (entry.Type == selectedFilter))
            {
                LicenseIcon item = LicenseIcon.Create(entry);
                if (item != null)
                {
                    item.ID = iD;
                    item.LoadImage(entry.Icon);
                    item.SetBackground();
                    this.icons.Add(item);
                    this.scroller.Add(item.transform, this.listMarginLeft, listMarginTop);
                    listMarginTop += item.Height;
                }
            }
        }
        float y = (this.scroller.Min.y + listMarginTop) - this.scroller.Size.y;
        this.scroller.Max = new Vector2(0f, y);
        this.scroller.Top();
        this.buyButton.Show(true);
        this.buyButton.Disable(!LicenseManager.GetIsAvailable(selectedLicense, false));
        this.buyButton.TextTint((Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold) ? this.StoreManager.ColorInsufficientGold : this.StoreManager.ColorSufficientGold);
        AlertManager.SeenAlert(AlertManager.AlertType.SeenStoreTreasurePurchase);
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
            this.licensePriceLabel.Text = Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) + " Gold";
            if (!LicenseManager.GetIsAvailable(productCode, false))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Coming");
                builder.Append("Soon");
                this.licensePriceLabel.Text = builder.ToString();
            }
            if (productCode == Constants.IAP_LICENSE_CHEST_TIER3)
            {
                StringBuilder builder2 = new StringBuilder();
                builder2.AppendLine("10% BONUS");
                builder2.Append("1 FREE CHEST");
                this.bonusLabel.Text = builder2.ToString();
                this.bonusLabel.Show(true);
                this.bonusSprite.gameObject.SetActive(true);
            }
            else if (productCode == Constants.IAP_LICENSE_CHEST_TIER4)
            {
                StringBuilder builder3 = new StringBuilder();
                builder3.AppendLine("20% BONUS");
                builder3.Append("8 FREE CHESTS");
                this.bonusLabel.Text = builder3.ToString();
                this.bonusLabel.Show(true);
                this.bonusSprite.gameObject.SetActive(true);
            }
            else
            {
                this.bonusLabel.Show(false);
                this.bonusSprite.gameObject.SetActive(false);
            }
            this.buyButton.Show(true);
            this.buyButton.Disable(!LicenseManager.GetIsAvailable(productCode, false));
            this.buyButton.TextTint((Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold) ? this.StoreManager.ColorInsufficientGold : this.StoreManager.ColorSufficientGold);
        }
        else
        {
            this.ClearSelection();
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
                selectedFilter = LicenseType.TreasurePurchase;
                selectedLicense = this.Select(selectedFilter);
            }
            this.Refresh();
            this.Select(selectedLicense);
        }
        else
        {
            this.ClearSelection();
            this.Clear();
            UI.Busy = false;
        }
    }

    public static void ShowLicense(string license, LicenseType filter)
    {
        selectedLicense = license;
        selectedFilter = filter;
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
}

