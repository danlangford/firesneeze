using System;
using UnityEngine;

public class GuiPanelMessage : GuiPanel
{
    [Tooltip("reference to a graphic background")]
    public GameObject Background;
    [Tooltip("reference to a text label contained in our hierarchy")]
    public GuiLabel MessageTextLabel;
    private Vector3 Scale;

    public override void Clear()
    {
        this.MessageTextLabel.Clear();
        this.Show(false);
    }

    public override void Initialize()
    {
        this.Show(false);
        this.Scale = this.MessageTextLabel.transform.localScale;
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (this.Background != null)
        {
            if (isVisible)
            {
                this.Background.SetActive(true);
                this.Background.transform.localScale = Vector3.zero;
                LeanTween.scale(this.Background.gameObject, Vector3.one, 0.1f);
            }
            else
            {
                this.Background.SetActive(false);
            }
        }
    }

    public void Show(string text)
    {
        this.Show(true);
        this.MessageTextLabel.Text = text;
        this.MessageTextLabel.transform.localScale = Vector3.zero;
        LeanTween.scale(this.MessageTextLabel.gameObject, this.Scale, 0.2f).setEase(LeanTweenType.easeInQuad);
    }
}

