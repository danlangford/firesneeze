using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class GameStateVillainIntro : GameState
{
    [DebuggerHidden]
    private IEnumerator DisplayVillain() => 
        new <DisplayVillain>c__Iterator43();

    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
            window.ShowCancelButton(false);
        }
        Game.Instance.StartCoroutine(this.DisplayVillain());
    }

    public override void Proceed()
    {
        if (Scenario.Current.IsCurrentVillain(Turn.Card.ID))
        {
            Scenario current = Scenario.Current;
            current.NumVillainEncounters++;
        }
        Turn.State = GameStateType.Villain;
    }

    public override GameStateType Type =>
        GameStateType.VillainIntro;

    [CompilerGenerated]
    private sealed class <DisplayVillain>c__Iterator43 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
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
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.15f));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if (this.<window>__0 != null)
                    {
                        this.<window>__0.villainPanel.Show(true);
                    }
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

