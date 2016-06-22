using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LocationPowerPickMove : LocationPower
{
    [Tooltip("the block to run at the end of this power")]
    public Block FinishBlock;
    [Tooltip("source deck")]
    public DeckType From = DeckType.Discard;
    [Tooltip("max amount of cards to pick and move. If less than zero it's infinite")]
    public int MaxAmount = 1;
    [Tooltip("when picking a card this helper text will guide you")]
    public StrRefType Message;
    [Tooltip("min amount of cards to pick and move.")]
    public int MinAmount = 1;
    [Tooltip("if true, this power only works on closing")]
    public bool OnClosing = true;
    [Tooltip("selects the cards that can be picked")]
    public CardSelector Selector;
    [Tooltip("if true, the destination deck will shuffle after the move finishes")]
    public bool Shuffle = true;
    [Tooltip("destination deck")]
    public DeckType To = DeckType.Character;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushReturnState();
        TurnStateData data = new TurnStateData(this.From.ToActionType(), this.Selector.ToFilter(), this.To.ToActionType(), this.MinAmount, this.MaxAmount);
        if (this.Message.file != "None")
        {
            data.Message = this.Message.ToString();
        }
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerPick_Finish"));
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerPick_Cancel"));
        Turn.State = GameStateType.Pick;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
            window.ShowProceedButton(false);
        }
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (this.Selector == null)
        {
            return false;
        }
        Deck deck = Turn.Character.GetDeck(this.From);
        if (this.Selector.Filter(deck) <= 0)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (this.OnClosing && !Location.Current.ClosedThisTurn)
        {
            return false;
        }
        if (Rules.IsCheck())
        {
            return false;
        }
        return ((((Turn.State == GameStateType.Finish) || (Turn.State == GameStateType.Setup)) || (Turn.State == GameStateType.Switch)) || (Turn.State == GameStateType.ConfirmPowerUse));
    }

    private void LocationPowerPick_Cancel()
    {
        this.PowerEnd();
        Turn.ReturnToReturnState();
    }

    private void LocationPowerPick_Finish()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        base.StartCoroutine(this.LocationPowerPick_Finish_Coroutine());
    }

    [DebuggerHidden]
    private IEnumerator LocationPowerPick_Finish_Coroutine() => 
        new <LocationPowerPick_Finish_Coroutine>c__Iterator95 { <>f__this = this };

    [CompilerGenerated]
    private sealed class <LocationPowerPick_Finish_Coroutine>c__Iterator95 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal LocationPowerPickMove <>f__this;
        internal Deck <deck>__0;

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
                    this.<deck>__0 = Turn.Character.GetDeck(this.<>f__this.To);
                    if (!this.<>f__this.Shuffle)
                    {
                        break;
                    }
                    VisualEffect.Shuffle(this.<>f__this.To);
                    this.<deck>__0.Shuffle();
                    this.$current = new WaitForSeconds(1.7f);
                    this.$PC = 1;
                    return true;

                case 1:
                    break;

                default:
                    goto Label_00B5;
            }
            if (this.<>f__this.FinishBlock != null)
            {
                this.<>f__this.FinishBlock.Invoke();
            }
            Turn.ReturnToReturnState();
            this.$PC = -1;
        Label_00B5:
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

