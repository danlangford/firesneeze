using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardPowerRecoverBlessing : CardPower
{
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            this.RecoverBlessings_Roll();
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowCancelButton(true);
            }
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            Turn.Dice.Clear();
            Turn.DiceBonus = 0;
            this.RefreshDicePanel();
            Turn.GotoCancelDestination();
            Turn.EmptyLayoutDecks = true;
        }
    }

    public override bool IsEqualOrBetter(CardPower x)
    {
        CardPowerRecoverBlessing blessing = x as CardPowerRecoverBlessing;
        if (blessing == null)
        {
            return false;
        }
        return (this.Dice.Length >= blessing.Dice.Length);
    }

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
        if (Scenario.Current.Discard.Count <= 0)
        {
            return false;
        }
        if ((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup))
        {
            return false;
        }
        return true;
    }

    [DebuggerHidden]
    private IEnumerator RecoverBlessings_Animate() => 
        new <RecoverBlessings_Animate>c__Iterator16 { <>f__this = this };

    private void RecoverBlessings_Finish()
    {
        Turn.EmptyLayoutDecks = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessLayoutDecks();
        }
        Game.Instance.StartCoroutine(this.RecoverBlessings_Animate());
    }

    private void RecoverBlessings_Roll()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        this.RefreshDicePanel();
        Turn.EmptyLayoutDecks = false;
        Turn.Checks = null;
        Turn.PushReturnState();
        Turn.PushCancelDestination(Turn.State);
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "RecoverBlessings_Finish"));
        Turn.State = GameStateType.Roll;
    }

    private void RefreshDicePanel()
    {
        Turn.DiceTarget = 0;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    [CompilerGenerated]
    private sealed class <RecoverBlessings_Animate>c__Iterator16 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CardPowerRecoverBlessing <>f__this;
        internal int <c>__2;
        internal Card <card>__10;
        internal Card <card>__5;
        internal Card <card>__8;
        internal Vector3 <deckSize>__6;
        internal int <i>__4;
        internal int <i>__7;
        internal int <i>__9;
        internal int <n>__1;
        internal GameObject <vfx>__3;
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
                    UI.Window.Pause(true);
                    this.<n>__1 = Mathf.Clamp(Turn.DiceTotal, 0, Scenario.Current.Discard.Count);
                    this.<c>__2 = Scenario.Current.Blessings.Count;
                    this.<vfx>__3 = VisualEffect.ApplyToPlayer(VisualEffectType.CardSummonFromBox, 3f);
                    if (this.<vfx>__3 != null)
                    {
                        this.<vfx>__3.transform.localScale = new Vector3(0.6f + (0.06f * this.<n>__1), 0.6f, 1f);
                        this.<vfx>__3.transform.position = new Vector3(0f, 0f, 0f);
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(2f));
                    this.$PC = 1;
                    goto Label_03B5;

                case 1:
                    Scenario.Current.Discard.Shuffle();
                    this.<i>__4 = 0;
                    while (this.<i>__4 < this.<n>__1)
                    {
                        this.<card>__5 = Scenario.Current.Discard[this.<i>__4];
                        this.<card>__5.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                        this.<card>__5.transform.position = new Vector3(0.3f * this.<i>__4, 0f, 0f);
                        this.<card>__5.SortingOrder = this.<i>__4;
                        this.<card>__5.Show(CardSideType.Back);
                        this.<i>__4++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
                    this.$PC = 2;
                    goto Label_03B5;

                case 2:
                    this.<deckSize>__6 = new Vector3(0.07f, 0.07f, 1f);
                    this.<i>__7 = 0;
                    break;

                case 3:
                    this.<card>__8.Show(false);
                    this.<window>__0.blessingsPanel.Increment((this.<c>__2 + this.<i>__7) + 1);
                    this.<i>__7++;
                    break;

                default:
                    goto Label_03B3;
            }
            if (this.<i>__7 < this.<n>__1)
            {
                this.<card>__8 = Scenario.Current.Discard[this.<i>__7];
                LeanTween.scale(this.<card>__8.gameObject, this.<deckSize>__6, 0.5f).setEase(LeanTweenType.easeInOutQuad);
                this.<card>__8.MoveCard(this.<window>__0.blessingsPanel.blessingsButton.transform.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
                this.$PC = 3;
                goto Label_03B5;
            }
            this.<i>__9 = 0;
            while (this.<i>__9 < this.<n>__1)
            {
                this.<card>__10 = Scenario.Current.Discard[0];
                Scenario.Current.Blessings.Add(this.<card>__10);
                this.<i>__9++;
            }
            Scenario.Current.Blessings.Shuffle();
            if (this.<window>__0 != null)
            {
                this.<window>__0.blessingsPanel.Refresh();
            }
            UI.Window.Pause(false);
            Turn.ReturnToReturnState();
            this.$PC = -1;
        Label_03B3:
            return false;
        Label_03B5:
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

