using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class GameStateReset : GameState
{
    private bool CanShareCards()
    {
        if (Game.GameType == GameType.LocalMultiPlayer)
        {
            for (int i = 0; i < Turn.Character.Hand.Count; i++)
            {
                if (Turn.Character.Hand[i].Shareable)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private int CountDisplayedCards()
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = 0; j < Party.Characters[i].Hand.Count; j++)
            {
                if (Party.Characters[i].Hand[j].Displayed)
                {
                    num++;
                }
            }
        }
        return num;
    }

    [DebuggerHidden]
    protected virtual IEnumerator DrawCards() => 
        new <DrawCards>c__Iterator40 { <>f__this = this };

    public override void Enter()
    {
        Game.Synchronize();
        if (this.CountDisplayedCards() > 0)
        {
            Turn.DestructiveActionsCount++;
            Turn.PushStateDestination(GameStateType.Reset);
            Turn.State = GameStateType.Undisplay;
        }
        else if (Turn.Character.IsOverHandSize() || Turn.Discard)
        {
            Turn.Discard = false;
            Turn.State = GameStateType.Discard;
        }
        else
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowProceedButton(false);
                window.ShowCancelButton(false);
            }
            Location.Current.OnHandReset();
            if (base.IsCurrentState())
            {
                Turn.Phase = TurnPhaseType.End;
                Game.Instance.StartCoroutine(this.DrawCards());
            }
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        false;

    public override void Proceed()
    {
        Scenario.Current.ResetTemporaryLocationClosures();
        Scenario.Current.ResetImpossibleLocationClosures();
        Turn.Card.OnTurnEnded();
        Scenario.Current.OnTurnEnded();
        Location.Current.OnTurnEnded();
        if (!Turn.Character.Alive)
        {
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Death_GetNextCharacter"));
            Turn.State = GameStateType.Death;
        }
        else if (this.CanShareCards())
        {
            Turn.State = GameStateType.Share;
        }
        else
        {
            Turn.Next();
            Turn.State = GameStateType.Switch;
        }
    }

    protected virtual void Reset_Finish()
    {
        this.Proceed();
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Reset;

    [CompilerGenerated]
    private sealed class <DrawCards>c__Iterator40 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameStateReset <>f__this;
        internal Card <card>__3;
        internal int <i>__2;
        internal int <n>__1;
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
                    this.<>f__this.Busy = true;
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if (this.<window>__0 == null)
                    {
                        goto Label_0212;
                    }
                    this.<n>__1 = Turn.Owner.HandSize - Turn.Owner.Hand.Count;
                    if (this.<n>__1 > 0)
                    {
                        Tutorial.Notify(TutorialEventType.TurnDrawCard);
                    }
                    this.<i>__2 = 0;
                    while (this.<i>__2 < this.<n>__1)
                    {
                        if (Turn.Owner.Deck.Count > 0)
                        {
                            this.<card>__3 = Turn.Owner.Deck.Draw();
                            this.<card>__3.Clear();
                            if (Turn.Owner.ID == Turn.Character.ID)
                            {
                                if (this.<i>__2 == 0)
                                {
                                    UI.Sound.Play(SoundEffectType.DrawCardStart);
                                }
                                else
                                {
                                    UI.Sound.Play(SoundEffectType.GenericFlickCard);
                                }
                                this.<window>__0.layoutRecharge.Position(this.<card>__3);
                                this.<window>__0.Draw(this.<card>__3);
                                this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.3f));
                                this.$PC = 1;
                                goto Label_0232;
                            }
                            Turn.Owner.Hand.Add(this.<card>__3);
                        }
                        else
                        {
                            Turn.Owner.Alive = false;
                            break;
                        }
                    Label_018F:
                        this.<i>__2++;
                    }
                    break;

                case 1:
                    goto Label_018F;

                case 2:
                    goto Label_01E0;

                case 3:
                    goto Label_0212;

                default:
                    goto Label_0230;
            }
            if (this.<n>__1 <= 0)
            {
                this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.5f));
                this.$PC = 2;
                goto Label_0232;
            }
        Label_01E0:
            if (this.<n>__1 > 0)
            {
                this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.25f));
                this.$PC = 3;
                goto Label_0232;
            }
        Label_0212:
            this.<>f__this.Busy = false;
            this.<>f__this.Reset_Finish();
            this.$PC = -1;
        Label_0230:
            return false;
        Label_0232:
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

