using System;
using UnityEngine;

public class TutorialCommandCharacterSheet : TutorialCommand
{
    [Tooltip("the character sheet pane to show")]
    public int Pane = 4;

    public override void Invoke()
    {
        GuiWindowScenario window = UI.Window as GuiWindowScenario;
        if (window != null)
        {
            if (this.Pane == 1)
            {
                window.characterPanel.SendMessage("OnPane1ButtonPushed");
            }
            if (this.Pane == 2)
            {
                window.characterPanel.SendMessage("OnPane2ButtonPushed");
            }
            if (this.Pane == 3)
            {
                window.characterPanel.SendMessage("OnPane3ButtonPushed");
            }
            if (this.Pane == 4)
            {
                window.characterPanel.SendMessage("OnPane4ButtonPushed");
            }
        }
    }
}

