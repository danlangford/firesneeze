using System;
using UnityEngine;

public abstract class TutorialCondition : MonoBehaviour
{
    [Tooltip("used in the toolset to identify this condition; never used in the game")]
    public string Label;
    [Tooltip("if true, this condition will be reversed")]
    public bool Not;

    protected TutorialCondition()
    {
    }

    public virtual bool Evaluate() => 
        true;
}

