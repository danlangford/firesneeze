using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelSwitch : GuiPanel
{
    [Tooltip("reference to this panel's continue button")]
    public GuiButton ContinueButton;
    [Tooltip("reference to this panel's character name label")]
    public GuiLabel DisplayName;
    [Tooltip("reference to this panel's menu button")]
    public GuiButton MenuButton;
    [Tooltip("reference to this panel's character portrait")]
    public SpriteRenderer Portrait;

    private void ContinueButtonPushed()
    {
        this.Show(false);
        this.ShowCards(true);
        this.ProcessDelayedLoads();
        UI.Window.Refresh();
        this.ProcessDelayedRecharges();
        if (UI.Window.Type == WindowType.Reward)
        {
            UI.Window.SendMessage("OnCharacterSwitchDone");
        }
    }

    private void Display(bool isVisible)
    {
        base.Show(isVisible);
        SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].color = new Color(componentsInChildren[i].color.r, componentsInChildren[i].color.g, componentsInChildren[i].color.b, 1f);
        }
        this.DisplayName.Color = new Color(this.DisplayName.Color.r, this.DisplayName.Color.g, this.DisplayName.Color.b, 1f);
        Switching = isVisible;
    }

    private void Fade(bool isVisible, float time)
    {
        base.Show(true);
        SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>();
        if (isVisible)
        {
            Switching = true;
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].color = new Color(componentsInChildren[i].color.r, componentsInChildren[i].color.g, componentsInChildren[i].color.b, 0f);
                LeanTween.alpha(componentsInChildren[i].gameObject, 1f, time).setEase(LeanTweenType.easeInQuad);
            }
        }
        else
        {
            for (int j = 0; j < componentsInChildren.Length; j++)
            {
                LeanTween.alpha(componentsInChildren[j].gameObject, 0f, time).setEase(LeanTweenType.easeInQuad);
            }
            LeanTween.delayedCall(time, delegate {
                base.Show(false);
                Switching = false;
            });
        }
        this.DisplayName.Fade(isVisible, time);
    }

    public override void Initialize()
    {
        Animations = true;
    }

    private void LoadLocation(string ID)
    {
        if (UI.Window.Type == WindowType.Location)
        {
            Location.Load(ID);
        }
    }

    private void OnMenuButtonPushed()
    {
    }

    private void OnSwitchContinueButtonPushed()
    {
        this.ContinueButton.Locked = true;
        LeanTween.scale(this.ContinueButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.ContinueButtonPushed));
    }

    private void ProcessDelayedLoads()
    {
        if (((Game.GameType == GameType.LocalMultiPlayer) && (UI.Window.Type == WindowType.Location)) && (Turn.Owner.Location != Location.Current.ID))
        {
            this.LoadLocation(Turn.Owner.Location);
        }
    }

    private void ProcessDelayedRecharges()
    {
        if (((Game.GameType == GameType.LocalMultiPlayer) && (Turn.State != GameStateType.Recharge)) && (Turn.Owner.Recharge.Count > 0))
        {
            Turn.PushStateDestination(Turn.State);
            Turn.State = GameStateType.Recharge;
        }
    }

    public override void Rebind()
    {
        this.ContinueButton.Rebind();
        this.MenuButton.Rebind();
    }

    public override void Show(bool isVisible)
    {
        Game.Instance.StartCoroutine(this.ShowCoroutine(isVisible));
    }

    private void ShowCards(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutHand.Show(isVisible);
        }
    }

    [DebuggerHidden]
    private IEnumerator ShowCoroutine(bool isVisible) => 
        new <ShowCoroutine>c__Iterator76 { 
            isVisible = isVisible,
            <$>isVisible = isVisible,
            <>f__this = this
        };

    public static bool Animations
    {
        [CompilerGenerated]
        get => 
            <Animations>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Animations>k__BackingField = value;
        }
    }

    public override bool Fullscreen =>
        true;

    public static bool Switching
    {
        [CompilerGenerated]
        get => 
            <Switching>k__BackingField;
        [CompilerGenerated]
        private set
        {
            <Switching>k__BackingField = value;
        }
    }

    [CompilerGenerated]
    private sealed class <ShowCoroutine>c__Iterator76 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal bool <$>isVisible;
        internal GuiPanelSwitch <>f__this;
        internal GuiWindowLocation <window>__0;
        internal bool isVisible;

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
                    Turn.Switch = Turn.Number;
                    this.<>f__this.Portrait.sprite = Turn.Character.PortraitLarge;
                    this.<>f__this.DisplayName.Text = Turn.Character.DisplayName;
                    if (!GuiPanelSwitch.Animations)
                    {
                        this.<>f__this.Display(this.isVisible);
                        break;
                    }
                    this.<>f__this.Fade(this.isVisible, 0.15f);
                    this.$current = new WaitForSeconds(0.2f);
                    this.$PC = 1;
                    return true;

                case 1:
                    break;

                default:
                    goto Label_017F;
            }
            if (this.isVisible)
            {
                this.<>f__this.LoadLocation(Turn.Character.Location);
            }
            if (this.isVisible)
            {
                this.<>f__this.ContinueButton.Locked = false;
                this.<>f__this.Rebind();
                Tutorial.Notify(TutorialEventType.ScreenSwitchShown);
            }
            else
            {
                this.<window>__0 = UI.Window as GuiWindowLocation;
                if ((this.<window>__0 != null) && ((Turn.SwitchType == SwitchType.AidAll) || (Turn.SwitchType == SwitchType.Aid)))
                {
                    this.<window>__0.ShowProceedAidButton(true);
                }
                Tutorial.Notify(TutorialEventType.ScreenWasClosed);
            }
            this.<>f__this.ShowCards(!this.isVisible);
            UI.Window.Pause(this.isVisible);
            this.$PC = -1;
        Label_017F:
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

