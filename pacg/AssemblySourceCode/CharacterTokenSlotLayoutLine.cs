using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterTokenSlotLayoutLine : MonoBehaviour
{
    private bool isPanning;
    [Tooltip("amount of world space between each slot")]
    public float Padding = 0.1f;
    private TKPanRecognizer panRecognizer;
    private List<CharacterTokenSlot> slots;
    [Tooltip("world space size of each slot")]
    public float SlotWidth = 0.5f;

    private CharacterTokenSlot GetFirstDisplayedSlot()
    {
        for (int i = 0; i < this.slots.Count; i++)
        {
            if (this.slots[i].Owner != null)
            {
                return this.slots[i];
            }
        }
        return null;
    }

    private CharacterTokenSlot GetLastDisplayedSlot()
    {
        for (int i = this.slots.Count - 1; i >= 0; i--)
        {
            if (this.slots[i].Owner != null)
            {
                return this.slots[i];
            }
        }
        return null;
    }

    private TKRect GetLayoutBounds()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        component.enabled = true;
        TKRect colliderScreenBounds = Geometry.GetColliderScreenBounds(component);
        component.enabled = false;
        return colliderScreenBounds;
    }

    private float GetLeftMargin()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        component.enabled = true;
        Vector3 position = new Vector3(component.bounds.min.x, component.bounds.min.y, component.bounds.min.z);
        component.enabled = false;
        return Camera.main.WorldToViewportPoint(position).x;
    }

    private float GetRightMargin()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        component.enabled = true;
        Vector3 position = new Vector3(component.bounds.max.x, component.bounds.max.y, component.bounds.max.z);
        component.enabled = false;
        return Camera.main.WorldToViewportPoint(position).x;
    }

    public Vector3 GetSlotPosition(int i) => 
        (base.transform.position + new Vector3((this.SlotWidth + this.Padding) * i, 0f, 0f));

    public void Initialize(CharacterTokenSlot[] slots)
    {
        this.panRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.panRecognizer.zIndex = 2;
        this.panRecognizer.boundaryFrame = new TKRect?(this.GetLayoutBounds());
        this.panRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy && !UI.Zoomed)
            {
                this.Finger.Calculate(this.panRecognizer.touchLocation());
                if (this.Finger.State == FingerGesture.FingerState.Slide)
                {
                    this.OnGuiPan(this.panRecognizer.deltaTranslation);
                }
            }
        };
        this.panRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if ((!UI.Busy && !UI.Zoomed) && (this.isPanning && (this.Finger.State == FingerGesture.FingerState.Slide)))
            {
                this.OnGuiPanStop(this.panRecognizer.deltaTranslation);
            }
        };
        TouchKit.addGestureRecognizer(this.panRecognizer);
        this.panRecognizer.enabled = false;
        this.slots = new List<CharacterTokenSlot>(slots);
        this.Layout();
    }

    private bool IsTokenInSlot(CharacterTokenSlot slot)
    {
        if (slot.Token == null)
        {
            return false;
        }
        return Geometry.IsNear(slot.gameObject, slot.Token.gameObject);
    }

    public void Layout()
    {
        for (int i = 0; i < this.slots.Count; i++)
        {
            CharacterTokenSlot slot = this.slots[i];
            this.SetSlotPosition(slot, this.GetSlotPosition(i));
        }
    }

    private void OnGuiPan(Vector2 deltaTranslation)
    {
        CharacterTokenSlot firstDisplayedSlot = this.GetFirstDisplayedSlot();
        CharacterTokenSlot lastDisplayedSlot = this.GetLastDisplayedSlot();
        if ((firstDisplayedSlot != null) && (lastDisplayedSlot != null))
        {
            this.isPanning = true;
            float x = Geometry.GetPanDistance(this.GetLeftMargin(), this.SlotWidth, this.GetRightMargin(), firstDisplayedSlot.transform, deltaTranslation, lastDisplayedSlot.transform);
            for (int i = 0; i < this.slots.Count; i++)
            {
                CharacterTokenSlot slot = this.slots[i];
                this.SetSlotPosition(slot, slot.transform.position + new Vector3(x, 0f, 0f));
            }
        }
    }

    private void OnGuiPanStop(Vector2 deltaTranslation)
    {
        CharacterTokenSlot firstDisplayedSlot = this.GetFirstDisplayedSlot();
        CharacterTokenSlot lastDisplayedSlot = this.GetLastDisplayedSlot();
        if ((firstDisplayedSlot != null) && (lastDisplayedSlot != null))
        {
            float x = Geometry.GetPanStopDistance(this.GetLeftMargin(), this.SlotWidth, this.GetRightMargin(), firstDisplayedSlot.transform, deltaTranslation, lastDisplayedSlot.transform);
            for (int i = 0; i < this.slots.Count; i++)
            {
                CharacterTokenSlot slot = this.slots[i];
                Vector3 position = slot.transform.position + new Vector3(x, 0f, 0f);
                if (!LeanTween.isTweening(slot.gameObject))
                {
                    this.TweenSlotPosition(slot, position, 0.3f);
                }
            }
        }
        this.isPanning = false;
        this.Finger.Reset();
    }

    public void Pause(bool isPaused)
    {
        this.panRecognizer.enabled = !isPaused;
    }

    public void Refresh()
    {
        for (int i = 0; i < this.slots.Count; i++)
        {
            this.TweenSlotPosition(this.slots[i], this.GetSlotPosition(i), 0.3f);
        }
    }

    private void SetSlotPosition(CharacterTokenSlot slot, Vector3 position)
    {
        if (this.IsTokenInSlot(slot))
        {
            Vector3 vector = new Vector3(position.x, position.y, slot.Token.transform.position.z);
            slot.Token.transform.position = vector;
        }
        slot.transform.position = position;
    }

    private void TweenSlotPosition(CharacterTokenSlot slot, Vector3 position, float time)
    {
        if (this.IsTokenInSlot(slot))
        {
            Vector3 to = new Vector3(position.x, position.y, slot.Token.transform.position.z);
            LeanTween.move(slot.Token.gameObject, to, time).setEase(LeanTweenType.easeOutQuad);
        }
        LeanTween.move(slot.gameObject, position, time).setEase(LeanTweenType.easeOutQuad);
    }

    public FingerGesture Finger { get; set; }

    public bool Scrolling
    {
        get
        {
            int num = 0;
            for (int i = this.slots.Count - 1; i >= 0; i--)
            {
                if (this.slots[i].Token != null)
                {
                    num = i;
                    break;
                }
            }
            return (num >= 9);
        }
    }
}

