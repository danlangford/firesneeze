using System;
using UnityEngine;

public class BlockSequence : Block
{
    [Tooltip("list of blocks to be invoked")]
    public Block[] Blocks;

    public override void Invoke()
    {
        for (int i = 0; i < this.Blocks.Length; i++)
        {
            this.Blocks[i].Invoke();
        }
    }

    public override bool Stateless
    {
        get
        {
            for (int i = 0; i < this.Blocks.Length; i++)
            {
                if (!this.Blocks[i].Stateless)
                {
                    return false;
                }
            }
            return base.Stateless;
        }
    }
}

