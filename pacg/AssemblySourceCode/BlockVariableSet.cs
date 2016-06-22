using System;
using UnityEngine;

public class BlockVariableSet : Block
{
    [Tooltip("name of the variable")]
    public string FlagName;

    public override void Invoke()
    {
        Turn.BlackBoard.Set<bool>(this.FlagName, true);
    }
}

