using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GuiSwipeRegion : GuiElement
{
    [Tooltip("...")]
    public string DownEventName;
    [Tooltip("...")]
    public string LeftEventName;
    private GuiPanel myPanel;
    private GuiWindow myWindow;
    private TKSwipeRecognizer recognizer;
    [Tooltip("...")]
    public string RightEventName;
    [Tooltip("names of methods to invoke in the panel or window")]
    public string UpEventName;

    protected override void Awake()
    {
        base.Awake();
        this.myWindow = UI.Window;
        this.myPanel = base.transform.GetComponentInParent<GuiPanel>();
    }

    private void DispatchEvent(string eventName)
    {
        if (!string.IsNullOrEmpty(eventName))
        {
            if (this.myPanel != null)
            {
                this.myPanel.SendMessage(eventName);
            }
            else if (this.myWindow != null)
            {
                this.myWindow.SendMessage(eventName);
            }
        }
    }

    private TKRect GetSwipeBounds()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        if (component != null)
        {
            component.enabled = true;
            TKRect colliderScreenBounds = Geometry.GetColliderScreenBounds(component);
            component.enabled = false;
            return colliderScreenBounds;
        }
        return new TKRect(0f, 0f, 0f, 0f);
    }

    public void Move(Vector2 delta)
    {
        TKRect rect = this.recognizer.boundaryFrame.Value;
        TKRect rect2 = new TKRect(rect.x + delta.x, rect.y + delta.y, rect.width, rect.height);
        this.recognizer.boundaryFrame = new TKRect?(rect2);
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

    private void OnGuiSwipe(TKSwipeDirection direction)
    {
        if (direction == TKSwipeDirection.Up)
        {
            this.DispatchEvent(this.UpEventName);
        }
        if (direction == TKSwipeDirection.Down)
        {
            this.DispatchEvent(this.DownEventName);
        }
        if (direction == TKSwipeDirection.Left)
        {
            this.DispatchEvent(this.LeftEventName);
        }
        if (direction == TKSwipeDirection.Right)
        {
            this.DispatchEvent(this.RightEventName);
        }
    }

    protected override void Start()
    {
        base.Start();
        this.recognizer = new TKSwipeRecognizer(0.5f, 3f);
        this.recognizer.boundaryFrame = new TKRect?(this.GetSwipeBounds());
        this.recognizer.gestureRecognizedEvent += delegate (TKSwipeRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiSwipe(this.recognizer.completedSwipeDirection);
            }
        };
        TouchKit.addGestureRecognizer(this.recognizer);
        this.recognizer.zIndex = 20;
        this.recognizer.enabled = true;
    }
}

