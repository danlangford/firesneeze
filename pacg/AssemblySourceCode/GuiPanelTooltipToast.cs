using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelTooltipToast : GuiPanel
{
    [Tooltip("reference to the animator in our hierarchy")]
    public UnityEngine.Animator Animator;
    [Tooltip("should the panel automatically close?")]
    public bool AutoClose;
    [Tooltip("if the panel has autoclose enabled, how long?")]
    public float AutoCloseTime;
    public GuiButtonMovable CloseButton;
    public GuiButtonMovable CornerCloseButton;
    [Tooltip("reference to the label in our hierarchy")]
    public GuiLabel Label;
    private bool m_hiding;
    private float m_timer;

    [DebuggerHidden]
    private IEnumerator Hide() => 
        new <Hide>c__Iterator79 { <>f__this = this };

    private void OnCloseButtonPushed()
    {
        this.Show(false);
    }

    public override void Rebind()
    {
        this.CornerCloseButton.Rebind();
        this.CloseButton.Rebind();
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            base.Show(isVisible);
            if (GuiWindow.Current != null)
            {
                base.transform.position = GuiWindow.Current.PreferredToastLocation;
            }
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
            this.Rebind();
        }
        else if (!this.m_hiding)
        {
            this.m_hiding = true;
            this.m_timer = 0f;
            this.Animator.SetTrigger("ToolTipOff");
            base.StartCoroutine(this.Hide());
        }
    }

    public void Show(string text)
    {
        this.Text = text;
        this.Show(true);
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
    private sealed class <Hide>c__Iterator79 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelTooltipToast <>f__this;

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
}

