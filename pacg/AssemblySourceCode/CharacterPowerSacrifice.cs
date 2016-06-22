using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterPowerSacrifice : CharacterPower
{
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("cards matching this filter will be recharged otherwise discarded")]
    public CardSelector RechargeSelector;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushReturnState();
        Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerSacrifice_Cancel"));
        Turn.SetStateData(new TurnStateData(ActionType.Discard, 1));
        Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerSacrifice_Route"));
        Turn.EmptyLayoutDecks = false;
        Turn.State = GameStateType.Sacrifice;
    }

    private void AddDice()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    [DebuggerHidden]
    private IEnumerator CharacterPowerSacrifice_Animate() => 
        new <CharacterPowerSacrifice_Animate>c__Iterator19 { <>f__this = this };

    private void CharacterPowerSacrifice_Cancel()
    {
        Turn.EmptyLayoutDecks = false;
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
        this.PowerEnd();
    }

    private void CharacterPowerSacrifice_Finish()
    {
        Turn.MarkPowerActive(this, true);
        if (Turn.ReturnState == GameStateType.Recharge)
        {
            Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", true);
        }
        if (Turn.ReturnState == GameStateType.Close)
        {
            Turn.BlackBoard.Set<int>("CloseLocationMenuChoice", 2);
        }
        this.AddDice();
        Turn.EmptyLayoutDecks = false;
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Refresh();
        }
        this.PowerEnd();
    }

    private void CharacterPowerSacrifice_Route()
    {
        Game.Instance.StartCoroutine(this.CharacterPowerSacrifice_Animate());
    }

    public override void Initialize(Character self)
    {
        for (int i = 0; i < self.Powers.Count; i++)
        {
            if (self.Powers[i].Modifies(base.ID))
            {
                CharacterPowerModifier modifier = self.Powers[i] as CharacterPowerModifier;
                if (modifier != null)
                {
                    CardType[] cardTypes = modifier.GetCardTypes();
                    if (cardTypes != null)
                    {
                        this.RechargeSelector.CardTypes = this.RechargeSelector.CardTypes.Union<CardType>(cardTypes).ToArray<CardType>();
                    }
                }
            }
        }
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.Character.Deck.Count <= 0)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!Rules.IsCheck())
        {
            return false;
        }
        if (Turn.IsResolved())
        {
            return false;
        }
        return true;
    }

    [CompilerGenerated]
    private sealed class <CharacterPowerSacrifice_Animate>c__Iterator19 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CharacterPowerSacrifice <>f__this;
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
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if ((this.<window>__0 == null) || (this.<window>__0.layoutDiscard.Deck.Count <= 0))
                    {
                        break;
                    }
                    this.<card>__1 = this.<window>__0.layoutDiscard.Deck[this.<window>__0.layoutDiscard.Deck.Count - 1];
                    this.<card>__1.Show(CardSideType.Front);
                    this.<card>__1.SortingOrder = 20;
                    LeanTween.cancel(this.<card>__1.gameObject);
                    LeanTween.scale(this.<card>__1.gameObject, new Vector3(0.3f, 0.3f, 1f), 0.25f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
                    this.$PC = 1;
                    goto Label_01CD;

                case 1:
                    if ((this.<>f__this.RechargeSelector == null) || !this.<>f__this.RechargeSelector.Match(this.<card>__1))
                    {
                        this.<window>__0.Discard(this.<card>__1);
                        break;
                    }
                    UI.Sound.Play(SoundEffectType.CardRechargeSuccess);
                    VisualEffect.ApplyToCard(VisualEffectType.CardWinBoon, this.<card>__1, 1.8f);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(2f));
                    this.$PC = 2;
                    goto Label_01CD;

                case 2:
                    this.<window>__0.Recharge(this.<card>__1);
                    break;

                default:
                    goto Label_01CB;
            }
            this.<>f__this.CharacterPowerSacrifice_Finish();
            this.$PC = -1;
        Label_01CB:
            return false;
        Label_01CD:
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

