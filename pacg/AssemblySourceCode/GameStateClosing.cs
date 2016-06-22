using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStateClosing : GameState
{
    public override void Enter()
    {
        base.SaveRechargableCards();
        base.ProcessLayoutDecks();
        Game.Events.Pause(true);
        Location.Current.Closed = true;
        Location.Current.OnLocationCloseAttempted();
        Game.Events.Pause(false);
        if (this.IsEmpty(Location.Current.Deck))
        {
            this.Proceed();
        }
        else
        {
            Game.Instance.StartCoroutine(this.RunCloseSequence());
        }
    }

    public override void Exit(GameStateType nextState)
    {
        if (Turn.CloseType == CloseType.CloseInsideTempClose)
        {
            Turn.CloseType = CloseType.Temporary;
        }
        else
        {
            Turn.CloseType = CloseType.None;
        }
        Turn.Close = false;
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        false;

    private bool IsEmpty(Deck deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if ((deck[i] != null) && (deck[i].Disposition != DispositionType.Destroy))
            {
                return false;
            }
        }
        return true;
    }

    public override void Proceed()
    {
        TurnStateCallback callback = Turn.PopStateDestination();
        Game.Events.Next();
        if (base.IsCurrentState())
        {
            Turn.PushStateDestination(callback);
            Turn.GotoStateDestination();
        }
    }

    [DebuggerHidden]
    private IEnumerator RunCloseSequence() => 
        new <RunCloseSequence>c__Iterator32();

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Closing;

    [CompilerGenerated]
    private sealed class <RunCloseSequence>c__Iterator32 : IDisposable, IEnumerator, IEnumerator<object>
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
                case 1:
                    if (GuiPanelExamine.Open)
                    {
                        this.$current = new WaitForSeconds(0.15f);
                        this.$PC = 1;
                    }
                    else
                    {
                        this.$current = new WaitForSeconds(0.5f);
                        this.$PC = 2;
                    }
                    return true;

                case 2:
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    this.<window>__0.layoutExamine.Deck = Location.Current.Deck;
                    this.<window>__0.layoutExamine.Script = new GuiScriptCloseLocation();
                    this.<window>__0.layoutExamine.Show(true);
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

