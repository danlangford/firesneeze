using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class GameStateEnd : GameState
{
    [DebuggerHidden]
    private IEnumerator AnimateSceneChange(GameEndingType endingType) => 
        new <AnimateSceneChange>c__Iterator35 { 
            endingType = endingType,
            <$>endingType = endingType
        };

    public override void Enter()
    {
        GameEndingType gameEndingType = this.GetGameEndingType();
        Turn.EmptyLayoutDecks = true;
        base.Enter();
        Scenario.Current.End();
        this.HideAllCards();
        Game.Instance.StartCoroutine(this.AnimateSceneChange(gameEndingType));
    }

    private GameEndingType GetGameEndingType()
    {
        if (Scenario.Current.Complete)
        {
            return GameEndingType.Win;
        }
        if (Scenario.Current.Blessings.Count <= 0)
        {
            return GameEndingType.OutOfTime;
        }
        return GameEndingType.Dead;
    }

    private void HideAllCards()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = 0; j < Party.Characters[i].Hand.Count; j++)
            {
                Party.Characters[i].Hand[j].Show(false);
            }
        }
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.End;

    [CompilerGenerated]
    private sealed class <AnimateSceneChange>c__Iterator35 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameStateEnd.GameEndingType <$>endingType;
        internal float <time>__1;
        internal GuiWindowLocation <window>__0;
        internal GameStateEnd.GameEndingType endingType;

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
                    switch (this.endingType)
                    {
                        case GameStateEnd.GameEndingType.Win:
                            Game.UI.ShowCutsceneScene();
                            break;

                        case GameStateEnd.GameEndingType.OutOfTime:
                            this.<window>__0 = UI.Window as GuiWindowLocation;
                            if (this.<window>__0 != null)
                            {
                                this.<time>__1 = this.<window>__0.blessingsPanel.End();
                                this.$current = GameState.WaitForTime(this.<time>__1);
                                this.$PC = 1;
                                return true;
                            }
                            goto Label_00A2;

                        case GameStateEnd.GameEndingType.Dead:
                            Game.UI.ShowGameOverScene();
                            break;
                    }
                    goto Label_00C0;

                case 1:
                    break;

                default:
                    goto Label_00C7;
            }
        Label_00A2:
            Game.UI.ShowGameOverScene();
        Label_00C0:
            this.$PC = -1;
        Label_00C7:
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

    private enum GameEndingType
    {
        Win,
        OutOfTime,
        Dead
    }
}

