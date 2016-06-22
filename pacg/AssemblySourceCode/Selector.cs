using System;
using UnityEngine;

public abstract class Selector : MonoBehaviour
{
    protected Selector()
    {
    }

    protected virtual bool IsEmpty() => 
        true;

    public virtual bool Match() => 
        false;
}

