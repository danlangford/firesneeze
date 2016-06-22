using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BlockExamineBanishCards : Block
{
    [Tooltip("amount of cards to banish from the top")]
    public int Amount;
    private readonly Vector3 BoxPosition = new Vector3(0f, 12f, 0f);
    [Tooltip("which types to banish and which types to leave alone")]
    public CardSelector Selector;

    [DebuggerHidden]
    private IEnumerator BanishCardsAnimation() => 
        new <BanishCardsAnimation>c__IteratorC { <>f__this = this };

    public override void Invoke()
    {
        Game.Instance.StartCoroutine(this.BanishCardsAnimation());
    }

    public override float Length =>
        (this.Amount * 0.3f);

    [CompilerGenerated]
    private sealed class <BanishCardsAnimation>c__IteratorC : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BlockExamineBanishCards <>f__this;
        internal int <amountToBanish>__1;
        internal Card <card>__3;
        internal int <i>__2;
        internal GuiWindowLocation <window>__0;

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
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    this.<amountToBanish>__1 = Mathf.Min(this.<>f__this.Amount, Location.Current.Deck.Count);
                    if (this.<window>__0 == null)
                    {
                        goto Label_0182;
                    }
                    this.<i>__2 = this.<amountToBanish>__1 - 1;
                    goto Label_0166;

                case 1:
                    Campaign.Box.Add(this.<card>__3, false);
                    this.<window>__0.locationPanel.RefreshCardList();
                    break;

                default:
                    goto Label_0189;
            }
        Label_0158:
            this.<i>__2--;
        Label_0166:
            if (this.<i>__2 >= 0)
            {
                if (this.<>f__this.Selector.Match(Location.Current.Deck[this.<i>__2]))
                {
                    this.<card>__3 = Location.Current.Deck[this.<i>__2];
                    this.<card>__3.Deck.Remove(this.<card>__3);
                    this.<card>__3.Show(true);
                    this.<window>__0.layoutExamine.Top--;
                    this.<card>__3.MoveCard(this.<>f__this.BoxPosition, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 1;
                    return true;
                }
                goto Label_0158;
            }
            this.<window>__0.layoutExamine.Refresh();
        Label_0182:
            this.$PC = -1;
        Label_0189:
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

