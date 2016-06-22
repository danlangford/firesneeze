using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventUndefeatedBlock : Event
{
    [Tooltip("when this card is undefeated invoke the block")]
    public CardSelector CardToUndefeat;
    [Tooltip("block to invoke if the selector matches our card")]
    public Block UndefeatedBlock;

    [DebuggerHidden]
    private IEnumerator InvokeBlock(Card card) => 
        new <InvokeBlock>c__Iterator27 { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    public override bool IsEventValid(Card card)
    {
        if ((this.CardToUndefeat != null) && !this.CardToUndefeat.Match(card))
        {
            return false;
        }
        return base.IsConditionValid(card);
    }

    public override void OnCardUndefeated(Card card)
    {
        base.StartCoroutine(this.InvokeBlock(card));
    }

    public override EventType Type =>
        EventType.OnCardUndefeated;

    [CompilerGenerated]
    private sealed class <InvokeBlock>c__Iterator27 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal EventUndefeatedBlock <>f__this;
        internal float <waitTime>__0;
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
                    if (!this.<>f__this.IsEventValid(this.card))
                    {
                        break;
                    }
                    this.<waitTime>__0 = this.<>f__this.UndefeatedBlock.Length;
                    this.<>f__this.UndefeatedBlock.Invoke();
                    this.$current = new WaitForSeconds(this.<waitTime>__0);
                    this.$PC = 1;
                    return true;

                case 1:
                    break;

                default:
                    goto Label_0086;
            }
            Event.Done();
            this.$PC = -1;
        Label_0086:
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

