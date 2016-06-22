using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelGoldBanner : GuiPanel
{
    [Tooltip("shows the amount of gold received")]
    public GuiLabel GoldAmount;
    [Tooltip("reference to this panel's animator")]
    public Animator GoldAnimator;
    private int goldGained;
    private int goldOwned;
    [Tooltip("shows the total amount of gold owned by the player")]
    public GuiLabel GoldTotal;
    private Queue<int> queue = new Queue<int>(3);

    private void AnimateGoldDelivery()
    {
        if (this.queue.Count > 0)
        {
            this.goldGained = this.queue.Dequeue();
            this.goldOwned = Game.Network.CurrentUser.Gold;
            this.GoldAmount.Text = this.goldGained.ToString();
            this.GoldTotal.Text = this.goldOwned.ToString();
            this.Show(true);
            this.GoldAnimator.SetTrigger("AddGold");
            UI.Sound.Play(SoundEffectType.MinorGold);
        }
    }

    private void OnAnimationGoldDeliveredEnd()
    {
        this.goldGained = 0;
        this.goldOwned = 0;
        if (this.queue.Count > 0)
        {
            this.AnimateGoldDelivery();
        }
    }

    private void OnAnimationGoldDeliveredStart()
    {
        base.StartCoroutine(this.OnAnimationGoldDeliveredStart_Coroutine());
    }

    [DebuggerHidden]
    private IEnumerator OnAnimationGoldDeliveredStart_Coroutine() => 
        new <OnAnimationGoldDeliveredStart_Coroutine>c__Iterator5D { <>f__this = this };

    public void Show(int gp)
    {
        if (gp > 0)
        {
            this.queue.Enqueue(gp);
            if (this.queue.Count == 1)
            {
                this.AnimateGoldDelivery();
            }
        }
    }

    [CompilerGenerated]
    private sealed class <OnAnimationGoldDeliveredStart_Coroutine>c__Iterator5D : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelGoldBanner <>f__this;
        internal int <i>__3;
        internal int <n>__2;
        internal int <step>__1;
        internal int <total>__0;

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
                    this.<total>__0 = this.<>f__this.goldOwned + this.<>f__this.goldGained;
                    this.<step>__1 = Mathf.CeilToInt(((float) this.<>f__this.goldGained) / 4f);
                    this.<n>__2 = this.<>f__this.goldOwned;
                    this.<i>__3 = 0;
                    break;

                case 1:
                    this.<i>__3++;
                    break;

                case 2:
                    this.<>f__this.OnAnimationGoldDeliveredEnd();
                    this.$PC = -1;
                    goto Label_0134;

                default:
                    goto Label_0134;
            }
            if (this.<i>__3 < 4)
            {
                this.<n>__2 = Mathf.Min(this.<n>__2 + this.<step>__1, this.<total>__0);
                this.<>f__this.GoldTotal.Text = this.<n>__2.ToString();
                this.$current = new WaitForSeconds(0.1f);
                this.$PC = 1;
            }
            else
            {
                this.<>f__this.GoldTotal.Text = this.<total>__0.ToString();
                this.$current = new WaitForSeconds(3f);
                this.$PC = 2;
            }
            return true;
        Label_0134:
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

