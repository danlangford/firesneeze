using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PowerModifierDraw : PowerModifier
{
    [Tooltip("number of cards to draw")]
    public int Amount = 1;
    [Tooltip("true if the player has the choice of not drawing a card")]
    public bool Optional = true;
    [Tooltip("where the optional prompt will appear")]
    public DeckType PromptPosition = DeckType.Character;

    public override void Activate(int powerIndex)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card component = base.GetComponent<Card>();
            if (this.Optional)
            {
                Turn.PushReturnState();
                if (Turn.State == GameStateType.Damage)
                {
                    Turn.EnqueueDamageData();
                }
                window.Popup.Clear();
                window.Popup.SetDeckPosition(this.PromptPosition);
                window.Popup.Add(StringTableManager.GetUIText(0x12f), new TurnStateCallback(component, "PowerModifierDraw_No"));
                window.Popup.Add(StringTableManager.GetUIText(0x1bb), new TurnStateCallback(component, "PowerModifierDraw_Yes"));
                Turn.EmptyLayoutDecks = false;
                Turn.State = GameStateType.Popup;
            }
            else
            {
                this.PowerModifierDraw_Yes();
            }
        }
    }

    public override void Deactivate()
    {
        if (Turn.State == GameStateType.Popup)
        {
            Turn.ReturnToReturnState();
        }
    }

    [DebuggerHidden]
    private IEnumerator DrawCardsSequence() => 
        new <DrawCardsSequence>c__Iterator99 { <>f__this = this };

    private void PowerModifierDraw_No()
    {
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    private void PowerModifierDraw_Yes()
    {
        base.StartCoroutine(this.DrawCardsSequence());
        Turn.EmptyLayoutDecks = true;
        Turn.ReturnToReturnState();
    }

    [CompilerGenerated]
    private sealed class <DrawCardsSequence>c__Iterator99 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal PowerModifierDraw <>f__this;
        internal Card <card>__2;
        internal int <i>__1;
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
                    if (this.<window>__0 == null)
                    {
                        goto Label_00C0;
                    }
                    this.<i>__1 = 0;
                    break;

                case 1:
                    this.<i>__1++;
                    break;

                default:
                    goto Label_00C7;
            }
            if (this.<i>__1 < this.<>f__this.Amount)
            {
                this.<card>__2 = Turn.Character.Deck.Draw();
                UI.Sound.Play(SoundEffectType.GenericFlickCard);
                this.<window>__0.Draw(this.<card>__2);
                this.$current = new WaitForSeconds(0.2f);
                this.$PC = 1;
                return true;
            }
        Label_00C0:
            this.$PC = -1;
        Label_00C7:
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

