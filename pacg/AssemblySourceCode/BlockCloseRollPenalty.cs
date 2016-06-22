using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BlockCloseRollPenalty : Block
{
    [Tooltip("number of penalty cards")]
    public int Amount = 1;
    [Tooltip("dice used to compute the penalty amount")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the penalty (total not per dice)")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("type of penalty to be performed by the player")]
    public ActionType Penalty = ActionType.Discard;
    [Tooltip("if you cannot pay the penalty have the remainder of the penalty applied to the character's deck automatically")]
    public bool RolloverAmountToDeck;

    [DebuggerHidden]
    private IEnumerator AnimateCards() => 
        new <AnimateCards>c__IteratorA { <>f__this = this };

    private void BlockRollPenalty_Done()
    {
        Game.Instance.StartCoroutine(this.AnimateCards());
    }

    private void BlockRollPenalty_Penalty()
    {
        this.Amount = Turn.DiceTotal;
        this.ClearDicePanel();
        Turn.SetStateData(new TurnStateData(this.Penalty, this.Amount));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "BlockRollPenalty_Done"));
        Turn.State = GameStateType.Penalty;
    }

    private void ClearDicePanel()
    {
        Turn.ClearCheckData();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Clear();
        }
    }

    public override void Invoke()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        Turn.RollReason = RollType.EnemyDamage;
        base.RefreshDicePanel();
        Turn.Checks = null;
        Turn.EmptyLayoutDecks = false;
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "BlockRollPenalty_Penalty"));
        Turn.State = GameStateType.Roll;
    }

    private void MoveCard(Card card, ActionType To)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            switch (To)
            {
                case ActionType.Recharge:
                    window.Recharge(card, DeckPositionType.Bottom);
                    break;

                case ActionType.Discard:
                case ActionType.Damage:
                    window.Discard(card);
                    break;

                case ActionType.Bury:
                    window.Bury(card);
                    break;

                case ActionType.Banish:
                    Campaign.Box.Add(card, false);
                    break;
            }
        }
    }

    public override bool Stateless =>
        false;

    [CompilerGenerated]
    private sealed class <AnimateCards>c__IteratorA : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BlockCloseRollPenalty <>f__this;
        internal int <i>__3;
        internal bool <killPlayer>__2;
        internal int <rolloverAmount>__1;
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
                    if (!this.<>f__this.RolloverAmountToDeck || (this.<window>__0 == null))
                    {
                        goto Label_01D0;
                    }
                    this.<rolloverAmount>__1 = this.<>f__this.Amount - this.<window>__0.GetLayoutDeck(this.<>f__this.Penalty).Deck.Count;
                    if (this.<rolloverAmount>__1 <= 0)
                    {
                        goto Label_01D0;
                    }
                    this.<killPlayer>__2 = Turn.Character.Deck.Count < this.<rolloverAmount>__1;
                    this.<rolloverAmount>__1 = Mathf.Min(Turn.Character.Deck.Count, this.<rolloverAmount>__1);
                    this.<i>__3 = 0;
                    break;

                case 1:
                    this.<i>__3++;
                    break;

                case 2:
                    Turn.Character.Alive = false;
                    if (Turn.CloseType != CloseType.Temporary)
                    {
                        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Death_GetNextCharacter"));
                        Turn.PushStateDestination(GameStateType.Death);
                        Turn.State = GameStateType.Closing;
                    }
                    else
                    {
                        Location.Current.Closed = true;
                        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Iterator_Next"));
                        Turn.State = GameStateType.Death;
                    }
                    goto Label_023F;

                default:
                    goto Label_023F;
            }
            if (this.<i>__3 < this.<rolloverAmount>__1)
            {
                this.<>f__this.MoveCard(Turn.Character.Deck[0], this.<>f__this.Penalty);
                this.$current = new WaitForSeconds(0.3f);
                this.$PC = 1;
                goto Label_0241;
            }
            if (this.<killPlayer>__2)
            {
                this.<window>__0.ProcessLayoutDecks();
                this.$current = new WaitForSeconds(0.4f);
                this.$PC = 2;
                goto Label_0241;
            }
        Label_01D0:
            if (this.<window>__0 != null)
            {
                Turn.RollReason = RollType.PlayerControlled;
                Turn.EmptyLayoutDecks = true;
                this.<window>__0.ProcessLayoutDecks();
            }
            if (Turn.CloseType == CloseType.Temporary)
            {
                Location.Current.Closed = true;
                Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Iterator_Next"));
                Turn.State = GameStateType.Recharge;
            }
            else
            {
                Turn.PushStateDestination(GameStateType.Done);
                Turn.State = GameStateType.Closing;
            }
            this.$PC = -1;
        Label_023F:
            return false;
        Label_0241:
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

