using System;

public class TutorialCommandDice : TutorialCommand
{
    public int D10 = 1;
    public int D12 = 1;
    public int D4 = 1;
    public int D6 = 1;
    public int D8 = 1;

    public override void Invoke()
    {
        Rules.CrookedDice(DiceType.D4, this.D4);
        Rules.CrookedDice(DiceType.D6, this.D6);
        Rules.CrookedDice(DiceType.D8, this.D8);
        Rules.CrookedDice(DiceType.D10, this.D10);
        Rules.CrookedDice(DiceType.D12, this.D12);
    }
}

