using System;
using UnityEngine;

public class GuiPanelBusy : GuiPanel
{
    private int currentFrame;
    [Tooltip("how long to wait before showing the first image")]
    public float Delay = 0.1f;
    [Tooltip("the images to display in this box (one at a time)")]
    public GameObject[] Frames;
    private float nextFrameTime;
    [Tooltip("the minimum amount of time that each image is displayed")]
    public float Speed = 0.2f;

    public void Center()
    {
        base.transform.position = Vector3.zero;
    }

    public void Center(float y)
    {
        base.transform.position = new Vector3(0f, y, 0f);
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            for (int i = 0; i < this.Frames.Length; i++)
            {
                this.Frames[i].SetActive(false);
            }
            this.nextFrameTime = Time.time + this.Delay;
            this.Tick();
        }
        else
        {
            base.Show(false);
        }
    }

    public void Tick()
    {
        if (Time.time >= this.nextFrameTime)
        {
            if (!this.Visible)
            {
                base.Show(true);
            }
            if (++this.currentFrame >= this.Frames.Length)
            {
                this.currentFrame = 0;
            }
            for (int i = 0; i < this.Frames.Length; i++)
            {
                this.Frames[i].SetActive(i == this.currentFrame);
            }
            this.nextFrameTime = Time.time + this.Speed;
        }
    }
}

