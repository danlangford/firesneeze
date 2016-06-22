using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelIntertitle : GuiPanel
{
    [Tooltip("reference to the background in our hierarchy")]
    public GuiImage Background;
    [Tooltip("reference to the label in our hierarchy")]
    public GuiLabel Text;

    [DebuggerHidden]
    private IEnumerator Epilogue(CutsceneIntertitle intertitle) => 
        new <Epilogue>c__Iterator1C { 
            intertitle = intertitle,
            <$>intertitle = intertitle,
            <>f__this = this
        };

    private void Exit(CutsceneIntertitle intertitle)
    {
        intertitle.Complete = true;
        this.Playing = false;
        UI.Window.Pause(false);
        GuiWindowCutscene window = UI.Window as GuiWindowCutscene;
        if (window != null)
        {
            if (intertitle.Type == CutsceneIntertitleType.Prologue)
            {
                window.Scene.Play();
            }
            else
            {
                window.Scene.Stop();
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator Prologue(CutsceneIntertitle intertitle) => 
        new <Prologue>c__Iterator1B { 
            intertitle = intertitle,
            <$>intertitle = intertitle,
            <>f__this = this
        };

    public void Show(CutsceneIntertitle intertitle)
    {
        if (!this.Playing)
        {
            if (intertitle.Type == CutsceneIntertitleType.Prologue)
            {
                Game.Instance.StartCoroutine(this.Prologue(intertitle));
            }
            else
            {
                Game.Instance.StartCoroutine(this.Epilogue(intertitle));
            }
        }
    }

    public bool Playing { get; private set; }

    [CompilerGenerated]
    private sealed class <Epilogue>c__Iterator1C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CutsceneIntertitle <$>intertitle;
        internal GuiPanelIntertitle <>f__this;
        internal CutsceneIntertitle intertitle;

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
                    this.<>f__this.Playing = true;
                    this.<>f__this.Text.Text = this.intertitle.Text;
                    this.<>f__this.Show(true);
                    this.<>f__this.Background.FadeIn(0.5f);
                    this.<>f__this.Text.Fade(true, 0.5f);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.intertitle.Duration));
                    this.$PC = 1;
                    goto Label_0111;

                case 1:
                    this.<>f__this.Text.Fade(false, 0.5f);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
                    this.$PC = 2;
                    goto Label_0111;

                case 2:
                    this.<>f__this.Exit(this.intertitle);
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0111:
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
    private sealed class <Prologue>c__Iterator1B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CutsceneIntertitle <$>intertitle;
        internal GuiPanelIntertitle <>f__this;
        internal CutsceneIntertitle intertitle;

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
                    this.<>f__this.Playing = true;
                    this.<>f__this.Text.Text = string.Empty;
                    this.<>f__this.Show(true);
                    UI.Window.Pause(true);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.intertitle.Delay));
                    this.$PC = 1;
                    goto Label_0187;

                case 1:
                    this.<>f__this.Text.Text = this.intertitle.Text;
                    this.<>f__this.Text.Fade(true, 0.3f);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 2;
                    goto Label_0187;

                case 2:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.intertitle.Duration));
                    this.$PC = 3;
                    goto Label_0187;

                case 3:
                    this.<>f__this.Text.Fade(false, 0.3f);
                    this.<>f__this.Background.FadeOut(0.5f);
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 4;
                    goto Label_0187;

                case 4:
                    this.<>f__this.Exit(this.intertitle);
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0187:
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

