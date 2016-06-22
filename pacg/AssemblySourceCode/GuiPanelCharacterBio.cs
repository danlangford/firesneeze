using System;
using UnityEngine;

public class GuiPanelCharacterBio : GuiPanel
{
    [Tooltip("reference to the biography text label in our hierarchy")]
    public GuiLabel Biography;
    protected Character currentCharacter;
    [Tooltip("reference to the tab button that brought us here")]
    public GuiButton TabButton;

    public override void Refresh()
    {
        if (this.Character != null)
        {
            this.Biography.Text = this.Character.DisplayText;
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.TabButton.Glow(isVisible);
        if (isVisible)
        {
            this.Refresh();
        }
    }

    public Character Character
    {
        get
        {
            if (this.currentCharacter != null)
            {
                return this.currentCharacter;
            }
            if (Party.Characters.Count > Turn.Number)
            {
                return Party.Characters[Turn.Number];
            }
            return null;
        }
        set
        {
            this.currentCharacter = value;
        }
    }
}

