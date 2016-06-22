using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class GuiPanelCollectionCards : GuiPanel
{
    [Tooltip("reference to the filter box by allies button")]
    public GuiButton allyFilterButton;
    [Tooltip("reference to the filter box by armors button")]
    public GuiButton armorFilterButton;
    [Tooltip("reference to the filter box by barriers button")]
    public GuiButton barrierFilterButton;
    [Tooltip("reference to the filter box by blessings button")]
    public GuiButton blessingFilterButton;
    private BoosterFilterType boosterFilter;
    [Tooltip("radio button: include boosters in the box")]
    public GuiButton boosterFilterAllButton;
    [Tooltip("displays number of cards filtered by booster choice")]
    public GuiLabel boosterFilterCounter;
    [Tooltip("radio button: only show boosters in the box")]
    public GuiButton boosterFilterOnlyButton;
    [Tooltip("radio button: include boosters in the box that are owned")]
    public GuiButton boosterFilterOwnedButton;
    [Tooltip("reference to the box deck in our hierarchy")]
    public Deck boxDeck;
    private CardTally[] boxTotals;
    private CardType cardFilter;
    [Tooltip("reference to the \"number of unlocked cards\" label")]
    public GuiLabel countLabel;
    private Box currentBox;
    [Tooltip("reference to the filter box by deck button: 1")]
    public GuiButton deck1Button;
    [Tooltip("reference to the filter box by deck button: 2")]
    public GuiButton deck2Button;
    [Tooltip("reference to the filter box by deck button: 3")]
    public GuiButton deck3Button;
    [Tooltip("reference to the filter box by deck button: 4")]
    public GuiButton deck4Button;
    [Tooltip("reference to the filter box by deck button: 5")]
    public GuiButton deck5Button;
    [Tooltip("reference to the filter box by deck button: 6")]
    public GuiButton deck6Button;
    [Tooltip("reference to the filter box by deck button: base")]
    public GuiButton deckBButton;
    [Tooltip("reference to the filter box by deck button: character")]
    public GuiButton deckCButton;
    private string deckFilter;
    [Tooltip("reference to the filter box by deck button: promo")]
    public GuiButton deckPButton;
    [Tooltip("reference to the empty panel in this scene")]
    public GuiPanelCollectionEmpty emptyPanel;
    [Tooltip("in screen order from left to right")]
    public GuiImage[] filterEmptyDeckIcons;
    [Tooltip("in screen order from left to right")]
    public GuiImage[] filterEmptyIcons;
    [Tooltip("reference to the filter box by henchmen button")]
    public GuiButton henchmenFilterButton;
    [Tooltip("reference to the filter box by items button")]
    public GuiButton itemFilterButton;
    [Tooltip("reference to the \"box tray\" in our hierarchy")]
    public GuiLayoutTray layoutBox;
    [Tooltip("reference to the filter box by loot button")]
    public GuiButton lootFilterButton;
    [Tooltip("reference to the filter box by monster button")]
    public GuiButton monsterFilterButton;
    [Tooltip("reference to the salvage panel in this scene")]
    public GuiPanelSalvage salvagePanel;
    [Tooltip("reference to the filter box by spells button")]
    public GuiButton spellFilterButton;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("pointer to the unknown card art template in resources")]
    public GameObject UnknownCardArt;
    [Tooltip("reference to the filter box by villains button")]
    public GuiButton villainFilterButton;
    [Tooltip("reference to the filter box by weapons button")]
    public GuiButton weaponFilterButton;
    private Card zoomedCard;
    [Tooltip("shift the zoom menu right by this world position on low res")]
    public float zoomMenuOffset;
    [Tooltip("reference to the salvage card button on the zoom menu")]
    public GuiButton zoomSalvageButton;

    private bool CanDisplayBox(string set)
    {
        string licenseIdentifierForDeck = LicenseManager.GetLicenseIdentifierForDeck(set);
        return ((licenseIdentifierForDeck == null) || (LicenseManager.GetIsLicensed(licenseIdentifierForDeck) && LicenseManager.GetIsSupported(licenseIdentifierForDeck)));
    }

    private bool CanLoadBoosters(string set) => 
        ((Campaign.Box.Count <= 0) || ((Game.GameMode == GameModeType.Quest) || this.CanLoadBox(set)));

    private bool CanLoadBox(string set)
    {
        if (Campaign.Box.Count <= 0)
        {
            return true;
        }
        if (Game.GameMode == GameModeType.Quest)
        {
            return false;
        }
        string item = "Tables/BoxTable_1" + set;
        return Campaign.Box.Tables.Contains(item);
    }

    public override void Clear()
    {
        for (int i = this.boxDeck.Count - 1; i >= 0; i--)
        {
            this.boxDeck.Destroy(this.boxDeck[i].ID);
        }
        this.boxDeck.Clear();
        this.layoutBox.Refresh();
        this.currentBox.Clear();
        this.cardFilter = CardType.None;
        this.deckFilter = null;
        this.GlowButtons(CardType.None);
        this.GlowButtons((string) null);
        this.zoomedCard = null;
        UI.Zoomed = false;
        UI.Busy = false;
        Resources.UnloadUnusedAssets();
    }

    public void CloseSubWindows()
    {
        Tutorial.Hide();
        this.UnZoomCard();
        this.HideZoomMenu();
        this.emptyPanel.Show(false);
    }

    private int ConvertSetToIndex(string set)
    {
        if (!string.IsNullOrEmpty(set))
        {
            if (set == "B")
            {
                return 0;
            }
            if (set == "1")
            {
                return 1;
            }
            if (set == "2")
            {
                return 2;
            }
            if (set == "3")
            {
                return 3;
            }
            if (set == "4")
            {
                return 4;
            }
            if (set == "5")
            {
                return 5;
            }
            if (set == "6")
            {
                return 6;
            }
            if (set == "C")
            {
                return 7;
            }
            if (set == "P")
            {
                return 8;
            }
        }
        return -1;
    }

    [DebuggerHidden]
    private IEnumerator CreateCards(CardType type, string set) => 
        new <CreateCards>c__Iterator52 { 
            type = type,
            set = set,
            <$>type = type,
            <$>set = set,
            <>f__this = this
        };

    private void DestroyZoomCard()
    {
        this.HideZoomMenu();
        base.StartCoroutine(this.DestroyZoomCardCoroutine());
    }

    [DebuggerHidden]
    private IEnumerator DestroyZoomCardCoroutine() => 
        new <DestroyZoomCardCoroutine>c__Iterator53 { <>f__this = this };

    private GameObject GetCardArt(string id, bool isCardOwned)
    {
        if (!isCardOwned && CardTable.LookupCardBooster(id))
        {
            return this.UnknownCardArt;
        }
        return null;
    }

    private void GlowBoosterButtons(BoosterFilterType filter)
    {
        this.boosterFilterOwnedButton.Glow(filter == BoosterFilterType.Owned);
        this.boosterFilterAllButton.Glow(filter == BoosterFilterType.On);
        this.boosterFilterOnlyButton.Glow(filter == BoosterFilterType.Only);
    }

    private void GlowButtons(CardType type)
    {
        this.weaponFilterButton.Glow(type == CardType.Weapon);
        this.spellFilterButton.Glow(type == CardType.Spell);
        this.itemFilterButton.Glow(type == CardType.Item);
        this.armorFilterButton.Glow(type == CardType.Armor);
        this.allyFilterButton.Glow(type == CardType.Ally);
        this.blessingFilterButton.Glow(type == CardType.Blessing);
        this.lootFilterButton.Glow(type == CardType.Loot);
        this.villainFilterButton.Glow(type == CardType.Villain);
        this.henchmenFilterButton.Glow(type == CardType.Henchman);
        this.barrierFilterButton.Glow(type == CardType.Barrier);
        this.monsterFilterButton.Glow(type == CardType.Monster);
    }

    private void GlowButtons(string box)
    {
        this.deckBButton.Glow(box == "B");
        this.deck1Button.Glow(box == "1");
        this.deck2Button.Glow(box == "2");
        this.deck3Button.Glow(box == "3");
        this.deck4Button.Glow(box == "4");
        this.deck5Button.Glow(box == "5");
        this.deck6Button.Glow(box == "6");
        this.deckPButton.Glow(box == "P");
        this.deckCButton.Glow(box == "C");
    }

    private void HideZoomMenu()
    {
        if (this.zoomSalvageButton.Visible)
        {
            this.zoomSalvageButton.Fade(false, 0.15f);
        }
    }

    public override void Initialize()
    {
        this.boxTotals = new CardTally[this.filterEmptyDeckIcons.Length];
        this.currentBox = new Box("GalleryCurrentBox");
        this.UpdateCounters();
        this.layoutBox.Initialize();
        this.salvagePanel.Initialize();
        this.emptyPanel.Initialize();
        this.layoutBox.Deck = this.boxDeck;
        this.boosterFilter = BoosterFilterType.Owned;
        this.cardFilter = CardType.None;
        this.deckFilter = null;
        this.layoutBox.Show(true);
        this.layoutBox.Refresh();
        this.layoutBox.Animations = false;
        this.zoomSalvageButton.Show(false);
        if (Device.GetScreenProfile() == DeviceScreenType.TabletLow)
        {
            Transform transform = this.zoomSalvageButton.transform;
            transform.position += new Vector3(this.zoomMenuOffset, 0f, 0f);
        }
    }

    private bool IsCardVisible(string id)
    {
        if (this.boosterFilter != BoosterFilterType.On)
        {
            bool cardBooster = CardTable.LookupCardBooster(id);
            if ((this.boosterFilter == BoosterFilterType.Off) && cardBooster)
            {
                return false;
            }
            if ((this.boosterFilter == BoosterFilterType.Only) && !cardBooster)
            {
                return false;
            }
            if (((this.boosterFilter == BoosterFilterType.Owned) && cardBooster) && !Collection.Contains(id))
            {
                return false;
            }
        }
        return true;
    }

    private void OnBoosterToggleAllButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(this.cardFilter, BoosterFilterType.On);
        }
    }

    private void OnBoosterToggleOnlyButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(this.cardFilter, BoosterFilterType.Only);
        }
    }

    private void OnBoosterToggleOwnedButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(this.cardFilter, BoosterFilterType.Owned);
        }
    }

    private void OnCardFilterAlliesButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Ally, this.boosterFilter);
        }
    }

    private void OnCardFilterArmorsButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Armor, this.boosterFilter);
        }
    }

    private void OnCardFilterBarriersButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Barrier, this.boosterFilter);
        }
    }

    private void OnCardFilterBlessingsButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Blessing, this.boosterFilter);
        }
    }

    private void OnCardFilterDeck0ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("B");
        }
    }

    private void OnCardFilterDeck1ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("1");
        }
    }

    private void OnCardFilterDeck2ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("2");
        }
    }

    private void OnCardFilterDeck3ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("3");
        }
    }

    private void OnCardFilterDeck4ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("4");
        }
    }

    private void OnCardFilterDeck5ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("5");
        }
    }

    private void OnCardFilterDeck6ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("6");
        }
    }

    private void OnCardFilterDeckCButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("C");
        }
    }

    private void OnCardFilterDeckPButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxDeck("P");
        }
    }

    private void OnCardFilterHenchmenButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Henchman, this.boosterFilter);
        }
    }

    private void OnCardFilterItemsButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Item, this.boosterFilter);
        }
    }

    private void OnCardFilterLootButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Loot, this.boosterFilter);
        }
    }

    private void OnCardFilterMonstersButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Monster, this.boosterFilter);
        }
    }

    private void OnCardFilterSpellsButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Spell, this.boosterFilter);
        }
    }

    private void OnCardFilterVillainsButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Villain, this.boosterFilter);
        }
    }

    private void OnCardFilterWeaponsButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.CloseSubWindows();
            this.SetupBoxCards(CardType.Weapon, this.boosterFilter);
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (this.zoomedCard != null)
        {
            if (!LeanTween.isTweening(this.zoomedCard.gameObject))
            {
                this.UnZoomCard();
            }
        }
        else
        {
            Card topCard = GuiLayout.GetTopCard(touchPos);
            if ((topCard != null) && !LeanTween.isTweening(topCard.gameObject))
            {
                this.ZoomCard(topCard);
            }
        }
    }

    private void OnSalvageButtonPushed()
    {
        this.HideZoomMenu();
        this.salvagePanel.Show(this.zoomedCard);
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        if (this.tapRecognizer != null)
        {
            this.tapRecognizer.enabled = !isPaused;
        }
        this.layoutBox.Pause(isPaused);
    }

    private void SetupBoxCards(CardType type, BoosterFilterType boosters)
    {
        if ((this.cardFilter != type) || (boosters != this.boosterFilter))
        {
            this.boosterFilter = boosters;
            this.cardFilter = type;
            this.GlowButtons(this.cardFilter);
            this.GlowBoosterButtons(this.boosterFilter);
            for (int i = this.boxDeck.Count - 1; i >= 0; i--)
            {
                this.boxDeck.Destroy(this.boxDeck[i].ID);
            }
            this.layoutBox.Refresh();
            base.StartCoroutine(this.CreateCards(type, this.deckFilter));
            this.UpdateFilters();
        }
    }

    private void SetupBoxDeck(string set)
    {
        this.GlowButtons(set);
        for (int i = this.boxDeck.Count - 1; i >= 0; i--)
        {
            this.boxDeck.Destroy(this.boxDeck[i].ID);
        }
        this.layoutBox.Refresh();
        this.currentBox.Clear();
        if (this.CanLoadBox(set))
        {
            if (this.CanDisplayBox(set))
            {
                string file = "Tables/BoxTable_1" + set;
                this.currentBox.Load(file, set);
            }
            for (int j = 0; j < Campaign.GalleryCards.Count; j++)
            {
                CardTableEntry entry = CardTable.Get(Campaign.GalleryCards[j]);
                if (entry.set == set)
                {
                    CardIdentity identity = new CardIdentity(entry.id, entry.set);
                    this.currentBox.Push(identity, true);
                }
            }
        }
        if (this.CanLoadBoosters(set))
        {
            for (int k = 0; k < CardTable.Count; k++)
            {
                CardTableEntry entry2 = CardTable.Get(k);
                if ((entry2.set == set) && CardTable.LookupCardBooster(entry2.id))
                {
                    CardIdentity identity2 = new CardIdentity(entry2.id, entry2.set);
                    this.currentBox.Push(identity2, true);
                }
            }
            foreach (CollectionEntry entry3 in Collection.GetEntries(set))
            {
                for (int m = 1; m < entry3.quantity; m++)
                {
                    CardIdentity identity3 = new CardIdentity(entry3.id, entry3.set);
                    this.currentBox.Push(identity3, true);
                }
            }
        }
        this.deckFilter = set;
        if (this.cardFilter != CardType.None)
        {
            base.StartCoroutine(this.CreateCards(this.cardFilter, this.deckFilter));
        }
        this.UpdateFilters();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.tapRecognizer = new TKTapRecognizer();
            this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
                if (!UI.Busy)
                {
                    this.OnGuiTap(this.tapRecognizer.touchLocation());
                }
            };
            TouchKit.addGestureRecognizer(this.tapRecognizer);
            this.SetupBoxDeck("B");
            this.SetupBoxCards(CardType.Villain, BoosterFilterType.Owned);
        }
        if (!isVisible)
        {
            this.Clear();
            if (this.tapRecognizer != null)
            {
                TouchKit.removeGestureRecognizer(this.tapRecognizer);
            }
        }
    }

    private void ShowZoomMenu()
    {
        if (((this.zoomedCard != null) && (this.zoomedCard.Cost > 0)) && (Game.Network.Connected && Game.Network.HasNetworkConnection))
        {
            this.zoomSalvageButton.gameObject.SetActive(true);
            this.zoomSalvageButton.Fade(true, 0.15f);
        }
    }

    private void UnZoomCard()
    {
        if (this.zoomedCard != null)
        {
            float delayTime = this.zoomedCard.OnGuiZoom(false);
            this.HideZoomMenu();
            LeanTween.delayedCall(delayTime, new Action(this.UnZoomCardDone));
        }
    }

    private void UnZoomCardDone()
    {
        UI.Zoomed = false;
        this.zoomedCard = null;
    }

    private void UpdateCounters()
    {
        int countUnique = 0;
        int num2 = 0;
        int num3 = 0;
        this.currentBox.Load("Tables/BoxTable_1B", "B");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[0].box = !this.CanLoadBox("B") ? 0 : countUnique;
        num2 += countUnique;
        num3 += countUnique;
        this.currentBox.Clear();
        this.currentBox.Load("Tables/BoxTable_11", "1");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[1].box = !this.CanLoadBox("1") ? 0 : countUnique;
        num2 += countUnique;
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_AD11))
        {
            num3 += countUnique;
        }
        this.currentBox.Clear();
        this.currentBox.Load("Tables/BoxTable_12", "2");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[2].box = !this.CanLoadBox("2") ? 0 : countUnique;
        num2 += countUnique;
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_AD12))
        {
            num3 += countUnique;
        }
        this.currentBox.Clear();
        this.currentBox.Load("Tables/BoxTable_13", "3");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[3].box = !this.CanLoadBox("3") ? 0 : countUnique;
        num2 += countUnique;
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_AD13))
        {
            num3 += countUnique;
        }
        this.currentBox.Clear();
        this.currentBox.Load("Tables/BoxTable_14", "4");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[4].box = !this.CanLoadBox("4") ? 0 : countUnique;
        num2 += countUnique;
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_AD14))
        {
            num3 += countUnique;
        }
        this.currentBox.Clear();
        this.currentBox.Load("Tables/BoxTable_15", "5");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[5].box = !this.CanLoadBox("5") ? 0 : countUnique;
        num2 += countUnique;
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_AD15))
        {
            num3 += countUnique;
        }
        this.currentBox.Clear();
        this.currentBox.Load("Tables/BoxTable_16", "6");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[6].box = !this.CanLoadBox("6") ? 0 : countUnique;
        num2 += countUnique;
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_AD16))
        {
            num3 += countUnique;
        }
        this.currentBox.Clear();
        this.currentBox.Load("Tables/BoxTable_1C", "C");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[7].box = !this.CanLoadBox("C") ? 0 : countUnique;
        num2 += countUnique;
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_CH11))
        {
            num3 += countUnique;
        }
        this.currentBox.Clear();
        this.currentBox.Load("Tables/BoxTable_1P", "P");
        countUnique = this.currentBox.CountUnique;
        this.boxTotals[8].box = !this.CanLoadBox("P") ? 0 : countUnique;
        num2 += countUnique;
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_BUNDLE_ROTR))
        {
            num3 += countUnique;
        }
        this.currentBox.Clear();
        countUnique = 0;
        for (int i = 0; i < CardTable.Count; i++)
        {
            CardTableEntry entry = CardTable.Get(i);
            if (CardTable.LookupCardBooster(entry.id))
            {
                int index = this.ConvertSetToIndex(entry.set);
                if ((index >= 0) && (index < this.boxTotals.Length))
                {
                    if (this.CanLoadBoosters(entry.set))
                    {
                        this.boxTotals[index].boost++;
                    }
                    countUnique++;
                }
            }
        }
        num2 += countUnique;
        countUnique = 0;
        foreach (string str in Collection.Cards)
        {
            string cardSet = CardTable.LookupCardSet(str);
            int num6 = this.ConvertSetToIndex(cardSet);
            if ((num6 >= 0) && (num6 < this.boxTotals.Length))
            {
                if (this.CanLoadBoosters(cardSet))
                {
                    this.boxTotals[num6].col++;
                }
                countUnique++;
            }
        }
        num3 += countUnique;
        this.countLabel.Text = num3 + " / " + num2;
    }

    private void UpdateFilters()
    {
        for (int i = 0; i < this.filterEmptyDeckIcons.Length; i++)
        {
            if (this.boosterFilter == BoosterFilterType.Owned)
            {
                this.filterEmptyDeckIcons[i].Show((this.boxTotals[i].box + this.boxTotals[i].col) <= 0);
            }
            if (this.boosterFilter == BoosterFilterType.On)
            {
                this.filterEmptyDeckIcons[i].Show((this.boxTotals[i].box + this.boxTotals[i].boost) <= 0);
            }
            if (this.boosterFilter == BoosterFilterType.Off)
            {
                this.filterEmptyDeckIcons[i].Show(this.boxTotals[i].box <= 0);
            }
            if (this.boosterFilter == BoosterFilterType.Only)
            {
                this.filterEmptyDeckIcons[i].Show(this.boxTotals[i].boost <= 0);
            }
        }
        int num2 = this.currentBox.Filter(CardType.Villain, this.boosterFilter);
        this.filterEmptyIcons[0].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Henchman, this.boosterFilter);
        this.filterEmptyIcons[1].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Monster, this.boosterFilter);
        this.filterEmptyIcons[2].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Barrier, this.boosterFilter);
        this.filterEmptyIcons[3].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Weapon, this.boosterFilter);
        this.filterEmptyIcons[4].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Spell, this.boosterFilter);
        this.filterEmptyIcons[5].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Armor, this.boosterFilter);
        this.filterEmptyIcons[6].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Item, this.boosterFilter);
        this.filterEmptyIcons[7].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Ally, this.boosterFilter);
        this.filterEmptyIcons[8].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Blessing, this.boosterFilter);
        this.filterEmptyIcons[9].Show(num2 <= 0);
        num2 = this.currentBox.Filter(CardType.Loot, this.boosterFilter);
        this.filterEmptyIcons[10].Show(num2 <= 0);
    }

    private void ZoomCard(Card card)
    {
        if (card != null)
        {
            UI.Zoomed = true;
            float delayTime = card.OnGuiZoom(true);
            this.zoomedCard = card;
            LeanTween.delayedCall(delayTime, new Action(this.ShowZoomMenu));
            UI.Lock(delayTime);
        }
    }

    [CompilerGenerated]
    private sealed class <CreateCards>c__Iterator52 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string <$>set;
        internal CardType <$>type;
        internal GuiPanelCollectionCards <>f__this;
        internal GameObject <art>__3;
        internal Card <card>__4;
        internal string[] <cards>__0;
        internal int <i>__1;
        internal bool <isInCollection>__2;
        internal string set;
        internal CardType type;

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
                    UI.Busy = true;
                    Game.UI.BusyBox.Center();
                    Game.UI.BusyBox.Show(true);
                    this.<>f__this.emptyPanel.Show(false);
                    Resources.UnloadUnusedAssets();
                    this.<cards>__0 = this.<>f__this.currentBox.GetCardList(this.type, CardRankType.None);
                    this.<i>__1 = 0;
                    goto Label_0192;

                case 1:
                    break;

                default:
                    goto Label_02AA;
            }
        Label_0184:
            this.<i>__1++;
        Label_0192:
            if (this.<i>__1 < this.<cards>__0.Length)
            {
                if (this.<>f__this.IsCardVisible(this.<cards>__0[this.<i>__1]))
                {
                    this.<isInCollection>__2 = Collection.Contains(this.<cards>__0[this.<i>__1]);
                    this.<art>__3 = this.<>f__this.GetCardArt(this.<cards>__0[this.<i>__1], this.<isInCollection>__2);
                    this.<card>__4 = CardTable.Create(this.<cards>__0[this.<i>__1], this.set, this.<art>__3);
                    if (this.<card>__4 != null)
                    {
                        this.<card>__4.Cost = 0;
                        if (this.<isInCollection>__2)
                        {
                            this.<card>__4.Cost = Collection.GetCost(this.<cards>__0[this.<i>__1]);
                        }
                        this.<>f__this.boxDeck.Add(this.<card>__4);
                    }
                    Game.UI.BusyBox.Tick();
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    return true;
                }
                goto Label_0184;
            }
            this.<>f__this.boosterFilterCounter.Text = this.<>f__this.boxDeck.Count + " / " + this.<cards>__0.Length;
            Game.UI.BusyBox.Show(false);
            UI.Busy = false;
            this.<>f__this.layoutBox.Deck.Sort(this.<>f__this.boosterFilter);
            this.<>f__this.layoutBox.Reset();
            this.<>f__this.layoutBox.Refresh();
            if (this.<>f__this.boxDeck.Count <= 0)
            {
                this.<>f__this.emptyPanel.Show(this.set, this.<>f__this.CanLoadBox(this.set) || this.<>f__this.CanLoadBoosters(this.set));
            }
            else
            {
                this.<>f__this.emptyPanel.Show(false);
            }
            this.$PC = -1;
        Label_02AA:
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
    private sealed class <DestroyZoomCardCoroutine>c__Iterator53 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelCollectionCards <>f__this;
        internal int <set>__0;

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
                    if (this.<>f__this.zoomedCard == null)
                    {
                        break;
                    }
                    UI.Busy = true;
                    VisualEffect.ApplyToCard(VisualEffectType.CardSalvage, this.<>f__this.zoomedCard, 2.5f);
                    UI.Sound.Play(SoundEffectType.SalvagedCard);
                    this.$current = new WaitForSeconds(1.3f);
                    this.$PC = 1;
                    goto Label_017A;

                case 1:
                    this.<set>__0 = this.<>f__this.ConvertSetToIndex(this.<>f__this.zoomedCard.Set);
                    if (this.<set>__0 >= 0)
                    {
                        this.<>f__this.boxTotals[this.<set>__0].col--;
                    }
                    this.<>f__this.currentBox.Remove(this.<>f__this.zoomedCard);
                    this.<>f__this.boxDeck.Remove(this.<>f__this.zoomedCard);
                    this.<>f__this.UpdateFilters();
                    this.$current = new WaitForSeconds(1.2f);
                    this.$PC = 2;
                    goto Label_017A;

                case 2:
                    this.<>f__this.UnZoomCardDone();
                    this.<>f__this.layoutBox.Animations = true;
                    this.<>f__this.layoutBox.Refresh();
                    this.<>f__this.layoutBox.Animations = false;
                    UI.Busy = false;
                    break;

                default:
                    goto Label_0178;
            }
            this.$PC = -1;
        Label_0178:
            return false;
        Label_017A:
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

    [StructLayout(LayoutKind.Sequential)]
    private struct CardTally
    {
        [Tooltip("number of cards from this deck in the collection")]
        public int col;
        [Tooltip("number of cards from this deck in the box table")]
        public int box;
        [Tooltip("number of booster cards in this deck")]
        public int boost;
    }
}

