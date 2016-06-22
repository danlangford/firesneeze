using System;

public class GuiPanelTutorialOverlay : GuiPanelTutorialPopup
{
    private bool controllingBusy;
    private bool controllingDice;
    private bool controllingWindow;

    public override void Display(int id)
    {
        this.LockScreen(true);
        base.Display(id);
    }

    private void LockScreen(bool isLocked)
    {
        if (isLocked)
        {
            base.InterceptBackgroundTouches(true);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                this.controllingDice = window.dicePanel.IsDiceVisible();
                if (this.controllingDice)
                {
                    this.ShowDice(false);
                }
            }
            this.controllingWindow = !UI.Window.Paused;
            if (this.controllingWindow)
            {
                UI.Window.Pause(true);
            }
            this.controllingBusy = !UI.Busy;
            if (this.controllingBusy)
            {
                UI.Busy = true;
            }
        }
        else
        {
            base.InterceptBackgroundTouches(false);
            GuiWindowLocation location2 = UI.Window as GuiWindowLocation;
            if (location2 != null)
            {
                if (this.controllingDice)
                {
                    this.ShowDice(true);
                }
                this.controllingDice = false;
            }
            if (this.controllingWindow)
            {
                UI.Window.Pause(false);
            }
            this.controllingWindow = false;
            if (this.controllingBusy)
            {
                UI.Busy = false;
            }
            this.controllingBusy = false;
        }
    }

    protected override void OnCloseButtonClicked()
    {
        this.LockScreen(false);
        base.OnCloseButtonClicked();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (!isVisible)
        {
            this.LockScreen(false);
        }
    }

    private void ShowDice(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Show(isVisible);
        }
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;
}

