using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventUndefeatedMovePlayer : Event
{
    [Tooltip("does the encountered card follow the player?")]
    public bool Follow;
    [Tooltip("which locations are valid?")]
    public LocationSelector LocationSelector;
    [Tooltip("who is going to move?")]
    public DamageTargetType Target = DamageTargetType.Player;

    [DebuggerHidden]
    private IEnumerator Animate(Card card) => 
        new <Animate>c__Iterator28 { 
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
                card.Deck.Remove(card);
                UnityEngine.Object.Destroy(card);
                break;
        }
    }

    private void EventUndefeatedMovePlayer_Activate()
    {
        Game.Instance.StartCoroutine(this.Animate(Turn.Card));
        Turn.State = GameStateType.Null;
    }

    public override void OnCardUndefeated(Card card)
    {
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventUndefeatedMovePlayer_Activate"));
        Turn.PushCancelDestination(GameStateType.Dispose);
        Turn.PushReturnState(GameStateType.Dispose);
        Turn.State = GameStateType.Move;
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;

    [CompilerGenerated]
    private sealed class <Animate>c__Iterator28 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal EventUndefeatedMovePlayer <>f__this;
        internal List<Character> <characters>__0;
        internal int <i>__2;
        internal int <i>__3;
        internal List<string> <locations>__1;
        internal VisualEffectType <vfx>__4;
        internal GuiWindowLocation <window>__5;
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
                    this.<characters>__0 = new List<Character>(Constants.MAX_PARTY_MEMBERS);
                    this.<locations>__1 = new List<string>(Constants.MAX_PARTY_MEMBERS);
                    switch (this.<>f__this.Target)
                    {
                        case DamageTargetType.Player:
                            if (Turn.Character.Alive)
                            {
                                this.<characters>__0.Add(Turn.Character);
                                this.<locations>__1.Add(this.<>f__this.LocationSelector.Random());
                            }
                            goto Label_01E2;

                        case DamageTargetType.Location:
                            this.<i>__2 = 0;
                            while (this.<i>__2 < Party.Characters.Count)
                            {
                                if ((Party.Characters[this.<i>__2].Location == Location.Current.ID) && Party.Characters[this.<i>__2].Alive)
                                {
                                    this.<characters>__0.Add(Party.Characters[this.<i>__2]);
                                    this.<locations>__1.Add(this.<>f__this.LocationSelector.Random());
                                }
                                this.<i>__2++;
                            }
                            goto Label_01E2;

                        case DamageTargetType.Party:
                            this.<i>__3 = 0;
                            while (this.<i>__3 < Party.Characters.Count)
                            {
                                if (Party.Characters[this.<i>__3].Alive)
                                {
                                    this.<characters>__0.Add(Party.Characters[this.<i>__3]);
                                    this.<locations>__1.Add(this.<>f__this.LocationSelector.Random());
                                }
                                this.<i>__3++;
                            }
                            goto Label_01E2;
                    }
                    break;

                case 1:
                    VisualEffect.Shuffle(DeckType.Location);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 2;
                    goto Label_0440;

                case 2:
                    goto Label_02E7;

                case 3:
                    if (this.<>f__this.Follow && (this.<locations>__1.Count > 0))
                    {
                        this.$current = Game.Instance.StartCoroutine(this.<window>__5.mapPanel.Animate(this.card, Location.Current.ID, this.<locations>__1[0]));
                        this.$PC = 4;
                        goto Label_0440;
                    }
                    goto Label_03DC;

                case 4:
                    goto Label_03DC;

                default:
                    goto Label_043E;
            }
        Label_01E2:
            if (this.<>f__this.Follow && (this.<locations>__1.Count > 0))
            {
                this.card.Disposition = DispositionType.Destroy;
                Location.Distribute(this.<locations>__1[0], Turn.Card, DeckPositionType.Shuffle, false);
            }
            if ((this.card.Disposition == DispositionType.Shuffle) && (this.card.Type != CardType.Villain))
            {
                this.card.Animate(AnimationType.Undefeated, true);
                this.<vfx>__4 = this.card.GetAnimationVfx(AnimationType.Undefeated);
                if (this.<vfx>__4 != VisualEffectType.None)
                {
                    VisualEffect.ApplyToScreen(this.<vfx>__4, 1.3f);
                }
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.3f));
                this.$PC = 1;
                goto Label_0440;
            }
        Label_02E7:
            this.<window>__5 = UI.Window as GuiWindowLocation;
            if (this.<window>__5 == null)
            {
                goto Label_041E;
            }
            this.card.Show(false);
            this.<window>__5.layoutLocation.GlowText(false);
            this.<window>__5.ShowMap(true);
            this.$current = Game.Instance.StartCoroutine(this.<window>__5.mapPanel.Animate(this.<characters>__0.ToArray(), this.<locations>__1.ToArray()));
            this.$PC = 3;
            goto Label_0440;
        Label_03DC:
            this.<window>__5.ShowMap(false);
            this.<>f__this.Dispose(this.card);
            Location.Load(Turn.Owner.Location);
            Scenario.Current.OnLocationChange();
            this.<window>__5.Refresh();
        Label_041E:
            Turn.Disposed = true;
            Turn.PushStateDestination(GameStateType.Dispose);
            Turn.State = GameStateType.Recharge;
            Event.Done();
            this.$PC = -1;
        Label_043E:
            return false;
        Label_0440:
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

