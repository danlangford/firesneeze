using System;
using UnityEngine;

public class AnimationEventTween : MonoBehaviour
{
    [Tooltip("A list of tweens that can be run in any order")]
    public AnimationTweenInfo[] Tweens;

    public void StartTween(int n)
    {
        if ((n >= 0) && (n < this.Tweens.Length))
        {
            this.Tweens[n].Invoke();
        }
    }
}

