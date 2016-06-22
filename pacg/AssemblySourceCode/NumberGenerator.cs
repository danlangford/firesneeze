using System;
using UnityEngine;

public abstract class NumberGenerator : MonoBehaviour
{
    [Tooltip("constant bonus added to the generated number")]
    public int Bonus;

    protected NumberGenerator()
    {
    }

    public virtual int Generate() => 
        this.Bonus;
}

