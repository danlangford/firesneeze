using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelBlessings : GuiPanel
{
    [Tooltip("sound played when the blessing deck advances")]
    public AudioClip blessingAdvanceSound;
    [Tooltip("reference to the blessings image animator in our hierarchy")]
    public Animator blessingsAnimator;
    [Tooltip("reference to the blessings button in our hierarchy")]
    public GuiButtonRegion blessingsButton;
    [Tooltip("reference to the blessings deck counter in our hierarchy")]
    public GuiLabel blessingsCounter;
    [Tooltip("reference to the current blessings image in our hierarchy")]
    public GuiImage blessingsCurrentImage;
    [Tooltip("reference to the next blessings image in our hierarchy")]
    public GuiImage blessingsNextImage;
    [Tooltip("length, in seconds, of the drop animation")]
    public float dropAnimationDuration = 0.5f;
    private bool isAnimationInProgress;

    public void Burn()
    {
        if (Scenario.Current.Blessings.Count > 0)
        {
            Scenario.Current.Discard.Add(Scenario.Current.Blessings[0]);
            Scenario.Current.Discard.Move(Scenario.Current.Discard.Count - 1, 0);
            this.blessingsCurrentImage.Image = Scenario.Current.Discard[0].Icon;
            this.blessingsAnimator.SetTrigger("ChangeFast");
            this.blessingsCounter.Text = Scenario.Current.Blessings.Count.ToString();
            this.PlayAnimation("DecrementFast");
        }
    }

    public void BurnStart()
    {
        this.PlayAnimation("DecrementFast");
    }

    public void Decrement()
    {
        this.PlayAnimation("Decrement");
    }

    public void Decrement(int count)
    {
        this.Decrement();
        this.blessingsCounter.Text = count.ToString();
    }

    public float End()
    {
        this.blessingsAnimator.SetTrigger("TimeUp");
        return 3.5f;
    }

    public void Increment()
    {
        this.PlayAnimation("Increment");
    }

    public void Increment(int count)
    {
        this.Increment();
        this.blessingsCounter.Text = count.ToString();
    }

    public void Next()
    {
        if (Scenario.Current.Blessings.Count > 0)
        {
            Scenario.Current.Discard.Add(Scenario.Current.Blessings[0]);
            Scenario.Current.Discard.Move(Scenario.Current.Discard.Count - 1, 0);
            this.blessingsNextImage.Image = Scenario.Current.Discard[0].Icon;
            this.blessingsAnimator.SetTrigger("Change");
            Game.Instance.StartCoroutine(this.SetBlessingImage(this.dropAnimationDuration, Scenario.Current.Discard[0].Icon));
            this.Decrement();
            this.blessingsCounter.Text = Scenario.Current.Blessings.Count.ToString();
        }
    }

    private void OnBlessingsButtonPushed()
    {
        if (!UI.Window.Paused)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowBlessingCard(true);
            }
        }
    }

    public override void Refresh()
    {
        if (!this.isAnimationInProgress)
        {
            this.blessingsCounter.Text = Scenario.Current.Blessings.Count.ToString();
            if (Scenario.Current.Discard.Count > 0)
            {
                this.blessingsCurrentImage.Image = Scenario.Current.Discard[0].Icon;
            }
            else
            {
                this.blessingsCurrentImage.Image = null;
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator SetBlessingImage(float delay, Sprite image) => 
        new <SetBlessingImage>c__Iterator51 { 
            delay = delay,
            image = image,
            <$>delay = delay,
            <$>image = image,
            <>f__this = this
        };

    public void Shuffle()
    {
        Scenario.Current.Blessings.Shuffle();
        if (Scenario.Current.Discard.Count > 0)
        {
            this.blessingsCurrentImage.Image = Scenario.Current.Discard[0].Icon;
        }
        else
        {
            this.blessingsCurrentImage.Image = null;
        }
    }

    [CompilerGenerated]
    private sealed class <SetBlessingImage>c__Iterator51 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>delay;
        internal Sprite <$>image;
        internal GuiPanelBlessings <>f__this;
        internal float delay;
        internal Sprite image;

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
                    this.<>f__this.isAnimationInProgress = true;
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.delay));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.blessingsCurrentImage.Image = this.image;
                    UI.Sound.Play(this.<>f__this.blessingAdvanceSound);
                    this.<>f__this.isAnimationInProgress = false;
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

