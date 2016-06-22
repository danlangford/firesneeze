using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiButton : GuiElement
{
    [Tooltip("sprite to display when button is activated (glowing)")]
    public Sprite activeSprite;
    [Tooltip("allow clicks even when disabled")]
    public bool alwaysActive;
    [Tooltip("reference to the click sound to play when button pushed")]
    public AudioClip clickSound;
    [Tooltip("sprite to display when button is disabled")]
    public Sprite disableSprite;
    [Tooltip("name of the event to send to our containing window")]
    public string EventName;
    [Tooltip("sprite to display when button is pushed")]
    public Sprite hiliteSprite;
    private bool isDisabled;
    private bool isGlowing;
    private bool isSelected;
    private GuiLabel myLabel;
    private GuiPanel myPanel;
    private SpriteRenderer mySprite;
    private GuiWindow myWindow;
    [Tooltip("sprite to display when button is idle")]
    public Sprite normalSprite;
    private TKButtonRecognizer recognizer;

    protected override void Awake()
    {
        base.Awake();
        this.myWindow = UI.Window;
        this.myPanel = base.transform.GetComponentInParent<GuiPanel>();
        if (this.myPanel is GuiScrollRegion)
        {
            this.myPanel = this.myPanel.transform.parent.GetComponentInParent<GuiPanel>();
        }
        this.mySprite = Geometry.GetSubComponent<SpriteRenderer>(this);
        this.myLabel = Geometry.GetSubComponent<GuiLabel>(this);
        if (this.mySprite != null)
        {
            this.RefreshMySprite();
        }
    }

    public void Disable(bool b)
    {
        this.isDisabled = b;
        if (this.recognizer != null)
        {
            this.recognizer.enabled = !b;
        }
        if (this.isDisabled)
        {
            this.SetSprite(this.disableSprite);
        }
        else
        {
            this.Glow(this.isGlowing);
        }
    }

    public void Fade(bool isVisible, float time)
    {
        TextMesh subComponent = Geometry.GetSubComponent<TextMesh>(this);
        if (this.mySprite == null)
        {
            this.mySprite = Geometry.GetSubComponent<SpriteRenderer>(this);
        }
        if (this.mySprite != null)
        {
            if (isVisible)
            {
                if (!this.Visible)
                {
                    this.mySprite.color = new Color(this.mySprite.color.r, this.mySprite.color.g, this.mySprite.color.b, 0f);
                    if (subComponent != null)
                    {
                        subComponent.GetComponent<Renderer>().material.color = new Color(subComponent.color.r, subComponent.color.g, subComponent.color.b, 0f);
                    }
                }
                this.Show(true);
                LeanTween.cancel(this.mySprite.gameObject);
                LeanTween.alpha(this.mySprite.gameObject, 1f, time);
                if (subComponent != null)
                {
                    LeanTween.cancel(subComponent.gameObject);
                    LeanTween.alpha(subComponent.gameObject, 1f, time);
                }
            }
            else if (this.Visible)
            {
                LeanTween.cancel(this.mySprite.gameObject);
                LeanTween.alpha(this.mySprite.gameObject, 0f, time).setOnComplete(() => this.Show(false));
                if (subComponent != null)
                {
                    LeanTween.cancel(subComponent.gameObject);
                    LeanTween.alpha(subComponent.gameObject, 0f, time);
                }
            }
        }
        this.Locked = !isVisible;
    }

    private TKRect GetColliderBounds()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        if (component != null)
        {
            TKRect colliderScreenBounds = Geometry.GetColliderScreenBounds(component);
            component.enabled = false;
            return colliderScreenBounds;
        }
        return new TKRect(0f, 0f, 0f, 0f);
    }

    public TKRect GetScreenBounds()
    {
        if (base.GetComponent<BoxCollider2D>() != null)
        {
            return this.GetColliderBounds();
        }
        return this.GetSpriteScreenBounds();
    }

    private TKRect GetSpriteScreenBounds()
    {
        if (this.mySprite != null)
        {
            Vector3 vector = this.myWindow.Camera.WorldToScreenPoint(this.mySprite.bounds.min);
            Vector3 vector2 = this.myWindow.Camera.WorldToScreenPoint(this.mySprite.bounds.max);
            return new TKRect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
        }
        return new TKRect(0f, 0f, 0f, 0f);
    }

    public void Glow(bool b)
    {
        this.isGlowing = b;
        if (this.isGlowing)
        {
            if (this.activeSprite != null)
            {
                this.SetSprite(this.activeSprite);
            }
            else if (this.hiliteSprite != null)
            {
                this.SetSprite(this.hiliteSprite);
            }
        }
        else if (this.normalSprite != null)
        {
            this.SetSprite(this.normalSprite);
        }
    }

    private bool IsButtonEnabled()
    {
        if (this.Locked)
        {
            return false;
        }
        if (!this.alwaysActive)
        {
            if (this.isDisabled)
            {
                return false;
            }
            if (!base.gameObject.activeInHierarchy)
            {
                return false;
            }
            if ((this.recognizer != null) && !this.recognizer.enabled)
            {
                return false;
            }
        }
        return true;
    }

    private void OnDisable()
    {
        if (this.recognizer != null)
        {
            this.recognizer.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (this.recognizer != null)
        {
            this.recognizer.enabled = true;
        }
    }

    public void Rebind()
    {
        if (this.myWindow != UI.Window)
        {
            this.myWindow = UI.Window;
            if (this.recognizer != null)
            {
                TouchKit.removeGestureRecognizer(this.recognizer);
            }
            this.Start();
        }
    }

    public override void Refresh()
    {
        if (this.recognizer != null)
        {
            this.recognizer.SetBoundaryFrame(this.GetScreenBounds());
        }
    }

    private void RefreshMySprite()
    {
        this.Glow(this.isGlowing);
        this.Disable(this.isDisabled);
        if (this.Selected && (this.hiliteSprite != null))
        {
            this.SetSprite(this.hiliteSprite);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        if (this.mySprite != null)
        {
            this.mySprite.sprite = sprite;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (this.myWindow != null)
        {
            this.recognizer = new TKButtonRecognizer(this.GetScreenBounds());
            this.recognizer.zIndex = Constants.ZINDEX_PANEL_BASE + 10;
            if (this.myPanel != null)
            {
                this.recognizer.zIndex = this.myPanel.zIndex + 10;
            }
            this.recognizer.onSelectedEvent += delegate (TKButtonRecognizer r) {
                if (this.IsButtonEnabled())
                {
                    this.isSelected = true;
                    if (this.hiliteSprite != null)
                    {
                        this.SetSprite(this.hiliteSprite);
                    }
                }
            };
            this.recognizer.onDeselectedEvent += delegate (TKButtonRecognizer r) {
                if (this.IsButtonEnabled())
                {
                    this.isSelected = false;
                    if ((this.normalSprite != null) && !this.isGlowing)
                    {
                        this.SetSprite(this.normalSprite);
                    }
                }
            };
            this.recognizer.onTouchUpInsideEvent += delegate (TKButtonRecognizer r) {
                if (this.IsButtonEnabled())
                {
                    this.isSelected = false;
                    UI.Sound.Play(this.clickSound);
                    if ((this.EventName != null) && (this.EventName.Length > 0))
                    {
                        if (this.myPanel != null)
                        {
                            this.myPanel.SendMessage(this.EventName);
                        }
                        else if (this.myWindow != null)
                        {
                            this.myWindow.SendMessage(this.EventName);
                        }
                    }
                    if (((this.normalSprite != null) && !this.isGlowing) && !this.isDisabled)
                    {
                        this.SetSprite(this.normalSprite);
                    }
                }
            };
            TouchKit.addGestureRecognizer(this.recognizer);
        }
    }

    public void TextTint(Color color)
    {
        if (this.myLabel == null)
        {
            this.myLabel = Geometry.GetSubComponent<GuiLabel>(this);
        }
        if (this.myLabel != null)
        {
            this.myLabel.Color = color;
        }
    }

    public void Tint(Color color)
    {
        SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
        if (componentInChildren != null)
        {
            componentInChildren.color = color;
            if (this.myLabel == null)
            {
                this.myLabel = Geometry.GetSubComponent<GuiLabel>(this);
            }
            if (this.myLabel != null)
            {
                this.myLabel.Color = color;
            }
        }
    }

    public bool Disabled =>
        this.isDisabled;

    public Sprite Image
    {
        get => 
            this.normalSprite;
        set
        {
            this.normalSprite = value;
            if (!this.isDisabled && !this.isGlowing)
            {
                this.SetSprite(this.normalSprite);
            }
        }
    }

    public Sprite ImageActive
    {
        get => 
            this.activeSprite;
        set
        {
            this.activeSprite = value;
            if (this.isGlowing)
            {
                this.SetSprite(this.activeSprite);
            }
        }
    }

    public Sprite ImageDisabled
    {
        get => 
            this.disableSprite;
        set
        {
            this.disableSprite = value;
            if (this.isDisabled)
            {
                this.SetSprite(this.disableSprite);
            }
        }
    }

    public Sprite ImageHilite
    {
        get => 
            this.hiliteSprite;
        set
        {
            this.hiliteSprite = value;
        }
    }

    public bool Locked { get; set; }

    public bool Selected =>
        this.isSelected;

    public string Text
    {
        get
        {
            if (this.myLabel != null)
            {
                return this.myLabel.Text;
            }
            return null;
        }
        set
        {
            if (this.myLabel == null)
            {
                this.myLabel = Geometry.GetSubComponent<GuiLabel>(this);
            }
            if (this.myLabel != null)
            {
                this.myLabel.Text = value;
            }
        }
    }
}

