using System;
using UnityEngine;

public class GuiPanelNotLoggedIn : GuiPanelBackStack
{
    private GuiWindowMainMenu.ButtonType callbackButtonType;
    [Tooltip("reference to the quit button on this panel")]
    public GuiButton ContinueButton;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CornerCloseButton;
    [Tooltip("reference to the login button on this panel")]
    public GuiButton LoginButton;

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    private void OnContinueButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
            GuiWindow current = GuiWindow.Current;
            if (current != null)
            {
                GuiWindowMainMenu menu = current as GuiWindowMainMenu;
                if (menu != null)
                {
                    menu.ExternalClick(this.callbackButtonType);
                }
            }
        }
    }

    private void OnLoginButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
            if (Game.Network.HasNetworkConnection)
            {
                Game.Network.Login();
            }
            else
            {
                GuiWindow current = GuiWindow.Current;
                if ((current != null) && (current is GuiWindowMainMenu))
                {
                    Game.UI.Toast.Show((current as GuiWindowMainMenu).StoreConnectionTooltip.ToString());
                }
            }
        }
    }

    public override void Rebind()
    {
        this.LoginButton.Rebind();
        this.CornerCloseButton.Rebind();
        this.ContinueButton.Rebind();
    }

    public override void Show(bool isVisible)
    {
        if (Game.UI.NetworkTooltip.Visible)
        {
            Game.UI.NetworkTooltip.Show(false);
        }
        if (((GuiWindow.Current != null) && (GuiWindow.Current is GuiWindowMainMenu)) && (GuiWindow.Current as GuiWindowMainMenu).statusPanel.Visible)
        {
            (GuiWindow.Current as GuiWindowMainMenu).statusPanel.Show(false);
        }
        base.Show(isVisible);
        if (base.Owner != null)
        {
            base.Owner.Pause(isVisible);
        }
        else
        {
            UI.Window.Pause(isVisible);
        }
        if (!isVisible)
        {
            base.Owner = null;
        }
        else
        {
            this.Refresh();
            UI.Busy = false;
        }
    }

    public GuiWindowMainMenu.ButtonType CallbackButtonType
    {
        get => 
            this.callbackButtonType;
        set
        {
            this.callbackButtonType = value;
        }
    }

    public override bool Fullscreen =>
        true;
}

