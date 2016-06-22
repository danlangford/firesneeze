using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GuiImage : GuiElement
{
    public void Clear()
    {
        this.Image = null;
    }

    public void Fade(bool isVisible, float time)
    {
        if (isVisible)
        {
            this.FadeIn(time);
        }
        else
        {
            this.FadeOut(time);
        }
    }

    public void FadeIn(float time)
    {
        if (this.Image != null)
        {
            this.Show(true);
            SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].color = new Color(componentsInChildren[i].color.r, componentsInChildren[i].color.g, componentsInChildren[i].color.b, 0f);
                LeanTween.alpha(componentsInChildren[i].gameObject, 1f, time);
            }
        }
    }

    public void FadeOut(float time)
    {
        if (this.Image != null)
        {
            SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                LeanTween.alpha(componentsInChildren[i].gameObject, 0f, time);
            }
        }
    }

    public Sprite Image
    {
        get
        {
            SpriteRenderer component = base.GetComponent<SpriteRenderer>();
            if (component != null)
            {
                return component.sprite;
            }
            return null;
        }
        set
        {
            SpriteRenderer component = base.GetComponent<SpriteRenderer>();
            if (component != null)
            {
                component.sprite = value;
            }
        }
    }
}

