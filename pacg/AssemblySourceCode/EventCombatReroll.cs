using System;

public class EventCombatReroll : Event
{
    public void CombatReroll()
    {
        Turn.BlackBoard.Set<bool>("CombatReroll", false);
    }

    public override bool IsEventValid(Card card)
    {
        if (!Turn.BlackBoard.Get<bool>("CombatReroll"))
        {
            return false;
        }
        if (((Turn.DiceTarget - Turn.DiceTotal) > 0) && !Turn.Defeat)
        {
            return false;
        }
        if ((Turn.DiceTotal == 0) && Turn.Defeat)
        {
            return false;
        }
        base.GlowCardText(true);
        return true;
    }

    public override void OnBeforeAct()
    {
        Turn.BlackBoard.Set<bool>("CombatReroll", true);
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

