using System;
using UnityEngine;

public class TutorialCommandLockDice : TutorialCommand
{
    [Tooltip("if true, the player will not be able to roll the dice")]
    public bool Locked = true;

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Locked = this.Locked;
        }
    }
}

