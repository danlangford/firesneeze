using System;
using UnityEngine;

public class BlockSetDamage : Block
{
    [Tooltip("is damage reduction available?")]
    public bool Reduction;

    public override void Invoke()
    {
        Turn.DamageReduction = this.Reduction;
    }
}

