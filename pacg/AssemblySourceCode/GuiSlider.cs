using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GuiSlider : GuiElement
{
    private float boundsMax;
    private float boundsMin;
    [Tooltip("name of the event to send to our containing window")]
    public string EventName;
    [Tooltip("reference to the knob in our hierarchy")]
    public GameObject myKnob;
    private GuiPanel myPanel;
    private GuiWindow myWindow;
    private TKPanRecognizer recognizer;

    protected override void Awake()
    {
        base.Awake();
        this.myWindow = UI.Window;
        this.myPanel = base.transform.GetComponentInParent<GuiPanel>();
    }

    private TKRect GetScrollBounds()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        if (component != null)
        {
            component.enabled = true;
            this.boundsMin = component.bounds.min.x;
            this.boundsMax = component.bounds.max.x;
            TKRect colliderScreenBounds = Geometry.GetColliderScreenBounds(component);
            component.enabled = false;
            return colliderScreenBounds;
        }
        return new TKRect(0f, 0f, 0f, 0f);
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

    private void OnGuiDrag(Vector2 position)
    {
        float x = Mathf.Clamp(Geometry.ScreenToWorldPoint(position).x, this.boundsMin, this.boundsMax);
        this.myKnob.transform.position = new Vector3(x, this.myKnob.transform.position.y, this.myKnob.transform.position.z);
    }

    private void OnGuiDragStop(Vector2 position)
    {
        UI.Sound.Play(SoundEffectType.GenericClick);
        if (!string.IsNullOrEmpty(this.EventName))
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

    public void Rebind()
    {
        if (this.myWindow != UI.Window)
        {
            this.myWindow = UI.Window;
            this.Start();
        }
    }

    protected override void Start()
    {
        base.Start();
        if (this.myWindow != null)
        {
            this.recognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
            this.recognizer.boundaryFrame = new TKRect?(this.GetScrollBounds());
            this.recognizer.zIndex = 10;
            if (this.myPanel != null)
            {
                this.recognizer.zIndex = this.myPanel.zIndex;
            }
            this.recognizer.gestureRecognizedEvent += r => this.OnGuiDrag(this.recognizer.touchLocation());
            this.recognizer.gestureCompleteEvent += r => this.OnGuiDragStop(this.recognizer.touchLocation());
            TouchKit.addGestureRecognizer(this.recognizer);
        }
    }

    public float Value
    {
        get
        {
            if ((this.boundsMin == 0f) && (this.boundsMax == 0f))
            {
                this.GetScrollBounds();
            }
            float num = this.boundsMax - this.boundsMin;
            if (num != 0f)
            {
                return ((this.myKnob.transform.position.x - this.boundsMin) / num);
            }
            return 0f;
        }
        set
        {
            if ((this.boundsMin == 0f) && (this.boundsMax == 0f))
            {
                this.GetScrollBounds();
            }
            float x = this.boundsMin + (value * (this.boundsMax - this.boundsMin));
            this.myKnob.transform.position = new Vector3(x, this.myKnob.transform.position.y, this.myKnob.transform.position.z);
        }
    }
}

