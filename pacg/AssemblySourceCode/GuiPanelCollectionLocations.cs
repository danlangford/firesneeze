using System;
using System.Collections.Generic;
using UnityEngine;

public class GuiPanelCollectionLocations : GuiPanel
{
    [Tooltip("label for card count of allies")]
    public GuiLabel CountAllyCardsLabel;
    [Tooltip("label for card count of armours")]
    public GuiLabel CountArmorCardsLabel;
    [Tooltip("label for card count of barriers")]
    public GuiLabel CountBarrierCardsLabel;
    [Tooltip("label for card count of blessings")]
    public GuiLabel CountBlessingCardsLabel;
    [Tooltip("label for card count of items")]
    public GuiLabel CountItemCardsLabel;
    [Tooltip("label for card count of monsters")]
    public GuiLabel CountMonsterCardsLabel;
    [Tooltip("label for card count of spells")]
    public GuiLabel CountSpellCardsLabel;
    [Tooltip("label for card count of weapons")]
    public GuiLabel CountWeaponCardsLabel;
    [Tooltip("reference to the filter list by deck button: 1")]
    public GuiButton deck1Button;
    [Tooltip("reference to the filter list by deck button: 2")]
    public GuiButton deck2Button;
    [Tooltip("reference to the filter list by deck button: 3")]
    public GuiButton deck3Button;
    [Tooltip("reference to the filter list by deck button: 4")]
    public GuiButton deck4Button;
    [Tooltip("reference to the filter list by deck button: 5")]
    public GuiButton deck5Button;
    [Tooltip("reference to the filter list by deck button: 6")]
    public GuiButton deck6Button;
    [Tooltip("reference to the filter list by deck button: base")]
    public GuiButton deckBButton;
    [Tooltip("reference to the filter box by deck button: character")]
    public GuiButton deckCButton;
    [Tooltip("reference to the deck count holder on this panel")]
    public GameObject deckCountHolder;
    [Tooltip("reference to the filter list by deck button: promo")]
    public GuiButton deckPButton;
    [Tooltip("text heading foreground color")]
    public Color HeadingColor;
    [Tooltip("reference to the hilite object in this panel")]
    public GameObject hiliteObject;
    private List<LocationIcon> icons;
    [Tooltip("left margin of items in scroll list (half of the item width)")]
    public float listMarginLeft;
    [Tooltip("top margin of items in the scroll list (half of the item height)")]
    public float listMarginTop;
    [Tooltip("reference to the left-hand side scrolling region on this panel")]
    public GuiScrollRegion listScroller;
    [Tooltip("reference to the location image holder on this panel")]
    public Transform locationArtHolder;
    [Tooltip("reference to the description field on this panel")]
    public GuiLabel locationDescriptionLabel;
    [Tooltip("reference to the name field on this panel")]
    public GuiLabel locationNameLabel;
    [Tooltip("reference to the right-hand side scrolling region on this panel")]
    public GuiScrollRegion paneScroller;
    private string selectedFilter;
    private TKTapRecognizer tapRecognizer;

    public override void Clear()
    {
        this.selectedFilter = null;
        this.ClearSelection();
        for (int i = 0; i < this.icons.Count; i++)
        {
            UnityEngine.Object.Destroy(this.icons[i].gameObject);
        }
        this.icons.Clear();
        this.GlowButtons(null);
        Resources.UnloadUnusedAssets();
    }

