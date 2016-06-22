using System;
using UnityEngine;

public class BlockModifyDifficulty : Block
{
    [Tooltip("apply this block to check 1")]
    public bool ApplyToCheck1 = true;
    [Tooltip("apply this block to check 2")]
    public bool ApplyToCheck2 = true;
    [Tooltip("a dice roll is in charge of the modifier")]
    public bool DiceRollModifiedCheck;

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (this.ApplyToCheck1)
        {
            Turn.CheckBoard.Set<bool>("BlockModifyCheck1", true);
        }
        if (this.ApplyToCheck2)
        {
            Turn.CheckBoard.Set<bool>("BlockModifyCheck2", true);
        }
        if (this.DiceRollModifiedCheck)
        {
            Turn.CheckBoard.Set<int>("DiceModifyCheck", Turn.DiceTotal);
        }
        Turn.DiceTarget = Rules.GetCheckValue(Turn.Card, Turn.Check);
        if (window != null)
        {
            window.dicePanel.SetTitle(Turn.Card, Turn.Check, Turn.DiceTarget);
            window.dicePanel.Refresh();
        }
    }
}

