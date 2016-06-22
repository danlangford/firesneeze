using System;
using UnityEngine;

public abstract class Comparator : MonoBehaviour
{
    protected Comparator()
    {
    }

    public virtual bool Compare() => 
        false;
}

