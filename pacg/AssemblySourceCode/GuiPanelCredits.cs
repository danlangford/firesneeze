using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelCredits : GuiScrollRegion
{
    private float bottom;
    [Tooltip("local y offset of the bottom of this panel")]
    public float BottomMargin = -5f;
    private float haltTime;
    [Tooltip("local y offset of hte top of this panel")]
    public float LeftMargin = -3f;
    [Tooltip("world space height of a standard line")]
    public float LineHeight = 0.5f;
    [Tooltip("not used")]
    public float RightMargin = 3f;
    [Tooltip("how fast should the panel scroll itself?")]
    public float Speed = 1f;
    private float top;
    [Tooltip("not used")]
    public float TopMargin = 5f;

    public void Add(float height)
    {
        this.top -= this.ConvertHeightToWorld(height);
    }

    public void Add(GuiImage image, TextAlignment alignment)
    {
        float x = this.ConvertAlignmentToWorld(alignment);
        image.transform.parent = base.ScrollObject.transform;
        image.transform.localPosition = new Vector3(x, this.top, 0f);
        this.top -= image.Image.bounds.extents.y;
    }

    public void Add(GuiLabel label, float height)
    {
        float x = this.ConvertAlignmentToWorld(label.Alignment);
        label.transform.parent = base.ScrollObject.transform;
        label.transform.localPosition = new Vector3(x, this.top, 0f);
        this.top -= this.ConvertHeightToWorld(height);
    }

    private float ConvertAlignmentToWorld(TextAlignment alignment)
    {
        if (alignment == TextAlignment.Left)
        {
            return this.LeftMargin;
        }
        if (alignment == TextAlignment.Right)
        {
            return this.RightMargin;
        }
        return 0f;
    }

    private float ConvertHeightToWorld(float height) => 
        ((this.LineHeight * height) / 10f);

    public override void Initialize()
    {
        base.Initialize();
        this.top = this.BottomMargin;
        this.bottom = this.BottomMargin;
        this.Pause(true);
    }

    protected override void OnGuiDrag(Vector2 deltaTranslation)
    {
        base.OnGuiDrag(deltaTranslation);
        this.haltTime = Time.time;
        this.Halted = true;
    }

    protected override void OnGuiDragStop(Vector2 deltaTranslation)
    {
        base.OnGuiDragStop(deltaTranslation);
        this.haltTime = Time.time;
        this.Halted = true;
    }

    public void Scroll()
    {
        this.bottom += this.top;
        base.Min = new Vector2(0f, 0f);
        base.Max = new Vector2(0f, -this.bottom);
        this.Pause(false);
    }

    private void ServiceHaltTimer()
    {
        if (this.Halted && (Time.time >= (this.haltTime + 5f)))
        {
            this.Halted = false;
        }
    }

    private void Update()
    {
        this.ServiceHaltTimer();
        if (!base.Paused && !this.Halted)
        {
            if (this.Finished)
            {
                base.Top();
                this.Finished = false;
            }
            else
            {
                float y = Time.deltaTime * this.Speed;
                Transform transform = base.ScrollObject.transform;
                transform.localPosition += new Vector3(0f, y, 0f);
                if (base.ScrollObject.transform.localPosition.y > Mathf.Abs(this.bottom))
                {
                    this.Finished = true;
                }
            }
        }
    }

    public bool Finished { get; private set; }

    public bool Halted { get; private set; }
}

