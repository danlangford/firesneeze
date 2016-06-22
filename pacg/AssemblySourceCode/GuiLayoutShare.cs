using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiLayoutShare : GuiLayout
{
    [Tooltip("reference to the panel animator")]
    public UnityEngine.Animator Animator;
    private readonly float leftx = 3f;
    private string[] originalDeckOrder;
    private readonly float padx = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        this.Deck = base.FindDeckChild("Deck");
        if (this.Deck != null)
        {
            this.Deck.Layout = this;
        }
    }

    [DebuggerHidden]
    private IEnumerator CloseCoroutine() => 
        new <CloseCoroutine>c__Iterator4E { <>f__this = this };

    private int GetCardSortingOrder(Card card, int index) => 
        (10 + index);

    private float GetCardWidth(Card card)
    {
        Vector3 localScale = card.transform.localScale;
        card.transform.localScale = this.Scale;
        float x = card.Size.x;
        card.transform.localScale = localScale;
        return x;
    }

    private Vector3 GetPosition(int i, int count)
    {
        if (i < this.Deck.Count)
        {
            Card card = this.Deck[i];
            float cardWidth = this.GetCardWidth(card);
            Vector3 vector = base.transform.position - new Vector3(this.leftx, 0f, 0f);
            float x = (count >= 6) ? ((cardWidth * 5f) / ((float) count)) : (cardWidth + this.padx);
            return (vector + ((Vector3) (i * new Vector3(x, 0f, 0f))));
        }
        return Vector3.zero;
    }

    public override bool IsDropPossible(Card card)
    {
        if (!base.IsDropPossible(card))
        {
            return false;
        }
        if (card.Displayed)
        {
            return false;
        }
        if (!GameStateShare.IsCardShareable(card))
        {
            return false;
        }
        return (card.Deck == this.Hand);
    }

    public override bool OnGuiDrop(Card card)
    {
        if (!this.IsDropPossible(card))
        {
            return false;
        }
        if ((card.Deck != null) && (card.Deck.Layout != null))
        {
            card.Deck.Layout.OnGuiHover(false);
        }
        this.Deck.Add(card);
        GameStateShare.Share(card, true);
        card.Show(true);
        this.Refresh();
        this.Hand.Layout.Refresh();
        return true;
    }

    [DebuggerHidden]
    private IEnumerator OpenCoroutine() => 
        new <OpenCoroutine>c__Iterator4D { <>f__this = this };

    public override void Refresh()
    {
        if (this.Deck != null)
        {
            int num = 0;
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card = this.Deck[i];
                card.SortingOrder = this.GetCardSortingOrder(card, i);
                LeanTween.cancel(card.gameObject);
                LeanTween.scale(card.gameObject, this.Scale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                card.MoveCard(this.GetPosition(num++, this.Deck.Count), 0.25f).setEase(LeanTweenType.easeInOutQuad);
            }
        }
    }

    private void SetupDeck(bool isVisible)
    {
        if ((this.Deck != null) && (this.Hand != null))
        {
            if (isVisible)
            {
                this.originalDeckOrder = this.Hand.GetCardList();
                for (int i = this.Hand.Count - 1; i >= 0; i--)
                {
                    Card card = this.Hand[i];
                    if (GameStateShare.IsCardSharedAsCard(card))
                    {
                        this.Deck.Add(card);
                        card.Show(true);
                    }
                    else if (GameStateShare.IsCardSharedAsFuel(card))
                    {
                        this.Deck.Add(card);
                        GameStateShare.AddDecoration(card, card.SharedPower);
                        card.Show(true);
                    }
                }
            }
            else
            {
                for (int j = 0; j < this.Deck.Count; j++)
                {
                    this.Deck[j].Decorations.Clear();
                }
                this.Hand.Combine(this.Deck);
                this.Hand.Sort(this.originalDeckOrder);
                this.originalDeckOrder = null;
            }
        }
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            base.gameObject.SetActive(true);
            Game.Instance.StartCoroutine(this.OpenCoroutine());
        }
        else if (base.Visible)
        {
            Game.Instance.StartCoroutine(this.CloseCoroutine());
        }
    }

    public override Deck Deck { get; set; }

    public Deck Hand { get; set; }

    [CompilerGenerated]
    private sealed class <CloseCoroutine>c__Iterator4E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiLayoutShare <>f__this;

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
                    this.<>f__this.Animator.SetTrigger("Hide");
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 1;
                    goto Label_00B9;

                case 1:
                    this.<>f__this.Show(false);
                    this.<>f__this.SetupDeck(false);
                    this.<>f__this.Hand.Layout.Refresh();
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 2;
                    goto Label_00B9;

                case 2:
                    this.<>f__this.Hand = null;
                    Turn.Proceed();
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_00B9:
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
    private sealed class <OpenCoroutine>c__Iterator4D : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiLayoutShare <>f__this;

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
                    this.<>f__this.Show(true);
                    this.<>f__this.Animator.SetTrigger("Show");
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.SetupDeck(true);
                    this.<>f__this.Refresh();
                    this.<>f__this.Hand.Layout.Refresh();
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

