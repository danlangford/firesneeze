using System;

public class EventCheckModifierRollBlock : Event
{
    public override int GetCheckModifier()
    {
        if (this.IsEventValid(Turn.Card))
        {
            if (Turn.CheckBoard.Get<bool>("BlockModifyCheck1") && (Turn.CombatCheckSequence == 1))
            {
                int num = Turn.CheckBoard.Get<int>("DiceModifyCheck");
                if (num > 0)
                {
                    return num;
                }
            }
            if (Turn.CheckBoard.Get<bool>("BlockModifyCheck2") && (Turn.CombatCheckSequence == 2))
            {
                int num2 = Turn.CheckBoard.Get<int>("DiceModifyCheck");
                if (num2 > 0)
                {
                    return num2;
                }
            }
        }
        return 0;
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }
}

