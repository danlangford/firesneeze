using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventDiceRolledSummon : Event
{
    [Tooltip("the deck where the summoned card is put")]
    public DeckType DeckType = DeckType.Location;
    [Tooltip("the monster to be summoned")]
    public SummonsSelector Selector;
    [Tooltip("should the deck be shuffled afterwards?")]
    public bool Shuffle = true;

    [DebuggerHidden]
    private IEnumerator EventUndefeated_Summon(Card card) => 
        new <EventUndefeated_Summon>c__Iterator24 { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }

    public override void OnDiceRolled()
    {
        if (this.IsEventValid(Turn.Card))
        {
            Card card = this.Selector.Draw();
            if (card != null)
            {
                base.StartCoroutine(this.EventUndefeated_Summon(card));
            }
        }
    }

    private Deck Deck
    {
        get
        {
            if (this.DeckType == DeckType.Character)
            {
                return Turn.Character.Deck;
            }
            if (this.DeckType == DeckType.Hand)
            {
                return Turn.Character.Hand;
            }
            return Location.Current.Deck;
        }
    }

    public override EventType Type =>
        EventType.OnDiceRolled;

    [CompilerGenerated]
    private sealed class <EventUndefeated_Summon>c__Iterator24 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal EventDiceRolledSummon <>f__this;
        internal GameObject <vfx>__1;
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
                    UI.Window.Pause(true);
                    this.card.Show(false);
                    this.<vfx>__1 = VisualEffect.ApplyToPlayer(VisualEffectType.CardSummonFromBox, 3f);
                    if (this.<vfx>__1 != null)
                    {
                        this.<vfx>__1.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                        this.<vfx>__1.transform.position = new Vector3(0f, 0f, 0f);
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(2f));
                    this.$PC = 1;
                    goto Label_02F0;

                case 1:
                    this.card.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                    this.card.transform.position = new Vector3(0.3f, 0f, 0f);
                    this.card.Show(CardSideType.Back);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
                    this.$PC = 2;
                    goto Label_02F0;

                case 2:
                    if ((this.<>f__this.DeckType != DeckType.Location) || (this.<window>__0 == null))
                    {
                        break;
                    }
                    this.card.MoveCard(this.<window>__0.layoutExplore.transform.position, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                    LeanTween.scale(this.card.gameObject, this.<window>__0.layoutExplore.Scale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 3;
                    goto Label_02F0;

                case 3:
                    break;

                default:
                    goto Label_02EE;
            }
            this.<>f__this.Deck.Add(this.card);
            if ((this.<>f__this.DeckType != DeckType.Hand) && this.<>f__this.Shuffle)
            {
                VisualEffect.Shuffle(this.<>f__this.DeckType);
                if (this.<>f__this.DeckType == DeckType.Location)
                {
                    this.<>f__this.Deck.ShuffleUnderTop();
                }
                else
                {
                    this.<>f__this.Deck.Shuffle();
                }
            }
            if ((this.<>f__this.DeckType == DeckType.Location) && (this.<window>__0 != null))
            {
                this.<window>__0.locationPanel.RefreshCardList();
                this.<window>__0.layoutLocation.Refresh();
            }
            UI.Window.Pause(false);
            this.$PC = -1;
        Label_02EE:
            return false;
        Label_02F0:
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

