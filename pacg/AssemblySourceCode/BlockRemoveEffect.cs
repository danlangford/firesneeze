using System;
using UnityEngine;

public class BlockRemoveEffect : Block
{
    [Tooltip("ID of the effect to remove")]
    public string EffectID;
    [Tooltip("by default remove effect from entire party")]
    public bool EntireParty = true;

    public override void Invoke()
    {
        if (this.EntireParty)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                Party.Characters[i].RemoveEffect(this.EffectID);
            }
        }
        else
        {
            Turn.Owner.RemoveEffect(this.EffectID);
        }
    }
}

