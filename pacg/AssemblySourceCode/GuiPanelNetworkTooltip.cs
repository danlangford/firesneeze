using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelNetworkTooltip : GuiPanel
{
    private MessageType activeMessage;
    [Tooltip("reference to the animator in our hierarchy")]
    public UnityEngine.Animator Animator;
    [Tooltip("should the panel automatically close?")]
    public bool AutoClose;
    [Tooltip("if the panel has autoclose enabled, how long?")]
    public float AutoCloseTime;
    [Tooltip("reference to the larger close button in our hierachy")]
    public GuiButtonMovable CloseButton;
    [Tooltip("reference to the smaller corner close button in our hierachy")]
    public GuiButtonMovable CornerCloseButton;
    [Tooltip("reference to the gold warning string")]
    public StrRefType GoldWarningString;
    [Tooltip("reference to the label in our hierarchy")]
    public GuiLabel Label;
    private bool m_hiding;
    private float m_timer;
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

    public override void Clear()
    {
        base.Clear();
        this.activeMessage = MessageType.None;
    }

    [DebuggerHidden]
    private IEnumerator Hide() => 
        new <Hide>c__Iterator65 { <>f__this = this };

    private void OnCloseButtonPushed()
    {
        this.Show(false);
    }

    public override void Rebind()
    {
        base.Rebind();
        this.CornerCloseButton.Rebind();
        this.CloseButton.Rebind();
    }

    public override void Refresh()
    {
        base.Refresh();
        this.Rebind();
        switch (this.activeMessage)
        {
            case MessageType.NetworkConnectionLost:
                this.Text = this.NetworkConnectionLostString.ToString();
                break;

            case MessageType.NetworkConnectionFound:
                this.Text = this.NetworkConnectionFoundString.ToString();
                break;

            case MessageType.GoldWarning:
                this.Text = this.GoldWarningString.ToString();
                break;

            case MessageType.MustBeConnected:
                this.Text = this.MustBeConnectedString.ToString();
                break;

            case MessageType.MustBeLoggedIn:
                this.Text = this.MustBeLoggedInString.ToString();
                break;

            case MessageType.OutOfDate:
                this.Text = this.OutOfDateString.ToString();
                break;
        }
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            if (((GuiWindow.Current != null) && (GuiWindow.Current is GuiWindowMainMenu)) && (GuiWindow.Current as GuiWindowMainMenu).statusPanel.Visible)
            {
                (GuiWindow.Current as GuiWindowMainMenu).statusPanel.Show(false);
            }
            base.Show(isVisible);
            if (this.activeMessage == MessageType.None)
            {
                this.Show(false);
            }
            else
            {
                if (this.AutoClose && (this.AutoCloseTime != 0f))
                {
                    if (this.m_timer <= 0f)
                    {
                        this.m_timer = this.AutoCloseTime;
                        this.Animator.SetTrigger("ToolTipOn");
                        this.Animator.SetBool("StayOn", true);
                    }
                    this.m_timer = this.AutoCloseTime;
                }
                else
                {
                    this.Animator.SetBool("StayOn", !this.AutoClose);
                    this.Animator.SetTrigger("ToolTipOn");
                }
                this.Refresh();
            }
        }
        else if (base.gameObject.activeInHierarchy && !this.m_hiding)
        {
            this.m_hiding = true;
            this.m_timer = 0f;
            this.Animator.SetTrigger("ToolTipOff");
            base.StartCoroutine(this.Hide());
        }
    }

    public void ShowMessage(MessageType mt)
    {
        Game.UI.NetworkTooltip.ActiveMessage = mt;
        Game.UI.NetworkTooltip.Show(true);
    }

    private void Update()
    {
        if (this.m_timer > 0f)
        {
            this.m_timer -= Time.deltaTime;
        }
        if (this.m_timer <= 0f)
        {
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

    public string Text
    {
        get => 
            this.Label.Text;
        set
        {
            this.Label.Text = value;
        }
    }

    [CompilerGenerated]
    private sealed class <Hide>c__Iterator65 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelNetworkTooltip <>f__this;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Show(false);
                    this.<>f__this.m_hiding = false;
                    this.$PC = -1;
                    break;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }

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

