using System;
using UnityEngine;

public class ScenarioPowerActivateBlock : ScenarioPower
{
    [Tooltip("the block to activate")]
    public Block Block;

    public override void Activate()
    {
        base.Activate();
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
    }
}

