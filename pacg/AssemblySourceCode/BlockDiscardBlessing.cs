using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BlockDiscardBlessing : Block
{
    [Tooltip("the amount of blessings to discard from the blessing deck")]
    public int DiscardAmount = 1;

    [DebuggerHidden]
    private IEnumerator AdvanceBlessing(int blessings) => 
        new <AdvanceBlessing>c__IteratorB { 
            blessings = blessings,
            <$>blessings = blessings
        };

    public override void Invoke()
    {
        base.StartCoroutine(this.AdvanceBlessing(this.DiscardAmount));
    }

    [CompilerGenerated]
    private sealed class <AdvanceBlessing>c__IteratorB : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>blessings;
        internal int <i>__1;
        internal GuiWindowLocation <window>__0;
        internal int blessings;

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
                    if (this.<window>__0 != null)
                    {
                        if (Scenario.Current.Blessings.Count <= 0)
                        {
                            Turn.State = GameStateType.End;
                        }
                        this.<window>__0.blessingsPanel.Next();
                        this.<i>__1 = 1;
                        while (this.<i>__1 < this.blessings)
                        {
                            this.$current = new WaitForSeconds(this.<window>__0.blessingsPanel.dropAnimationDuration + 1.25f);
                            this.$PC = 1;
                            return true;
                        Label_00A7:
                            if (Scenario.Current.Blessings.Count <= 0)
                            {
                                Turn.State = GameStateType.End;
                            }
                            this.<window>__0.blessingsPanel.Next();
                            this.<i>__1++;
                        }
                    }
                    this.$PC = -1;
                    break;

                case 1:
                    goto Label_00A7;
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

