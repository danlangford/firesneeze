using System;
using UnityEngine;

public class AnimationRandomizer : MonoBehaviour
{
    private int frameCounter;
    private Animator myAnimator;
    [Tooltip("Number of seconds between randomizations; zero means only once")]
    public float Period = 1f;
    [Tooltip("Number of random seconds plus-or-minus applied to the period")]
    public float PeriodDelta;
    [Tooltip("Maximum number that can be generated")]
    public float RangeMax = 1f;
    [Tooltip("Minimum number that can be generated")]
    public float RangeMin;
    [Tooltip("Mecanim variable name")]
    public string VariableName = "RandomFloat";

    private void Awake()
    {
        this.myAnimator = base.GetComponent<Animator>();
        this.SetRandomNumber();
    }

    private void SetRandomNumber()
    {
        if (this.myAnimator != null)
        {
            float num = UnityEngine.Random.Range(this.RangeMin, this.RangeMax);
            this.myAnimator.SetFloat(this.VariableName, num);
        }
    }

    private void Update()
    {
        if (this.Period > 0f)
        {
            if (this.frameCounter <= 0)
            {
                float num = this.Period + UnityEngine.Random.Range(-this.PeriodDelta, this.PeriodDelta);
                num = Mathf.Clamp(num, 0f, num);
                this.frameCounter = Timer.ConvertSecondsToFrames(num);
                this.SetRandomNumber();
            }
            else
            {
                this.frameCounter--;
            }
        }
    }
}

