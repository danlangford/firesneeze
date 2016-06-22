using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelVillain : GuiPanel
{
    [Tooltip("reference to the background animator in our hierarchy")]
    public Animator backgroundAnimator;
    [Tooltip("reference to the continue button in our hierarchy")]
    public GuiButton continueButton;

    [DebuggerHidden]
    private IEnumerator DisplayVillain() => 
        new <DisplayVillain>c__Iterator7A { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator HideVillain() => 
        new <HideVillain>c__Iterator7B { <>f__this = this };

    private void OnContinueButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.Show(false);
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            Game.Instance.StartCoroutine(this.DisplayVillain());
        }
        else
        {
            Game.Instance.StartCoroutine(this.HideVillain());
        }
    }

    public override bool Fullscreen =>
        true;

    [CompilerGenerated]
    private sealed class <DisplayVillain>c__Iterator7A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelVillain <>f__this;

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
                    UI.Window.Pause(true);
                    this.<>f__this.continueButton.Show(false);
                    UI.Sound.Play(SoundEffectType.VillainIntroFire);
                    Turn.Card.SortingOrder = Constants.SPRITE_SORTING_ZOOM;
                    Turn.Card.MoveCard(Vector3.zero, 0.25f).setEase(LeanTweenType.easeOutQuad);
                    LeanTween.scale(Turn.Card.gameObject, Device.GetCardZoomScale(), 0.25f).setEase(LeanTweenType.easeOutQuad);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Show(true);
                    this.<>f__this.backgroundAnimator.SetBool("Glow", true);
                    this.<>f__this.continueButton.Show(true);
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

    [CompilerGenerated]
    private sealed class <HideVillain>c__Iterator7B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelVillain <>f__this;
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
                    this.$current = new WaitForSeconds(0.1f);
                    this.$PC = 1;
                    goto Label_013F;

                case 1:
                    if (Campaign.IsCardEncountered(Turn.Card.ID) || !Cutscene.Exists(CutsceneType.Villain))
                    {
                        this.<>f__this.backgroundAnimator.SetBool("Glow", false);
                        this.<>f__this.Show(false);
                        this.<window>__0 = UI.Window as GuiWindowLocation;
                        if (this.<window>__0 != null)
                        {
                            Turn.Card.SortingOrder = 2;
                            Turn.Card.MoveCard(this.<window>__0.layoutLocation.transform.position, 0.25f).setEase(LeanTweenType.easeOutQuad);
                            LeanTween.scale(Turn.Card.gameObject, this.<window>__0.layoutLocation.Scale, 0.25f).setEase(LeanTweenType.easeOutQuad);
                            this.$current = new WaitForSeconds(0.25f);
                            this.$PC = 2;
                            goto Label_013F;
                        }
                    }
                    break;

                case 2:
                    break;

                default:
                    goto Label_013D;
            }
            UI.Busy = false;
            UI.Window.Pause(false);
            Turn.Proceed();
            this.$PC = -1;
        Label_013D:
            return false;
        Label_013F:
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

