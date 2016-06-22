using System;
using UnityEngine;

public class AnimationFader : MonoBehaviour
{
    [Tooltip("Easing function used while fading")]
    public LeanTweenType Ease = LeanTweenType.easeInOutQuad;

    private void Fade(float alpha, float time)
    {
        SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>();
        if (componentsInChildren != null)
        {
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i].color.a != alpha)
                {
                    LeanTween.alpha(componentsInChildren[i].gameObject, alpha, time).setEase(this.Ease);
                }
            }
        }
    }

    public void FadeIn(float time)
    {
        this.Fade(1f, time);
    }

    public void FadeOut(float time)
    {
        this.Fade(0f, time);
    }
}

