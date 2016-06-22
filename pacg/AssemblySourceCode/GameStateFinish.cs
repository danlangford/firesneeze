using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class GameStateFinish : GameState
{
    public override void Enter()
    {
        Game.Instance.StartCoroutine(this.Enter_Coroutine());
    }

    [DebuggerHidden]
    private IEnumerator Enter_Coroutine() => 
        new <Enter_Coroutine>c__Iterator36 { <>f__this = this };

    public override void Exit(GameStateType nextState)
    {
        if (nextState != GameStateType.Confirm)
        {
            base.ProcessLayoutDecks();
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.commandsPanel.ShowDiscardButton(false);
            window.commandsPanel.ShowEndTurnButton(false);
            if (Turn.End)
            {
                window.layoutSummoner.Clear();
            }
            if (nextState == GameStateType.Close)
            {
                if (Turn.Close)
                {
                    Turn.CloseType = CloseType.None;
                }
                Turn.Close = false;
            }
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    public override void Proceed()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && Rules.IsExplorePromptNecessary())
        {
            Turn.EmptyLayoutDecks = false;
            window.Popup.Clear();
            window.Popup.SetDeckPosition(DeckType.Character);
            window.Popup.Add(StringTableManager.GetUIText(0x107), new TurnStateCallback(TurnStateCallbackType.Global, "GameStateFinish_Back"));
            window.Popup.Add(StringTableManager.GetUIText(0x108), new TurnStateCallback(TurnStateCallbackType.Global, "GameStateFinish_Continue"));
            Turn.State = GameStateType.Popup;
        }
        else
        {
            Tutorial.Notify(TutorialEventType.TurnEnded);
            Turn.ClearTurnData();
            Turn.State = GameStateType.EndTurn;
        }
    }

    public override GameStateType Type =>
        GameStateType.Finish;

    [CompilerGenerated]
    private sealed class <Enter_Coroutine>c__Iterator36 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameStateFinish <>f__this;
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
                    if (this.<window>__0 != null)
                    {
                        this.<window>__0.layoutLocation.Show(false);
                        this.<>f__this.Enter();
                        this.<window>__0.dicePanel.Clear();
                    }
                    if ((Turn.CountExplores <= 0) && Turn.Explore)
                    {
                        Turn.Explore = false;
                    }
                    if (Scenario.Current.IsScenarioOver())
                    {
                        if (Scenario.Current.IsScenarioWon())
                        {
                            Scenario.Current.Complete = true;
                        }
                        Turn.State = GameStateType.End;
                    }
                    if (Turn.Owner.Location != Location.Current.ID)
                    {
                        this.$current = Game.Instance.StartCoroutine(Location.LoadAsync(Turn.Owner.Location));
                        this.$PC = 1;
                        return true;
                    }
                    break;

                case 1:
                    break;

                default:
                    goto Label_020A;
            }
            if (Turn.End)
            {
                this.<>f__this.Proceed();
            }
            else if (!Turn.Owner.Alive)
            {
                Turn.Next();
                Turn.State = GameStateType.Switch;
                if (this.<window>__0 != null)
                {
                    this.<window>__0.Refresh();
                }
            }
            else
            {
                if (Rules.IsPermanentClosePossible())
                {
                    Turn.PushReturnState();
                    Turn.State = GameStateType.ClosePrompt;
                }
                if (this.<window>__0 != null)
                {
                    Turn.Phase = TurnPhaseType.Explore;
                    if (Turn.Owner.IsOverHandSize())
                    {
                        this.<window>__0.ShowProceedDiscardButton(true);
                    }
                    else
                    {
                        this.<window>__0.ShowProceedEndButton(true);
                    }
                    if (Rules.IsExplorePossible())
                    {
                        this.<window>__0.ShowExploreButton(Turn.Explore);
                    }
                    this.<window>__0.commandsPanel.ShowDiscardButton(true);
                    this.<window>__0.commandsPanel.ShowEndTurnButton(true);
                    this.<window>__0.ShowMapButton(true);
                    this.<>f__this.ShowAidButton();
                }
            }
            this.$PC = -1;
        Label_020A:
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

