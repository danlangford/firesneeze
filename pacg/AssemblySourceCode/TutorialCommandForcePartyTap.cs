using System;
using UnityEngine;

public class TutorialCommandForcePartyTap : TutorialCommand
{
    private int helpingMember;
    private TKTapRecognizer tapRecognizer;

    private int GetHelpingMember()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((i != Turn.Current) && (Party.Characters[i].CanPlayCard() || Party.Characters[i].CanPlayPower()))
            {
                return i;
            }
        }
        return 0;
    }

    private GuiButton GetPartyMember(int n)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            return window.partyPanel.characterButtons[n];
        }
        return null;
    }

    private void GlowPartyMember(bool isGlowing)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            GuiButton partyMember = this.GetPartyMember(this.helpingMember);
            UI.Window.Glow(partyMember, ButtonType.Portrait, isGlowing);
        }
    }

    public override void Invoke()
    {
        this.helpingMember = this.GetHelpingMember();
        GuiButton partyMember = this.GetPartyMember(this.helpingMember);
        if (partyMember != null)
        {
            UI.Window.Pause(true);
            this.GlowPartyMember(true);
            this.tapRecognizer = new TKTapRecognizer();
            this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
            this.tapRecognizer.boundaryFrame = new TKRect?(partyMember.GetScreenBounds());
            this.tapRecognizer.zIndex = Constants.ZINDEX_PANEL_TOP;
            this.tapRecognizer.enabled = true;
            TouchKit.addGestureRecognizer(this.tapRecognizer);
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        UI.Window.Pause(false);
        this.GlowPartyMember(false);
        this.tapRecognizer.enabled = false;
        TouchKit.removeGestureRecognizer(this.tapRecognizer);
        Turn.Number = this.helpingMember;
        UI.Window.Refresh();
    }
}

