using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventClosedBoostLocation : Event
{
    [Tooltip("if true, the top card will be acquired")]
    public bool AcquireTopCard = true;
    [Tooltip("dice used for random cards from box")]
    public DiceType Dice = DiceType.D6;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("what cards should be choosen from the box")]
    public CardSelector Selector;

    private void EventClosedBoostLocation_Acquire()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (((window != null) && this.AcquireTopCard) && (Location.Current.Deck.Count > 0))
        {
            Location.Current.Deck[0].transform.position = window.layoutExplore.transform.position;
            Location.Current.Deck[0].transform.localScale = window.layoutExplore.Size;
            Location.Current.Deck[0].Show(CardSideType.Back);
            Location.Current.Deck[0].MoveCard(window.layoutLocation.transform.position, 0.25f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(new Action(this.FinishAfterAcquire));
            LeanTween.scale(Location.Current.Deck[0].gameObject, window.layoutLocation.Scale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            this.EventClosedBoostLocation_Finish();
        }
    }

    private void EventClosedBoostLocation_Cards()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        Card[] cardArray = new Card[Turn.DiceTotal];
        for (int i = 0; i < Turn.DiceTotal; i++)
        {
            Card card = Campaign.Box.Draw(this.Selector);
            if (card != null)
            {
                card.transform.position = window.boxPanel.transform.position;
                card.transform.localScale = window.layoutExplore.Size;
                cardArray[i] = card;
            }
        }
        GuiScriptBoostLocation location2 = new GuiScriptBoostLocation {
            Cards = cardArray,
            Callback = new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedBoostLocation_Acquire")
        };
        window.layoutExamine.Deck = Location.Current.Deck;
        window.layoutExamine.Script = location2;
        window.layoutExamine.Show(true);
    }

    private void EventClosedBoostLocation_Finish()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessLayoutDecks();
            window.Refresh();
        }
        bool flag = false;
        if ((this.AcquireTopCard && (Location.Current.Deck.Count > 0)) && (window != null))
        {
            window.layoutLocation.Show(true);
            window.layoutLocation.Refresh();
            Turn.State = GameStateType.Acquire;
            flag = true;
        }
        if (!flag)
        {
            Turn.State = GameStateType.Done;
        }
        Event.Done();
    }

    private void EventClosedBoostLocation_Roll()
    {
        Turn.Checks = null;
        Turn.Dice.Clear();
        Turn.Dice.Add(this.Dice);
        base.RefreshDicePanel();
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Location, "EventClosedBoostLocation_Cards"));
        Turn.State = GameStateType.Roll;
    }

    private void FinishAfterAcquire()
    {
        base.StartCoroutine(this.FinishAfterAcquireCoroutine());
    }

    [DebuggerHidden]
    private IEnumerator FinishAfterAcquireCoroutine() => 
        new <FinishAfterAcquireCoroutine>c__Iterator20 { <>f__this = this };

    public override void OnLocationClosed()
    {
        this.EventClosedBoostLocation_Roll();
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnLocationClosed;

    [CompilerGenerated]
    private sealed class <FinishAfterAcquireCoroutine>c__Iterator20 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal EventClosedBoostLocation <>f__this;

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
                    this.$current = null;
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.EventClosedBoostLocation_Finish();
                    this.$PC = -1;
                    break;
            }
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

