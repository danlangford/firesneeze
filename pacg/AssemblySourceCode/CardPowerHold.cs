using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardPowerHold : CardPower
{
    [Tooltip("the duration of this effect")]
    public int Duration = 1;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Character.MarkCardType(card.Type, true);
            UI.Window.Pause(true);
            LeanTween.scale(Turn.Card.gameObject, card.transform.localScale, 0.35f).setEase(LeanTweenType.easeInOutQuad);
            Turn.Card.MoveCard(card.transform.position, 0.35f).setOnComplete(new Action(this.ActivateDone)).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    private void ActivateDone()
    {
        base.StartCoroutine(this.ActivateDoneCoroutine());
    }

    [DebuggerHidden]
    private IEnumerator ActivateDoneCoroutine() => 
        new <ActivateDoneCoroutine>c__Iterator15 { <>f__this = this };

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if ((Turn.State != GameStateType.Combat) && (Turn.State != GameStateType.EvadeOption))
        {
            return false;
        }
        return true;
    }

    public override bool IsPowerDeactivationAllowed(Card card) => 
        false;

    public override PowerType Type =>
        PowerType.Evade;

    [CompilerGenerated]
    private sealed class <ActivateDoneCoroutine>c__Iterator15 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CardPowerHold <>f__this;
        internal EffectHoldCard <effect>__0;
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
                    this.$current = null;
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<effect>__0 = new EffectHoldCard(Turn.Card.ID, this.<>f__this.Duration);
                    Turn.Owner.ApplyEffect(this.<effect>__0);
                    this.<effect>__0.Invoke();
                    this.<window>__1 = UI.Window as GuiWindowLocation;
                    if (this.<window>__1 != null)
                    {
                        this.<window>__1.messagePanel.Clear();
                        this.<window>__1.layoutLocation.ShowPreludeFX(false);
                    }
                    UI.Window.Pause(false);
                    Turn.Disposed = true;
                    Turn.State = GameStateType.Damage;
                    this.$PC = -1;
                    break;
            }
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

