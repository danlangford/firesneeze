using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class GuiScript
{
    protected GuiScript()
    {
    }

    public virtual bool Play() => 
        false;

    public virtual void Stop()
    {
    }

    [DebuggerHidden]
    protected IEnumerator WaitForTime(float time) => 
        new <WaitForTime>c__Iterator7C { 
            time = time,
            <$>time = time
        };

    public virtual float Length =>
        0f;

    [CompilerGenerated]
    private sealed class <WaitForTime>c__Iterator7C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>time;
        internal GuiWindowLocation <window>__0;
        internal float time;

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
                        this.$current = new WaitForSeconds(this.time);
                        this.$PC = 2;
                        goto Label_00BB;
                    }
                    break;

                case 1:
                    break;

                case 2:
                    goto Label_00B2;

                default:
                    goto Label_00B9;
            }
            if (this.time > 0f)
            {
                if (this.<window>__0.Visible)
                {
                    this.time -= Time.deltaTime;
                }
                this.$current = null;
                this.$PC = 1;
                goto Label_00BB;
            }
        Label_00B2:
            this.$PC = -1;
        Label_00B9:
            return false;
        Label_00BB:
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

