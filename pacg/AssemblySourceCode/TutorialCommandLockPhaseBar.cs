using System;
using UnityEngine;

public class TutorialCommandLockPhaseBar : TutorialCommand
{
    [Tooltip("should the phase bar be locked or unlocked?")]
    public bool Locked = true;

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.commandsPanel.Locked = this.Locked;
        }
    }
}

