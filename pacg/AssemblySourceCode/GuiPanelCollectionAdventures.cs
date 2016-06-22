using System;
using System.Collections.Generic;
using UnityEngine;

public class GuiPanelCollectionAdventures : GuiPanel
{
    [Tooltip("reference to the adventure image holder on this panel")]
    public Transform adventureArtHolder;
    private string adventureArtLayerName;
    private int adventureArtLayerOrder;
    [Tooltip("reference to the description field on this panel")]
    public GuiLabel adventureDescriptionLabel;
    [Tooltip("reference to the name field on this panel")]
    public GuiLabel adventureNameLabel;
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
    [Tooltip("reference to the filter list by deck button: promo")]
    public GuiButton deckPButton;
    [Tooltip("text heading foreground color")]
    public Color HeadingColor;
    [Tooltip("reference to the hilite object in this panel")]
    public GameObject hiliteObject;
    private List<ScenarioIcon> icons;
    [Tooltip("left margin of items in scroll list (half of the item width)")]
    public float listMarginLeft;
    [Tooltip("top margin of items in the scroll list (half of the item height)")]
    public float listMarginTop;
    [Tooltip("reference to the left-hand side scrolling region on this panel")]
    public GuiScrollRegion listScroller;
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
        this.adventureNameLabel.Clear();
        this.adventureDescriptionLabel.Clear();
        int childCount = this.adventureArtHolder.childCount;
        for (int i = 0; i < childCount; i++)
        {
            UnityEngine.Object.Destroy(this.adventureArtHolder.GetChild(i).gameObject);
        }
        this.hiliteObject.SetActive(false);
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

    private void HilightScenarioIcon(string id)
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
        this.icons = new List<ScenarioIcon>(10);
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
                    ScenarioIcon component = hitd.collider.transform.GetComponent<ScenarioIcon>();
                    if (component != null)
                    {
                        component.Tap();
                        this.SelectScenario(component);
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
        for (int j = 0; j < ScenarioTable.Count; j++)
        {
            string iD = ScenarioTable.Key(j);
            ScenarioTableEntry entry = ScenarioTable.Get(iD);
            if (((entry != null) && (entry.number > 0)) && (entry.set == this.selectedFilter))
            {
                ScenarioIcon item = ScenarioIcon.Create(entry);
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
            this.SelectScenario(this.icons[0]);
        }
    }

    private void SelectScenario(ScenarioIcon icon)
    {
        if (icon != null)
        {
            ScenarioTableEntry entry = ScenarioTable.Get(icon.ID);
            if (entry != null)
            {
                this.ClearSelection();
                this.HilightScenarioIcon(icon.ID);
                this.adventureNameLabel.Text = entry.Name;
                this.adventureDescriptionLabel.Text = icon.GetDescription(this.HeadingColor);
                Geometry.SetLayerRecursively(icon.LoadArt(this.adventureArtHolder), this.adventureArtLayerName, this.adventureArtLayerOrder);
                float y = -1f + this.adventureDescriptionLabel.Size.y;
                this.paneScroller.Max = new Vector2(0f, y);
                this.paneScroller.Top();
            }
        }
    }

    private void SetupAdventureArtHolder()
    {
        SpriteRenderer componentInChildren = this.adventureArtHolder.GetComponentInChildren<SpriteRenderer>();
        if (componentInChildren != null)
        {
            this.adventureArtLayerName = componentInChildren.sortingLayerName;
            this.adventureArtLayerOrder = componentInChildren.sortingOrder;
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.SetupAdventureArtHolder();
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

