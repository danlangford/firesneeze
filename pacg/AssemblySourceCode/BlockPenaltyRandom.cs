using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BlockPenaltyRandom : Block
{
    [Tooltip("penalty amount to pay")]
    public int Amount = 1;
    [Tooltip("location the characters must randomly pay from. The deck will get shuffled.")]
    public DeckType From = DeckType.Hand;
    [Tooltip("force each character to pay this penalty")]
    public ActionType Penalty = ActionType.Recharge;
    [Tooltip("by default this power only works on the location")]
    public DamageTargetType Range = DamageTargetType.Location;

    public override void Invoke()
    {
        base.StartCoroutine(this.ShowPayment());
    }

    [DebuggerHidden]
    private IEnumerator PayPenalty(Character character) => 
        new <PayPenalty>c__IteratorE { 
            character = character,
            <$>character = character,
            <>f__this = this
        };

    [DebuggerHidden]
    private IEnumerator ShowPayment() => 
        new <ShowPayment>c__IteratorD { <>f__this = this };

    public override float Length
    {
        get
        {
            int num = 0;
            switch (this.Range)
            {
                case DamageTargetType.Player:
                    num = 1;
                    break;

                case DamageTargetType.Location:
                    num = Location.CountCharactersAtLocation(Location.Current.ID);
                    break;

                case DamageTargetType.Party:
                    num = Party.CountLivingMembers();
                    break;
            }
            return ((num * 1.5f) * this.Amount);
        }
    }

    public override bool Stateless =>
        false;

    [CompilerGenerated]
    private sealed class <PayPenalty>c__IteratorE : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Character <$>character;
        internal BlockPenaltyRandom <>f__this;
        internal Deck <deck>__1;
        internal int <i>__2;
        internal GuiWindowLocation <window>__0;
        internal Character character;

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
                        goto Label_00EA;
                    }
                    this.<deck>__1 = this.character.GetDeck(this.<>f__this.From);
                    this.<deck>__1.Shuffle();
                    this.<i>__2 = 0;
                    break;

                case 1:
                    this.<i>__2++;
                    break;

                default:
                    goto Label_00F1;
            }
            if (this.<i>__2 < this.<>f__this.Amount)
            {
                if (this.<>f__this.Penalty == ActionType.Recharge)
                {
                    this.<window>__0.Recharge(this.<deck>__1[this.<deck>__1.Count - 1], DeckPositionType.Bottom);
                }
                this.$current = new WaitForSeconds(2f);
                this.$PC = 1;
                return true;
            }
        Label_00EA:
            this.$PC = -1;
        Label_00F1:
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

    [CompilerGenerated]
    private sealed class <ShowPayment>c__IteratorD : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BlockPenaltyRandom <>f__this;
        internal int <i>__1;
        internal int <i>__2;
        internal string <loc>__0;

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
                    UI.Window.Pause(true);
                    switch (this.<>f__this.Range)
                    {
                        case DamageTargetType.Player:
                            Turn.Number = Turn.Current;
                            this.$current = this.<>f__this.StartCoroutine(this.<>f__this.PayPenalty(Turn.Owner));
                            this.$PC = 1;
                            goto Label_0228;

                        case DamageTargetType.Location:
                            this.<loc>__0 = Location.Current.ID;
                            this.<i>__1 = 0;
                            while (this.<i>__1 < Party.Characters.Count)
                            {
                                if (Party.Characters[this.<i>__1].Alive && (this.<loc>__0 == Party.Characters[this.<i>__1].Location))
                                {
                                    Turn.Number = this.<i>__1;
                                    UI.Window.Refresh();
                                    this.$current = this.<>f__this.StartCoroutine(this.<>f__this.PayPenalty(Party.Characters[this.<i>__1]));
                                    this.$PC = 2;
                                    goto Label_0228;
                                }
                            Label_013D:
                                this.<i>__1++;
                            }
                            goto Label_0200;

                        case DamageTargetType.Party:
                            this.<i>__2 = 0;
                            while (this.<i>__2 < Party.Characters.Count)
                            {
                                if (Party.Characters[this.<i>__2].Alive)
                                {
                                    Turn.Number = this.<i>__2;
                                    UI.Window.Refresh();
                                    this.$current = this.<>f__this.StartCoroutine(this.<>f__this.PayPenalty(Party.Characters[this.<i>__2]));
                                    this.$PC = 3;
                                    goto Label_0228;
                                }
                            Label_01D8:
                                this.<i>__2++;
                            }
                            goto Label_0200;
                    }
                    break;

                case 1:
                    break;

                case 2:
                    goto Label_013D;

                case 3:
                    goto Label_01D8;

                default:
                    goto Label_0226;
            }
        Label_0200:
            Turn.Number = Turn.Current;
            UI.Window.Refresh();
            UI.Window.Pause(false);
            this.$PC = -1;
        Label_0226:
            return false;
        Label_0228:
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

