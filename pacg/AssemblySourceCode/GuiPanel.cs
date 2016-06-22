using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanel : GuiElement
{
    private static int fullScreenPanelCount;
    private bool isCancelVisible;
    private bool isPaused;
    private bool isProceedVisible;
    private TKEveryTouchRecognizer touchInterceptor;

    public virtual void Clear()
    {
    }

    public virtual void Initialize()
    {
    }

    protected void InterceptBackgroundTouches(bool isIntercepting)
    {
        if (isIntercepting)
        {
            if (this.touchInterceptor == null)
            {
                this.touchInterceptor = new TKEveryTouchRecognizer();
                this.touchInterceptor.zIndex = this.zIndex - 1;
                TouchKit.addGestureRecognizer(this.touchInterceptor);
            }
        }
        else if (this.touchInterceptor != null)
        {
            TouchKit.removeGestureRecognizer(this.touchInterceptor);
            this.touchInterceptor = null;
        }
    }

    public static bool IsFullScreenPanelShowing() => 
        (fullScreenPanelCount > 0);

    public virtual void Pause(bool isPaused)
    {
        this.Paused = isPaused;
    }

    public virtual float PlayAnimation(string animationName)
    {
        Animator component = base.GetComponent<Animator>();
        if (component != null)
        {
            component.SetTrigger(animationName);
        }
        return 0f;
    }

    public virtual float PlayAnimation(string animationName, bool animationState)
    {
        Animator component = base.GetComponent<Animator>();
        if (component != null)
        {
            component.SetBool(animationName, animationState);
        }
        return 0f;
    }

    public virtual void Rebind()
    {
    }

    public virtual void Reset()
    {
    }

    protected Vector2 ScreenToWorldPoint(Vector2 screenPoint)
    {
        Vector3 vector = UI.Camera.ScreenToWorldPoint((Vector3) screenPoint);
        return new Vector2(vector.x, vector.y);
    }

    public override void Show(bool isVisible)
    {
        if (this.Fullscreen)
        {
            if (isVisible && !this.Visible)
            {
                fullScreenPanelCount++;
                UI.CameraManager.Dimensions = 2;
            }
            if ((!isVisible && this.Visible) && (fullScreenPanelCount > 0))
            {
                fullScreenPanelCount--;
                if (fullScreenPanelCount == 0)
                {
                    UI.CameraManager.Dimensions = 3;
                }
            }
            this.InterceptBackgroundTouches(isVisible);
            if (isVisible)
            {
                Tutorial.Hide();
            }
        }
        base.Show(isVisible);
        if (isVisible)
        {
            this.Rebind();
        }
    }

    protected void ShowWindowButtons(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (!isVisible)
            {
                this.isProceedVisible = window.proceedButton.Visible;
                window.ShowProceedButton(false);
                this.isCancelVisible = window.cancelButton.Visible;
                window.ShowCancelButton(false);
            }
            else
            {
                if (this.isProceedVisible)
                {
                    window.ShowProceedButton(true);
                }
                this.isProceedVisible = false;
                if (this.isCancelVisible)
                {
                    window.ShowCancelButton(true);
                }
                this.isCancelVisible = false;
            }
        }
    }

    public static void Unload()
    {
        fullScreenPanelCount = 0;
        UI.CameraManager.Dimensions = 3;
    }

    [DebuggerHidden]
    protected IEnumerator WaitForTime(float time) => 
        new <WaitForTime>c__Iterator1A { 
            time = time,
            <$>time = time,
            <>f__this = this
        };

    public virtual bool Fullscreen =>
        false;

    public bool Locked { get; set; }

    public GuiPanel Owner { get; set; }

    public bool Paused
    {
        get => 
            this.isPaused;
        private set
        {
            this.isPaused = value;
        }
    }

    public virtual uint zIndex
    {
        get
        {
            if (this.Fullscreen)
            {
                return Constants.ZINDEX_PANEL_FULL;
            }
            return Constants.ZINDEX_PANEL_BASE;
        }
    }

    [CompilerGenerated]
    private sealed class <WaitForTime>c__Iterator1A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>time;
        internal GuiPanel <>f__this;
        internal float time;

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
                case 1:
                    if (this.time > 0f)
                    {
                        if (this.<>f__this != null)
                        {
                            if (this.<>f__this.Visible)
                            {
                                this.time -= Time.deltaTime;
                            }
                            this.$current = null;
                            this.$PC = 1;
                            return true;
                        }
                        break;
                    }
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