    private void ClearSelection()
    {
        this.locationNameLabel.Clear();
        this.locationDescriptionLabel.Clear();
        int childCount = this.locationArtHolder.childCount;
        for (int i = 0; i < childCount; i++)
        {
            UnityEngine.Object.Destroy(this.locationArtHolder.GetChild(i).gameObject);
        }
        this.CountMonsterCardsLabel.Clear();
        this.CountBarrierCardsLabel.Clear();
        this.CountWeaponCardsLabel.Clear();
        this.CountSpellCardsLabel.Clear();
        this.CountArmorCardsLabel.Clear();
        this.CountItemCardsLabel.Clear();
        this.CountAllyCardsLabel.Clear();
        this.CountBlessingCardsLabel.Clear();
        this.hiliteObject.SetActive(false);
        this.deckCountHolder.SetActive(false);
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

    private void HilightLocationIcon(string id)
    {
        this.hiliteObject.SetActive(false);
        for (int i = 0; i < this.icons.Count; i++)
        {
            if (this.icons[i].ID == id)
            {
                this.hiliteObject.transform.position = this.icons[i].transform.position;
                this.hiliteObject.SetActive(true);
                break;
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        this.listScroller.Initialize();
        this.paneScroller.Initialize();
        this.icons = new List<LocationIcon>(30);
    }

    private void OnCardFilterDeck0ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.SelectDeck("B");
        }
    }

    private void OnCardFilterDeck1ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.SelectDeck("1");
        }
    }

    private void OnCardFilterDeck2ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.SelectDeck("2");
        }
    }

    private void OnCardFilterDeck3ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.SelectDeck("3");
        }
    }

    private void OnCardFilterDeck4ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.SelectDeck("4");
        }
    }

    private void OnCardFilterDeck5ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.SelectDeck("5");
        }
    }

    private void OnCardFilterDeck6ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.SelectDeck("6");
        }
    }

    private void OnCardFilterDeckCButtonPushed()
    {
    }

    private void OnCardFilterDeckPButtonPushed()
    {
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            Vector2 origin = base.ScreenToWorldPoint(touchPos);
            if (this.listScroller.Contains((Vector3) origin))
            {
                RaycastHit2D hitd = Physics2D.Raycast(origin, Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_ICON);
                if (hitd != 0)
                {
                    LocationIcon component = hitd.collider.transform.GetComponent<LocationIcon>();
                    if (component != null)
                    {
                        component.Tap();
                        this.SelectLocation(component);
                    }
                }
            }
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        if (this.tapRecognizer != null)
        {
            this.tapRecognizer.enabled = !isPaused;
        }
        this.listScroller.Pause(isPaused);
        this.paneScroller.Pause(isPaused);
    }

    public override void Refresh()
    {
        for (int i = 0; i < this.icons.Count; i++)
        {
            UnityEngine.Object.Destroy(this.icons[i].gameObject);
        }
        this.icons.Clear();
        float listMarginTop = this.listMarginTop;
        for (int j = 0; j < LocationTable.Count; j++)
        {
            string iD = LocationTable.Key(j);
            LocationTableEntry entry = LocationTable.Get(iD);
            if ((entry != null) && (entry.set == this.selectedFilter))
            {
                LocationIcon item = LocationIcon.Create(entry);
                if (item != null)
                {
                    item.ID = iD;
                    item.LoadImage(iD);
                    this.icons.Add(item);
                    this.listScroller.Add(item.transform, this.listMarginLeft, listMarginTop);
                    listMarginTop += item.Height;
                }
            }
        }
        float y = (this.listScroller.Min.y + listMarginTop) - this.listScroller.Size.y;
        this.listScroller.Max = new Vector2(0f, y);
        this.listScroller.Top();
    }

    private void SelectDeck(string deck)
    {
        this.paneScroller.Top();
        this.selectedFilter = deck;
        this.ClearSelection();
        this.GlowButtons(deck);
        this.Refresh();
        if (this.icons.Count > 0)
        {
            this.SelectLocation(this.icons[0]);
        }
    }

    private void SelectLocation(LocationIcon icon)
    {
        if (icon != null)
        {
            LocationTableEntry entry = LocationTable.Get(icon.ID);
            if (entry != null)
            {
                this.HilightLocationIcon(icon.ID);
                this.locationNameLabel.Text = entry.Name;
                this.locationDescriptionLabel.Text = icon.GetDesciption(this.HeadingColor);
                icon.LoadArt(this.locationArtHolder);
                this.deckCountHolder.SetActive(true);
                this.CountMonsterCardsLabel.Text = entry.GetCardCount(CardType.Monster).ToString();
                this.CountBarrierCardsLabel.Text = entry.GetCardCount(CardType.Barrier).ToString();
                this.CountWeaponCardsLabel.Text = entry.GetCardCount(CardType.Weapon).ToString();
                this.CountSpellCardsLabel.Text = entry.GetCardCount(CardType.Spell).ToString();
                this.CountArmorCardsLabel.Text = entry.GetCardCount(CardType.Armor).ToString();
                this.CountItemCardsLabel.Text = entry.GetCardCount(CardType.Item).ToString();
                this.CountAllyCardsLabel.Text = entry.GetCardCount(CardType.Ally).ToString();
                this.CountBlessingCardsLabel.Text = entry.GetCardCount(CardType.Blessing).ToString();
                float y = -1f + this.locationDescriptionLabel.Size.y;
                this.paneScroller.Max = new Vector2(0f, y);
                this.paneScroller.Top();
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.ClearSelection();
            this.tapRecognizer = new TKTapRecognizer();
            this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
            TouchKit.addGestureRecognizer(this.tapRecognizer);
            this.SelectDeck("B");
            this.deckCButton.Show(false);
            this.deckPButton.Show(false);
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
}

