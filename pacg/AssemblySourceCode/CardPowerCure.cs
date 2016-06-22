using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardPowerCure : CardPower
{
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("final destination the card should go to after finishing the cure action")]
    public ActionType FinalDestination;
    [Tooltip("if you use this to heal somebody that is not you heal yourself an equal amount")]
    public bool HealSelfAgain;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("where should the new cards go in the deck?")]
    public DeckPositionType Position = DeckPositionType.Shuffle;
    [Tooltip("who can be targeted by this power")]
    public TargetType Range = TargetType.AllAtLocation;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.PushReturnState();
            if ((Rules.GetCharactersWithDiscardCount(Turn.Character.Location) > 1) && Rules.IsTargetRequired(this.Range))
            {
                this.SpellCure_Target(card);
            }
            else
            {
                Turn.Target = this.CharacterWithDiscard(Turn.Character.Location);
                if (Turn.Target >= 0)
                {
                    this.SpellCure_Roll();
                }
            }
        }
    }

    private int CharacterWithDiscard(string locID)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Alive && (Party.Characters[i].Discard.Count > 0))
            {
                switch (this.Range)
                {
                    case TargetType.None:
                        return Turn.Number;

                    case TargetType.All:
                        return i;

                    case TargetType.AllAtLocation:
                        if (Party.Characters[i].Location != locID)
                        {
                            break;
                        }
                        return i;

                    case TargetType.Another:
                        if (i == Turn.Number)
                        {
                            break;
                        }
                        return i;

                    case TargetType.AnotherAtLocation:
                        if ((Party.Characters[i].Location != locID) || (i == Turn.Number))
                        {
                            break;
                        }
                        return i;
                }
            }
        }
        return -1;
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            Turn.Dice.Clear();
            Turn.DiceBonus = 0;
            this.RefreshDicePanel();
            Turn.PopStateDestination();
            Turn.PopCancelDestination();
            Turn.EmptyLayoutDecks = false;
            Turn.ReturnToReturnState();
            Turn.EmptyLayoutDecks = true;
        }
    }

    private bool DiscardedCardsAtLocation(string locID, TargetType range)
    {
        if (range == TargetType.None)
        {
            return ((Turn.Character.Discard.Count > 0) && Turn.Character.Alive);
        }
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Alive)
            {
                switch (range)
                {
                    case TargetType.AllAtLocation:
                        if ((Party.Characters[i].Location != locID) || (Party.Characters[i].Discard.Count <= 0))
                        {
                            break;
                        }
                        return true;

                    case TargetType.Another:
                        if ((Party.Characters[i].Discard.Count <= 0) || (Party.Characters[i].ID == Turn.Character.ID))
                        {
                            break;
                        }
                        return true;

                    case TargetType.AnotherAtLocation:
                        if (((Party.Characters[i].Discard.Count > 0) && (Party.Characters[i].ID != Turn.Character.ID)) && (Party.Characters[i].Location == locID))
                        {
                            return true;
                        }
                        break;
                }
            }
        }
        return false;
    }

    private bool IsCardRechargable(Character character, Card card)
    {
        for (int i = 0; i < card.Recharge.Length; i++)
        {
            if (character.GetSkillRank(card.Recharge[i].skill) > 0)
            {
                return true;
            }
        }
        return false;
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if (!this.DiscardedCardsAtLocation(Turn.Character.Location, this.Range))
        {
            return false;
        }
        if ((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup))
        {
            return false;
        }
        return true;
    }

    public override bool IsValidationRequired() => 
        false;

    private void RefreshDicePanel()
    {
        Turn.DiceTarget = 0;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    private void SpellCure_Cancel()
    {
        base.Card.ActionDeactivate(true);
        Turn.EmptyLayoutDecks = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutHand.Refresh();
        }
    }

    private void SpellCure_Finish()
    {
        base.StartCoroutine(this.SpellCure_Finish_Coroutine());
    }

    [DebuggerHidden]
    private IEnumerator SpellCure_Finish_Coroutine() => 
        new <SpellCure_Finish_Coroutine>c__Iterator13 { <>f__this = this };

    private void SpellCure_Roll()
    {
        Turn.SwitchCharacter(Turn.Target);
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        this.RefreshDicePanel();
        Turn.Checks = null;
        Turn.EmptyLayoutDecks = false;
        Turn.PushCancelDestination(new TurnStateCallback(base.Card, "SpellCure_Cancel"));
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "SpellCure_Finish"));
        Turn.State = GameStateType.Roll;
    }

    private void SpellCure_Target(Card card)
    {
        Turn.TargetType = this.Range;
        Turn.EmptyLayoutDecks = false;
        Turn.PushCancelDestination(new TurnStateCallback(base.Card, "SpellCure_Cancel"));
        Turn.PushStateDestination(new TurnStateCallback(card, "SpellCure_Roll"));
        GameStateTarget.DisplayText = card.DisplayName;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Discard.Count <= 0)
            {
                Party.Characters[i].Active = ActiveType.Inactive;
            }
        }
        Turn.State = GameStateType.Target;
    }

    public override ActionType RechargeAction
    {
        get
        {
            if (this.FinalDestination != ActionType.None)
            {
                return this.FinalDestination;
            }
            return base.RechargeAction;
        }
    }

    [CompilerGenerated]
    private sealed class <SpellCure_Finish_Coroutine>c__Iterator13 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CardPowerCure <>f__this;
        internal Card[] <cards>__3;
        internal int <i>__4;
        internal int <i>__5;
        internal Character <member>__0;
        internal int <n>__2;
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
                    this.<member>__0 = Party.Characters[Turn.Target];
                    this.<window>__1 = UI.Window as GuiWindowLocation;
                    if (this.<window>__1 == null)
                    {
                        goto Label_032C;
                    }
                    this.<member>__0.Discard.Shuffle();
                    this.<n>__2 = Mathf.Clamp(Turn.DiceTotal, 0, this.<member>__0.Discard.Count);
                    if (Turn.Number != Turn.Target)
                    {
                        this.<i>__5 = this.<n>__2 - 1;
                        while (this.<i>__5 >= 0)
                        {
                            this.<member>__0.Deck.Add(this.<member>__0.Discard[this.<i>__5], this.<>f__this.Position);
                            this.<i>__5--;
                        }
                        break;
                    }
                    this.<cards>__3 = new Card[this.<n>__2];
                    this.<i>__4 = this.<n>__2 - 1;
                    while (this.<i>__4 >= 0)
                    {
                        this.<cards>__3[this.<i>__4] = this.<member>__0.Discard[this.<i>__4];
                        this.<i>__4--;
                    }
                    this.<window>__1.Heal(this.<member>__0, this.<cards>__3, this.<>f__this.Position);
                    break;

                case 1:
                    this.<window>__1.GetLayoutDeck(this.<>f__this.FinalDestination).Deck.Add(this.<>f__this.Card);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.7f));
                    this.$PC = 2;
                    goto Label_03A8;

                case 2:
                    goto Label_032C;

                default:
                    goto Label_03A6;
            }
            if ((this.<>f__this.HealSelfAgain && (this.<>f__this.Card.PlayedOwner != this.<member>__0.ID)) && (Party.Characters[this.<>f__this.Card.PlayedOwner].Discard.Count > 0))
            {
                this.<>f__this.Card.Locked = true;
                Turn.Target = Party.IndexOf(this.<>f__this.Card.PlayedOwner);
                this.<>f__this.SpellCure_Roll();
                Turn.PopCancelDestination();
                goto Label_03A6;
            }
            if (this.<>f__this.FinalDestination != ActionType.None)
            {
                if (!this.<>f__this.IsCardRechargable(Party.Find(this.<>f__this.Card.PlayedOwner), this.<>f__this.Card))
                {
                    VisualEffect.ApplyToCard(VisualEffectType.CardBanishFromDisplay, this.<>f__this.Card, 2.1f);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 1;
                    goto Label_03A8;
                }
                this.<window>__1.GetLayoutDeck(this.<>f__this.FinalDestination).Deck.Add(this.<>f__this.Card);
            }
        Label_032C:
            if (Rules.IsCardRechargable(Party.Find(this.<>f__this.Card.PlayedOwner), this.<>f__this.Card))
            {
                Turn.SwitchCharacter(Turn.InitialCharacter);
            }
            if (this.<>f__this.Dice.Length == 0)
            {
                Turn.DiceBonus -= this.<>f__this.DiceBonus;
            }
            Turn.EmptyLayoutDecks = true;
            Turn.PushStateDestination(Turn.PopReturnState());
            Turn.State = GameStateType.Recharge;
            this.$PC = -1;
        Label_03A6:
            return false;
        Label_03A8:
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

