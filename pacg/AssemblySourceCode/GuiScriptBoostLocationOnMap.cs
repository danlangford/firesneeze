using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiScriptBoostLocationOnMap : GuiScript
{
    [Tooltip("called when the script is done")]
    public TurnStateCallback Callback;
    [Tooltip("the card to add to the location")]
    public Card Card;
    [Tooltip("true means the card was drawn from the box")]
    public bool Dispose;
    [Tooltip("the location to boost")]
    public string LocationID;
    [Tooltip("add the card at this deck position")]
    public DeckPositionType Position;
    [Tooltip("should the card be shown face-up or face-down?")]
    public CardSideType Side = CardSideType.Front;

    [DebuggerHidden]
    private IEnumerator DrawFromBox(Card card) => 
        new <DrawFromBox>c__Iterator7F { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    public override bool Play()
    {
        if ((this.Card == null) || string.IsNullOrEmpty(this.LocationID))
        {
            return false;
        }
        Game.Instance.StartCoroutine(this.PlayCoroutine());
        return true;
    }

    [DebuggerHidden]
    private IEnumerator PlayCoroutine() => 
        new <PlayCoroutine>c__Iterator7E { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator Shuffle(ScenarioMapIcon icon, Card card) => 
        new <Shuffle>c__Iterator80 { 
            icon = icon,
            <$>icon = icon,
            <>f__this = this
        };

    public override void Stop()
    {
        if (this.Dispose && (this.Card != null))
        {
            UnityEngine.Object.Destroy(this.Card.gameObject);
        }
        if (this.Callback != null)
        {
            this.Callback.Invoke();
        }
    }

    [CompilerGenerated]
    private sealed class <DrawFromBox>c__Iterator7F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal GuiScriptBoostLocationOnMap <>f__this;
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
                    this.card.transform.position = this.<window>__0.boxPanel.transform.position;
                    this.card.transform.localScale = this.<window>__0.layoutHand.Scale;
                    this.card.Show(this.<>f__this.Side);
                    this.<window>__0.boxPanel.Show(true);
                    this.<window>__0.boxPanel.PlayAnimation("AddCard", true);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.4f));
                    this.$PC = 1;
                    goto Label_01B4;

                case 1:
                    this.card.MoveCard(this.<window>__0.layoutLocation.transform.position, 0.85f).setEase(LeanTweenType.easeOutBack);
                    LeanTween.scale(this.card.gameObject, this.<window>__0.layoutLocation.Scale, 0.85f).setEase(LeanTweenType.easeOutBack);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.85f));
                    this.$PC = 2;
                    goto Label_01B4;

                case 2:
                    this.<window>__0.boxPanel.Show(false);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
                    this.$PC = 3;
                    goto Label_01B4;

                case 3:
                    this.$PC = -1;
                    break;
            }
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
    private sealed class <PlayCoroutine>c__Iterator7E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiScriptBoostLocationOnMap <>f__this;
        internal GameObject <flash>__1;
        internal int <i>__3;
        internal ScenarioMapIcon <icon>__2;
        internal Vector3 <mapSize>__4;
        internal Vector3 <upSize>__5;
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
                    this.<flash>__1 = null;
                    this.<icon>__2 = null;
                    this.<i>__3 = 0;
                    while (this.<i>__3 < this.<window>__0.mapPanel.Icons.Count)
                    {
                        if (this.<window>__0.mapPanel.Icons[this.<i>__3].ID == this.<>f__this.LocationID)
                        {
                            this.<icon>__2 = this.<window>__0.mapPanel.Icons[this.<i>__3];
                            break;
                        }
                        this.<i>__3++;
                    }
                    break;

                case 1:
                    if (!this.<>f__this.Dispose)
                    {
                        goto Label_0215;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.DrawFromBox(this.<>f__this.Card));
                    this.$PC = 2;
                    goto Label_052E;

                case 2:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 3;
                    goto Label_052E;

                case 3:
                    goto Label_0215;

                case 4:
                    this.<flash>__1 = this.<icon>__2.Flash();
                    UI.Sound.Play(SoundEffectType.CardDroppedIntoLocationMap);
                    goto Label_02DD;

                case 5:
                    LeanTween.scale(this.<>f__this.Card.gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeOutQuad);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
                    this.$PC = 6;
                    goto Label_052E;

                case 6:
                    if ((this.<>f__this.Position != DeckPositionType.Shuffle) || (this.<icon>__2 == null))
                    {
                        goto Label_045E;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.Shuffle(this.<icon>__2, this.<>f__this.Card));
                    this.$PC = 7;
                    goto Label_052E;

                case 7:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
                    this.$PC = 8;
                    goto Label_052E;

                case 8:
                    goto Label_045E;

                case 9:
                    this.<window>__0.ShowMap(false);
                    if (this.<>f__this.Card.GUID != Turn.Card.GUID)
                    {
                        Turn.Card.Show(true);
                    }
                    Location.Distribute(this.<>f__this.LocationID, this.<>f__this.Card, this.<>f__this.Position, true);
                    this.<>f__this.Card.Show(false);
                    if (this.<flash>__1 != null)
                    {
                        UnityEngine.Object.Destroy(this.<flash>__1);
                    }
                    this.<>f__this.Stop();
                    this.$PC = -1;
                    goto Label_052C;

                default:
                    goto Label_052C;
            }
            if (this.<>f__this.Card.GUID != Turn.Card.GUID)
            {
                Turn.Card.Show(false);
            }
            this.<window>__0.layoutLocation.GlowText(false);
            this.<window>__0.ShowMap(true);
            this.<window>__0.mapPanel.CenterAllIcons(Location.Current.ID);
            this.<window>__0.mapPanel.RefreshIconLines();
            this.<window>__0.mapPanel.Pause(true);
            this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
            this.$PC = 1;
            goto Label_052E;
        Label_0215:
            if (this.<icon>__2 != null)
            {
                this.<mapSize>__4 = new Vector3(0.15f, 0.15f, 1f);
                LeanTween.scale(this.<>f__this.Card.gameObject, this.<mapSize>__4, 0.75f).setEase(LeanTweenType.easeInOutQuad);
                this.<>f__this.Card.MoveCard(this.<icon>__2.transform.position, 0.75f).setEase(LeanTweenType.easeInOutBack);
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.75f));
                this.$PC = 4;
                goto Label_052E;
            }
        Label_02DD:
            this.<upSize>__5 = new Vector3(1.25f * this.<>f__this.Card.transform.localScale.x, 1.25f * this.<>f__this.Card.transform.localScale.y, 1f);
            LeanTween.scale(this.<>f__this.Card.gameObject, this.<upSize>__5, 0.25f).setEase(LeanTweenType.easeOutQuad);
            this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.25f));
            this.$PC = 5;
            goto Label_052E;
        Label_045E:
            this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
            this.$PC = 9;
            goto Label_052E;
        Label_052C:
            return false;
        Label_052E:
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
    private sealed class <Shuffle>c__Iterator80 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal ScenarioMapIcon <$>icon;
        internal GuiScriptBoostLocationOnMap <>f__this;
        internal Vector3 <mapSize>__1;
        internal float <shuffleTime>__2;
        internal GuiWindowLocation <window>__0;
        internal ScenarioMapIcon icon;

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
                    this.<mapSize>__1 = new Vector3(0.15f, 0.15f, 1f);
                    this.<window>__0.shufflePanel.transform.position = this.icon.transform.position;
                    this.<window>__0.shufflePanel.transform.localScale = this.<mapSize>__1;
                    this.<window>__0.shufflePanel.Show(true);
                    this.<window>__0.shufflePanel.SortingOrder = 100;
                    this.<shuffleTime>__2 = this.<window>__0.shufflePanel.Shuffle(5);
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 1;
                    goto Label_012C;

                case 1:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.<shuffleTime>__2));
                    this.$PC = 2;
                    goto Label_012C;

                case 2:
                    this.<window>__0.shufflePanel.Show(false);
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_012C:
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

