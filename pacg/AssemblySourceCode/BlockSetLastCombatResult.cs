using System;
using UnityEngine;

public class BlockSetLastCombatResult : Block
{
    [Tooltip("should we set ourself or the turn card?")]
    public bool ApplyToSelf = true;
    [Tooltip("the disposition of This card")]
    public DispositionType Disposition;
    [Tooltip("the result that will be set")]
    public CombatResultType Result;

    public override void Invoke()
    {
        Turn.LastCombatResult = this.Result;
        if (this.ApplyToSelf)
        {
            base.Card.Disposition = this.Disposition;
        }
        else
        {
            Turn.Card.Disposition = this.Disposition;
        }
    }
}

