using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventDefeatedExamine : Event
{
    [Tooltip("true will force the \"Acquire\" button to display")]
    public bool Acquire;
    [Tooltip("where acquired cards will go")]
    public DeckType AcquireDestination = DeckType.Hand;
    [Tooltip("text displayed on the examine action button")]
    public StrRefType ActionButtonText;
    [Tooltip("when not null, the player can pick one card to acquire")]
    public CardSelector Choose;
    [Tooltip("text displayed when the \"Choose\" selector did not match")]
    public StrRefType MessageNo;
    [Tooltip("text displayed when the \"Choose\" selector matched something")]
    public StrRefType MessageYes;
    [Tooltip("true means that cards can be dragged from/to the bottom of the deck")]
    public bool ModifyBottom;
    [Tooltip("true means that cards can be dragged from/to the top of the deck")]
    public bool ModifyTop;
    [Tooltip("number of cards to examine")]
    public int Number = 1;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;

    [DebuggerHidden]
    private IEnumerator DisposeAndExamine(Card card) => 
        new <DisposeAndExamine>c__Iterator23 { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    private int GetNumMatches()
    {
        int num = 0;
        if (this.Choose != null)
        {
            int num2 = Mathf.Min(this.Number, this.Deck.Count);
            for (int i = 0; i < num2; i++)
            {
                if (this.Position == DeckPositionType.Top)
                {
                    if (this.Choose.Match(this.Deck[i]))
                    {
                        num++;
                    }
                }
                else if (this.Choose.Match(this.Deck[(this.Deck.Count - 1) - i]))
                {
                    num++;
                }
            }
        }
        return num;
    }

    public override bool IsEventValid(Card card)
    {
        if (this.Deck.Count <= 1)
        {
            return false;
        }
        return base.IsConditionValid(card);
    }

    public override void OnCardDefeated(Card card)
    {
        if (this.IsEventValid(card))
        {
            base.StartCoroutine(this.DisposeAndExamine(card));
        }
        else
        {
            Event.Done();
        }
    }

    private Deck Deck =>
        Location.Current.Deck;

    public override bool Stateless =>
        !this.IsEventValid(Turn.Card);

    public override EventType Type =>
        EventType.OnCardDefeated;

    [CompilerGenerated]
    private sealed class <DisposeAndExamine>c__Iterator23 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal EventDefeatedExamine <>f__this;
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
                    Turn.PushReturnState();
                    Turn.State = GameStateType.Null;
                    if (!this.card.IsBoon() && (this.card.Type != CardType.Barrier))
                    {
                        VisualEffect.ApplyToCard(VisualEffectType.CardDefeatedBanish, Turn.Card, 3f);
                        UI.Sound.Play(SoundEffectType.BaneDefeatBanish);
                        this.card.Animate(AnimationType.Defeated, true);
                        this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
                        this.$PC = 3;
                    }
                    else
                    {
                        VisualEffect.ApplyToCard(VisualEffectType.CardBanishToBox, Turn.Card, 3f);
                        UI.Sound.Play(SoundEffectType.BoonFailAcquireBanish);
                        this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.32f));
                        this.$PC = 1;
                    }
                    goto Label_0316;

                case 1:
                    this.card.Show(false);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.2f));
                    this.$PC = 2;
                    goto Label_0316;

                case 2:
                    break;

                case 3:
                    this.card.Show(false);
                    break;

                default:
                    goto Label_0314;
            }
            if (!this.<>f__this.Acquire)
            {
                Turn.Disposed = true;
            }
            Campaign.Box.Add(this.card, false);
            this.<window>__0 = UI.Window as GuiWindowLocation;
            if (this.<window>__0 != null)
            {
                this.<window>__0.layoutExamine.Mode = ExamineModeType.Reveal;
                this.<window>__0.layoutExamine.Source = DeckType.Location;
                this.<window>__0.layoutExamine.Number = Mathf.Min(this.<>f__this.Number, this.<>f__this.Deck.Count);
                this.<window>__0.layoutExamine.RevealPosition = this.<>f__this.Position;
                this.<window>__0.layoutExamine.ModifyTop = this.<>f__this.ModifyTop;
                this.<window>__0.layoutExamine.ModifyBottom = this.<>f__this.ModifyBottom;
                if (this.<>f__this.Choose != null)
                {
                    this.<window>__0.layoutExamine.Choose = this.<>f__this.Choose.ToFilter();
                }
                if (this.<>f__this.Acquire)
                {
                    this.<window>__0.layoutExamine.Action = ExamineActionType.Acquire;
                }
                this.<window>__0.layoutExamine.AcquireDestination = this.<>f__this.AcquireDestination;
                this.<window>__0.layoutExamine.ActionButtonText = this.<>f__this.ActionButtonText.ToString();
            }
            if (this.<>f__this.GetNumMatches() > 0)
            {
                Turn.SetStateData(new TurnStateData(this.<>f__this.MessageYes));
            }
            else
            {
                Turn.SetStateData(new TurnStateData(this.<>f__this.MessageNo));
            }
            Turn.State = GameStateType.Examine;
            Event.Done();
            this.$PC = -1;
        Label_0314:
            return false;
        Label_0316:
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

