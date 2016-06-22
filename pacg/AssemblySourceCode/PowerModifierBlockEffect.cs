using System;

public class PowerModifierBlockEffect : PowerModifier
{
    public BlockEffect Block;

    public override void Activate(int powerIndex)
    {
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
    }

    public override void Deactivate()
    {
        this.Block.RemoveEffect();
    }
}

