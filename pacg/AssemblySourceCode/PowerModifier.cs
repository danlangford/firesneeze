using System;
using UnityEngine;

public abstract class PowerModifier : MonoBehaviour
{
    protected PowerModifier()
    {
    }

    public virtual void Activate(int powerIndex)
    {
    }

    public virtual void Deactivate()
    {
    }

    public virtual bool IsValidationRequired() => 
        true;
}

