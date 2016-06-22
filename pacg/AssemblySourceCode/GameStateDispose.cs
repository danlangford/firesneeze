using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class GameStateDispose : GameState
{
    private bool AdvanceCombatSequence(Card card)
    {
        if (card.NumCheckSequences > 1)
        {
            Turn.CombatCheckSequence++;
            if (Turn.CombatCheckSequence <= card.NumCheckSequences)
            {
                return true;
            }
        }
        return false;
    }

    [DebuggerHidden]
    private IEnumerator Animate(Card card) => 
        new <Animate>c__Iterator34 { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    [DebuggerHidden]
    public static IEnumerator BanishBoon(Card card) => 
        new <BanishBoon>c__Iterator33 { 
            card = card,
            <$>card = card
        };

    public override void Enter()
    {
        if (Turn.Disposed)
        {
            this.HideLocationCard();
            Turn.Card.Show(false);
            Turn.ClearEncounterData();
            this.Proceed();
        }
        else if (!this.IsDisposePossible(Turn.Card))
        {
            this.AdvanceCombatSequence(Turn.Card);
            this.Proceed();
        }
        else
        {
            if (!Rules.IsCardSummons(Turn.Card) && (Location.Current.ID != Turn.EncounteredLocation))
            {
                Location.Load(Turn.EncounteredLocation);
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    window.layoutLocation.Refresh();
                }
            }
            if (Location.Current.Deck.Count > 0)
            {
                Game.Instance.StartCoroutine(this.Animate(Turn.Card));
            }
            else
            {
                this.Proceed();
            }
        }
    }

    private void HideLocationCard()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutLocation.Show(false);
            window.layoutLocation.GlowText(false);
            window.layoutExplore.Refresh();
            window.ShowExploreButton(false);
        }
    }

    private bool IsAnimationPossible(Card card, AnimationType animation, bool isSummons)
    {
        if (!Turn.Evade)
        {
            if ((((animation == AnimationType.Defeated) && (card.Type == CardType.Villain)) && (!isSummons && (card.GetComponent<EventDefeatedVillain>() != null))) && !card.GetComponent<EventDefeatedVillain>().IsAnimationPossible(animation))
            {
                return false;
            }
            if ((((animation == AnimationType.Undefeated) && (card.Type == CardType.Villain)) && (!isSummons && (card.GetComponent<EventDefeatedVillain>() != null))) && !card.GetComponent<EventDefeatedVillain>().IsAnimationPossible(animation))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsDisposePossible(Card card)
    {
        if (Turn.Disposed)
        {
            return false;
        }
        if (Turn.Iterators.IsRunning(TurnStateIteratorType.Check))
        {
            return false;
        }
        if (Turn.Iterators.IsRunning(TurnStateIteratorType.Damage))
        {
            return false;
        }
        if (Turn.Iterators.IsRunning(TurnStateIteratorType.DamageRoll))
        {
            return false;
        }
        if (((Turn.Iterators.Current != null) && Turn.Iterators.Current.HasPostEvent) && !Rules.IsCardSummons(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsEncounterOver())
        {
            return false;
        }
        return true;
    }

    public override void Proceed()
    {
        if ((Turn.PeekStateDestination() != null) && base.IsCurrentState())
        {
            Turn.GotoStateDestination();
        }
        else if (base.IsCurrentState())
        {
            Turn.PushStateDestination(GameStateType.Done);
            Turn.State = GameStateType.Recharge;
        }
    }

    private void RefreshCardList()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.locationPanel.RefreshCardList();
        }
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Dispose;

    [CompilerGenerated]
    private sealed class <Animate>c__Iterator34 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal GameStateDispose <>f__this;
        internal bool <isSummons>__1;
        internal VisualEffectType <vfx>__2;
        internal VisualEffectType <vfx>__3;
        internal GuiWindowLocation <window>__0;
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
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if (this.<window>__0 != null)
                    {
                        this.<window>__0.layoutLocation.GlowText(false);
                        this.<window>__0.layoutLocation.ShowPreludeFX(false);
                    }
                    Turn.Number = Turn.Current;
                    UI.Window.Pause(true);
                    this.<isSummons>__1 = Turn.Summons;
                    Turn.ClearEncounterData();
                    Turn.Card.Decorations.Clear();
                    if ((this.card.Disposition != DispositionType.Banish) || !this.<>f__this.IsAnimationPossible(this.card, AnimationType.Defeated, this.<isSummons>__1))
                    {
                        break;
                    }
                    if (!this.card.IsBoon())
                    {
                        VisualEffect.ApplyToCard(VisualEffectType.CardDefeatedBanish, Turn.Card, 3f);
                        UI.Sound.Play(SoundEffectType.BaneDefeatBanish);
                        this.card.Animate(AnimationType.Defeated, true);
                        this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1f));
                        this.$PC = 2;
                    }
                    else
                    {
                        this.$current = Game.Instance.StartCoroutine(GameStateDispose.BanishBoon(this.card));
                        this.$PC = 1;
                    }
                    goto Label_0562;

                case 1:
                    break;

                case 2:
                    Campaign.Box.Add(this.card, false);
                    break;

                case 3:
                    goto Label_021A;

                case 4:
                    UI.Sound.Play(SoundEffectType.SuccessCardAcquire);
                    VisualEffect.ApplyToCard(VisualEffectType.CardWinBoon, Turn.Card, 1f);
                    this.card.Animate(AnimationType.Acquire, true);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1f));
                    this.$PC = 5;
                    goto Label_0562;

                case 5:
                    Turn.Character.Hand.Add(this.card);
                    Turn.Character.Hand.Layout.Display();
                    this.card.Disposition = DispositionType.None;
                    goto Label_031D;

                case 6:
                    goto Label_03C9;

                case 7:
                    UI.Sound.Play(SoundEffectType.SuccessCardAcquire);
                    VisualEffect.ApplyToCard(VisualEffectType.CardWinBoonSetAside, Turn.Card, 2.5f);
                    this.card.Animate(AnimationType.Acquire, true);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1f));
                    this.$PC = 8;
                    goto Label_0562;

                case 8:
                    if (this.<window>__0 != null)
                    {
                        this.<window>__0.effectsPanel.RefreshTopButton();
                    }
                    this.card.Disposition = DispositionType.Destroy;
                    goto Label_04B1;

                default:
                    goto Label_0560;
            }
            if (this.card.Disposition != DispositionType.Shuffle)
            {
                goto Label_0261;
            }
            if (this.<>f__this.IsAnimationPossible(this.card, AnimationType.Undefeated, this.<isSummons>__1))
            {
                this.card.Animate(AnimationType.Undefeated, true);
                UI.Sound.Play(SoundEffectType.BaneShuffled);
                this.<vfx>__2 = this.card.GetAnimationVfx(AnimationType.Undefeated);
                if (this.<vfx>__2 != VisualEffectType.None)
                {
                    VisualEffect.ApplyToScreen(this.<vfx>__2, 1.3f);
                }
                this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1.3f));
                this.$PC = 3;
                goto Label_0562;
            }
        Label_021A:
            VisualEffect.Shuffle(DeckType.Location);
            Location.Current.Deck.Shuffle();
            this.card.Disposition = DispositionType.None;
            this.card.Show(false);
            this.<window>__0.layoutExplore.Position(this.card);
        Label_0261:
            if (this.card.Disposition == DispositionType.Acquire)
            {
                this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.1f));
                this.$PC = 4;
                goto Label_0562;
            }
        Label_031D:
            if ((this.card.Disposition != DispositionType.Top) && (this.card.Disposition != DispositionType.Bottom))
            {
                goto Label_03FC;
            }
            if (this.<>f__this.IsAnimationPossible(this.card, AnimationType.Undefeated, this.<isSummons>__1))
            {
                this.card.Animate(AnimationType.Undefeated, true);
                UI.Sound.Play(SoundEffectType.BaneShuffled);
                this.<vfx>__3 = this.card.GetAnimationVfx(AnimationType.Undefeated);
                if (this.<vfx>__3 != VisualEffectType.None)
                {
                    VisualEffect.ApplyToScreen(this.<vfx>__3, 1.3f);
                }
                this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1.3f));
                this.$PC = 6;
                goto Label_0562;
            }
        Label_03C9:
            if (this.card.Disposition == DispositionType.Bottom)
            {
                Location.Current.Deck.Add(this.card, DeckPositionType.Bottom);
            }
            this.card.Disposition = DispositionType.None;
        Label_03FC:
            if (this.card.Disposition == DispositionType.SetAside)
            {
                this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.1f));
                this.$PC = 7;
                goto Label_0562;
            }
        Label_04B1:
            if (this.card.Disposition == DispositionType.None)
            {
                this.card.Show(false);
                if (this.card.IsBlocker())
                {
                    this.card.Show(true);
                }
            }
            if (this.card.Disposition == DispositionType.Destroy)
            {
                Location.Current.Deck.Remove(this.card);
                this.card.Destroy();
            }
            Turn.ClearCheckData();
            Location.Load(Turn.Owner.Location);
            this.<>f__this.HideLocationCard();
            UI.Window.Refresh();
            UI.Window.Pause(false);
            this.<>f__this.Proceed();
            this.$PC = -1;
        Label_0560:
            return false;
        Label_0562:
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
    private sealed class <BanishBoon>c__Iterator33 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
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
                    VisualEffect.ApplyToCard(VisualEffectType.CardBanishToBox, this.card, 3f);
                    UI.Sound.Play(SoundEffectType.BoonFailAcquireBanish);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.32f));
                    this.$PC = 1;
                    goto Label_00AA;

                case 1:
                    Campaign.Box.Add(this.card, false);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1.2f));
                    this.$PC = 2;
                    goto Label_00AA;

                case 2:
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_00AA:
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

