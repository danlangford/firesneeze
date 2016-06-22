using System;
using UnityEngine;

public class GuiPanelWelcome : GuiPanelBackStack
{
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CornerCloseButton;
    [Tooltip("reference to the create account button on this panel")]
    public GuiButton CreateAccountButton;
    [Tooltip("reference to the login with facebook button on this panel")]
    public GuiButton FacebookButton;
    [Tooltip("reference to the login with existing button on this panel")]
    public GuiButton LoginButton;
    [Tooltip("reference to the description label on this panel")]
    public GuiLabel WelcomeDescriptionLabel;
    [Tooltip("reference to the title label on this panel")]
    public GuiLabel WelcomeLabel;

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    private void OnCreateAccountButtonPushed()
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
                    menu.createAccountPanel.Show(true);
                }
            }
        }
    }

    private void OnFacebookButtonPushed()
    {
        if (!UI.Busy)
        {
            if (!Game.Network.OutOfDate && Game.Network.HasNetworkConnection)
            {
                this.Show(false);
                if (Game.Network.Connected)
                {
                    PlayFabLoginCalls.LinkFBAccount();
                }
                else
                {
                    PlayFabLoginCalls.LoginWithDeviceIdAndLinkFBAccount();
                }
            }
            else if (Game.Network.OutOfDate)
            {
                GuiWindow current = GuiWindow.Current;
                if ((current != null) && (current is GuiWindowMainMenu))
                {
                    (current as GuiWindowMainMenu).outOfDatePanel.Show(true);
                }
            }
            else if (!Game.Network.HasNetworkConnection)
            {
                GuiWindow window2 = GuiWindow.Current;
                if ((window2 != null) && (window2 is GuiWindowMainMenu))
                {
                    Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeConnected);
                }
            }
        }
    }

    private void OnLoginButtonPushed()
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
                    menu.loginPanel.Show(true);
                }
            }
        }
    }

    public override void Rebind()
    {
        this.CornerCloseButton.Rebind();
        this.LoginButton.Rebind();
        this.FacebookButton.Rebind();
        this.CreateAccountButton.Rebind();
    }

    public override void Show(bool isVisible)
    {
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
            UI.Busy = false;
        }
    }

    public override bool Fullscreen =>
        true;
}

