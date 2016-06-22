using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GuiScrollRegion : GuiPanel
{
    private float currentDeltaTranslation;
    private const float DELTA_SOUND_THRESHOLD = 0.8f;
    private TKPanRecognizer dragRecognizer;
    [Tooltip("allow horizontal scrolling when true")]
    public bool Horizontal;
    private Vector2 maxBounds;
    private Vector2 minBounds;
    [Tooltip("optional: scroll bar graphic")]
    public GameObject ScrollBar;
    [Tooltip("optional: scroll down indicator in scene")]
    public GameObject ScrollIndicator;
    [Tooltip("this is the object that moves")]
    public GameObject ScrollObject;
    private Vector3 scrollStartPosition;
    private Vector3 scrollTop;
    [Tooltip("allow vertical scrolling when true")]
    public bool Vertical = true;

    public void Add(Transform child, float leftOffset, float topOffset)
    {
        child.transform.parent = this.ScrollObject.transform;
        Vector2 scrollSize = this.GetScrollSize();
        child.transform.localPosition = new Vector3((-scrollSize.x / 2f) - leftOffset, (scrollSize.y / 2f) - topOffset, 0f);
    }

    public bool Contains(Vector3 point)
    {
        bool flag = false;
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        if (component != null)
        {
            component.enabled = true;
            flag = component.bounds.Contains(point);
            component.enabled = false;
        }
        return flag;
    }

    private Vector3 GetScrollBarPosition(Vector3 pos)
    {
        float y = this.ScrollBar.GetComponent<Renderer>().bounds.size.y;
        float num2 = (pos.y - this.Min.y) / (this.Max.y - this.Min.y);
        float num3 = (this.GetScrollSize().y - y) * num2;
        return new Vector3(this.ScrollBar.transform.localPosition.x, this.scrollTop.y - num3, this.ScrollBar.transform.localPosition.z);
    }

    public TKRect GetScrollBounds()
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

    private Vector2 GetScrollSize()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        if (component != null)
        {
            component.enabled = true;
            Vector2 size = component.bounds.size;
            component.enabled = false;
            return size;
        }
        return Vector2.zero;
    }

    private Vector3 GetScrollVector(float dx, float dy, float buffer)
    {
        if (this.Vertical)
        {
            if ((this.ScrollObject.transform.localPosition.y + dy) < (this.minBounds.y - buffer))
            {
                dy = (this.minBounds.y - buffer) - this.ScrollObject.transform.localPosition.y;
            }
            if ((this.ScrollObject.transform.localPosition.y + dy) > (this.maxBounds.y + buffer))
            {
                dy = (this.maxBounds.y + buffer) - this.ScrollObject.transform.localPosition.y;
            }
        }
        if (this.Horizontal)
        {
            if ((this.ScrollObject.transform.localPosition.x + dx) < (this.minBounds.x - buffer))
            {
                dx = (this.minBounds.x - buffer) - this.ScrollObject.transform.localPosition.x;
            }
            if ((this.ScrollObject.transform.localPosition.x + dx) < (this.maxBounds.x + buffer))
            {
                dx = (this.maxBounds.x + buffer) - this.ScrollObject.transform.localPosition.x;
            }
        }
        return new Vector3(dx, dy, 0f);
    }

    public override void Initialize()
    {
        this.Rebind();
        this.Reset();
    }

    private bool IsScrollPossible()
    {
        if (this.Vertical && (this.Max.y < 0f))
        {
            return false;
        }
        if (this.Horizontal && (this.Max.x < 0f))
        {
            return false;
        }
        return true;
    }

    protected virtual void OnGuiDrag(Vector2 deltaTranslation)
    {
        if (this.ScrollObject != null)
        {
            float dy = 0f;
            float dx = 0f;
            if (this.Vertical)
            {
                dy = Geometry.ConvertScreenDistanceToWorldDistance(deltaTranslation.y);
            }
            if (this.Horizontal)
            {
                dx = Geometry.ConvertScreenDistanceToWorldDistance(deltaTranslation.x);
            }
            Vector3 vector = this.GetScrollVector(dx, dy, 1f);
            this.PlayScrollSfx(vector.x, vector.y);
            Vector3 pos = this.ScrollObject.transform.localPosition + vector;
            this.SetScrollPosition(pos);
        }
    }

    protected virtual void OnGuiDragStop(Vector2 deltaTranslation)
    {
        if (this.ScrollObject != null)
        {
            float dy = 0f;
            float dx = 0f;
            if (this.Vertical)
            {
                dy = Geometry.ConvertScreenDistanceToWorldDistance(deltaTranslation.y);
            }
            if (this.Horizontal)
            {
                dx = Geometry.ConvertScreenDistanceToWorldDistance(deltaTranslation.x);
            }
            Vector3 to = this.ScrollObject.transform.localPosition + this.GetScrollVector(dx, dy, 0f);
            LeanTween.moveLocal(this.ScrollObject, to, 0.15f).setEase(LeanTweenType.easeOutQuad);
            if (this.ScrollBar != null)
            {
                Vector3 scrollBarPosition = this.GetScrollBarPosition(to);
                LeanTween.moveLocal(this.ScrollBar, scrollBarPosition, 0.15f).setEase(LeanTweenType.easeOutQuad);
            }
            this.currentDeltaTranslation = 0f;
        }
    }

    public override void Pause(bool isPaused)
    {
        if (this.dragRecognizer != null)
        {
            this.dragRecognizer.enabled = !isPaused;
        }
    }

    private void PlayScrollSfx(float dx, float dy)
    {
        this.currentDeltaTranslation = (Mathf.Abs(dy) + Mathf.Abs(dx)) + this.currentDeltaTranslation;
        if (this.currentDeltaTranslation > 0.8f)
        {
            UI.Sound.Play(SoundEffectType.Scrolling);
            this.currentDeltaTranslation = 0f;
        }
    }

    public override void Rebind()
    {
        this.dragRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.dragRecognizer.zIndex = this.zIndex;
        if (base.Owner != null)
        {
            this.dragRecognizer.zIndex = base.Owner.zIndex;
        }
        this.dragRecognizer.boundaryFrame = new TKRect?(this.GetScrollBounds());
        this.dragRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if ((!UI.Busy && !base.Paused) && this.IsScrollPossible())
            {
                this.OnGuiDrag(this.dragRecognizer.deltaTranslation);
            }
        };
        this.dragRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if ((!UI.Busy && !base.Paused) && this.IsScrollPossible())
            {
                this.OnGuiDragStop(this.dragRecognizer.deltaTranslation);
            }
        };
        TouchKit.addGestureRecognizer(this.dragRecognizer);
        this.dragRecognizer.enabled = true;
    }

    public void Reposition()
    {
        this.OnGuiDragStop(Vector2.zero);
    }

    public override void Reset()
    {
        if (this.ScrollObject != null)
        {
            this.scrollStartPosition = this.ScrollObject.transform.localPosition;
            this.minBounds = new Vector2(this.ScrollObject.transform.localPosition.x, this.ScrollObject.transform.localPosition.y);
            this.maxBounds = new Vector2(this.ScrollObject.transform.localPosition.x, this.ScrollObject.transform.localPosition.y);
        }
        if (this.ScrollBar != null)
        {
            this.scrollTop = this.ScrollBar.transform.localPosition;
        }
        if (this.dragRecognizer != null)
        {
            this.dragRecognizer.boundaryFrame = new TKRect?(this.GetScrollBounds());
        }
    }

    private void SetScrollIndicator(Vector3 pos)
    {
        bool flag = pos.y >= this.Max.y;
        this.ScrollIndicator.SetActive(!flag);
    }

    private void SetScrollPosition(Vector3 pos)
    {
        if (this.ScrollObject != null)
        {
            this.ScrollObject.transform.localPosition = pos;
        }
        if (this.ScrollBar != null)
        {
            this.ScrollBar.transform.localPosition = this.GetScrollBarPosition(pos);
        }
        if (this.ScrollIndicator != null)
        {
            this.SetScrollIndicator(pos);
        }
    }

    public void Top()
    {
        this.SetScrollPosition(this.scrollStartPosition);
    }

    public Vector2 Max
    {
        get => 
            this.maxBounds;
        set
        {
            this.maxBounds = value;
        }
    }

    public Vector2 Min
    {
        get => 
            this.minBounds;
        set
        {
            this.minBounds = value;
        }
    }

    public Vector3 Size =>
        ((Vector3) this.GetScrollSize());

    public override uint zIndex =>
        (Constants.ZINDEX_PANEL_FULL + 1);
}

