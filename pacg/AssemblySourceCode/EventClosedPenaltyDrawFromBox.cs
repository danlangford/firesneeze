using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventClosedPenaltyDrawFromBox : Event
{
    public ActionType Action;
    public int ActionAmount;
    public CardSelector DrawSelector;
    public CardSelector PenaltyCardSelector;

    [DebuggerHidden]
    private IEnumerator DoDrawAnimation() => 
        new <DoDrawAnimation>c__Iterator21 { <>f__this = this };

    private void EventClosedPenaltyDraw_Draw()
    {
        base.StartCoroutine(this.DoDrawAnimation());
    }

    public override void OnLocationClosed()
    {
        if (this.ActionAmount > 0)
        {
            Turn.SetStateData(new TurnStateData(this.Action, this.PenaltyCardSelector.ToFilter(), this.ActionAmount));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedPenaltyDraw_Draw"));
            Turn.State = GameStateType.Penalty;
        }
        else
        {
            this.EventClosedPenaltyDraw_Draw();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnLocationClosed;

    [CompilerGenerated]
    private sealed class <DoDrawAnimation>c__Iterator21 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal EventClosedPenaltyDrawFromBox <>f__this;
        internal Card <card>__1;
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
                {
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if (this.<window>__0 == null)
                    {
                        break;
                    }
                    this.<card>__1 = Campaign.Box.Draw(this.<>f__this.DrawSelector);
                    if (this.<card>__1 == null)
                    {
                        break;
                    }
                    Card[] cards = new Card[] { this.<card>__1 };
                    this.<window>__0.DrawCardsFromBox(cards, Turn.Character.Hand, Turn.Number);
                    this.$current = new WaitForSeconds(3.75f);
                    this.$PC = 1;
                    return true;
                }
                case 1:
                    break;

                default:
                    goto Label_00CD;
            }
            Turn.PushStateDestination(GameStateType.Done);
            Turn.State = GameStateType.Recharge;
            Event.Done();
            this.$PC = -1;
        Label_00CD:
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

