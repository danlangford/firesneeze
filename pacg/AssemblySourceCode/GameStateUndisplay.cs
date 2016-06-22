using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class GameStateUndisplay : GameState
{
    public override void Enter()
    {
        Game.Instance.StartCoroutine(this.UndisplayAllCards());
    }

    public override void Proceed()
    {
        Turn.GotoStateDestination();
    }

    [DebuggerHidden]
    private IEnumerator UndisplayAllCards() => 
        new <UndisplayAllCards>c__Iterator42 { <>f__this = this };

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Undisplay;

    [CompilerGenerated]
    private sealed class <UndisplayAllCards>c__Iterator42 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameStateUndisplay <>f__this;
        internal int <c>__0;
        internal Card <card>__3;
        internal int <i>__1;
        internal int <j>__2;
        internal CardPropertyDisplay <property>__4;

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
                    this.<c>__0 = 0;
                    this.<i>__1 = 0;
                    while (this.<i>__1 < Party.Characters.Count)
                    {
                        this.<j>__2 = Party.Characters[this.<i>__1].Hand.Count - 1;
                        while (this.<j>__2 >= 0)
                        {
                            this.<card>__3 = Party.Characters[this.<i>__1].Hand[this.<j>__2];
                            if (this.<card>__3.Displayed)
                            {
                                this.<card>__3.Locked = false;
                                if (Rules.IsRechargePossible(Party.Characters[this.<i>__1], this.<card>__3))
                                {
                                    Party.Characters[this.<i>__1].Recharge.Add(this.<card>__3);
                                    this.<c>__0++;
                                }
                                else
                                {
                                    this.<card>__3.Clear();
                                    this.<property>__4 = this.<card>__3.GetComponent<CardPropertyDisplay>();
                                    if (this.<property>__4 != null)
                                    {
                                        Party.Characters[this.<i>__1].Bury.Add(this.<card>__3);
                                    }
                                    else
                                    {
                                        if (!Rules.IsRechargePossible(Party.Characters[this.<i>__1], this.<card>__3))
                                        {
                                            VisualEffect.ApplyToCard(VisualEffectType.CardBanishFromDisplay, this.<card>__3, 2.1f);
                                            UI.Sound.Play(SoundEffectType.BoonFailAcquireBanish);
                                            this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.3f));
                                            this.$PC = 1;
                                            goto Label_0277;
                                        }
                                        Party.Characters[this.<i>__1].Discard.Add(this.<card>__3);
                                    }
                                }
                            }
                        Label_0207:
                            this.<j>__2--;
                        }
                        this.<i>__1++;
                    }
                    if (this.<c>__0 > 0)
                    {
                        Turn.PushStateDestination(GameStateType.Reset);
                        Turn.State = GameStateType.Recharge;
                    }
                    else
                    {
                        this.<>f__this.Proceed();
                    }
                    this.$PC = -1;
                    break;

                case 1:
                    Campaign.Box.Add(this.<card>__3, false);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1.7f));
                    this.$PC = 2;
                    goto Label_0277;

                case 2:
                    goto Label_0207;
            }
            return false;
        Label_0277:
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

