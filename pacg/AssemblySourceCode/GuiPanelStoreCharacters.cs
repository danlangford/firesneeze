using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GuiPanelStoreCharacters : GuiPanelBackStack
{
    [Tooltip("reference to the proceed button contained in the scene")]
    public GuiButton BuyButton;
    public List<StorePortraitPairing> characterPortraits;
    private Dictionary<string, Character> characters;
    [Tooltip("reference to the character traits label in the scene")]
    public GuiLabel CharacterTraitsLabel;
    [Tooltip("reference to the charisma skill label in this panel")]
    public GuiLabel charismaDieLabel;
    [Tooltip("reference to the charisma skill label in this panel")]
    public GuiLabel charismaLabel;
    [Tooltip("reference to the constitution skill label in this panel")]
    public GuiLabel constitutionDieLabel;
    [Tooltip("reference to the constitution skill label in this panel")]
    public GuiLabel constitutionLabel;
    private Character currentCharacter;
    private LicenseOperationType currentOperation;
    private int currentPane;
    [Tooltip("reference to the dexterity skill label in this panel")]
    public GuiLabel dexterityDieLabel;
    [Tooltip("reference to the dexterity skill label in this panel")]
    public GuiLabel dexterityLabel;
    [Tooltip("reference to the ezren sidebar art renderer")]
    public SpriteRenderer ezrenSpriteRenderer;
    private FingerGesture finger;
    [Tooltip("reference to the gold sprite art renderer")]
    public SpriteRenderer goldSpriteRenderer;
    [Tooltip("reference to the hilite object in this panel")]
    public GameObject hiliteObject;
    private List<LicenseIcon> icons;
    [Tooltip("reference to the intelligence skill label in this panel")]
    public GuiLabel intelligenceDieLabel;
    [Tooltip("reference to the intelligence skill label in this panel")]
    public GuiLabel intelligenceLabel;
    [Tooltip("reference to the character description label in the scene")]
    public GuiLabel licenseDescriptionLabel;
    [Tooltip("reference to the selected license image on this panel")]
    public GuiImage licenseImage;
    [Tooltip("reference to the character name label in the scene")]
    public GuiLabel licenseNameLabel;
    [Tooltip("reference to the selected license price field on this panel")]
    public GuiLabel licensePriceLabel;
    [Tooltip("left margin of items in scroll list (half of the item width)")]
    public float listMarginLeft;
    [Tooltip("top margin of items in the scroll list (half of the item height)")]
    public float listMarginTop;
    [Tooltip("reference to the message label in this panel")]
    public GuiLabel messageLine;
    [Tooltip("reference to biography sub-panel in our hierarchy")]
    public GuiPanelCharacterBio Pane0;
    [Tooltip("reference to skills sub-panel 1 in our hierarchy")]
    public GuiPanelCharacterSkills Pane1;
    [Tooltip("reference to powers sub-panel 2 in our hierarchy")]
    public GuiPanelCharacterPowers Pane2;
    [Tooltip("reference to cards sub-panel 3 in our hierarchy")]
    public GuiPanelCharacterCards Pane3;
    [Tooltip("reference to the \"yes/no\" popup in this scene")]
    public GuiPanelMenuAsk Popup;
    [Tooltip("reference to the scrolling region on this panel")]
    public GuiScrollRegion scroller;
    [Tooltip("reference to the 'see more' button contained in this scene")]
    public GuiButton SeeMoreButton;
    [Tooltip("reference to the corner close button on the 'see more' panel in the scene")]
    public GuiButton SeeMoreCloseButton;
    [Tooltip("reference to the 'see more' panel in the scene")]
    public GuiPanelStoreCharactersInfo SeeMorePanel;
    private static LicenseType selectedFilter;
    private static string selectedLicense;
    [Tooltip("reference to the skills sidebar label in this panel")]
    public GuiLabel skillsLabel;
    [Tooltip("reference to the store manager window in the scene")]
    public GuiWindowStore StoreManager;
    [Tooltip("reference to the strength skill label in this panel")]
    public GuiLabel strengthDieLabel;
    [Tooltip("reference to the strength skill label in this panel")]
    public GuiLabel strengthLabel;
    [Tooltip("sound played when a character is selected via the tokens")]
    public AudioClip SwitchCharacterSound;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the wisdom skill label in this panel")]
    public GuiLabel wisdomDieLabel;
    [Tooltip("reference to the wisdom skill label in this panel")]
    public GuiLabel wisdomLabel;

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
        this.licensePriceLabel.Clear();
        this.SeeMoreButton.Show(false);
        this.hiliteObject.SetActive(false);
        this.messageLine.Clear();
    }

    public void CloseSeeMorePanel()
    {
        if (!UI.Busy)
        {
            this.scroller.Pause(false);
            this.SeeMorePanel.Show(false);
        }
    }

    private void ConfigureGestureRecognizers()
    {
    }

    private void DisplayCharacterDetails(Character c)
    {
        if (c != null)
        {
            CharacterTableEntry entry = CharacterTable.Get(c.ID);
            this.ShowSkills(true);
            this.strengthDieLabel.Text = c.GetAttributeDice(AttributeType.Strength).ToString();
            this.constitutionDieLabel.Text = c.GetAttributeDice(AttributeType.Constitution).ToString();
            this.dexterityDieLabel.Text = c.GetAttributeDice(AttributeType.Dexterity).ToString();
            this.wisdomDieLabel.Text = c.GetAttributeDice(AttributeType.Wisdom).ToString();
            this.intelligenceDieLabel.Text = c.GetAttributeDice(AttributeType.Intelligence).ToString();
            this.charismaDieLabel.Text = c.GetAttributeDice(AttributeType.Charisma).ToString();
            this.licenseNameLabel.Text = entry.Name;
            this.licenseDescriptionLabel.Text = entry.Description;
            this.licensePriceLabel.Text = Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) + " Gold";
            if (LicenseManager.GetIsLicensed(selectedLicense))
            {
                this.licensePriceLabel.Text = "Owned";
            }
            string[] textArray1 = new string[] { c.Gender.ToText(), " \x00b7 ", c.Race.ToText(), " \x00b7 ", c.Class.ToText() };
            this.CharacterTraitsLabel.Text = string.Concat(textArray1);
            this.Pane0.Character = c;
            this.Pane1.Character = c;
            this.Pane2.Character = c;
            this.Pane3.Character = c;
            if (this.currentPane == 0)
            {
                this.Pane0.Refresh();
            }
            if (this.currentPane == 1)
            {
                this.Pane1.Refresh();
            }
            this.Pane2.Show(this.currentPane == 2);
            this.Pane2.Refresh();
            if (this.currentPane == 3)
            {
                this.Pane3.Refresh();
            }
            this.UpdateBuyButton(c);
        }
    }

    private string GetIDFromLicense(string license)
    {
        if (license.Equals(Constants.IAP_LICENSE_CH1B_EZREN))
        {
            return "CH1B_Ezren";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1B_HARSK))
        {
            return "CH1B_Harsk";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1B_KYRA))
        {
            return "CH1B_Kyra";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1B_LEM))
        {
            return "CH1B_Lem";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1B_MERISIEL))
        {
            return "CH1B_Merisiel";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1B_SEONI))
        {
            return "CH1B_Seoni";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1B_VALEROS))
        {
            return "CH1B_Valeros";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1C_AMIRI))
        {
            return "CH1C_Amiri";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1C_LINI))
        {
            return "CH1C_Lini";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1C_SAJAN))
        {
            return "CH1C_Sajan";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1C_SEELAH))
        {
            return "CH1C_Seelah";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1T_AMEIKO))
        {
            return "CH1T_Ameiko";
        }
        if (license.Equals(Constants.IAP_LICENSE_CH1T_ORIK))
        {
            return "CH1T_Orik";
        }
        return string.Empty;
    }

    private int GetTokenIndex(CharacterToken token) => 
        -1;

    private void HilightLicenseIcon(string id)
    {
        this.hiliteObject.SetActive(false);
        for (int i = 0; i < this.icons.Count; i++)
        {
            if (id.Contains(this.icons[i].ID.ToLower()))
            {
                this.hiliteObject.transform.position = this.icons[i].transform.position;
                this.hiliteObject.SetActive(true);
            }
        }
    }

    public override void Initialize()
    {
        this.icons = new List<LicenseIcon>(Constants.NUM_IAP_CHARACTERS);
        this.characters = new Dictionary<string, Character>();
        this.scroller.Initialize();
        selectedFilter = LicenseType.Character;
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 3;
        this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.Pane0.Initialize();
        this.Pane0.Show(true);
        this.Pane1.Initialize();
        this.Pane1.Show(false);
        this.Pane2.Initialize();
        this.Pane2.Show(false);
        this.Pane3.Initialize();
        this.Pane3.Show(false);
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
        if (!UI.Busy && !UI.Window.Paused)
        {
            if (((selectedLicense == Constants.IAP_LICENSE_CH11) && (Store.GetPrice(Constants.IAP_LICENSE_CH11, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold)) || ((selectedLicense != Constants.IAP_LICENSE_CH11) && (Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold)))
            {
                this.StoreManager.ShowInsufficientGoldPanel();
            }
            else if ((selectedLicense != Constants.IAP_LICENSE_CH11) && Store.CanPurchase(selectedLicense, Store.CurrencyCategory.Gold, true, 1))
            {
                this.StoreManager.ShowPendingPanelType(this.licenseNameLabel.Text, string.Empty + Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold), 1, GuiPanelStorePendingPurchase.PendingType.Confirmation, this);
            }
            else if ((selectedLicense == Constants.IAP_LICENSE_CH11) && Store.CanPurchase(Constants.IAP_LICENSE_CH11, Store.CurrencyCategory.Gold, true, 1))
            {
                this.StoreManager.ShowPendingPanelType(this.licenseNameLabel.Text, string.Empty + Store.GetPrice(Constants.IAP_LICENSE_CH11, Store.CurrencyCategory.Gold), 1, GuiPanelStorePendingPurchase.PendingType.Confirmation, this);
            }
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (!UI.Busy && !this.SeeMorePanel.Visible)
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

    private void OnLicenseYesButtonPushed()
    {
        this.Popup.Show(false);
    }

    private void OnMenuButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.Pause(true);
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnSeeMoreButtonPushed()
    {
        if (!UI.Busy)
        {
            this.scroller.Pause(true);
            this.SeeMorePanel.Show(true);
            this.SeeMorePanel.DisplayCharacterDetails(this.currentCharacter);
            this.SeeMoreCloseButton.Show(true);
        }
    }

    public void ProceedWithPurchase()
    {
        UI.Busy = true;
        Store.PurchaseItem(selectedLicense, Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold));
    }

    public override void Refresh()
    {
        this.Clear();
        this.ConfigureGestureRecognizers();
        this.licensePriceLabel.Text = string.Empty;
        float listMarginTop = this.listMarginTop;
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            string iD = LicenseTable.Key(i);
            LicenseTableEntry entry = LicenseTable.Get(iD);
            if (((entry != null) && (entry.Type == selectedFilter)) && !iD.Contains("t_"))
            {
                LicenseIcon item = LicenseIcon.Create(entry);
                if (item != null)
                {
                    item.ID = iD;
                    item.LoadImage(entry.Icon);
                    item.SetBackground();
                    this.icons.Add(item);
                    Character character = null;
                    if (!this.characters.TryGetValue(iD, out character))
                    {
                        if (this.GetIDFromLicense(iD).Length > 0)
                        {
                            character = CharacterTable.Create(this.GetIDFromLicense(iD));
                        }
                        if (character != null)
                        {
                            this.characters.Add(iD, character);
                        }
                    }
                    for (int j = 0; j < this.characterPortraits.Count; j++)
                    {
                        if (this.characterPortraits[j].GetMatchingLicense(this.characterPortraits[j].Character).Equals(iD))
                        {
                            item.Image.Image = this.characterPortraits[j].Image;
                            break;
                        }
                    }
                    this.scroller.Add(item.transform, this.listMarginLeft, listMarginTop);
                    listMarginTop += item.Height;
                }
            }
        }
        float y = (this.scroller.Min.y + listMarginTop) - this.scroller.Size.y;
        this.scroller.Max = new Vector2(0f, y);
        this.scroller.Top();
        this.BuyButton.Show(true);
        this.BuyButton.Disable(!LicenseManager.GetIsAvailable(selectedLicense, false) || LicenseManager.GetIsLicensed(selectedLicense));
        this.BuyButton.TextTint((Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold) ? this.StoreManager.ColorInsufficientGold : this.StoreManager.ColorSufficientGold);
        AlertManager.SeenAlert(AlertManager.AlertType.SeenStoreCharacters);
        AlertManager.HandleAlerts();
        if (!string.IsNullOrEmpty(selectedLicense))
        {
            this.Select(selectedLicense);
        }
    }

    private string Select()
    {
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            string iD = LicenseTable.Key(i);
            LicenseTableEntry entry = LicenseTable.Get(iD);
            if ((entry != null) && (entry.Type == LicenseType.Character))
            {
                return iD;
            }
        }
        return null;
    }

    private string Select(LicenseType category)
    {
        selectedFilter = category;
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            string iD = LicenseTable.Key(i);
            if (((!iD.Equals(Constants.IAP_LICENSE_CH1B_KYRA) && !iD.Equals(Constants.IAP_LICENSE_CH1B_MERISIEL)) && !iD.Equals(Constants.IAP_LICENSE_CH1T_AMEIKO)) && !iD.Equals(Constants.IAP_LICENSE_CH1T_ORIK))
            {
                LicenseTableEntry entry = LicenseTable.Get(iD);
                if ((entry != null) && (entry.Type == selectedFilter))
                {
                    return iD;
                }
            }
        }
        return null;
    }

    private void Select(string productCode)
    {
        LicenseTableEntry entry = LicenseTable.Get(productCode);
        if (entry == null)
        {
            this.ClearSelection();
        }
        else
        {
            selectedLicense = productCode;
            this.HilightLicenseIcon(selectedLicense);
            this.licensePriceLabel.Text = Store.GetPrice(productCode, Store.CurrencyCategory.Gold) + " Gold";
            if (LicenseManager.GetIsLicensed(productCode))
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
            this.BuyButton.Show(true);
            this.BuyButton.Disable(!LicenseManager.GetIsAvailable(productCode, false) || LicenseManager.GetIsLicensed(productCode));
            this.BuyButton.TextTint((Store.GetPrice(selectedLicense, Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold) ? this.StoreManager.ColorInsufficientGold : this.StoreManager.ColorSufficientGold);
            this.SeeMoreButton.Show(true);
            Character character = null;
            if (!this.characters.TryGetValue(productCode, out character))
            {
                this.ShowSkills(false);
                for (int i = 0; i < this.icons.Count; i++)
                {
                    if (this.icons[i].ID == productCode)
                    {
                        this.licenseNameLabel.Text = entry.Name;
                        this.licenseImage.Image = this.icons[i].Image.Image;
                        this.licenseDescriptionLabel.Text = entry.Description;
                        this.CharacterTraitsLabel.Text = string.Empty;
                        this.SeeMoreButton.Show(false);
                    }
                }
            }
            else
            {
                this.currentCharacter = character;
                for (int j = 0; j < this.characterPortraits.Count; j++)
                {
                    if (this.characterPortraits[j].GetMatchingLicense(this.characterPortraits[j].Character).Equals(productCode))
                    {
                        this.licenseImage.Image = this.characterPortraits[j].Image;
                        break;
                    }
                }
                this.DisplayCharacterDetails(character);
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
                selectedFilter = LicenseType.Character;
                selectedLicense = this.Select(selectedFilter);
            }
            this.Refresh();
        }
        else
        {
            this.SeeMorePanel.Show(false);
            this.SeeMoreCloseButton.Show(false);
            this.ClearSelection();
            this.Clear();
            UI.Busy = false;
        }
    }

    public void Show(string productCode, LicenseType category)
    {
        selectedLicense = productCode;
        this.Show(true);
    }

    public static void ShowLicense(string license, LicenseType filter)
    {
        selectedLicense = license;
    }

    public void ShowLicensePopup()
    {
        this.Popup.YesButtonCallback = "OnLicenseYesButtonPushed";
        this.Popup.MessageLabel.Text = this.currentCharacter.ID.ToLower();
        this.Popup.YesButton.Text = StringTableManager.GetHelperText(0x2c);
        this.Popup.NoButtonText = StringTableManager.GetHelperText(0x2d);
        this.Popup.Show(true);
    }

    private void ShowSkills(bool isVisible)
    {
        this.goldSpriteRenderer.gameObject.SetActive(isVisible);
        this.ezrenSpriteRenderer.gameObject.SetActive(!isVisible);
        this.skillsLabel.Show(isVisible);
        this.strengthLabel.Show(isVisible);
        this.strengthDieLabel.Show(isVisible);
        this.constitutionLabel.Show(isVisible);
        this.constitutionDieLabel.Show(isVisible);
        this.dexterityLabel.Show(isVisible);
        this.dexterityDieLabel.Show(isVisible);
        this.wisdomLabel.Show(isVisible);
        this.wisdomDieLabel.Show(isVisible);
        this.intelligenceLabel.Show(isVisible);
        this.intelligenceDieLabel.Show(isVisible);
        this.charismaLabel.Show(isVisible);
        this.charismaDieLabel.Show(isVisible);
    }

    private void UpdateBuyButton(Character c)
    {
        this.BuyButton.Show(true);
        this.BuyButton.Disable(!LicenseManager.GetIsAvailable(Constants.IAP_LICENSE_CH_PREFIX + c.ID.ToLower(), false) || LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_CH_PREFIX + c.ID.ToLower()));
        this.BuyButton.TextTint((Store.GetPrice(Constants.IAP_LICENSE_CH_PREFIX + c.ID.ToLower(), Store.CurrencyCategory.Gold) > Game.Network.CurrentUser.Gold) ? this.StoreManager.ColorInsufficientGold : this.StoreManager.ColorSufficientGold);
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
    public class StorePortraitPairing
    {
        public CharacterName Character;
        public Sprite Image;

        public string GetMatchingLicense(CharacterName name)
        {
            switch (name)
            {
                case CharacterName.Ezren:
                    return Constants.IAP_LICENSE_CH1B_EZREN;

                case CharacterName.Harsk:
                    return Constants.IAP_LICENSE_CH1B_HARSK;

                case CharacterName.Kyra:
                    return Constants.IAP_LICENSE_CH1B_KYRA;

                case CharacterName.Lem:
                    return Constants.IAP_LICENSE_CH1B_LEM;

                case CharacterName.Merisiel:
                    return Constants.IAP_LICENSE_CH1B_MERISIEL;

                case CharacterName.Seoni:
                    return Constants.IAP_LICENSE_CH1B_SEONI;

                case CharacterName.Valeros:
                    return Constants.IAP_LICENSE_CH1B_VALEROS;

                case CharacterName.Amiri:
                    return Constants.IAP_LICENSE_CH1C_AMIRI;

                case CharacterName.Lini:
                    return Constants.IAP_LICENSE_CH1C_LINI;

                case CharacterName.Sajan:
                    return Constants.IAP_LICENSE_CH1C_SAJAN;

                case CharacterName.Seelah:
                    return Constants.IAP_LICENSE_CH1C_SEELAH;

                case CharacterName.Ameiko:
                    return Constants.IAP_LICENSE_CH1T_AMEIKO;

                case CharacterName.Orik:
                    return Constants.IAP_LICENSE_CH1T_ORIK;
            }
            return string.Empty;
        }

        public enum CharacterName
        {
            Ezren,
            Harsk,
            Kyra,
            Lem,
            Merisiel,
            Seoni,
            Valeros,
            Amiri,
            Lini,
            Sajan,
            Seelah,
            Ameiko,
            Orik
        }
    }
}

