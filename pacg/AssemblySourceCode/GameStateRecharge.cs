using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStateRecharge : GameState
{
    private static void ClearPlayerChoice()
    {
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_No", false);
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", false);
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_" + ActionType.Shuffle.ToString(), false);
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_" + ActionType.Top.ToString(), false);
    }

    private void EmptyRechargeDeck()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Game.GameType == GameType.LocalSinglePlayer) || (Party.Characters[i].ID == Turn.Owner.ID))
            {
                for (int j = 0; j < Party.Characters[i].Recharge.Count; j++)
                {
                    Card card = Party.Characters[i].Recharge[j];
                    if (!Rules.IsRechargePossible(Party.Characters[i], card))
                    {
                        Party.Characters[i].MarkCardType(card.Type, false);
                        VisualEffect.ApplyToCard(VisualEffectType.CardBanishFromDiscard, card, 3f);
                        UI.Sound.Play(SoundEffectType.BoonFailAcquireBanish);
                        Campaign.Box.Add(card, false);
                    }
                    else
                    {
                        card.Clear();
                        Party.Characters[i].Discard.Add(card);
                    }
                }
            }
        }
    }

    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (!Turn.CheckBoard.Get<bool>("GameStateRecharge_KeepLayout"))
            {
                Turn.EmptyLayoutDecks = true;
            }
            base.Enter();
            if (Rules.IsRechargeNecessary())
            {
                if (Turn.Check != SkillCheckType.None)
                {
                    window.dicePanel.Show(true);
                    window.dicePanel.SetCheck(null, Turn.Checks, Turn.Check);
                }
                window.layoutLocation.ShowPreludeFX(false);
                if (!Turn.Iterators.IsRunning(TurnStateIteratorType.Recharge))
                {
                    Turn.Iterators.Start(TurnStateIteratorType.Recharge);
                }
                if (!Turn.CheckBoard.Get<bool>("GameStateRecharge_KeepLayout"))
                {
                    Turn.Iterators.Invoke();
                }
                Tutorial.Notify(TutorialEventType.ScreenRechargeShown);
            }
            else
            {
                this.Proceed();
            }
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (((window != null) && (Turn.ReturnState != GameStateType.Recharge)) && (nextState != GameStateType.Roll))
        {
            Turn.Dice.Clear();
            window.dicePanel.Clear();
        }
        if ((nextState == GameStateType.Sacrifice) || (nextState == GameStateType.Power))
        {
            Turn.CheckBoard.Set<bool>("GameStateRecharge_KeepLayout", true);
        }
    }

    private static DeckPositionType GetRechargePosition()
    {
        if (!Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_Yes"))
        {
            if (Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_" + ActionType.Top.ToString()))
            {
                return DeckPositionType.Top;
            }
            if (Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_" + ActionType.Shuffle.ToString()))
            {
                return DeckPositionType.Shuffle;
            }
        }
        return DeckPositionType.Bottom;
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    public override bool IsResolveSuccess() => 
        (Turn.DiceTotal >= Turn.DiceTarget);

    public override void Proceed()
    {
        if (Turn.Defeat && (Turn.Owner.Recharge.Count > 0))
        {
            Turn.Defeat = false;
            base.SaveRechargableCards();
            base.ProcessLayoutDecks();
            Card card = Turn.Owner.Recharge[0];
            StartRechargeSuccess(card);
            Turn.ClearCheckData();
        }
        else
        {
            base.ProcessLayoutDecks();
            this.EmptyRechargeDeck();
            if (Turn.State == GameStateType.Recharge)
            {
                Turn.GotoStateDestination();
            }
        }
    }

    [DebuggerHidden]
    private static IEnumerator RechargeFailure(Card card) => 
        new <RechargeFailure>c__Iterator3F { 
            card = card,
            <$>card = card
        };

    [DebuggerHidden]
    private static IEnumerator RechargeSuccessful(Card card) => 
        new <RechargeSuccessful>c__Iterator3E { 
            card = card,
            <$>card = card
        };

    public override void Resolve()
    {
        base.Resolve();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card card = Turn.Owner.Recharge[0];
            if (this.IsResolveSuccess())
            {
                StartRechargeSuccess(card);
            }
            else
            {
                StartRechargeFail(card);
            }
        }
        Turn.ClearCheckData();
    }

    public static void StartRechargeFail(Card card)
    {
        Game.Instance.StartCoroutine(RechargeFailure(card));
    }

    public static void StartRechargeSuccess(Card card)
    {
        Game.Instance.StartCoroutine(RechargeSuccessful(card));
    }

    public override GameStateType Type =>
        GameStateType.Recharge;

    [CompilerGenerated]
    private sealed class <RechargeFailure>c__Iterator3F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal GuiLayout <layout>__3;
        internal CardPower <power>__2;
        internal DeckType <rechargeDeck>__1;
        internal GuiWindowLocation <window>__0;
        internal Card card;

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
                    if (this.card == null)
                    {
                        break;
                    }
                    UI.Sound.Play(SoundEffectType.CardRechargeFail);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1f));
                    this.$PC = 1;
                    goto Label_01ED;

                case 1:
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if (this.<window>__0 == null)
                    {
                        break;
                    }
                    this.<rechargeDeck>__1 = DeckType.Discard;
                    this.<power>__2 = this.card.GetPlayedCardPower();
                    if ((this.<power>__2 != null) && (this.<power>__2.RechargeAction.ToDeckType() != DeckType.None))
                    {
                        this.<rechargeDeck>__1 = this.<power>__2.RechargeAction.ToDeckType();
                    }
                    this.<layout>__3 = this.<window>__0.GetLayoutDeck(this.<rechargeDeck>__1);
                    this.card.MoveCard(this.<layout>__3.transform.position, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                    LeanTween.scale(this.card.gameObject, this.<layout>__3.Scale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.75f));
                    this.$PC = 2;
                    goto Label_01ED;

                case 2:
                    this.card.Clear();
                    Turn.Owner.MarkCardType(this.card.Type, false);
                    if (Turn.Owner.GetDeck(this.<rechargeDeck>__1) == null)
                    {
                        Campaign.Box.Add(this.card, false);
                        break;
                    }
                    Turn.Owner.GetDeck(this.<rechargeDeck>__1).Add(this.card);
                    break;

                default:
                    goto Label_01EB;
            }
            GameStateRecharge.ClearPlayerChoice();
            Turn.Iterators.Invoke();
            this.$PC = -1;
        Label_01EB:
            return false;
        Label_01ED:
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

    [CompilerGenerated]
    private sealed class <RechargeSuccessful>c__Iterator3E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal ActionType <action>__2;
        internal DeckPositionType <rechargePostion>__3;
        internal CardPropertyRecharge <rechargeProp>__1;
        internal GuiWindowLocation <window>__0;
        internal Card card;

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
                    if (this.card != null)
                    {
                        this.<window>__0 = UI.Window as GuiWindowLocation;
                        if (this.<window>__0 != null)
                        {
                            this.<rechargeProp>__1 = this.card.GetComponent<CardPropertyRecharge>();
                            this.<action>__2 = ActionType.Recharge;
                            if (this.<rechargeProp>__1 != null)
                            {
                                this.<action>__2 = this.<rechargeProp>__1.SuccessDestination;
                            }
                            switch (this.<action>__2)
                            {
                                case ActionType.Recharge:
                                    UI.Sound.Play(SoundEffectType.CardRechargeSuccess);
                                    VisualEffect.ApplyToCard(VisualEffectType.CardWinBoonDeck, this.card, 3f);
                                    this.card.Animate(AnimationType.Acquire, true);
                                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1f));
                                    this.$PC = 1;
                                    goto Label_0220;

                                case ActionType.Discard:
                                    this.<window>__0.Discard(this.card);
                                    this.$current = new WaitForSeconds(1f);
                                    this.$PC = 3;
                                    goto Label_0220;

                                case ActionType.Bury:
                                    this.<window>__0.Bury(this.card);
                                    this.$current = new WaitForSeconds(1f);
                                    this.$PC = 2;
                                    goto Label_0220;

                                case ActionType.Banish:
                                    this.$current = Game.Instance.StartCoroutine(GameStateDispose.BanishBoon(this.card));
                                    this.$PC = 4;
                                    goto Label_0220;
                            }
                        }
                    }
                    break;

                case 1:
                    this.card.Clear();
                    Turn.Owner.MarkCardType(this.card.Type, false);
                    this.<rechargePostion>__3 = GameStateRecharge.GetRechargePosition();
                    Turn.Owner.Deck.Add(this.card, this.<rechargePostion>__3);
                    UI.Sound.Play(SoundEffectType.CardRecharged);
                    if (this.<rechargePostion>__3 == DeckPositionType.Shuffle)
                    {
                        VisualEffect.Shuffle(DeckType.Character);
                    }
                    break;

                case 2:
                case 3:
                case 4:
                    break;

                default:
                    goto Label_021E;
            }
            GameStateRecharge.ClearPlayerChoice();
            Turn.Iterators.Invoke();
            this.$PC = -1;
        Label_021E:
            return false;
        Label_0220:
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

