using System;
using UnityEngine;

public abstract class TutorialCommand : MonoBehaviour
{
    [Tooltip("used in the toolset to identify this command; never used in the game")]
    public string Label;

    protected TutorialCommand()
    {
    }

    public virtual void Invoke()
    {
    }
}

