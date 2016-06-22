using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiScriptBoostLocation : GuiScript
{
    [Tooltip("called when the script is done")]
    public TurnStateCallback Callback;
    [Tooltip("the cards to be added")]
    public Card[] Cards;

    private void OnTrayCloseFinished()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Clear();
            window.locationPanel.RefreshCardList();
        }
        if (this.Callback != null)
        {
            this.Callback.Invoke();
        }
    }

    public override bool Play()
    {
        Game.Instance.StartCoroutine(this.PlayCoroutine());
        return true;
    }

    [DebuggerHidden]
    private IEnumerator PlayCoroutine() => 
        new <PlayCoroutine>c__Iterator7D { <>f__this = this };

    public override void Stop()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            LeanTween.delayedCall(window.layoutExamine.Close(), new Action(this.OnTrayCloseFinished));
        }
    }

    [CompilerGenerated]
    private sealed class <PlayCoroutine>c__Iterator7D : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiScriptBoostLocation <>f__this;
        internal int <i>__0;

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
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
                    this.$PC = 1;
                    goto Label_0162;

                case 1:
                    if (this.<>f__this.Cards == null)
                    {
                        goto Label_0122;
                    }
                    this.<i>__0 = 0;
                    goto Label_010A;

                case 2:
                    break;

                case 3:
                    this.<>f__this.Stop();
                    this.$PC = -1;
                    goto Label_0160;

                default:
                    goto Label_0160;
            }
        Label_00FC:
            this.<i>__0++;
        Label_010A:
            if (this.<i>__0 < this.<>f__this.Cards.Length)
            {
                if (this.<>f__this.Cards[this.<i>__0] != null)
                {
                    Location.Current.Deck.Add(this.<>f__this.Cards[this.<i>__0], DeckPositionType.Top);
                    UI.Sound.Play(SoundEffectType.GenericFlickCard);
                    (UI.Window as GuiWindowLocation).layoutExamine.Refresh();
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 2;
                    goto Label_0162;
                }
                goto Label_00FC;
            }
        Label_0122:
            this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
            this.$PC = 3;
            goto Label_0162;
        Label_0160:
            return false;
        Label_0162:
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

