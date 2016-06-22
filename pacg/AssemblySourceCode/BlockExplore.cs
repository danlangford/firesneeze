using System;

public class BlockExplore : Block
{
    public override void Invoke()
    {
        if (Rules.IsTurnOwner())
        {
            Turn.Explore = true;
        }
    }
}

