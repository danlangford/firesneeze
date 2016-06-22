using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventCallbackMovePlayerThenDispose : EventCallbackMovePlayer
{
    [DebuggerHidden]
    private IEnumerator Animate(Card card) => 
        new <Animate>c__Iterator1E { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    private void Dispose(Card card)
    {
        switch (card.Disposition)
        {
            case DispositionType.Shuffle:
                Location.Current.Deck.Shuffle();
                break;

            case DispositionType.Acquire:
                Turn.Character.Hand.Add(card);
                break;

            case DispositionType.Banish:
                Campaign.Box.Add(card, false);
                break;

            case DispositionType.Destroy:
                if (card.Deck != null)
                {
                    card.Deck.Remove(card);
                }
                UnityEngine.Object.Destroy(card);
                break;
        }
    }

    private void EventCallbackMovePlayer_Finish()
    {
        base.StartCoroutine(this.FinishMove());
    }

    [DebuggerHidden]
    private IEnumerator FinishMove() => 
        new <FinishMove>c__Iterator1F { <>f__this = this };

    public override void OnCallback()
    {
        Turn.State = GameStateType.Null;
        Game.Instance.StartCoroutine(this.Animate(base.Card));
    }

    public override bool Stateless =>
        false;

    [CompilerGenerated]
    private sealed class <Animate>c__Iterator1E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal EventCallbackMovePlayerThenDispose <>f__this;
        internal string <locID>__0;
        internal VisualEffectType <vfx>__1;
        internal GuiWindowLocation <window>__2;
        internal Card card;

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
                    this.<locID>__0 = this.<>f__this.LocationSelector.Random(Turn.Owner);
                    if (this.<locID>__0 == null)
                    {
                        goto Label_01AB;
                    }
                    if ((this.card.Disposition != DispositionType.Shuffle) || (this.card.Type == CardType.Villain))
                    {
                        break;
                    }
                    this.card.Animate(AnimationType.Undefeated, true);
                    this.<vfx>__1 = this.card.GetAnimationVfx(AnimationType.Undefeated);
                    if (this.<vfx>__1 != VisualEffectType.None)
                    {
                        VisualEffect.ApplyToScreen(this.<vfx>__1, 1.3f);
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.3f));
                    this.$PC = 1;
                    goto Label_01B4;

                case 1:
                    VisualEffect.Shuffle(DeckType.Location);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 2;
                    goto Label_01B4;

                case 2:
                    break;

                default:
                    goto Label_01B2;
            }
            this.<window>__2 = UI.Window as GuiWindowLocation;
            if (this.<window>__2 != null)
            {
                Turn.PushCancelDestination(GameStateType.Post);
                Turn.PushReturnState(GameStateType.Post);
                Turn.PushStateDestination(new TurnStateCallback(this.card, "EventCallbackMovePlayer_Finish"));
                this.<window>__2.mapPanel.Center(this.<locID>__0);
                if (this.card.Type == CardType.Villain)
                {
                    Turn.FocusedCard = this.card;
                    this.card.Deck.Remove(this.card);
                    this.card.Show(true);
                }
                Turn.State = GameStateType.Move;
            }
        Label_01AB:
            this.$PC = -1;
        Label_01B2:
            return false;
        Label_01B4:
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

    [CompilerGenerated]
    private sealed class <FinishMove>c__Iterator1F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal EventCallbackMovePlayerThenDispose <>f__this;
        internal string <locID>__0;
        internal GuiWindowLocation <window>__1;

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
                    this.<locID>__0 = this.<>f__this.LocationSelector.Random(Turn.Owner);
                    this.<window>__1 = UI.Window as GuiWindowLocation;
                    if (this.<window>__1 == null)
                    {
                        break;
                    }
                    this.<window>__1.mapPanel.Center(this.<locID>__0);
                    this.<window>__1.ShowMap(true);
                    this.$current = Game.Instance.StartCoroutine(this.<window>__1.mapPanel.MoveCharacter(Turn.Owner, this.<locID>__0));
                    this.$PC = 1;
                    return true;

                case 1:
                    break;

                default:
                    goto Label_00C8;
            }
            Event.Done();
            Turn.FocusedCard = null;
            this.$PC = -1;
        Label_00C8:
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

