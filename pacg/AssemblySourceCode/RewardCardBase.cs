using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class RewardCardBase : Reward
{
    private int cardAnimationState;
    protected Card myCard;
    protected bool[] selectedCard;
    protected TKTapRecognizer tapRecognizer;

    protected RewardCardBase()
    {
    }

    [DebuggerHidden]
    private IEnumerator AnimateCardDelivery() => 
        new <AnimateCardDelivery>c__Iterator9C { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator AnimateCardReveal() => 
        new <AnimateCardReveal>c__Iterator9B { <>f__this = this };

    public override void Deliver()
    {
        this.tapRecognizer.enabled = false;
        base.myWindow.OnRewardChosen(this.myCard);
        this.myCard = null;
        this.cardAnimationState = 0;
    }

    public override void Display()
    {
        this.tapRecognizer.enabled = true;
        UI.Busy = true;
        base.Locked = false;
    }

    protected override float GetStartTime() => 
        0.5f;

    public override bool HasReward(int n) => 
        this.IsSelected(n);

    public override void Initialize(GuiWindowReward window)
    {
        base.Initialize(window);
        this.selectedCard = new bool[Party.Characters.Count];
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!base.Locked)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        this.tapRecognizer.zIndex = 100;
        this.tapRecognizer.enabled = false;
        TouchKit.addGestureRecognizer(this.tapRecognizer);
    }

    public override bool IsSelected(int n)
    {
        if (this.selectedCard == null)
        {
            return false;
        }
        return this.selectedCard[n];
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (Physics2D.Raycast(UI.Camera.ScreenToWorldPoint((Vector3) touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD).collider != null)
        {
            if (this.cardAnimationState == 0)
            {
                base.StartCoroutine(this.AnimateCardReveal());
            }
            if (this.cardAnimationState == 2)
            {
                base.StartCoroutine(this.AnimateCardDelivery());
            }
        }
    }

    public override void Pause(bool isPaused)
    {
        this.tapRecognizer.enabled = !isPaused;
    }

    public override void Select(Card card)
    {
        this.selectedCard[Turn.Number] = true;
    }

    public override int Priority =>
        1;

    [CompilerGenerated]
    private sealed class <AnimateCardDelivery>c__Iterator9C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal RewardCardBase <>f__this;

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
                    UI.Busy = true;
                    this.<>f__this.Locked = true;
                    this.<>f__this.cardAnimationState = 3;
                    this.<>f__this.PlayPanelAnimation("TakeReward");
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 1;
                    goto Label_00C9;

                case 1:
                    this.<>f__this.Deliver();
                    this.$current = new WaitForSeconds(0.2f);
                    this.$PC = 2;
                    goto Label_00C9;

                case 2:
                    this.<>f__this.Show(false);
                    this.<>f__this.Locked = false;
                    this.<>f__this.cardAnimationState = 0;
                    UI.Busy = false;
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_00C9:
            return true;
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

    [CompilerGenerated]
    private sealed class <AnimateCardReveal>c__Iterator9B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal RewardCardBase <>f__this;

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
                    UI.Busy = true;
                    this.<>f__this.Locked = true;
                    this.<>f__this.cardAnimationState = 1;
                    UI.Sound.Play(SoundEffectType.RewardCardClickFlip);
                    this.<>f__this.PlayPanelAnimation("ShowReward");
                    this.$current = new WaitForSeconds(1.5f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Locked = false;
                    this.<>f__this.cardAnimationState = 2;
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

