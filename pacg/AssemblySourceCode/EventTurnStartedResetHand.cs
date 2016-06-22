using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class EventTurnStartedResetHand : Event
{
    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        return true;
    }

    public override void OnTurnStarted()
    {
        if (this.IsEventValid(Turn.Card))
        {
            Turn.State = GameStateType.None;
            base.StartCoroutine(this.ResetHandCoroutine());
        }
        Event.Done();
    }

    [DebuggerHidden]
    private IEnumerator ResetHandCoroutine() => 
        new <ResetHandCoroutine>c__Iterator26 { <>f__this = this };

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnTurnStarted;

    [CompilerGenerated]
    private sealed class <ResetHandCoroutine>c__Iterator26 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal EventTurnStartedResetHand <>f__this;
        internal int <i>__1;
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
                    if (this.<window>__0 == null)
                    {
                        goto Label_018D;
                    }
                    this.<window>__0.Pause(true);
                    this.<i>__1 = Turn.Character.Hand.Count - 1;
                    break;

                case 1:
                    this.<i>__1--;
                    break;

                case 2:
                    this.<i>__2 = 0;
                    goto Label_0165;

                case 3:
                    this.<i>__2++;
                    goto Label_0165;

                default:
                    goto Label_0194;
            }
            if (this.<i>__1 >= 0)
            {
                this.<window>__0.Recharge(Turn.Character.Hand[this.<i>__1]);
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                this.$PC = 1;
            }
            else
            {
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
                this.$PC = 2;
            }
            goto Label_0196;
        Label_0165:
            if (this.<i>__2 < Turn.Character.HandSize)
            {
                this.<window>__0.Draw(Turn.Character.Deck[0]);
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                this.$PC = 3;
                goto Label_0196;
            }
            this.<window>__0.Pause(false);
            Turn.State = GameStateType.StartTurn;
        Label_018D:
            this.$PC = -1;
        Label_0194:
            return false;
        Label_0196:
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

