using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStateHenchmanIntro : GameState
{
    [DebuggerHidden]
    private IEnumerator DisplayHenchman() => 
        new <DisplayHenchman>c__Iterator3C();

    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
            window.ShowCancelButton(false);
        }
        Game.Instance.StartCoroutine(this.DisplayHenchman());
    }

    public override void Proceed()
    {
        Turn.State = GameStateType.Henchman;
    }

    public override GameStateType Type =>
        GameStateType.HenchmanIntro;

    [CompilerGenerated]
    private sealed class <DisplayHenchman>c__Iterator3C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;

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
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.15f));
                    this.$PC = 1;
                    goto Label_00F6;

                case 1:
                    Turn.Card.SortingOrder = Constants.SPRITE_SORTING_ZOOM;
                    Turn.Card.MoveCard(Vector3.zero, 0.25f).setEase(LeanTweenType.easeOutQuad);
                    LeanTween.scale(Turn.Card.gameObject, Device.GetCardZoomScale(), 0.25f).setEase(LeanTweenType.easeOutQuad);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 2;
                    goto Label_00F6;

                case 2:
                    UI.Window.Pause(false);
                    Game.UI.ContinuePanel.Callback = new TurnStateCallback(GameStateType.Henchman);
                    Game.UI.ContinuePanel.Show(true);
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_00F6:
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

