using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TutorialCommandLockStartLocation : TutorialCommand
{
    [Tooltip("the starting location for all players")]
    public string ID = "LO1B_Farmhouse";

    public override void Invoke()
    {
        if (!string.IsNullOrEmpty(this.ID))
        {
            base.StartCoroutine(this.MoveTokensAndLock());
        }
        else
        {
            this.LockTokens(true);
        }
    }

    private void LockTokens(bool isLocked)
    {
        GuiWindowScenario window = UI.Window as GuiWindowScenario;
        if (window != null)
        {
            for (int i = 0; i < window.LayoutStartLocation.Tokens.Count; i++)
            {
                window.LayoutStartLocation.Tokens[i].Locked = isLocked;
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator MoveTokensAndLock() => 
        new <MoveTokensAndLock>c__IteratorA9 { <>f__this = this };

    [CompilerGenerated]
    private sealed class <MoveTokensAndLock>c__IteratorA9 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal TutorialCommandLockStartLocation <>f__this;
        internal int <i>__2;
        internal int <i>__3;
        internal CharacterTokenSlotPartyLocation <slot>__1;
        internal GuiWindowScenario <window>__0;

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
                    this.<window>__0 = UI.Window as GuiWindowScenario;
                    this.<>f__this.LockTokens(true);
                    this.$current = new WaitForSeconds(0.35f);
                    this.$PC = 1;
                    goto Label_01CC;

                case 1:
                    this.<slot>__1 = null;
                    if (this.<window>__0 != null)
                    {
                        this.<i>__2 = 0;
                        while (this.<i>__2 < this.<window>__0.MapPanel.Icons.Count)
                        {
                            if (this.<window>__0.MapPanel.Icons[this.<i>__2].ID == this.<>f__this.ID)
                            {
                                this.<slot>__1 = this.<window>__0.MapPanel.Icons[this.<i>__2].GetComponent<CharacterTokenSlotPartyLocation>();
                                break;
                            }
                            this.<i>__2++;
                        }
                    }
                    break;

                case 2:
                    goto Label_0195;

                default:
                    goto Label_01CA;
            }
            if ((this.<window>__0 != null) && (this.<slot>__1 != null))
            {
                this.<i>__3 = 0;
                while (this.<i>__3 < this.<window>__0.LayoutStartLocation.Tokens.Count)
                {
                    this.<>f__this.LockTokens(false);
                    this.<slot>__1.OnDrop(this.<window>__0.LayoutStartLocation.Tokens[this.<i>__3]);
                    this.<>f__this.LockTokens(true);
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 2;
                    goto Label_01CC;
                Label_0195:
                    this.<i>__3++;
                }
            }
            this.$PC = -1;
        Label_01CA:
            return false;
        Label_01CC:
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

