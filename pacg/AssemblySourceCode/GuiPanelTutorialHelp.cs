using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelTutorialHelp : GuiPanel
{
    private bool controllingDice;
    [Tooltip("reference to the button that starts the sequence")]
    public GuiButton HelpButton;
    [Tooltip("list of overlay resources to display in order")]
    public string[] Overlays;

    public override void Initialize()
    {
        this.Show(false);
    }

    private bool IsDiceControlPossible()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        return ((window != null) && window.dicePanel.IsDiceVisible());
    }

    private bool IsHelpAllowed()
    {
        if (UI.Busy)
        {
            return false;
        }
        if (UI.Window.Paused)
        {
            return false;
        }
        return true;
    }

    private void OnHelpButtonPushed()
    {
        if (this.IsHelpAllowed())
        {
            base.StartCoroutine(this.ShowOverlays());
        }
    }

    private void ShowDice(bool isVisible)
    {
        if (this.controllingDice)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.dicePanel.Show(isVisible);
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator ShowOverlays() => 
        new <ShowOverlays>c__IteratorA5 { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator WaitForOverlay(string art) => 
        new <WaitForOverlay>c__IteratorA6 { 
            art = art,
            <$>art = art
        };

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;

    [CompilerGenerated]
    private sealed class <ShowOverlays>c__IteratorA5 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelTutorialHelp <>f__this;
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
                    this.<>f__this.controllingDice = this.<>f__this.IsDiceControlPossible();
                    this.<>f__this.HelpButton.Show(false);
                    this.<>f__this.ShowDice(false);
                    this.<>f__this.InterceptBackgroundTouches(true);
                    this.<i>__0 = 0;
                    break;

                case 1:
                    this.<i>__0++;
                    break;

                default:
                    goto Label_00FC;
            }
            if (this.<i>__0 < this.<>f__this.Overlays.Length)
            {
                this.$current = this.<>f__this.StartCoroutine(this.<>f__this.WaitForOverlay(this.<>f__this.Overlays[this.<i>__0]));
                this.$PC = 1;
                return true;
            }
            this.<>f__this.HelpButton.Show(true);
            this.<>f__this.ShowDice(true);
            this.<>f__this.InterceptBackgroundTouches(false);
            this.$PC = -1;
        Label_00FC:
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

    [CompilerGenerated]
    private sealed class <WaitForOverlay>c__IteratorA6 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string <$>art;
        internal GameObject <go>__3;
        internal GuiPanelTutorial <panel>__1;
        internal string <path>__0;
        internal GameObject <prefab>__2;
        internal string art;

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
                    this.<path>__0 = "Blueprints/Gui/" + this.art;
                    this.<panel>__1 = null;
                    this.<prefab>__2 = Resources.Load<GameObject>(this.<path>__0);
                    if (this.<prefab>__2 != null)
                    {
                        this.<go>__3 = Game.Instance.Create(this.<prefab>__2);
                        if (this.<go>__3 != null)
                        {
                            this.<panel>__1 = this.<go>__3.GetComponent<GuiPanelTutorial>();
                        }
                    }
                    if (this.<panel>__1 == null)
                    {
                        goto Label_00F2;
                    }
                    break;

                case 1:
                    break;
                    this.$PC = -1;
                    goto Label_00F2;

                default:
                    goto Label_00F2;
            }
            if (!this.<panel>__1.Visible)
            {
                UnityEngine.Object.Destroy(this.<panel>__1.gameObject);
            }
            else
            {
                this.$current = null;
                this.$PC = 1;
                return true;
            }
        Label_00F2:
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

