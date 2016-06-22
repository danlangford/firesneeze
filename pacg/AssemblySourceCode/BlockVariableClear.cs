using System;
using UnityEngine;

public class BlockVariableClear : Block
{
    [Tooltip("name of the variable")]
    public string FlagName;

    public override void Invoke()
    {
        Turn.BlackBoard.Set<bool>(this.FlagName, false);
    }
}

