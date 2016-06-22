using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PrizeTreasureCard : Prize
{
    private Card myCard;
    [Tooltip("ID of the prize card awarded to the player")]
    public string PrizeCard;
    private TKTapRecognizer tapRecognizer;

    private void Continue()
    {
        if (this.tapRecognizer != null)
        {
            this.tapRecognizer.enabled = false;
            TouchKit.removeGestureRecognizer(this.tapRecognizer);
        }
        if (this.myCard != null)
        {
            LeanTween.cancel(this.myCard.gameObject);
            this.myCard.Show(false);
            this.myCard.Destroy();
        }
        UI.Window.SendMessage("RewardSequenceController");
    }

    public override void Deliver()
    {
        base.StartCoroutine(this.Deliver_Coroutine());
    }

    [DebuggerHidden]
    private IEnumerator Deliver_Coroutine() => 
        new <Deliver_Coroutine>c__Iterator9A { <>f__this = this };

    public override bool HasPrize() => 
        Collection.Contains(this.PrizeCard);

    public override bool IsPrizeAllowed()
    {
        if (Adventure.Current == null)
        {
            return false;
        }
        if ((!Game.Network.Connected || Game.Network.OutOfDate) || !Game.Network.HasNetworkConnection)
        {
            return false;
        }
        if (this.HasPrize())
        {
            return false;
        }
        for (int i = 0; i < Adventure.Current.Scenarios.Length; i++)
        {
            string iD = Adventure.Current.Scenarios[i];
            if (Mathf.Max(Campaign.GetScenarioDifficulty(iD), Conquests.GetComplete(iD)) < 2)
            {
                return false;
            }
        }
        return true;
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (Physics2D.Raycast(UI.Camera.ScreenToWorldPoint((Vector3) touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD).collider != null)
        {
            this.Continue();
        }
    }

    [CompilerGenerated]
    private sealed class <Deliver_Coroutine>c__Iterator9A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal PrizeTreasureCard <>f__this;
        internal Animator <anim>__2;
        internal GameObject <myRoot>__1;
        internal bool <success>__0;

        internal void <>m__166(TKTapRecognizer r)
        {
            this.<>f__this.OnGuiTap(this.<>f__this.tapRecognizer.touchLocation());
        }

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
                    this.<success>__0 = false;
                    if (!Collection.Add(this.<>f__this.PrizeCard))
                    {
                        break;
                    }
                    this.<>f__this.myCard = CardTable.Create(this.<>f__this.PrizeCard);
                    if (this.<>f__this.myCard == null)
                    {
                        break;
                    }
                    this.<>f__this.myCard.transform.position = Vector3.zero;
                    this.<>f__this.myCard.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
                    this.<>f__this.myCard.Show(false);
                    this.<myRoot>__1 = this.<>f__this.GetRewardPanel("Booster Item Center from Box");
                    if (this.<myRoot>__1 == null)
                    {
                        break;
                    }
                    this.<myRoot>__1.SetActive(true);
                    this.<anim>__2 = this.<myRoot>__1.GetComponent<Animator>();
                    if (this.<anim>__2 == null)
                    {
                        break;
                    }
                    this.<anim>__2.SetTrigger("Start");
                    this.$current = new WaitForSeconds(1.1f);
                    this.$PC = 1;
                    goto Label_022A;

                case 1:
                    this.<anim>__2.SetTrigger("Open");
                    UI.Sound.Play(SoundEffectType.RewardCardClickFlip);
                    this.$current = new WaitForSeconds(0.2f);
                    this.$PC = 2;
                    goto Label_022A;

                case 2:
                    this.<>f__this.myCard.Show(CardSideType.Front);
                    LeanTween.scale(this.<>f__this.myCard.gameObject, Device.GetCardZoomScale(), 0.5f).setEase(LeanTweenType.easeOutBounce);
                    this.<>f__this.tapRecognizer = new TKTapRecognizer();
                    this.<>f__this.tapRecognizer.gestureRecognizedEvent += new Action<TKTapRecognizer>(this.<>m__166);
                    this.<>f__this.tapRecognizer.zIndex = 100;
                    TouchKit.addGestureRecognizer(this.<>f__this.tapRecognizer);
                    this.<success>__0 = true;
                    break;

                default:
                    goto Label_0228;
            }
            if (!this.<success>__0)
            {
                this.<>f__this.Continue();
            }
            this.$PC = -1;
        Label_0228:
            return false;
        Label_022A:
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
}

