using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FingerGesture
{
    private Vector2 fingerStartLocation;
    private float fingerStartTime;
    private FingerState fingerState;

    public event Action<FingerGesture> gestureRecognizedEvent;

    public FingerState Calculate(Vector2 touchPos)
    {
        if (!this.Locked)
        {
            if (this.fingerState == FingerState.None)
            {
                this.fingerStartLocation = touchPos;
                this.fingerStartTime = Time.time;
                this.fingerState = FingerState.Test;
            }
            else if ((this.fingerState == FingerState.Test) && (Mathf.Abs((float) (Time.time - this.fingerStartTime)) >= 0.025f))
            {
                float num2 = Mathf.Abs((float) (touchPos.y - this.fingerStartLocation.y));
                if (Mathf.Abs((float) (touchPos.x - this.fingerStartLocation.x)) > (4f * num2))
                {
                    this.fingerState = FingerState.Slide;
                }
                else
                {
                    this.fingerState = FingerState.Drag;
                }
                this.FireGestureRecognized();
            }
        }
        return this.fingerState;
    }

    private void FireGestureRecognized()
    {
        if (this.gestureRecognizedEvent != null)
        {
            this.gestureRecognizedEvent(this);
        }
    }

    public void Lock(FingerState state)
    {
        this.fingerState = state;
        this.Locked = true;
    }

    public void Reset()
    {
        if (!this.Locked)
        {
            this.fingerState = FingerState.None;
        }
    }

    public bool Locked { get; set; }

    public FingerState State
    {
        get => 
            this.fingerState;
        set
        {
            this.fingerState = value;
        }
    }

    public enum FingerState
    {
        None,
        Test,
        Drag,
        Slide
    }
}

