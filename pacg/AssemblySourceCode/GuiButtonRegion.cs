using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GuiButtonRegion : GuiElement
{
    [Tooltip("sound to play when button is pushed")]
    public AudioClip ClickSound;
    [Tooltip("if true, our collider will stay disabled")]
    public bool DisableCollider = true;
    [Tooltip("name of the event to send to our containing window")]
    public string EventName;
    private GuiPanel myPanel;
    private GuiWindow myWindow;
    private TKButtonRecognizer recognizer;

    protected override void Awake()
    {
        base.Awake();
        this.myWindow = UI.Window;
        this.myPanel = base.transform.parent.GetComponentInParent<GuiPanel>();
    }

    private TKRect GetRegionBounds()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        if (component != null)
        {
            component.enabled = true;
            Vector3 vector = this.myWindow.Camera.WorldToScreenPoint(component.bounds.min);
            Vector3 vector2 = this.myWindow.Camera.WorldToScreenPoint(component.bounds.max);
            component.enabled = !this.DisableCollider;
            return new TKRect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
        }
        return new TKRect(0f, 0f, 0f, 0f);
    }

    private bool IsButtonEnabled()
    {
        if (this.Locked)
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

    public override void Refresh()
    {
        if (this.recognizer != null)
        {
            TKRect regionBounds = this.GetRegionBounds();
            if ((regionBounds.width > 0f) && (regionBounds.height > 0f))
            {
                this.recognizer.SetBoundaryFrame(regionBounds);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        if (this.myWindow != null)
        {
            this.recognizer = new TKButtonRecognizer(this.GetRegionBounds());
            this.recognizer.zIndex = Constants.ZINDEX_PANEL_BASE + 10;
            if (this.myPanel != null)
            {
                this.recognizer.zIndex = this.myPanel.zIndex + 10;
            }
            this.recognizer.onTouchUpInsideEvent += delegate (TKButtonRecognizer r) {
                if (this.IsButtonEnabled())
                {
                    UI.Sound.Play(this.ClickSound);
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
                }
            };
            TouchKit.addGestureRecognizer(this.recognizer);
        }
    }

    public void Tint(Color color)
    {
        SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
        if (componentInChildren != null)
        {
            componentInChildren.color = color;
        }
    }

    public Sprite Image
    {
        set
        {
            SpriteRenderer component = base.GetComponent<SpriteRenderer>();
            if (component != null)
            {
                component.sprite = value;
            }
        }
    }

    public bool Locked { get; set; }
}

