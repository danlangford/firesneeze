using System;
using UnityEngine;

public class TutorialCommandLockCharacterSheet : TutorialCommand
{
    public bool CloseButton;
    [Header("[Buttons] - Select to Lock; Unselect to Unlock")]
    public bool[] PartyButtons;
    public bool[] TabButtons;

    public override void Invoke()
    {
        GuiWindowScenario window = UI.Window as GuiWindowScenario;
        if (window != null)
        {
            bool flag = this.IsEverythingUnlocked();
            int num = Mathf.Min(window.characterPanel.PartyButtons.Length, this.PartyButtons.Length);
            for (int i = 0; i < num; i++)
            {
                if (window.characterPanel.PartyButtons[i] != null)
                {
                    window.characterPanel.PartyButtons[i].Locked = this.PartyButtons[i];
                    bool flag2 = !flag && !this.PartyButtons[i];
                    window.Glow(window.characterPanel.PartyButtons[i], ButtonType.Portrait, flag2);
                }
            }
            int num3 = Mathf.Min(window.characterPanel.TabButtons.Length, this.TabButtons.Length);
            for (int j = 0; j < num3; j++)
            {
                if (window.characterPanel.TabButtons[j] != null)
                {
                    window.characterPanel.TabButtons[j].Locked = this.TabButtons[j];
                    bool flag3 = !flag && !this.TabButtons[j];
                    window.Glow(window.characterPanel.TabButtons[j], ButtonType.Tab, flag3);
                }
            }
            window.characterPanel.CloseButton.Locked = this.CloseButton;
            bool isGlowing = flag;
            window.Glow(window.characterPanel.CloseButton, ButtonType.Select, isGlowing);
        }
    }

    private bool IsEverythingUnlocked()
    {
        for (int i = 0; i < this.PartyButtons.Length; i++)
        {
            if (this.PartyButtons[i])
            {
                return false;
            }
        }
        for (int j = 0; j < this.TabButtons.Length; j++)
        {
            if (this.TabButtons[j])
            {
                return false;
            }
        }
        return !this.CloseButton;
    }
}

