using System;

public class EventBeforeActBlock : Event
{
    public Block Block;

    public override void OnBeforeAct()
    {
        if (this.Block != null)
        {
            this.Block.Invoke();
        }
        base.OnBeforeAct();
    }

    public override bool Stateless =>
        ((this.Block == null) || this.Block.Stateless);

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

