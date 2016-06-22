using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiScriptCloseLocation : GuiScript
{
    private Vector3 BoxPosition = new Vector3(0f, 12f, 0f);

    private bool IsCardBanished(Card card) => 
        ((Location.Current.Keepers == null) || !Location.Current.Keepers.Match(card));

    private bool LocationContainsVillain(Deck deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i].Type == CardType.Villain)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTrayCloseFinished()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Clear();
            window.locationPanel.RefreshCardList();
        }
        Turn.Proceed();
    }

    public override bool Play()
    {
        Game.Instance.StartCoroutine(this.PlayCoroutine());
        return true;
    }

    [DebuggerHidden]
    private IEnumerator PlayCoroutine() => 
        new <PlayCoroutine>c__Iterator81 { <>f__this = this };

    public override void Stop()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            LeanTween.delayedCall(window.layoutExamine.Close(), new Action(this.OnTrayCloseFinished));
        }
    }

    [CompilerGenerated]
    private sealed class <PlayCoroutine>c__Iterator81 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiScriptCloseLocation <>f__this;
        internal float <delay>__6;
        internal int <i>__1;
        internal int <i>__3;
        internal int <i>__5;
        internal bool <locationContainsVillain>__0;
        internal SoundEffectType <sound>__2;
        internal GuiWindowLocation <window>__4;

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
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 1;
                    goto Label_0446;

                case 1:
                    this.<locationContainsVillain>__0 = this.<>f__this.LocationContainsVillain(Location.Current.Deck);
                    this.<i>__1 = Location.Current.Deck.Count - 1;
                    goto Label_01B8;

                case 2:
                    Campaign.Box.Add(Location.Current.Deck[this.<i>__1], false);
                    break;

                case 3:
                    Location.Current.Deck[this.<i>__3].Show(CardSideType.Front);
                    Location.Current.Deck[this.<i>__3].Animate(AnimationType.FlipToFront, true);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
                    this.$PC = 4;
                    goto Label_0446;

                case 4:
                    goto Label_02A3;

                case 5:
                    this.<window>__4.layoutExamine.Mode = ExamineModeType.All;
                    this.<window>__4.layoutExamine.Refresh();
                    this.<delay>__6 = Mathf.Clamp((float) (0.2f * Location.Current.Deck.Count), (float) 0.5f, (float) 1.5f);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.<delay>__6));
                    this.$PC = 6;
                    goto Label_0446;

                case 6:
                    goto Label_0406;

                case 7:
                    this.<>f__this.Stop();
                    this.$PC = -1;
                    goto Label_0444;

                default:
                    goto Label_0444;
            }
        Label_01AA:
            this.<i>__1--;
        Label_01B8:
            if (this.<i>__1 >= 0)
            {
                if ((!Location.Current.Deck[this.<i>__1].Visible || (Location.Current.Deck[this.<i>__1].Type == CardType.Villain)) || (!this.<locationContainsVillain>__0 && !this.<>f__this.IsCardBanished(Location.Current.Deck[this.<i>__1])))
                {
                    goto Label_01AA;
                }
                this.<sound>__2 = SoundEffectType.GenericFlickCard;
                if (this.<i>__1 == 0)
                {
                    this.<sound>__2 = SoundEffectType.FinalCardRemovedLocationClose;
                }
                Location.Current.Deck[this.<i>__1].MoveCard(this.<>f__this.BoxPosition, 0.3f, this.<sound>__2).setEase(LeanTweenType.easeInOutQuad);
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                this.$PC = 2;
                goto Label_0446;
            }
            if (Location.Current.Deck.Count <= 1)
            {
                this.<i>__3 = Location.Current.Deck.Count - 1;
                while (this.<i>__3 >= 0)
                {
                    if (Location.Current.Deck[this.<i>__3].Visible)
                    {
                        this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.1f));
                        this.$PC = 3;
                        goto Label_0446;
                    }
                Label_02A3:
                    this.<i>__3--;
                }
            }
            if (Location.Current.Deck.Count > 1)
            {
                this.<window>__4 = UI.Window as GuiWindowLocation;
                if (this.<window>__4 != null)
                {
                    this.<i>__5 = 0;
                    while (this.<i>__5 < Location.Current.Deck.Count)
                    {
                        Location.Current.Deck[this.<i>__5].MoveCard(this.<window>__4.layoutExamine.transform.position, 0.15f).setEase(LeanTweenType.easeInOutQuad);
                        this.<i>__5++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 5;
                    goto Label_0446;
                }
            }
        Label_0406:
            this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
            this.$PC = 7;
            goto Label_0446;
        Label_0444:
            return false;
        Label_0446:
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

