using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RewardCardTotalAcquired : Reward
{
    [Tooltip("the list of Card IDs to give to the player's collection")]
    public List<string> Rewards;
    [Tooltip("scale of reward cards that will be shown")]
    public float Scale = 0.35f;

    public override void Deliver()
    {
        if (this.Rewards.Count > 0)
        {
            Campaign.Distributions.AddRange(this.Rewards);
            this.Rewards.Clear();
        }
        base.myWindow.OnRewardChosen((string) null);
    }

    private Vector3 GetCardScale(Card card, int total) => 
        new Vector3(this.Scale, this.Scale, 1f);

    public override int GetNumRewards(Character character) => 
        this.Rewards.Count;

    protected override string GetRewardPanelName() => 
        "Reward_Prefab_Card";

    public override bool HasReward(int n) => 
        (this.Rewards.Count == 0);

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            base.StartCoroutine(this.ShowRewardSequence());
        }
        base.isPanelShowing = isVisible;
    }

    [DebuggerHidden]
    private IEnumerator ShowRewardSequence() => 
        new <ShowRewardSequence>c__Iterator9D { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator ShowSingleCard(Card card) => 
        new <ShowSingleCard>c__Iterator9E { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    [CompilerGenerated]
    private sealed class <ShowRewardSequence>c__Iterator9D : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal RewardCardTotalAcquired <>f__this;
        internal Card <card>__2;
        internal List<Card> <cards>__0;
        internal int <i>__1;
        internal int <i>__5;
        internal float <left>__4;
        internal float <offset>__3;

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
                    if (this.<>f__this.Rewards.Count <= 0)
                    {
                        break;
                    }
                    this.<cards>__0 = new List<Card>();
                    this.<i>__1 = 0;
                    while (this.<i>__1 < this.<>f__this.Rewards.Count)
                    {
                        this.<card>__2 = CardTable.Create(this.<>f__this.Rewards[this.<i>__1]);
                        this.<card>__2.Show(CardSideType.Back);
                        this.<card>__2.Show(false);
                        this.<card>__2.transform.localScale = this.<>f__this.GetCardScale(this.<card>__2, this.<>f__this.Rewards.Count);
                        this.<card>__2.SortingOrder = this.<i>__1;
                        this.<cards>__0.Add(this.<card>__2);
                        this.<i>__1++;
                    }
                    this.<offset>__3 = 1f;
                    this.<left>__4 = (((float) this.<>f__this.Rewards.Count) / 2f) * -this.<offset>__3;
                    if (this.<cards>__0.Count > 0)
                    {
                        this.<cards>__0[0].transform.localScale = this.<>f__this.GetCardScale(this.<cards>__0[0], this.<>f__this.Rewards.Count);
                        this.<offset>__3 = 0.5f * this.<cards>__0[0].Size.x;
                        this.<left>__4 = (((float) this.<>f__this.Rewards.Count) / 2f) * -this.<offset>__3;
                    }
                    this.<i>__5 = 0;
                    while (this.<i>__5 < this.<cards>__0.Count)
                    {
                        this.<cards>__0[this.<i>__5].transform.position = new Vector3(this.<left>__4 + (this.<i>__5 * this.<offset>__3), 0f, 0f);
                        this.<>f__this.StartCoroutine(this.<>f__this.ShowSingleCard(this.<cards>__0[this.<i>__5]));
                        this.$current = new WaitForSeconds(0.66f);
                        this.$PC = 1;
                        goto Label_02B2;
                    Label_025E:
                        this.<i>__5++;
                    }
                    this.$current = new WaitForSeconds(2.25f);
                    this.$PC = 2;
                    goto Label_02B2;

                case 1:
                    goto Label_025E;

                case 2:
                    break;

                default:
                    goto Label_02B0;
            }
            this.<>f__this.Deliver();
            this.$PC = -1;
        Label_02B0:
            return false;
        Label_02B2:
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
    private sealed class <ShowSingleCard>c__Iterator9E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal RewardCardTotalAcquired <>f__this;
        internal Vector3 <scale>__0;
        internal Card card;

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
                    this.<scale>__0 = this.<>f__this.GetCardScale(this.card, this.<>f__this.Rewards.Count);
                    this.card.transform.localScale = Vector3.zero;
                    this.card.Show(true);
                    LeanTween.scale(this.card.gameObject, this.<scale>__0, 0.75f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = new WaitForSeconds(0.8f);
                    this.$PC = 1;
                    goto Label_00FF;

                case 1:
                    this.card.Show(CardSideType.Front);
                    this.card.Animate(AnimationType.FlipToFront, true);
                    this.$current = new WaitForSeconds(1.45f);
                    this.$PC = 2;
                    goto Label_00FF;

                case 2:
                    this.<>f__this.myWindow.OnCommunismChosen(this.card);
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_00FF:
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

