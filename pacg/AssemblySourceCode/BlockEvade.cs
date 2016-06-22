using System;

public class BlockEvade : Block
{
    public override void Invoke()
    {
        Turn.Evade = true;
        Turn.Proceed();
    }
}

