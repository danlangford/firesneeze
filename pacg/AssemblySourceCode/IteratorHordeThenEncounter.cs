using System;

public class IteratorHordeThenEncounter : IteratorHorde
{
    protected override void FinishDispose()
    {
        Turn.Card.Show(true);
        GameStateType state = Turn.State;
        Turn.Card.OnBeforeAct();
        if (Turn.State == state)
        {
            Turn.State = GameStateType.Combat;
        }
    }

    public override bool IsType(TurnStateIteratorType type) => 
        (base.IsType(type) || (type == TurnStateIteratorType.Horde));

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.HordeThenEncounter;
}

