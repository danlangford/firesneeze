using System;
using UnityEngine;

public class CardPropertyDamageAmount : CardProperty
{
    [Tooltip("amount added to the damage")]
    public int Bonus;
    [Tooltip("amount multiplied by the damage")]
    public float Multiplier = 1f;

    public int Activate(int damage)
    {
        float f = (this.Multiplier * damage) + this.Bonus;
        return Mathf.CeilToInt(f);
    }
}

