using System;
using UnityEngine;

public class AnimationScreenShake : MonoBehaviour
{
    [Tooltip("Number of meters of movement")]
    public float Distance = 0.1f;
    [Tooltip("Number of times to shake the screen")]
    public int Duration = 10;
    [Tooltip("Number of seconds that each movement takes")]
    public float Speed = 0.03f;

    public void Shake()
    {
        UI.CameraManager.Shake(this.Duration, this.Distance, this.Speed);
    }
}

