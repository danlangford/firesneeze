using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelHeal : GuiPanel
{
    private bool healing;
    [Tooltip("reference to the healing animation in this panel")]
    public Animator HealingAnimation;
    private List<HealInfo> healQueue = new List<HealInfo>(6);

    public void Heal(Character character, Card[] cards, DeckPositionType position)
    {
        HealInfo item = new HealInfo {
            Character = character,
            Cards = new List<Card>(cards.Length)
        };
        item.Cards.AddRange(cards);
        item.Position = position;
        this.healQueue.Add(item);
        base.StartCoroutine(this.TriggerHeal());
    }

    private void OnAnimationFinished()
    {
        UI.Window.Pause(false);
        if (((this.CurrentHeal.Character != null) && (Party.Characters[Turn.Number].ID == this.CurrentHeal.Character.ID)) && (this.CurrentHeal.Position == DeckPositionType.Shuffle))
        {
            VisualEffect.Shuffle(0.1f, DeckType.Character);
        }
        this.healing = false;
        this.healQueue.RemoveAt(0);
        if (this.healQueue.Count > 0)
        {
            base.StartCoroutine(this.TriggerHeal());
        }
        else
        {
            Turn.SwitchCharacter(Turn.Current);
        }
    }

    public void OnAnimationTriggerHeal()
    {
        if (((this.CurrentHeal.Cards != null) && (this.CurrentHeal.Character != null)) && (this.CurrentHeal.Cards.Count > 0))
        {
            Card card = this.CurrentHeal.Cards[0];
            this.CurrentHeal.Character.Deck.Add(card, this.CurrentHeal.Position);
            card.PreviousDeck = null;
            this.CurrentHeal.Cards.RemoveAt(0);
            UI.Sound.Play(SoundEffectType.GenericCardLand);
            if (this.CurrentHeal.Cards.Count <= 0)
            {
                this.OnAnimationFinished();
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator TriggerHeal() => 
        new <TriggerHeal>c__Iterator5E { <>f__this = this };

    private HealInfo CurrentHeal =>
        this.healQueue[0];

    [CompilerGenerated]
    private sealed class <TriggerHeal>c__Iterator5E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        private static Comparison<GuiPanelHeal.HealInfo> <>f__am$cache4;
        internal GuiPanelHeal <>f__this;
        internal int <i>__0;

        private static int <>m__10E(GuiPanelHeal.HealInfo x, GuiPanelHeal.HealInfo y) => 
            -x.Cards.Count.CompareTo(y.Cards.Count);

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
                    if (this.<>f__this.healing)
                    {
                        break;
                    }
                    this.<>f__this.healing = true;
                    UI.Window.Pause(true);
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    goto Label_013A;

                case 1:
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = new Comparison<GuiPanelHeal.HealInfo>(GuiPanelHeal.<TriggerHeal>c__Iterator5E.<>m__10E);
                    }
                    this.<>f__this.healQueue.Sort(<>f__am$cache4);
                    this.<>f__this.HealingAnimation.SetInteger("HealAmount", this.<>f__this.CurrentHeal.Cards.Count);
                    this.<>f__this.HealingAnimation.SetTrigger("Show");
                    this.<i>__0 = 0;
                    while (this.<i>__0 < this.<>f__this.CurrentHeal.Cards.Count)
                    {
                        UI.Sound.Play(SoundEffectType.GenericFlickCard);
                        this.$current = new WaitForSeconds(0.25f);
                        this.$PC = 2;
                        goto Label_013A;
                    Label_0103:
                        this.<i>__0++;
                    }
                    break;

                case 2:
                    goto Label_0103;

                default:
                    goto Label_0138;
            }
            this.$PC = -1;
        Label_0138:
            return false;
        Label_013A:
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

    private class HealInfo
    {
        [Tooltip("the cards that will be given to the character")]
        public List<Card> Cards;
        [Tooltip("the character being healed")]
        public Character Character;
        [Tooltip("the position where the cards will be added")]
        public DeckPositionType Position;
    }
}

