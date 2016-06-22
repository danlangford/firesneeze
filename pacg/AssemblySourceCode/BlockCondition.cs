using System;
using UnityEngine;

public class BlockCondition : Block
{
    [Tooltip("optional: the block to run when the conditions are false")]
    public Block BlockFalse;
    [Tooltip("optional: the block to run when the conditions are true")]
    public Block BlockTrue;
    [Tooltip("conditions determine which block to run")]
    public PowerConditionType[] Conditions;

    public override void Invoke()
    {
        if ((this.Conditions != null) && !this.IsConditionValid(Turn.Card))
        {
            if (this.BlockFalse != null)
            {
                this.BlockFalse.Invoke();
            }
        }
        else if (this.BlockTrue != null)
        {
            this.BlockTrue.Invoke();
        }
    }

    protected bool IsConditionValid(Card card) => 
        PowerCondition.Evaluate(card, this.Conditions);

    public override float Length
    {
        get
        {
            if ((this.Conditions != null) && !this.IsConditionValid(Turn.Card))
            {
                if (this.BlockFalse != null)
                {
                    return this.BlockFalse.Length;
                }
                return 0f;
            }
            if (this.BlockTrue != null)
            {
                return this.BlockTrue.Length;
            }
            return 0f;
        }
    }
}

