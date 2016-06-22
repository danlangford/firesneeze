using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventDefeatedChest : Event
{
    [Tooltip("should the player be able to choose the card type? Still follow the rules of the selector.")]
    public bool Choose;
    [Tooltip("dice used to determine number of cards")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the dice (total not per dice)")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("what cards should be choosen from the box")]
    public CardSelector Selector;

    [DebuggerHidden]
    private IEnumerator DoDrawAnimation() => 
        new <DoDrawAnimation>c__Iterator22 { <>f__this = this };

    private void EventDefeatedChest_Add()
    {
        base.StartCoroutine(this.DoDrawAnimation());
    }

    private void EventDefeatedChest_Roll()
    {
        Turn.Checks = null;
        Turn.Dice.Clear();
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus = this.DiceBonus;
        Turn.DiceTarget = 0;
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventDefeatedChest_Add"));
        Turn.State = GameStateType.Roll;
    }

    public override void OnCardDefeated(Card card)
    {
        if (this.Choose)
        {
            Turn.EmptyLayoutDecks = false;
            Turn.PushStateDestination(new TurnStateCallback(card, "EventDefeatedChest_Roll"));
            Turn.SetStateData(new TurnStateData(ActionType.None, this.Selector.ToFilter(), 0));
            Turn.State = GameStateType.SelectType;
        }
        else
        {
            this.EventDefeatedChest_Roll();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardDefeated;

    [CompilerGenerated]
    private sealed class <DoDrawAnimation>c__Iterator22 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal EventDefeatedChest <>f__this;
        internal Card[] <cards>__1;
        internal int <i>__2;
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
                    if ((this.<window>__0 == null) || (this.<>f__this.Selector == null))
                    {
                        break;
                    }
                    this.<cards>__1 = new Card[Turn.DiceTotal];
                    this.<i>__2 = 0;
                    while (this.<i>__2 < Turn.DiceTotal)
                    {
                        if (this.<>f__this.Choose)
                        {
                            this.<cards>__1[this.<i>__2] = Campaign.Box.Draw(this.<window>__0.chooseTypePanel.SelectedCardType);
                        }
                        else
                        {
                            this.<cards>__1[this.<i>__2] = Campaign.Box.Draw(this.<>f__this.Selector);
                        }
                        this.<i>__2++;
                    }
                    this.<window>__0.DrawCardsFromBox(this.<cards>__1, Turn.Owner.Hand, Turn.Current);
                    this.$current = new WaitForSeconds(3.75f);
                    this.$PC = 1;
                    return true;

                case 1:
                    break;

                default:
                    goto Label_014A;
            }
            Turn.Card.Disposition = DispositionType.Banish;
            Turn.State = GameStateType.Dispose;
            Event.Done();
            this.$PC = -1;
        Label_014A:
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

