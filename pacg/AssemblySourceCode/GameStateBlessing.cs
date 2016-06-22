using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class GameStateBlessing : GameState
{
    public override void Enter()
    {
        Game.Instance.StartCoroutine(this.Enter_Coroutine());
    }

    [DebuggerHidden]
    private IEnumerator Enter_Coroutine() => 
        new <Enter_Coroutine>c__Iterator30 { <>f__this = this };

    public override void Proceed()
    {
        Tutorial.Notify(TutorialEventType.TurnStarted);
        Scenario.Current.OnBeforeTurnStart();
        if (base.IsCurrentState())
        {
            Turn.State = GameStateType.StartTurn;
        }
    }

    [DebuggerHidden]
    private IEnumerator ProceedAfterDelay(float delay) => 
        new <ProceedAfterDelay>c__Iterator31 { 
            delay = delay,
            <$>delay = delay,
            <>f__this = this
        };

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Blessing;

    [CompilerGenerated]
    private sealed class <Enter_Coroutine>c__Iterator30 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameStateBlessing <>f__this;
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
                    Turn.Phase = TurnPhaseType.Blessing;
                    if (Scenario.Current.Blessings.Count > 0)
                    {
                        if (Turn.Character.Location != Location.Current.ID)
                        {
                            this.$current = Game.Instance.StartCoroutine(Location.LoadAsync(Turn.Character.Location));
                            this.$PC = 1;
                            return true;
                        }
                        break;
                    }
                    Turn.State = GameStateType.End;
                    goto Label_0152;

                case 1:
                    break;

                default:
                    goto Label_0152;
            }
            if (this.<window>__0 != null)
            {
                this.<window>__0.blessingsPanel.Next();
            }
            if (this.<window>__0 != null)
            {
                if (!Turn.IsSwitchingCharacters())
                {
                    this.<window>__0.Refresh();
                }
                this.<window>__0.ShowExploreButton(false);
                this.<window>__0.ShowProceedButton(false);
                this.<window>__0.ShowCancelButton(false);
                this.<window>__0.mapPanel.Center(Turn.Owner.Location);
            }
            UI.Window.Reset();
            Game.Instance.StartCoroutine(this.<>f__this.ProceedAfterDelay(0.5f));
            this.$PC = -1;
        Label_0152:
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
    private sealed class <ProceedAfterDelay>c__Iterator31 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>delay;
        internal GameStateBlessing <>f__this;
        internal float delay;

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
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(this.delay));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Proceed();
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
}

