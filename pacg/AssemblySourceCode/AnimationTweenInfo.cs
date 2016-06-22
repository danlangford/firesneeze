using System;
using UnityEngine;

[Serializable]
public class AnimationTweenInfo
{
    [Tooltip("The tween duration in seconds")]
    public float Duration;
    [Tooltip("Optional: easing function.")]
    public LeanTweenType Ease = LeanTweenType.animationCurve;
    [Tooltip("Optional: use this curve for easing.")]
    public AnimationCurve EaseCurve;
    [Tooltip("Optional: move the object to this position.")]
    public ScreenPoint Endpoint;
    [Tooltip("Optional: instantly move the object here.")]
    public ScreenPoint Startpoint;
    [Tooltip("The game object to move")]
    public GameObject Target;

    public void Invoke()
    {
        if (this.Target != null)
        {
            this.Target.SetActive(true);
            if (this.Startpoint != null)
            {
                this.Target.transform.position = this.Startpoint.ToWorldPosition(this.Target);
            }
            if ((this.Endpoint != null) && (this.Duration > 0f))
            {
                if (this.Ease == LeanTweenType.animationCurve)
                {
                    LeanTween.move(this.Target, this.Endpoint.ToWorldPosition(this.Target), this.Duration).setEase(this.EaseCurve);
                }
                else if (this.Ease != LeanTweenType.notUsed)
                {
                    LeanTween.move(this.Target, this.Endpoint.ToWorldPosition(this.Target), this.Duration).setEase(this.Ease);
                }
                else
                {
                    LeanTween.move(this.Target, this.Endpoint.ToWorldPosition(this.Target), this.Duration);
                }
            }
        }
    }
}

