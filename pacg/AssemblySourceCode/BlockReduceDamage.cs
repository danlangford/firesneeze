using System;
using UnityEngine;

public class BlockReduceDamage : Block
{
    [Tooltip("all damage is reduced to 0 if set to true")]
    public bool All;
    [Tooltip("the amount of damage to be reduced")]
    public int Amount;

    private int GetAmount()
    {
        if (this.All)
        {
            return Turn.Damage;
        }
        return this.Amount;
    }

    public override void Invoke()
    {
        Turn.Damage -= this.GetAmount();
    }
}

