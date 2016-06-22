using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelRules : GuiPanelBackStack
{
    [Tooltip("ref to the main body of the Rules")]
    public GuiLabel Body;
    [Tooltip("the sound to make when clicking entries beneath categories")]
    public AudioClip ClickSound;
    [Tooltip("ref to the close button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("default color of the Glossary Entries")]
    public Color DefaultColor;
    [Tooltip("the master glossary list")]
    public GlossaryEntryList GlossaryEntryList;
    [Tooltip("ref to the left scroll in this screen")]
    public GuiScrollRegion LeftScroll;
    [Tooltip("the approximate height of one line of text in the body")]
    public float LineHeight = 1f;
    [Tooltip("the approximate height of the title")]
    public float LineOffset = 1f;
    private TextMesh prevButton;
    private TKTapRecognizer recognizer;
    [Tooltip("ref to the right scroll in this screen")]
    public GuiScrollRegion RightScroll;
    [Tooltip("color of entries after clicking/tapping them once")]
    public Color SelectedColor;
    [Tooltip("Icon to move to the selected entry/hide if none are selected")]
    public Transform SelectedIcon;
    [Tooltip("when you first open this panel set the right side to this")]
    public UIPopulator StartingButton;
    [Tooltip("the button tabs that expand into more buttons")]
    public GuiButton[] TabButtons;
    [Tooltip("ref to the title of the Rules")]
    public GuiLabel Title;

    private void ConfigureTouchHandler()
    {
        this.recognizer = new TKTapRecognizer();
        this.recognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!base.Paused)
            {
                this.OnGuiTap(this.recognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.recognizer);
        this.recognizer.zIndex = this.LeftScroll.zIndex - 1;
        this.recognizer.enabled = false;
    }

    public override void Initialize()
    {
        if (MainGlossaryEntryList == null)
        {
            MainGlossaryEntryList = this.GlossaryEntryList;
        }
        else
        {
            this.LeftScroll.Top();
            this.RightScroll.Top();
        }
        this.LeftScroll.Initialize();
        this.RightScroll.Initialize();
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy && !base.Paused)
        {
            this.Show(false);
            UI.Window.Pause(false);
        }
    }

    private void OnGuiTap(Vector2 tapLoc)
    {
        RaycastHit2D[] hitdArray = Physics2D.RaycastAll(base.ScreenToWorldPoint(tapLoc), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT);
        if (hitdArray != null)
        {
            for (int i = 0; i < hitdArray.Length; i++)
            {
                GlossaryEntryButton component = hitdArray[i].collider.GetComponent<GlossaryEntryButton>();
                if (component != null)
                {
                    this.Select(component);
                    break;
                }
            }
        }
    }

    private void OnPaneButtonPushed0()
    {
        this.TogglePane(0);
    }

    private void OnPaneButtonPushed1()
    {
        this.TogglePane(1);
    }

    private void OnPaneButtonPushed2()
    {
        this.TogglePane(2);
    }

    private void OnPaneButtonPushed3()
    {
        this.TogglePane(3);
    }

    private void OnPaneButtonPushed4()
    {
        this.TogglePane(4);
    }

    private void OnPaneButtonPushed5()
    {
        this.TogglePane(5);
    }

    public override void Rebind()
    {
        this.ConfigureTouchHandler();
        this.LeftScroll.Rebind();
        this.RightScroll.Rebind();
        this.CloseButton.Rebind();
        for (int i = 0; i < this.TabButtons.Length; i++)
        {
            this.TabButtons[i].Rebind();
        }
    }

    private void RefreshLeftScroll()
    {
        float y = this.LeftScroll.GetComponent<BoxCollider2D>().size.y;
        float num2 = this.TabButtons[this.TabButtons.Length - 1].transform.localPosition.y - 1f;
        GuiGrid grid = this.TabButtons[this.TabButtons.Length - 1].GetComponentsInChildren<GuiGrid>(true)[0];
        if ((grid != null) && grid.gameObject.activeInHierarchy)
        {
            num2 -= grid.TotalHeight;
        }
        this.LeftScroll.Max = new Vector2(0f, Mathf.Max((float) ((this.TabButtons[0].transform.localPosition.y - num2) - y), (float) 0f));
        this.LeftScroll.Reposition();
    }

    private void RefreshRightScroll()
    {
        this.RightScroll.Max = new Vector2(0f, (((this.Body.GetLineCount() * this.LineHeight) + this.RightScroll.Min.y) + this.LineOffset) - this.RightScroll.GetComponent<BoxCollider2D>().size.y);
        this.RightScroll.Top();
    }

    private void RefreshSelectedIcon()
    {
        if (this.prevButton != null)
        {
            this.SelectedIcon.position = new Vector3(this.SelectedIcon.position.x, this.prevButton.transform.position.y, 0f);
        }
    }

    private void Select(GlossaryEntryButton button)
    {
        if (button != null)
        {
            this.UnselectPrevButton();
            UI.Sound.Play(this.ClickSound);
            this.SetSelectedButton(button);
        }
    }

    public void SetRightPanelText(GlossaryCategory category, int index)
    {
        GlossaryEntry entry = category.GetEntryArray()[index];
        if (entry != null)
        {
            this.Title.Text = entry.Title.ToString();
            this.Body.Text = entry.Body.ToString();
        }
    }

    private void SetSelectedButton(GlossaryEntryButton button)
    {
        this.prevButton = button.GetComponent<TextMesh>();
        this.prevButton.color = this.SelectedColor;
        this.SetRightPanelText(button.Category, button.Index);
        this.RefreshSelectedIcon();
        this.SelectedIcon.gameObject.SetActive(true);
        this.RefreshRightScroll();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.recognizer.enabled = isVisible;
        base.ShowWindowButtons(!isVisible);
        this.RefreshLeftScroll();
        this.RefreshRightScroll();
        if (string.IsNullOrEmpty(this.Title.Text))
        {
            this.OnPaneButtonPushed0();
            GlossaryEntryButton componentInChildren = this.StartingButton.GetComponentInChildren<GlossaryEntryButton>(true);
            this.SetSelectedButton(componentInChildren);
        }
    }

    private void ShowPane(int paneNumber, bool show)
    {
        GuiGrid grid = this.TabButtons[paneNumber].GetComponentsInChildren<GuiGrid>(true)[0];
        this.ShowPane(paneNumber, grid, show);
    }

    private void ShowPane(int paneNumber, GuiGrid grid, bool show)
    {
        Vector3 zero;
        GlossaryPopulator component = grid.GetComponent<GlossaryPopulator>();
        if (show)
        {
            component.Load(null);
        }
        if (!grid.gameObject.activeInHierarchy && !show)
        {
            grid.Show(show);
            zero = Vector3.zero;
        }
        else
        {
            grid.Show(show);
            zero = !show ? ((Vector3) (Vector3.up * grid.ActualHeight)) : ((Vector3) (Vector3.down * grid.TotalHeight));
        }
        for (int i = paneNumber + 1; i < this.TabButtons.Length; i++)
        {
            Transform transform = this.TabButtons[i].transform;
            transform.position += zero;
            this.TabButtons[i].Refresh();
        }
        this.RefreshSelectedIcon();
        if ((this.prevButton != null) && (this.prevButton.GetComponent<GlossaryEntryButton>().Category == component.Category))
        {
            this.SelectedIcon.gameObject.SetActive(grid.gameObject.activeInHierarchy);
        }
        this.RefreshLeftScroll();
        this.RefreshRightScroll();
    }

    public void TogglePane(int paneNumber)
    {
        GuiGrid grid = this.TabButtons[paneNumber].GetComponentsInChildren<GuiGrid>(true)[0];
        this.ShowPane(paneNumber, grid, !grid.gameObject.activeInHierarchy);
    }

    private void UnselectPrevButton()
    {
        if (this.prevButton != null)
        {
            this.prevButton.color = this.DefaultColor;
        }
    }

    public override bool Fullscreen =>
        true;

    public static GlossaryEntryList MainGlossaryEntryList
    {
        [CompilerGenerated]
        get => 
            <MainGlossaryEntryList>k__BackingField;
        [CompilerGenerated]
        private set
        {
            <MainGlossaryEntryList>k__BackingField = value;
        }
    }
}

