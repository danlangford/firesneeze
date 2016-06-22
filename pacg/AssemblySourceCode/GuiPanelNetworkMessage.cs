using System;
using UnityEngine;

public class GuiPanelNetworkMessage : GuiPanelBackStack
{
    [Tooltip("reference to the tutorial messages button on this panel")]
    public GuiButton AcceptButton;
    private MessageType activeMessage;
    private readonly float autoCloseTimer = 30f;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the gold warning string")]
    public StrRefType GoldWarningString;
    [Tooltip("reference to the message label on this panel")]
    public GuiLabel MessageLabel;
    [Tooltip("reference to the must be connected string")]
    public StrRefType MustBeConnectedString;
    [Tooltip("reference to the must be logged in string")]
    public StrRefType MustBeLoggedInString;
    [Tooltip("reference to the network connection found string")]
    public StrRefType NetworkConnectionFoundString;
    [Tooltip("reference to the network connection lost string")]
    public StrRefType NetworkConnectionLostString;
    [Tooltip("reference to the out of date string")]
    public StrRefType OutOfDateString;
    private float timer = 30f;

    public override void Clear()
    {
        base.Clear();
        this.activeMessage = MessageType.None;
    }

    private void OnCloseButtonPushed()
    {
        this.Show(false);
    }

    public override void Refresh()
    {
        switch (this.activeMessage)
        {
            case MessageType.NetworkConnectionLost:
                this.MessageLabel.Text = this.NetworkConnectionLostString.ToString();
                break;

            case MessageType.NetworkConnectionFound:
                this.MessageLabel.Text = this.NetworkConnectionFoundString.ToString();
                break;

            case MessageType.GoldWarning:
                this.MessageLabel.Text = this.GoldWarningString.ToString();
                break;

            case MessageType.MustBeConnected:
                this.MessageLabel.Text = this.MustBeConnectedString.ToString();
                break;

            case MessageType.MustBeLoggedIn:
                this.MessageLabel.Text = this.MustBeLoggedInString.ToString();
                break;

            case MessageType.OutOfDate:
                this.MessageLabel.Text = this.OutOfDateString.ToString();
                break;
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            if (this.activeMessage == MessageType.None)
            {
                this.Show(false);
            }
            else
            {
                this.timer = this.autoCloseTimer;
                this.Refresh();
            }
        }
        else
        {
            this.Clear();
            UI.Busy = false;
        }
    }

    protected override void Update()
    {
        base.Update();
        this.timer -= Time.deltaTime;
        if (this.timer <= 0f)
        {
            this.timer = this.autoCloseTimer;
            this.Show(false);
        }
    }

    public MessageType ActiveMessage
    {
        get => 
            this.activeMessage;
        set
        {
            this.activeMessage = value;
        }
    }

    public override bool Fullscreen =>
        true;

    public enum MessageType
    {
        None,
        NetworkConnectionLost,
        NetworkConnectionFound,
        GoldWarning,
        MustBeConnected,
        MustBeLoggedIn,
        OutOfDate
    }
}

