using System;
using UnityEngine;

public class BlockModifyCheck : Block
{
    [Tooltip("amount to modify the checks")]
    public int ModifierAmount = 1;
    [Tooltip("number of turns to apply this modifier")]
    public int ModifierDuration = 1;

    public override void Invoke()
    {
        Effect e = new EffectModifyCheck(Effect.GetEffectID(this), this.ModifierDuration, this.ModifierAmount);
        Turn.Character.ApplyEffect(e);
    }
}

